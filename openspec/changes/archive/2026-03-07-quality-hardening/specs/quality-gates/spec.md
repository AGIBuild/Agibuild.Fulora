## Purpose

Define the quality gates that must pass before the codebase is considered release-ready. These are validation requirements, not feature requirements.

## Requirements

### Requirement: All unit tests pass

#### Scenario: Full unit test suite green

- **GIVEN** the complete unit test project `Agibuild.Fulora.UnitTests`
- **WHEN** `dotnet test` is executed
- **THEN** all tests SHALL pass with 0 failures and 0 skipped

### Requirement: All integration tests pass

#### Scenario: Full integration test suite green

- **GIVEN** the integration test projects `Agibuild.Fulora.Integration.Tests` and `Agibuild.Fulora.Integration.Tests.Automation`
- **WHEN** `dotnet test` is executed
- **THEN** all tests SHALL pass with 0 failures and 0 skipped

### Requirement: Code coverage meets thresholds

#### Scenario: Line coverage at or above 96%

- **WHEN** code coverage is collected via `dotnet-coverage`
- **THEN** overall line coverage SHALL be ≥ 96%

#### Scenario: Branch coverage at or above 93%

- **WHEN** code coverage is collected via `dotnet-coverage`
- **THEN** overall branch coverage SHALL be ≥ 93%

### Requirement: No new compiler warnings

#### Scenario: Solution builds without new warnings

- **WHEN** `dotnet build` is run on the full solution
- **THEN** no new compiler warnings SHALL be introduced beyond pre-existing baseline (5 CS1591/CS1572 XML doc warnings)

### Requirement: All sample and template projects build

#### Scenario: Each sample project compiles successfully

- **GIVEN** all sample projects under `samples/` and `templates/`
- **AND** web frontends have been pre-built (`npm run build`)
- **WHEN** `dotnet build` is run for each project
- **THEN** each project SHALL build with 0 errors

#### Scenario: Avalonia version alignment

- **GIVEN** the core framework targets Avalonia `12.0.0-preview1`
- **WHEN** any sample or template project is built
- **THEN** it SHALL NOT produce `NU1605` package downgrade warnings
