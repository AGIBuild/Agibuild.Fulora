## Purpose

Define requirements for relocating `GlobalShortcutService` and `ThemeService` from `Agibuild.Fulora.Avalonia` (UI assembly) to `Agibuild.Fulora.Runtime`, preserving all APIs, behavior, and cross-assembly internal access contracts.

## Requirements

### Requirement: GlobalShortcutService resides in Runtime assembly

`GlobalShortcutService` SHALL be located in `src/Agibuild.Fulora.Runtime/Shell/GlobalShortcutService.cs` with `namespace Agibuild.Fulora`.

#### Scenario: Service compiles in Runtime with existing dependencies

- **GIVEN** `GlobalShortcutService` depends on `IGlobalShortcutPlatformProvider`, `IWebViewHostCapabilityPolicy`, `BridgeEvent<T>`, `GlobalShortcutBinding`, `GlobalShortcutResult`, `ShortcutKey`, `ShortcutModifiers` (all from Core)
- **WHEN** the project is built
- **THEN** `Agibuild.Fulora.Runtime` SHALL compile without errors

#### Scenario: Internal methods accessible from Avalonia assembly

- **GIVEN** `WebViewShortcutRouter` in `Agibuild.Fulora.Avalonia` (AssemblyName: `Agibuild.Fulora`) calls `GlobalShortcutService.SuppressNextActivation()` and `FindIdByChord()`
- **AND** `Agibuild.Fulora.Runtime.csproj` declares `InternalsVisibleTo Include="Agibuild.Fulora"`
- **WHEN** the solution is built
- **THEN** `Agibuild.Fulora.Avalonia` SHALL compile without access errors

#### Scenario: Original file removed from Avalonia

- **WHEN** relocation is complete
- **THEN** `src/Agibuild.Fulora.Avalonia/Shell/GlobalShortcutService.cs` SHALL NOT exist

### Requirement: ThemeService resides in Runtime assembly

`ThemeService` SHALL be located in `src/Agibuild.Fulora.Runtime/Shell/ThemeService.cs` with `namespace Agibuild.Fulora.Shell`.

#### Scenario: Service compiles in Runtime with existing dependencies

- **GIVEN** `ThemeService` depends on `IPlatformThemeProvider`, `BridgeEvent<T>`, `ThemeChangedEvent`, `ThemeInfo` (all from Core)
- **WHEN** the project is built
- **THEN** `Agibuild.Fulora.Runtime` SHALL compile without errors

#### Scenario: Original file removed from Avalonia

- **WHEN** relocation is complete
- **THEN** `src/Agibuild.Fulora.Avalonia/Shell/ThemeService.cs` SHALL NOT exist

### Requirement: Existing tests pass without modification

All existing unit tests and integration tests for `GlobalShortcutService` and `ThemeService` SHALL continue passing without code changes after relocation.

#### Scenario: Unit tests remain green

- **GIVEN** `GlobalShortcutServiceTests` and `ThemeServiceTests` reference services via Core interfaces
- **WHEN** `dotnet test` is run
- **THEN** all tests SHALL pass

#### Scenario: Integration tests remain green

- **GIVEN** `GlobalShortcutServiceIntegrationTests` and `ThemeServiceIntegrationTests` exercise end-to-end service behavior
- **WHEN** `dotnet test` is run
- **THEN** all tests SHALL pass

### Requirement: No public API surface change

#### Scenario: Namespace preserved

- **WHEN** external code references `Agibuild.Fulora.GlobalShortcutService` or `Agibuild.Fulora.Shell.ThemeService`
- **THEN** the fully-qualified type names SHALL resolve correctly

#### Scenario: DI registration unaffected

- **GIVEN** services are registered via DI from the Avalonia assembly
- **WHEN** the application starts
- **THEN** `GlobalShortcutService` and `ThemeService` SHALL be resolvable from the service provider
