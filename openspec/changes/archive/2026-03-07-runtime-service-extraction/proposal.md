## Why

`GlobalShortcutService` and `ThemeService` live in `Agibuild.Fulora.Avalonia` (UI layer) despite having zero Avalonia dependencies. This violates the layering principle — they are framework services that should be in `Agibuild.Fulora.Runtime`. Their current placement makes them impossible to test without the Avalonia UI assembly and incorrectly couples non-UI logic to the UI layer.

Traces to **G4 (Testability)** and stabilization before Phase 12.

## What Changes

- Move `GlobalShortcutService` from `Avalonia/Shell/` to `Runtime/Shell/`
- Move `ThemeService` from `Avalonia/Shell/` to `Runtime/Shell/`
- `WebViewShortcutRouter` remains in Avalonia — it has a hard dependency on `Avalonia.Input` types (`Key`, `KeyModifiers`, `KeyEventArgs`). Partial extraction deferred to Phase 12.
- Update DI registration to reflect new assembly locations
- Existing Avalonia-specific types (`AvaloniaThemeProvider`, `SharpHookGlobalShortcutProvider`, `ShortcutKeyMapper`) stay in Avalonia

## Non-goals

- Extracting `WebViewShortcutRouter` (Avalonia input type dependency)
- Changing any service API or behavior
- Modifying platform-specific providers

## Capabilities

### New Capabilities
(none)

### Modified Capabilities
- `webview-shortcut-service`: Service implementation moves to Runtime layer; API unchanged

## Impact

- **Code**: `src/Agibuild.Fulora.Avalonia/Shell/` (remove 2 files), `src/Agibuild.Fulora.Runtime/Shell/` (add 2 files)
- **DI**: Registration in `FuloraServiceCollectionExtensions` may need updating to reference Runtime assembly
- **Tests**: Existing tests continue to pass; services now testable without Avalonia assembly
- **APIs**: No public API changes
