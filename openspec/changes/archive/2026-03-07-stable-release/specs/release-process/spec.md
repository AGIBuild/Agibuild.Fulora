## Purpose

Define requirements for the v1.1.0 stable release process, covering version tagging, changelog documentation, pipeline execution, and post-release validation.

## Requirements

### Requirement: Version tag created correctly

#### Scenario: Git tag follows semver format

- **WHEN** the release is prepared
- **THEN** a git tag `v1.1.0` SHALL be created on the main branch
- **AND** the tag SHALL point to the commit that includes all stabilization changes

#### Scenario: MinVer derives correct version

- **GIVEN** the tag `v1.1.0` exists
- **WHEN** `dotnet build` is run
- **THEN** all package versions SHALL be `1.1.0`

### Requirement: CHANGELOG documents all stabilization changes

#### Scenario: CHANGELOG has v1.1.0 section

- **WHEN** the release is prepared
- **THEN** `CHANGELOG.md` SHALL contain a `## [1.1.0]` section with the release date
- **AND** the section SHALL document: adapter shared utilities, runtime service relocation, mutation testing infrastructure, quality hardening, Avalonia 12 alignment

### Requirement: Release pipeline executes successfully

#### Scenario: Tag push triggers CI pipeline

- **WHEN** the `v1.1.0` tag is pushed to origin
- **THEN** the release CI workflow SHALL trigger automatically

#### Scenario: NuGet packages published

- **WHEN** the pipeline completes
- **THEN** the following NuGet packages SHALL be available on nuget.org at version `1.1.0`:
  - `Agibuild.Fulora.Core`
  - `Agibuild.Fulora.Runtime`
  - `Agibuild.Fulora.Avalonia`
  - `Agibuild.Fulora.DependencyInjection`
  - `Agibuild.Fulora.Adapters.Abstractions`
  - Platform adapter packages (Windows, macOS, Linux, Android, iOS)

#### Scenario: npm package published

- **WHEN** the pipeline completes
- **THEN** the `@agibuild/bridge` npm package SHALL be available at the corresponding version

### Requirement: Post-release validation

#### Scenario: Packages are installable

- **WHEN** `dotnet add package Agibuild.Fulora.Avalonia --version 1.1.0` is run in a new project
- **THEN** the package SHALL install successfully

#### Scenario: Template references updated

- **WHEN** the release is complete
- **THEN** template project references SHOULD be verified against the published version
