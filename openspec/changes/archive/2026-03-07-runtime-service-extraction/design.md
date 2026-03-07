## Context

`GlobalShortcutService` and `ThemeService` were implemented in `Agibuild.Fulora.Avalonia` (UI assembly) despite having zero Avalonia framework dependencies. Their dependencies are exclusively on Core abstractions (`IGlobalShortcutPlatformProvider`, `IPlatformThemeProvider`, `BridgeEvent<T>`, etc.).

This creates three problems:
1. **Testing coupling**: Unit tests must reference the Avalonia assembly to test pure business logic
2. **Layer violation**: Non-UI services live in UI layer, blurring the architectural boundary
3. **Mutation testing scope**: These services contain testable business logic that should be covered by mutation testing, but the Avalonia assembly is explicitly excluded from mutation testing scope

The Avalonia assembly contains platform-specific UI providers (`AvaloniaThemeProvider`, `SharpHookGlobalShortcutProvider`, `ShortcutKeyMapper`, `WebViewShortcutRouter`) that depend on Avalonia types and must remain in the UI layer.

## Goals / Non-Goals

**Goals:**
- Move `GlobalShortcutService` and `ThemeService` to `Agibuild.Fulora.Runtime`
- Preserve all existing APIs, behavior, and DI registration
- Bring these services into mutation testing scope
- Maintain `internal` method accessibility from the Avalonia assembly

**Non-Goals:**
- Extract `WebViewShortcutRouter` (hard dependency on `Avalonia.Input.Key`, `KeyModifiers`, `KeyEventArgs`)
- Change any service public API or behavior
- Modify platform-specific providers
- Refactor DI registration architecture

## Decisions

### D1: Target assembly — Runtime, not Core

`GlobalShortcutService` uses `BridgeEvent<T>` which is defined in Core, but the service itself is a runtime lifecycle component (registration, disposal, event routing). `ThemeService` similarly manages runtime state (last known theme mode) and event deduplication. Both fit the Runtime layer's responsibility of "orchestrating Core contracts at runtime."

**Alternative considered:** Core. Rejected because Core is contracts-only; adding stateful service implementations would expand its responsibility.

### D2: Namespace preservation

`GlobalShortcutService` keeps `namespace Agibuild.Fulora` (root namespace). `ThemeService` keeps `namespace Agibuild.Fulora.Shell`. This avoids any namespace-breaking change for consumers who reference these types.

### D3: InternalsVisibleTo for cross-assembly internal access

`WebViewShortcutRouter` (Avalonia assembly, `AssemblyName=Agibuild.Fulora`) calls `GlobalShortcutService.SuppressNextActivation()` and `FindIdByChord()` — both `internal` methods. Runtime already declares `InternalsVisibleTo Include="Agibuild.Fulora"`, so no change is needed.

### D4: WebViewShortcutRouter stays in Avalonia

`WebViewShortcutRouter` depends on `Avalonia.Input.Key`, `KeyModifiers`, and `KeyEventArgs`. Extracting it would require abstracting Avalonia input types, which is a much larger refactor with no clear benefit. Deferred to Phase 12 if needed.

### D5: File placement — Shell/ subdirectory in Runtime

Both services placed in `src/Agibuild.Fulora.Runtime/Shell/` to mirror the original Avalonia/Shell/ structure. This maintains organizational consistency with the provider interfaces defined in `Core/Shell/`.

## Testing Strategy

- **Existing unit tests**: `GlobalShortcutServiceTests` and `ThemeServiceTests` should continue passing unchanged — they test via Core interfaces, not assembly locations
- **Existing integration tests**: `GlobalShortcutServiceIntegrationTests` and `ThemeServiceIntegrationTests` should pass unchanged
- **Build verification**: Avalonia assembly must compile without errors (internal access via `InternalsVisibleTo`)
- **Mutation testing**: Both services are now in Runtime (mutation testing scope), verifiable via Stryker

## Risks / Trade-offs

- **[Risk]** Future Runtime changes could accidentally break internal method contracts used by Avalonia → **Mitigation**: Integration tests cover the cross-assembly interaction; `WebViewShortcutRouter` tests exercise `SuppressNextActivation`/`FindIdByChord`
- **[Trade-off]** Two assemblies now collaborate via `internal` methods across assembly boundaries → Accepted: This is the established pattern in the codebase (Runtime exposes internals to Avalonia for multiple other components)
