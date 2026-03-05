# Hybrid E2E Testing Guide

## Architecture Overview

The hybrid E2E testing stack consists of:

- **FuloraTestApp** – Test fixture that creates a `WebViewCore` with a configurable adapter and `BridgeTestTracer`.
- **WebViewTestHandle** – Handle for interacting with the WebView (JS execution, DOM waiting, bridge observation).
- **MockWebViewAdapter** – Used in CI (especially Linux); does not execute JavaScript but supports navigation and script invocation stubs.
- **Real adapter** – On macOS/Windows, you can use the platform WebView adapter for full E2E with real JS execution.

## Writing Tests with BridgeTestTracer

`BridgeTestTracer` implements `IBridgeTracer` and records all bridge calls (export and import) for assertions. Use `app.Tracer` to simulate or observe calls:

```csharp
app.Tracer.OnExportCallStart("TodoService", "addTodo", """{"text":"Buy milk"}""");
app.Tracer.OnExportCallEnd("TodoService", "addTodo", 5, "void");
var calls = app.Tracer.GetBridgeCalls("TodoService");
Assert.Single(calls);
Assert.Equal("addTodo", calls[0].MethodName);
```

## WebViewTestHandle API Reference

| Method | Description |
|--------|-------------|
| `EvaluateJsAsync(script, ct)` | Executes JavaScript and returns the result as a string. |
| `WaitForElementAsync(selector, timeout?, ct)` | Waits until an element matching the selector exists in the DOM. |
| `WaitForBridgeReadyAsync(timeout?, ct)` | Waits until `window.__agibuild?.bridge?.ready` is true. |
| `ClickElementAsync(selector, ct)` | Clicks the element matching the selector. |
| `TypeTextAsync(selector, text, ct)` | Types text into the element (sets value and fires input/change events). |
| `GetBridgeCalls(serviceFilter?)` | Returns recorded bridge calls, optionally filtered by service name. |
| `WaitForBridgeCallAsync(service, method, timeout?, ct)` | Waits for a bridge call matching the given service and method. |

## CI Setup

- **macOS/Windows**: Use the real WebView adapter for full E2E (JS execution, DOM, bridge).
- **Linux CI**: Use `MockWebViewAdapter` via `FuloraTestApp.Create()`. JS is not executed, but bridge calls are traceable via the tracer.
- **Example xUnit test class**:

```csharp
public sealed class HybridE2ETestingTests
{
    [Fact]
    public async Task Example_FullLifecycle_create_use_dispose()
    {
        await using var app = FuloraTestApp.Create();
        var handle = app.GetWebView();
        Assert.NotNull(handle);
        // Simulate bridge activity, assert via app.Tracer
        app.Tracer.Reset();
        Assert.Empty(app.Tracer.GetBridgeCalls());
    }
}
```

Use `TestContext.Current.CancellationToken` for all async test methods that accept a `CancellationToken`.

## Limitations

- **MockWebViewAdapter** does not execute JavaScript; `InvokeScriptAsync` returns a configurable stub (e.g. `ScriptResult` or `ScriptCallback`).
- **WaitForElementAsync** and **WaitForBridgeReadyAsync** will not work with the mock adapter because they rely on JS execution returning `"true"`.
- The bridge tracer captures call metadata (service, method, params, result type) but does not execute the actual bridge logic; it records what the bridge layer reports.
