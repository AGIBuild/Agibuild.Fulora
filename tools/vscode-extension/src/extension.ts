import * as vscode from 'vscode';

import { BridgeInspectorProvider } from './bridgeInspector';

export function activate(context: vscode.ExtensionContext): void {
  const provider = new BridgeInspectorProvider(context.extensionUri);

  context.subscriptions.push(
    vscode.window.registerWebviewViewProvider(
      'fulora.bridgeInspector',
      provider,
      { webviewOptions: { retainContextWhenHidden: true } }
    )
  );

  context.subscriptions.push(
    vscode.commands.registerCommand('fulora.connect', () => provider.connect())
  );

  context.subscriptions.push(
    vscode.commands.registerCommand('fulora.disconnect', () => provider.disconnect())
  );

  context.subscriptions.push(
    vscode.commands.registerCommand('fulora.clear', () => provider.clear())
  );
}

export function deactivate(): void {
  // Cleanup handled by extension context disposal
}
