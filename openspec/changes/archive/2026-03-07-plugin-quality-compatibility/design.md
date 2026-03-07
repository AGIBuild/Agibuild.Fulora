## Context

The plugin convention (`IBridgePlugin`, `BridgePluginServiceDescriptor`, `fulora-plugin` NuGet tag) is established, but lacks machine-readable metadata for version compatibility and platform support. The `fulora-plugin.json` schema was defined in the plugin-registry-discovery spec but never implemented.

## Decisions

### D1: fulora-plugin.json embedded as content in NuGet package

Each plugin includes a `fulora-plugin.json` at the project root. It is embedded as NuGet content via `<Content Include="fulora-plugin.json" PackagePath="fulora-plugin.json" />`. This makes the manifest available both at development time (in the project) and at consumption time (in the NuGet package).

### D2: Manifest schema

```json
{
  "id": "Agibuild.Fulora.Plugin.Database",
  "displayName": "Database (SQLite)",
  "services": ["DatabaseService"],
  "npmPackage": "@agibuild/bridge-plugin-database",
  "minFuloraVersion": "1.0.0",
  "platforms": ["windows", "macos", "linux", "android", "ios"]
}
```

Fields: `id` (required), `displayName` (required), `services` (required array), `npmPackage` (optional), `minFuloraVersion` (required semver), `platforms` (optional array, null = all platforms).

### D3: PluginManifest model in Core

A simple POCO + STJ deserialization in `Agibuild.Fulora.Core`. No dependency on plugin assemblies. Used by CLI for validation.

### D4: CLI --check flag on list plugins

`fulora list plugins --check` reads each installed plugin's `fulora-plugin.json` from the NuGet cache or project, compares `minFuloraVersion` with the installed Fulora version, and reports compatibility status.

### D5: Fix LocalStorage missing tag

`Agibuild.Fulora.Plugin.LocalStorage.csproj` is missing `fulora-plugin` in its `PackageTags`. Add it for consistency.

## Testing Strategy

- Unit tests for `PluginManifest` deserialization (valid, missing fields, null)
- Unit tests for version compatibility comparison
- Verify all 7 plugins have valid `fulora-plugin.json`
