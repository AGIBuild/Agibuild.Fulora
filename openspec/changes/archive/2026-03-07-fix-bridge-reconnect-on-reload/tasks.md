## 1. Cache JS Stubs in RuntimeBridgeService

- [x] 1.1 Add `JsStub` field to `ExposedService` record in `RuntimeBridgeService.cs`. Pass the JS stub string when constructing `ExposedService` in both source-generated and reflection-based `Expose` paths.
- [x] 1.2 Add internal `ReinjectServiceStubs()` method to `RuntimeBridgeService` that iterates `_exportedServices` and re-invokes each cached `JsStub` via `_invokeScript`.

## 2. Re-inject Bridge Stubs on Navigation Completion

- [x] 2.1 Add internal method `ReinjectBridgeStubsIfEnabled()` to `WebViewCore` that checks `_webMessageBridgeEnabled`, injects `WebViewRpcService.JsStub` via `InvokeScriptAsync`, then calls `_bridgeService?.ReinjectServiceStubs()`.
- [x] 2.2 Call `ReinjectBridgeStubsIfEnabled()` from `CompleteActiveNavigation` when `status != Failure`.

## 3. Unit Tests

- [x] 3.1 Add test: after `EnableWebMessageBridge` + `Expose` + simulate navigation completion, verify base RPC stub and service stubs appear in captured scripts.
- [x] 3.2 Add test: failed navigation does NOT re-inject stubs.
- [x] 3.3 Add test: bridge disabled — navigation completion does NOT inject stubs.

## 4. Build & Verify

- [x] 4.1 Build entire solution, run all unit tests, verify no regressions.
