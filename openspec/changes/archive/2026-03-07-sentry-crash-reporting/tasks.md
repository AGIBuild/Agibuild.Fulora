## Tasks

### Task 1: Create project and add Sentry dependency ✅

**File**: `src/Agibuild.Fulora.Telemetry.Sentry/Agibuild.Fulora.Telemetry.Sentry.csproj`

- [x] Create new project following OpenTelemetry package structure
- [x] Add `Sentry` NuGet package reference (v5.6.0)
- [x] Add project references to Core and DependencyInjection
- [x] Add project to solution

### Task 2: Implement SentryFuloraOptions ✅

**File**: `src/Agibuild.Fulora.Telemetry.Sentry/SentryFuloraOptions.cs`

- [x] Create options class with `CaptureBridgeParams`, `MaxBreadcrumbParamsLength`, `FlushTimeout`
- [x] Add unit tests for default values

### Task 3: Implement SentryTelemetryProvider ✅

**File**: `src/Agibuild.Fulora.Telemetry.Sentry/SentryTelemetryProvider.cs`

- [x] Implement `ITelemetryProvider` with Sentry `IHub` for testability
- [x] `TrackEvent` → breadcrumb with category "fulora.event"
- [x] `TrackMetric` → breadcrumb with category "fulora.metric"
- [x] `TrackException` → `CaptureException` with properties as extras
- [x] `Flush` → `FlushAsync` with configurable timeout
- [x] Add unit tests for all 4 methods

### Task 4: Implement SentryBridgeTracer ✅

**File**: `src/Agibuild.Fulora.Telemetry.Sentry/SentryBridgeTracer.cs`

- [x] Implement `IBridgeTracer` with Sentry `IHub` for testability
- [x] Export/import call start → breadcrumb with bridge context
- [x] Export call error → `CaptureException` with scope enrichment (service, method, elapsed)
- [x] Respect `CaptureBridgeParams` option for param inclusion
- [x] Truncate params to `MaxBreadcrumbParamsLength`
- [x] Add unit tests for all tracer methods

### Task 5: Implement DI extension method ✅

**File**: `src/Agibuild.Fulora.Telemetry.Sentry/FuloraSentryExtensions.cs`

- [x] `AddSentry()` extension on `FuloraServiceBuilder`
- [x] `AddSentry(Action<SentryFuloraOptions>)` overload for configuration
- [x] Register `SentryTelemetryProvider` as `ITelemetryProvider`
- [x] Register `SentryBridgeTracer` as `IBridgeTracer`
- [x] Add unit test for DI registration

### Task 6: Final verification ✅

- [x] All 27 new unit tests pass
- [x] Solution builds without errors (0 warnings, 0 errors)
- [x] Existing tests unaffected
