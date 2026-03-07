## Why

Plugin ecosystem needs machine-checkable quality signals: minimum Fulora version compatibility, platform support declarations, and consistent NuGet tagging. Currently `fulora-plugin.json` is spec'd but not implemented in any plugin, and the CLI has no validation of plugin/framework version compatibility.

Traces to ROADMAP Phase 12 M12.4.

## What Changes

- Implement `fulora-plugin.json` manifest in all 7 official plugins
- Add `PluginManifest` model and parser in Core
- Add `fulora list plugins --check` CLI command to validate installed plugin compatibility
- Fix `LocalStorage` plugin missing `fulora-plugin` NuGet tag
- Add unit tests for manifest parsing and compatibility checking

## Non-goals

- Plugin marketplace or registry beyond NuGet.org
- Runtime version checking (compile-time is sufficient)
- Third-party plugin certification

## Capabilities

### New Capabilities
- `plugin-compatibility`: Machine-checkable plugin version compatibility and quality signals

### Modified Capabilities
- `fulora-cli`: Enhanced `list plugins` with `--check` validation flag

## Impact

- **Code**: New `PluginManifest` model in Core, CLI enhancement, manifest files in 7 plugins
- **Tests**: New unit tests for manifest parsing and compatibility logic
- **Packages**: Updated plugin NuGet packages with embedded manifest
