/**
 * Simple C# → TypeScript type generation for [JsExport] interfaces.
 * Parses interface methods from C# text and generates TypeScript interface.
 */

export interface ParsedMethod {
  name: string;
  returnType: string;
  params: { name: string; type: string; optional: boolean }[];
}

function mapCsharpToTs(type: string): string {
  const t = type.trim();
  if (t === 'string') return 'string';
  if (t === 'int' || t === 'long' || t === 'float' || t === 'double') return 'number';
  if (t === 'bool') return 'boolean';
  if (t === 'void') return 'void';
  if (t.startsWith('Task<')) {
    const inner = t.slice(5, -1).trim();
    return `Promise<${mapCsharpToTs(inner)}>`;
  }
  if (t.endsWith('[]')) {
    const inner = t.slice(0, -2).trim();
    return `${mapCsharpToTs(inner)}[]`;
  }
  return type;
}

export function parseCsharpInterface(text: string): { interfaceName: string; methods: ParsedMethod[] } | null {
  const interfaceMatch = text.match(/\[JsExport\]\s*(?:public\s+)?interface\s+(\w+)\s*\{([^}]*)\}/s);
  if (!interfaceMatch) return null;

  const interfaceName = interfaceMatch[1];
  const body = interfaceMatch[2];

  const methods: ParsedMethod[] = [];
  const methodRegex = /(?:Task<[^>]+>|Task|void)\s+(\w+)\s*\(([^)]*)\)/g;
  let m: RegExpExecArray | null;
  while ((m = methodRegex.exec(body)) !== null) {
    const fullMatch = m[0];
    const returnPart = fullMatch.match(/^(Task<[^>]+>|Task|void)/);
    const returnType = returnPart ? returnPart[1] : 'void';
    const paramsStr = m[2].trim();
    const params: { name: string; type: string; optional: boolean }[] = [];

    if (paramsStr) {
      for (const p of paramsStr.split(',')) {
        const parts = p.trim().split(/\s+/);
        if (parts.length >= 2) {
          let type = parts[0];
          let name = parts[1].split('=')[0].trim();
          const optional = type.endsWith('?') || name.endsWith('?');
          if (type.endsWith('?')) type = type.slice(0, -1);
          if (name.endsWith('?')) name = name.slice(0, -1);
          params.push({ name, type, optional });
        }
      }
    }

    methods.push({
      name: m[1],
      returnType,
      params,
    });
  }

  return { interfaceName, methods };
}

export function generateTypeScript(csharpText: string): string {
  const parsed = parseCsharpInterface(csharpText);
  if (!parsed) {
    return '// Paste a [JsExport] C# interface to generate TypeScript';
  }

  const { interfaceName, methods } = parsed;
  const serviceName = interfaceName.startsWith('I') ? interfaceName.slice(1) : interfaceName;

  const lines: string[] = [
    `// Generated from ${interfaceName}`,
    '',
    `interface ${interfaceName}Bridge {`,
  ];

  for (const method of methods) {
    const paramsType =
      method.params.length === 0
        ? 'void'
        : `{ ${method.params.map((p) => `${p.name}${p.optional ? '?' : ''}: ${mapCsharpToTs(p.type)}`).join('; ')} }`;
    const returnType = mapCsharpToTs(method.returnType);
    lines.push(`  ${method.name}: (params?: ${paramsType}) => ${returnType};`);
  }

  lines.push('}');
  lines.push('');
  lines.push(`const ${serviceName}Service = bridge.getService<${interfaceName}Bridge>('${serviceName}');`);
  lines.push('');
  lines.push('// Usage:');
  for (const method of methods.slice(0, 2)) {
    const args = method.params.length > 0
      ? `{ ${method.params.map((p) => `${p.name}: value`).join(', ')} }`
      : '';
    lines.push(`// ${serviceName}Service.${method.name}(${args})`);
  }

  return lines.join('\n');
}
