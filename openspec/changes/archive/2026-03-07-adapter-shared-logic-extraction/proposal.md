## Why

Five platform adapters (Windows, macOS, GTK, Android, iOS) each contain independent implementations of error mapping, permission mapping, cookie parsing, and navigation correlation. This violates the single-maintenance-point principle — a fix or enhancement to any shared logic must be replicated across all adapters, increasing defect risk and maintenance burden. Extracting these into `Adapters.Abstractions` enables independent unit testing (currently impossible) and prepares the codebase for mutation testing on non-UI logic.

Traces to **G4 (Contract-Driven Testability)** and the stabilization track before Phase 12.

## What Changes

- Extract duplicated error-mapping logic (platform error codes → `WebView*Exception`) from all 5 adapters into a shared `SharedErrorMapper` in `Adapters.Abstractions`
- Extract duplicated permission-mapping logic (platform permission kinds → `WebViewPermissionKind`) into a shared `SharedPermissionMapper`
- Consolidate identical `ParseCookiesJson` / `ExtractJsonString` / `ExtractJsonRaw` from macOS, GTK, and iOS adapters into a shared `CookieJsonParser`
- Extract navigation correlation state machine (API vs native navigation, redirect tracking, correlation IDs) into a shared `NavigationCorrelationTracker`
- Update all 5 adapters to delegate to shared implementations
- Add unit tests for all extracted shared logic

## Non-goals

- Changing adapter public API surface or contracts
- Modifying adapter registration or discovery mechanisms
- Altering navigation or permission behavior (pure refactor)

## Capabilities

### New Capabilities
- `adapter-shared-utilities`: Shared utility classes in Adapters.Abstractions for cross-adapter logic deduplication (error mapping, permission mapping, cookie parsing, navigation correlation)

### Modified Capabilities
- `webview-adapter-abstraction`: Add shared utility types that adapters delegate to for common cross-cutting concerns

## Impact

- **Code**: `src/Agibuild.Fulora.Adapters.Abstractions/` (new files), all 5 adapter projects (refactor to use shared logic)
- **Tests**: New unit tests for shared utilities; existing adapter tests should continue to pass unchanged
- **APIs**: No public API changes — internal refactor only
- **Dependencies**: No dependency changes — Abstractions is already a dependency of all adapters
