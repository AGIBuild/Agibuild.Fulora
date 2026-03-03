import { Editor as MonacoEditor } from '@monaco-editor/react';

interface PreviewProps {
  value: string;
  height?: string;
}

export function Preview({ value, height = '300px' }: PreviewProps) {
  return (
    <MonacoEditor
      height={height}
      defaultLanguage="typescript"
      value={value}
      options={{
        readOnly: true,
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
