## Purpose

Define the mutation testing infrastructure for non-UI C# code.

## Requirements

### Requirement: Stryker.NET is installed as a local .NET tool

- `dotnet-stryker` SHALL be registered in `.config/dotnet-tools.json`
- Running `dotnet tool restore && dotnet stryker --version` SHALL succeed

### Requirement: Configuration targets non-UI projects only

- The `stryker-config.json` SHALL specify mutation scope including: Core, Adapters.Abstractions, Runtime, DependencyInjection, CLI, all Plugins, Telemetry.OpenTelemetry
- The configuration SHALL exclude: Avalonia UI project, all platform adapter projects, Bridge.Generator, test projects

### Requirement: Global mutation score threshold is 80%

- The Stryker `break` threshold SHALL be set to 80
- When the global mutation score falls below 80%, the Stryker process SHALL exit with a non-zero code

### Requirement: Nuke build target exists

- A `MutationTest` target SHALL exist in the Nuke build
- It SHALL execute `dotnet stryker --config-file stryker-config.json`
- It SHALL output reports to `artifacts/mutation-report/`

### Requirement: CI workflow runs mutation testing weekly

- A GitHub Actions workflow SHALL run mutation testing weekly (scheduled) and on manual trigger
- It SHALL upload mutation reports as artifacts
