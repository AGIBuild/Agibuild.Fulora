## Why

Enterprise adopters need production crash reporting with context-rich bridge call breadcrumbs. Sentry is the industry-standard crash reporting service with native .NET SDK support (MIT license). The framework already has `ITelemetryProvider` and `IBridgeTracer` contracts — a Sentry package just needs to implement them, following the exact same pattern as `Agibuild.Fulora.Telemetry.OpenTelemetry`.

Traces to ROADMAP Phase 12 M12.1.

## What Changes

- New NuGet package: `Agibuild.Fulora.Telemetry.Sentry`
- `SentryTelemetryProvider` — implements `ITelemetryProvider` using `SentrySdk`
- `SentryBridgeTracer` — implements `IBridgeTracer` adding bridge call breadcrumbs to Sentry scope
- `AddSentry(Action<SentryFuloraOptions>)` extension on `FuloraServiceBuilder`
- Unit tests for provider, tracer, and DI registration

## Non-goals

- Initializing the Sentry SDK (app-level responsibility; Fulora integration adds breadcrumbs and exception capture to an existing Sentry session)
- Replacing OpenTelemetry integration (both can coexist via `CompositeTelemetryProvider`)
- UI-level crash dialogs or user feedback flows

## Capabilities

### New Capabilities
- `sentry-telemetry`: Sentry-backed telemetry provider for crash reporting with bridge breadcrumbs

### Modified Capabilities
(none)

## Impact

- **Code**: New project `src/Agibuild.Fulora.Telemetry.Sentry/` (4-5 files)
- **Tests**: New unit tests in `tests/Agibuild.Fulora.UnitTests/`
- **Packages**: New NuGet package `Agibuild.Fulora.Telemetry.Sentry`
- **Dependencies**: `Sentry` NuGet package (MIT license)
