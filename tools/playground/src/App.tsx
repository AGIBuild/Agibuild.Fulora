import { useState, useEffect, useCallback } from 'react';
import { Editor } from './components/Editor';
import { Preview } from './components/Preview';
import { generateTypeScript, parseCsharpInterface } from './utils/generator';

const DEFAULT_CSHARP = `[JsExport]
public interface ITodoService
{
    Task<TodoItem[]> GetAll();
    Task<TodoItem> Create(string title);
    Task<TodoItem> Update(int id, string? title = null, bool? completed = null);
    Task Delete(int id);
}`;

function getInitialCode(): string {
  if (typeof window === 'undefined') return DEFAULT_CSHARP;
  const params = new URLSearchParams(window.location.search);
  const encoded = params.get('code');
  if (encoded) {
    try {
      return decodeURIComponent(atob(encoded));
    } catch {
      return DEFAULT_CSHARP;
    }
  }
  return DEFAULT_CSHARP;
}

function encodeToUrl(code: string): void {
  if (typeof window === 'undefined') return;
  try {
    const encoded = btoa(encodeURIComponent(code));
    const url = new URL(window.location.href);
    url.searchParams.set('code', encoded);
    window.history.replaceState({}, '', url.toString());
  } catch {
    // ignore
  }
}

export function App() {
  const [csharpCode, setCsharpCode] = useState(getInitialCode);
  const [tsOutput, setTsOutput] = useState('');
  const [mockMethod, setMockMethod] = useState('');
  const [mockParams, setMockParams] = useState('{}');
  const [mockResult, setMockResult] = useState('');

  const updateTs = useCallback((code: string) => {
    setTsOutput(generateTypeScript(code));
  }, []);

  useEffect(() => {
    updateTs(csharpCode);
  }, [csharpCode, updateTs]);

  useEffect(() => {
    encodeToUrl(csharpCode);
  }, [csharpCode]);

  const handleMockCall = () => {
    const parsed = parseCsharpInterface(csharpCode);
    if (!parsed || !mockMethod.trim()) {
      setMockResult('Enter a method name (e.g. GetAll, Create)');
      return;
    }
    const method = parsed.methods.find((m) => m.name.toLowerCase() === mockMethod.trim().toLowerCase());
    if (!method) {
      setMockResult(`Method "${mockMethod}" not found. Available: ${parsed.methods.map((m) => m.name).join(', ')}`);
      return;
    }
    try {
      const params = JSON.parse(mockParams || '{}');
      setMockResult(`Mock call: ${parsed.interfaceName}.${method.name}(${JSON.stringify(params)})`);
    } catch {
      setMockResult('Invalid JSON params');
    }
  };

  return (
    <div style={{ fontFamily: 'system-ui', padding: 16, maxWidth: 1400, margin: '0 auto' }}>
      <h1 style={{ marginBottom: 16 }}>Agibuild Bridge Playground</h1>
      <p style={{ color: '#666', marginBottom: 24 }}>
        Edit C# [JsExport] interface on the left; TypeScript preview updates on the right. Share via URL.
      </p>

      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 16, marginBottom: 24 }}>
        <div>
          <h3 style={{ marginBottom: 8 }}>C# Bridge Interface</h3>
          <Editor value={csharpCode} onChange={setCsharpCode} height="400px" />
        </div>
        <div>
          <h3 style={{ marginBottom: 8 }}>Generated TypeScript</h3>
          <Preview value={tsOutput} height="400px" />
        </div>
      </div>

      <div style={{ border: '1px solid #ddd', borderRadius: 8, padding: 16, background: '#f9f9f9' }}>
        <h3 style={{ marginBottom: 12 }}>Mock Bridge Call</h3>
        <div style={{ display: 'flex', gap: 12, flexWrap: 'wrap', alignItems: 'center' }}>
          <input
            type="text"
            placeholder="Method name (e.g. Create)"
            value={mockMethod}
            onChange={(e) => setMockMethod(e.target.value)}
            style={{ padding: 8, width: 180 }}
          />
          <input
            type="text"
            placeholder='Params JSON e.g. {"title":"Buy milk"}'
            value={mockParams}
            onChange={(e) => setMockParams(e.target.value)}
            style={{ padding: 8, flex: 1, minWidth: 200 }}
          />
          <button onClick={handleMockCall} style={{ padding: '8px 16px' }}>
            Test
          </button>
        </div>
        {mockResult && (
          <pre style={{ marginTop: 12, padding: 12, background: '#fff', borderRadius: 4, overflow: 'auto' }}>
            {mockResult}
          </pre>
        )}
      </div>
    </div>
  );
}
