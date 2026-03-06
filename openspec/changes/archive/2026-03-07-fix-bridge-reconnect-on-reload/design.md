## Context

When a WebView page reloads (right-click → Reload, programmatic navigation, or browser-level refresh), the JavaScript execution context is destroyed and recreated. The bridge JS stub (`window.agWebView`) and all service stubs (`window.agWebView.bridge.<Service>`) are lost. However, the C# side — `WebViewRpcService` handlers, `RuntimeBridgeService._exportedServices` — survives intact. The gap: `WebViewCore` never re-injects the JS stubs after `NavigationCompleted`, leaving the frontend in an infinite "Connecting to bridge..." poll.

This aligns with G1 (Type-Safe Bridge) reliability and E2 (Developer Experience). The architecture doc specifies contract-driven and MockAdapter-testable design.

## Goals / Non-Goals

**Goals:**
- After any successful navigation completion, if the bridge is enabled, re-inject the base RPC JS stub and all exposed service JS stubs.
- Maintain a single source of truth for stub injection — no duplicate logic.
- Cover the fix with unit tests using MockAdapter (no real browser needed).

**Non-Goals:**
- Preload scripts (F4) — separate feature for document-start injection.
- Handling in-flight RPC calls during reload (existing CTS-based cancellation covers this).
- Cross-origin policy changes on reload.

## Decisions

### D1: Cache JS stubs in ExposedService record

**Decision**: Store the JS stub string in the `ExposedService` record at `Expose()` time, alongside existing metadata.

**Rationale**: Avoids re-generating stubs (which for source-generated services would require storing a reference to the `IBridgeRegistration`; for reflection would need `Type` and `[DynamicallyAccessedMembers]`). A string cache is simple, immutable, and allocation-free at reinject time.

**Alternative considered**: Store `IBridgeRegistration` / `Type` and regenerate on demand — more complex, no benefit since stubs don't change post-exposure.

### D2: Add `ReinjectServiceStubs()` to RuntimeBridgeService

**Decision**: Add an internal method `ReinjectServiceStubs()` that iterates `_exportedServices` and re-invokes each cached JS stub.

**Rationale**: Single ownership — only `RuntimeBridgeService` knows about exposed services. The method is callable from `WebViewCore` where navigation events occur.

### D3: Reinject in `CompleteActiveNavigation` on success

**Decision**: In `WebViewCore.CompleteActiveNavigation`, after a successful navigation when `_webMessageBridgeEnabled` is true, inject the base RPC stub then call `_bridgeService.ReinjectServiceStubs()`.

**Rationale**: `CompleteActiveNavigation` is the single funnel point for all navigation outcomes (programmatic, native, reload, forward/back). Injecting here guarantees every successful page load gets the stubs. Guard on `_webMessageBridgeEnabled` ensures no injection when bridge is disabled.

**Alternative considered**: Hook into `OnAdapterNavigationCompletedOnUiThread` — but `CompleteActiveNavigation` is the canonical single-writer for navigation outcomes. Also considered a separate event handler, but that adds indirection without benefit.

### D4: Expose a `ReinjectBridgeStubs` internal method on WebViewCore

**Decision**: Rather than putting all logic inline in `CompleteActiveNavigation`, expose an `internal` method `ReinjectBridgeStubsIfEnabled()` that encapsulates the check + injection. This keeps `CompleteActiveNavigation` clean and enables direct unit testing.

## Risks / Trade-offs

- **[Risk] Double injection on initial load**: The first `EnableWebMessageBridge` + `Expose` will inject stubs, then `CompleteActiveNavigation` may inject again if navigation completes after bridge is enabled. → **Mitigation**: JS stubs are idempotent — re-assigning `window.agWebView` and `window.agWebView.bridge.<Service>` is harmless. The base stub uses `window.agWebView = window.agWebView || {}` pattern which is safe.
- **[Risk] Ordering**: Base RPC stub must be injected before service stubs. → **Mitigation**: `ReinjectBridgeStubsIfEnabled` injects base stub first, then calls `ReinjectServiceStubs`.
- **[Risk] Concurrency**: `_exportedServices` is a `ConcurrentDictionary`, so iteration during `ReinjectServiceStubs` is thread-safe. `InvokeScriptAsync` is queued through the adapter's operation queue.

## Testing Strategy

- **CT (Unit Tests)**: Use `MockWebViewAdapter` to simulate navigation → completion → verify JS stub scripts are re-invoked in captured scripts. Verify both base stub and service stubs appear after navigation completion.
- **No IT changes needed**: The fix is internal plumbing; integration tests already exercise the full navigation flow.
