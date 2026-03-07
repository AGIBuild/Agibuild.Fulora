## Context

The framework provides `ITelemetryProvider` (events, metrics, exceptions) and `IBridgeTracer` (bridge call lifecycle hooks). The OpenTelemetry package demonstrates the integration pattern: implement both interfaces, register via `FuloraServiceBuilder` extension. Sentry follows the same pattern but maps to Sentry-specific APIs (breadcrumbs, `CaptureException`, optional metrics).

## Goals / Non-Goals

**Goals:**
- One-line Sentry integration: `services.AddFulora().AddSentry(o => o.Dsn = "...")`
- Bridge call breadcrumbs automatically attached to Sentry error events
- Exception capture with bridge context (service name, method, params)
- Composable with OpenTelemetry via `CompositeTelemetryProvider`

**Non-Goals:**
- Manage Sentry SDK initialization lifecycle (user's responsibility)
- Implement Sentry performance monitoring / transactions (use Sentry SDK directly)
- Replace the OpenTelemetry integration

## Decisions

### D1: Sentry SDK initialization — user-managed

The Fulora Sentry package does NOT call `SentrySdk.Init()`. The app must initialize Sentry before calling `AddSentry()`. Rationale: Sentry DSN, environment, release, and other options are app-level concerns. The Fulora package only adds bridge-specific enrichment on top.

However, `AddSentry()` accepts an optional `Action<SentryFuloraOptions>` to configure Fulora-specific behavior (breadcrumb verbosity, param capture toggle).

### D2: ITelemetryProvider mapping

| ITelemetryProvider method | Sentry API |
|--------------------------|------------|
| `TrackEvent` | `SentrySdk.AddBreadcrumb(name, category: "fulora.event", data: properties)` |
| `TrackMetric` | `SentrySdk.AddBreadcrumb(name, category: "fulora.metric", data: {value, ...dimensions})` |
| `TrackException` | `SentrySdk.CaptureException(exception)` with properties as extra data |
| `Flush` | `SentrySdk.FlushAsync(TimeSpan).GetAwaiter().GetResult()` |

Events and metrics are breadcrumbs (not Sentry events) to avoid quota consumption. Only exceptions are captured as Sentry events.

### D3: IBridgeTracer mapping

| IBridgeTracer method | Sentry API |
|---------------------|------------|
| `OnExportCallStart` | `SentrySdk.AddBreadcrumb("bridge.export.start", data: {service, method, params?})` |
| `OnExportCallEnd` | `SentrySdk.AddBreadcrumb("bridge.export.end", data: {service, method, elapsed})` |
| `OnExportCallError` | `SentrySdk.CaptureException(error)` with bridge scope context |
| `OnImportCallStart` | `SentrySdk.AddBreadcrumb("bridge.import.start", ...)` |
| `OnImportCallEnd` | `SentrySdk.AddBreadcrumb("bridge.import.end", ...)` |
| `OnServiceExposed` | `SentrySdk.AddBreadcrumb("bridge.service.exposed", ...)` |
| `OnServiceRemoved` | `SentrySdk.AddBreadcrumb("bridge.service.removed", ...)` |

For `OnExportCallError`, the exception is captured with `SentrySdk.CaptureException` and the Sentry scope is enriched with bridge context tags (`fulora.service_name`, `fulora.method_name`, `fulora.elapsed_ms`).

### D4: SentryFuloraOptions

```csharp
public sealed class SentryFuloraOptions
{
    public bool CaptureBridgeParams { get; set; } = false; // security: off by default
    public int MaxBreadcrumbParamsLength { get; set; } = 512;
    public TimeSpan FlushTimeout { get; set; } = TimeSpan.FromSeconds(2);
}
```

`CaptureBridgeParams` is off by default to prevent sensitive data leakage into Sentry.

### D5: Package dependency — Sentry (MIT)

Depend on `Sentry` NuGet package (core SDK, not `Sentry.AspNetCore`). This keeps the dependency minimal and desktop-friendly.

## Testing Strategy

- Unit tests for `SentryTelemetryProvider`: verify `SentrySdk` static calls via Sentry's `IHub` abstraction or verify breadcrumb/capture behavior
- Unit tests for `SentryBridgeTracer`: verify breadcrumb messages and exception capture
- Unit tests for `SentryFuloraOptions` defaults
- Unit test for DI extension method registration
- Existing telemetry tests continue passing (no changes to shared infrastructure)

## Risks / Trade-offs

- **[Risk]** Sentry SDK uses static `SentrySdk` class — hard to unit test → **Mitigation**: Use Sentry's `IHub` abstraction for testability, inject via constructor
- **[Trade-off]** Breadcrumbs for events/metrics vs Sentry events → Accepted: avoids quota consumption, breadcrumbs provide sufficient context for crash investigation
- **[Trade-off]** `CaptureBridgeParams` off by default → Accepted: security-first; users opt-in to capture method params
