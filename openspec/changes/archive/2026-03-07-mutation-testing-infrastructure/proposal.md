## Why

Line/branch coverage (currently 96%/93%) measures what code tests execute, not whether tests actually detect defects. Mutation testing measures test suite effectiveness by introducing small code changes (mutants) and checking if tests catch them. A global mutation score threshold ensures the test suite is meaningful, not just ceremonial.

Traces to **G4 (Testability)** and stabilization track.

## What Changes

- Install `dotnet-stryker` as a local .NET tool
- Create Stryker configuration targeting non-UI projects: Core, Adapters.Abstractions, Runtime, DependencyInjection, Plugins (LocalStorage, HttpClient, Database, FileSystem, Notifications, AuthToken, Biometric), Telemetry.OpenTelemetry, CLI
- Exclude UI projects (Avalonia, all platform Adapters with native interop) and Bridge.Generator (Roslyn analyzer, tested via integration tests)
- Add `MutationTest` Nuke build target
- Add CI pipeline step for mutation testing
- Set global threshold: mutation score ≥ 80%

## Non-goals

- Mutation testing for UI layer or native-interop adapter code
- Mutation testing for Bridge.Generator (Roslyn source generator)
- Changing existing test logic to improve mutation scores (that's Change 4)

## Capabilities

### New Capabilities
- `mutation-testing`: Stryker.NET integration for mutation testing of non-UI C# code

### Modified Capabilities
(none)

## Impact

- **Tools**: New `.config/dotnet-tools.json` entry for `dotnet-stryker`
- **Build**: New Nuke target `MutationTest`
- **CI**: New pipeline step (can run on schedule or per-PR)
- **Config**: `stryker-config.json` at repo root
