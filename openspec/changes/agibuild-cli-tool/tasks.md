## 1. CLI Project Setup

- [ ] 1.1 Create `src/Agibuild.Fulora.Cli/` project with `DotnetToolManifest` package type and `agibuild` as tool command
- [ ] 1.2 Add System.CommandLine (or equivalent) for command parsing
- [ ] 1.3 Implement `--help` and `--version` for root and subcommands

## 2. agibuild new

- [ ] 2.1 Implement `agibuild new <name> --frontend react|vue|svelte` subcommand
- [ ] 2.2 Map `--frontend` to `dotnet new agibuild-hybrid --framework` (or equivalent template parameter)
- [ ] 2.3 Invoke `dotnet new` in the current directory; create project folder named `<name>`
- [ ] 2.4 Add validation: require `--frontend` or document default; fail with clear message if invalid

## 3. agibuild generate types

- [ ] 3.1 Implement `agibuild generate types` subcommand
- [ ] 3.2 Detect solution/Bridge project (e.g. from current directory or `--project`)
- [ ] 3.3 Build Bridge project to trigger source generator; locate emitted TypeScript declarations
- [ ] 3.4 Copy or write generated `.d.ts` to web project types directory (e.g. `web/src/bridge/` or per-template convention)

## 4. agibuild dev

- [ ] 4.1 Implement `agibuild dev` subcommand
- [ ] 4.2 Detect web project (package.json) and Desktop project (.csproj)
- [ ] 4.3 Start Vite dev server (`npm run dev` or `npx vite`) in web project directory
- [ ] 4.4 Start Avalonia app (`dotnet run`) in Desktop project directory
- [ ] 4.5 Run both processes in parallel; forward stdout/stderr
- [ ] 4.6 Handle Ctrl+C / SIGINT to terminate both processes gracefully

## 5. agibuild add service

- [ ] 5.1 Implement `agibuild add service <name>` subcommand
- [ ] 5.2 Generate C# interface with `[JsExport]` in Bridge project (or `[JsImport]` with `--import` flag)
- [ ] 5.3 Generate C# implementation class in Bridge project
- [ ] 5.4 Generate TypeScript proxy/stub in web project
- [ ] 5.5 Use consistent naming: PascalCase for C#, camelCase for TS

## 6. Packaging and Documentation

- [ ] 6.1 Add `Agibuild.Fulora.Cli` to solution and build pipeline
- [ ] 6.2 Configure NuGet package metadata (version, description, authors)
- [ ] 6.3 Update getting-started docs to recommend `agibuild new` and `agibuild dev`
- [ ] 6.4 Add CLI usage section to README or docs
