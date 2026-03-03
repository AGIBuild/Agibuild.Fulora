import * as vscode from 'vscode';

const DEFAULT_PORT = 9229;
const WS_URL = `ws://127.0.0.1:${DEFAULT_PORT}/`;

export interface BridgeEvent {
  type: string;
  serviceName?: string;
  methodName?: string;
  methodCount?: number;
  direction?: 'export' | 'import';
  paramsJson?: string;
  elapsedMs?: number;
  resultType?: string;
  error?: { message?: string; stack?: string };
  services?: Array<{ serviceName: string; methodCount: number }>;
  timestamp?: number;
}

export class BridgeInspectorProvider implements vscode.WebviewViewProvider {
  private _view?: vscode.WebviewView;
  private _ws?: WebSocket;
  private _events: BridgeEvent[] = [];
  private _services: Array<{ serviceName: string; methodCount: number }> = [];

  constructor(private readonly _extensionUri: vscode.Uri) {}

  resolveWebviewView(
    webviewView: vscode.WebviewView,
    _context: vscode.WebviewViewResolveContext,
    _token: vscode.CancellationToken
  ): void {
    this._view = webviewView;

    webviewView.webview.options = {
      enableScripts: true,
      localResourceRoots: [this._extensionUri]
    };

    webviewView.webview.html = this._getHtml(webviewView.webview);
    webviewView.webview.onDidReceiveMessage((msg: { type: string }) => {
      if (msg.type === 'ready') {
        this._pushState();
      }
    });
  }

  connect(): void {
    if (this._ws?.readyState === WebSocket.OPEN) {
      vscode.window.showInformationMessage('Bridge Inspector: Already connected.');
      return;
    }

    try {
      this._ws = new WebSocket(WS_URL);

      this._ws.onopen = () => {
        vscode.window.showInformationMessage('Bridge Inspector: Connected to debug server.');
      };

      this._ws.onmessage = (ev) => {
        try {
          const data = JSON.parse(ev.data as string) as BridgeEvent;
          this._handleMessage(data);
        } catch {
          // Ignore malformed messages
        }
      };

      this._ws.onerror = () => {
        vscode.window.showErrorMessage('Bridge Inspector: WebSocket error.');
      };

      this._ws.onclose = () => {
        this._ws = undefined;
        this._pushState();
      };
    } catch (err) {
      vscode.window.showErrorMessage(
        `Bridge Inspector: Failed to connect to ${WS_URL}. Is the Fulora app running with the debug server enabled?`
      );
    }
  }

  disconnect(): void {
    if (this._ws) {
      this._ws.close();
      this._ws = undefined;
      vscode.window.showInformationMessage('Bridge Inspector: Disconnected.');
    } else {
      vscode.window.showInformationMessage('Bridge Inspector: Not connected.');
    }
  }

  clear(): void {
    this._events = [];
    this._pushState();
    vscode.window.showInformationMessage('Bridge Inspector: Cleared.');
  }

  private _handleMessage(data: BridgeEvent): void {
    switch (data.type) {
      case 'service-registry':
        this._services = data.services ?? [];
        break;
      case 'call-start':
      case 'call-end':
      case 'call-error':
      case 'service-exposed':
      case 'service-removed':
        this._events.push(data);
        if (data.type === 'service-exposed') {
          const methodCount = (data as { methodCount?: number }).methodCount ?? 0;
          const existing = this._services.findIndex((s) => s.serviceName === data.serviceName);
          const entry = { serviceName: data.serviceName ?? '', methodCount };
          if (existing >= 0) {
            this._services[existing] = entry;
          } else {
            this._services.push(entry);
          }
        } else if (data.type === 'service-removed') {
          this._services = this._services.filter((s) => s.serviceName !== data.serviceName);
        }
        break;
      default:
        break;
    }
    this._pushState();
  }

  private _pushState(): void {
    if (this._view?.webview) {
      this._view.webview.postMessage({
        type: 'state',
        connected: this._ws?.readyState === WebSocket.OPEN,
        events: this._events,
        services: this._services
      });
    }
  }

  private _getHtml(webview: vscode.Webview): string {
    return `<!DOCTYPE html>
<html>
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>Bridge Inspector</title>
  <style>
    body { font-family: var(--vscode-font-family); font-size: 12px; padding: 8px; margin: 0; }
    .status { padding: 4px 0; color: var(--vscode-descriptionForeground); }
    .status.connected { color: var(--vscode-testing-iconPassed); }
    .section { margin-top: 12px; }
    .section-title { font-weight: 600; margin-bottom: 4px; }
    .event { padding: 4px 6px; margin: 2px 0; border-left: 3px solid var(--vscode-editor-foreground); background: var(--vscode-editor-inactiveSelectionBackground); }
    .event.call-error { border-left-color: var(--vscode-errorForeground); }
    .event-dir { font-size: 10px; opacity: 0.8; }
    .empty { color: var(--vscode-descriptionForeground); font-style: italic; }
  </style>
</head>
<body>
  <div class="status" id="status">Disconnected. Use Connect command to connect.</div>
  <div class="section">
    <div class="section-title">Services</div>
    <div id="services" class="empty">No services</div>
  </div>
  <div class="section">
    <div class="section-title">Events</div>
    <div id="events" class="empty">No events</div>
  </div>
  <script>
    const vscode = acquireVsCodeApi();
    vscode.postMessage({ type: 'ready' });

    window.addEventListener('message', (e) => {
      const msg = e.data;
      if (msg.type !== 'state') return;
      document.getElementById('status').textContent = msg.connected ? 'Connected' : 'Disconnected';
      document.getElementById('status').className = 'status' + (msg.connected ? ' connected' : '');
      document.getElementById('services').innerHTML = msg.services?.length
        ? msg.services.map(s => \`<div>\${s.serviceName} (\${s.methodCount} methods)</div>\`).join('')
        : '<span class="empty">No services</span>';
      document.getElementById('events').innerHTML = msg.events?.length
        ? msg.events.map(ev => {
            const cls = ev.type === 'call-error' ? 'event call-error' : 'event';
            const dir = ev.direction ? \` [\${ev.direction}]\` : '';
            const info = ev.type === 'call-end' || ev.type === 'call-error'
              ? \` \${ev.elapsedMs ?? '-'}ms\`
              : '';
            return \`<div class="\${cls}"><span class="event-dir">\${ev.type}\${dir}\${info}</span> \${ev.serviceName || ''}.\${ev.methodName || ''}</div>\`;
          }).join('')
        : '<span class="empty">No events</span>';
    });
  </script>
</body>
</html>`;
  }
}
