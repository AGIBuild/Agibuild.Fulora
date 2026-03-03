import { Editor as MonacoEditor } from '@monaco-editor/react';

interface EditorProps {
  value: string;
  onChange: (value: string) => void;
  height?: string;
}

const DEFAULT_CSHARP = `[JsExport]
public interface ITodoService
{
    Task<TodoItem[]> GetAll();
    Task<TodoItem> Create(string title);
    Task<TodoItem> Update(int id, string? title = null, bool? completed = null);
    Task Delete(int id);
}

public class TodoItem
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public bool Completed { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}`;

export function Editor({ value, onChange, height = '300px' }: EditorProps) {
  return (
    <MonacoEditor
      height={height}
      defaultLanguage="csharp"
      value={value || DEFAULT_CSHARP}
      onChange={(v) => onChange(v ?? '')}
      options={{
        minimap: { enabled: false },
        fontSize: 13,
        lineNumbers: 'on',
        wordWrap: 'on',
        scrollBeyondLastLine: false,
      }}
      theme="vs-dark"
    />
  );
}
