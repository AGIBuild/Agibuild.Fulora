# Agibuild Bridge Playground

Interactive playground for designing and testing C# bridge interfaces. Edit `[JsExport]` C# interfaces and see the generated TypeScript client code in real time.

## Features

- **Left panel**: Monaco editor for C# bridge interface code
- **Right panel**: Generated TypeScript preview
- **Bottom**: Mock bridge call testing area
- **URL state**: Code is encoded in the URL for sharing

## Run

```bash
cd tools/playground
npm install
npm run dev
```

## Type Mapping

The generator maps common C# types to TypeScript:

- `string` → `string`
- `int`, `long`, `float`, `double` → `number`
- `bool` → `boolean`
- `Task<T>` → `Promise<T>`
- `T[]` → `T[]`
