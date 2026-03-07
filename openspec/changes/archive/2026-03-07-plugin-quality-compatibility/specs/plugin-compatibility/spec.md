## Purpose

Define requirements for plugin quality metadata and version compatibility checking.

## Requirements

### Requirement: fulora-plugin.json manifest in every official plugin

#### Scenario: Each plugin has valid manifest

- **GIVEN** each of the 7 official plugins
- **WHEN** `fulora-plugin.json` is read from the plugin project directory
- **THEN** it SHALL contain valid JSON with required fields: `id`, `displayName`, `services`, `minFuloraVersion`

### Requirement: PluginManifest model deserializes manifest

#### Scenario: Valid manifest parses correctly

- **WHEN** a valid `fulora-plugin.json` is parsed
- **THEN** all fields SHALL be populated correctly

#### Scenario: Missing optional fields default to null

- **WHEN** `npmPackage` or `platforms` are omitted
- **THEN** they SHALL default to `null`

### Requirement: Version compatibility check

#### Scenario: Compatible plugin version

- **GIVEN** installed Fulora version is `1.1.0`
- **AND** plugin `minFuloraVersion` is `1.0.0`
- **THEN** the plugin SHALL be reported as compatible

#### Scenario: Incompatible plugin version

- **GIVEN** installed Fulora version is `0.9.0`
- **AND** plugin `minFuloraVersion` is `1.0.0`
- **THEN** the plugin SHALL be reported as incompatible

### Requirement: NuGet tagging consistency

#### Scenario: All plugins have fulora-plugin tag

- **GIVEN** all 7 official plugin `.csproj` files
- **THEN** each SHALL contain `fulora-plugin` in `PackageTags`
