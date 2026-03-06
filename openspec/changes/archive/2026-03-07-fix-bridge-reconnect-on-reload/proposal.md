## Why

After a page reload (right-click → Reload or programmatic navigation), the WebView's JavaScript context is replaced. The bridge JS stub (`window.agWebView`) and all service stubs are lost, but `WebViewCore` does not re-inject them on `NavigationCompleted`. The frontend polls `window.agWebView.rpc` forever and displays "Connecting to bridge..." indefinitely. This is a critical usability bug — any accidental reload bricks the app until restart.

Aligns with **G1 (Type-Safe Bridge)** reliability and **E2 (Developer Experience)** — a bridge that silently breaks on reload violates developer expectations. Related ROADMAP item: Preload Script support (F4) is a long-term solution, but this fix addresses the immediate regression in the current bridge lifecycle.

## What Changes

- Re-inject the base RPC JS stub (`WebViewRpcService.JsStub`) on every successful `NavigationCompleted` when the bridge is enabled.
- Re-inject all exposed service JS stubs after the base stub, so `window.agWebView.bridge.<Service>` is available again.
- Add an internal method on `RuntimeBridgeService` to replay service stub injection for all currently-exposed services.
- Add unit tests covering the reload scenario (navigate → verify bridge → navigate again → verify bridge reconnects).

## Non-goals

- Preload script integration (F4) — that's a separate, larger feature.
- Reconnecting C#-side handler registrations — they already survive reload.
- Handling cross-origin navigation policy changes on reload.

## Capabilities

### New Capabilities

- `bridge-reconnect-on-reload`: Re-inject bridge JS stubs on page reload/navigation so the frontend can reconnect without app restart.

### Modified Capabilities

- `js-csharp-rpc`: The RPC lifecycle now includes re-injection on navigation; NavigationCompleted triggers stub replay.

## Impact

- `src/Agibuild.Fulora.Runtime/WebViewCore.cs` — `OnAdapterNavigationCompletedOnUiThread` gains stub re-injection logic.
- `src/Agibuild.Fulora.Runtime/RuntimeBridgeService.cs` — new `ReinjectServiceStubs()` method.
- Unit tests in `tests/Agibuild.Fulora.UnitTests/` — new reload-scenario tests.
- No breaking API changes. No new dependencies.
