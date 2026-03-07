## Purpose

Define requirements for the `Agibuild.Fulora.Telemetry.Sentry` package that provides Sentry crash reporting with bridge call breadcrumbs via `ITelemetryProvider` and `IBridgeTracer` contracts.

## Requirements

### Requirement: SentryTelemetryProvider implements ITelemetryProvider

#### Scenario: TrackEvent adds a breadcrumb

- **WHEN** `TrackEvent("user.login", properties)` is called
- **THEN** a Sentry breadcrumb SHALL be added with message "user.login", category "fulora.event", and properties as data

#### Scenario: TrackMetric adds a breadcrumb

- **WHEN** `TrackMetric("bridge.latency", 42.5, dimensions)` is called
- **THEN** a Sentry breadcrumb SHALL be added with message "bridge.latency", category "fulora.metric", and value + dimensions as data

#### Scenario: TrackException captures Sentry event

- **WHEN** `TrackException(exception, properties)` is called
- **THEN** the exception SHALL be captured via `SentrySdk.CaptureException`
- **AND** properties SHALL be set as extra data on the Sentry scope

#### Scenario: Flush calls Sentry flush

- **WHEN** `Flush()` is called
- **THEN** `SentrySdk.FlushAsync()` SHALL be called with the configured timeout

### Requirement: SentryBridgeTracer implements IBridgeTracer

#### Scenario: Export call start adds breadcrumb

- **WHEN** `OnExportCallStart("AppService", "getUser", paramsJson)` is called
- **THEN** a Sentry breadcrumb SHALL be added with category "fulora.bridge" and type "bridge.export.start"
- **AND** data SHALL include service name and method name

#### Scenario: Export call error captures exception

- **WHEN** `OnExportCallError("AppService", "getUser", 150, exception)` is called
- **THEN** the exception SHALL be captured via Sentry with bridge context tags
- **AND** tags SHALL include `fulora.service_name`, `fulora.method_name`, `fulora.elapsed_ms`

#### Scenario: Params capture respects CaptureBridgeParams option

- **GIVEN** `SentryFuloraOptions.CaptureBridgeParams` is `false`
- **WHEN** `OnExportCallStart` is called with paramsJson
- **THEN** the breadcrumb SHALL NOT include params data

- **GIVEN** `SentryFuloraOptions.CaptureBridgeParams` is `true`
- **WHEN** `OnExportCallStart` is called with paramsJson
- **THEN** the breadcrumb SHALL include truncated params data

### Requirement: DI registration via AddSentry extension

#### Scenario: AddSentry registers both providers

- **WHEN** `services.AddFulora().AddSentry()` is called
- **THEN** `ITelemetryProvider` SHALL resolve to `SentryTelemetryProvider`
- **AND** `IBridgeTracer` SHALL resolve to `SentryBridgeTracer`

#### Scenario: AddSentry with options configures behavior

- **WHEN** `services.AddFulora().AddSentry(o => o.CaptureBridgeParams = true)` is called
- **THEN** the `SentryBridgeTracer` SHALL capture params in breadcrumbs

### Requirement: SentryFuloraOptions defaults

#### Scenario: Default option values

- **WHEN** `SentryFuloraOptions` is created with default constructor
- **THEN** `CaptureBridgeParams` SHALL be `false`
- **AND** `MaxBreadcrumbParamsLength` SHALL be `512`
- **AND** `FlushTimeout` SHALL be `TimeSpan.FromSeconds(2)`
