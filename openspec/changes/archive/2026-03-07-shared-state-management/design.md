## Context

The existing `IWebViewMessageBus` provides topic-based pub/sub for cross-WebView messaging. M12.2 builds a higher-level abstraction: a shared key-value state store that automatically syncs across all WebView consumers.

## Decisions

### D1: Interface in Core, implementation in Runtime

`ISharedStateStore` lives in `Agibuild.Fulora.Core`. `SharedStateStore` lives in `Agibuild.Fulora.Runtime` alongside `WebViewMessageBus`. This follows the existing Core/Runtime layering.

### D2: Last-Writer-Wins (LWW) conflict resolution

Each value carries a `DateTimeOffset` timestamp. When concurrent writes occur, the one with the later timestamp wins. This is deterministic, simple, and sufficient for typical hybrid app state (theme, user preferences, navigation context). No vector clocks or CRDTs needed.

### D3: Change notifications via events

`ISharedStateStore` exposes `StateChanged` event with `StateChangedEventArgs` (key, old value, new value, source). Subscribers receive notifications synchronously on the writing thread. For cross-WebView delivery, the store internally publishes to `IWebViewMessageBus` on a reserved topic `__fulora:state`.

### D4: String key, JSON string value

Keys are non-null strings. Values are nullable JSON strings. This keeps the store transport-agnostic and matches the message bus payload format. Typed access is provided via generic `Get<T>` / `Set<T>` convenience methods using STJ.

### D5: Singleton lifetime

The store is registered as singleton via `AddSharedState()`, same as `IWebViewMessageBus`. All WebViews share the same store instance.

### D6: Snapshot support

`GetSnapshot()` returns an immutable dictionary of the current state. Useful for initializing new WebViews that join after state has been established.

## Testing Strategy

- Unit tests for Set/Get/Remove/TryGet operations
- Unit tests for LWW conflict resolution (later timestamp wins)
- Unit tests for change notification delivery
- Unit tests for snapshot consistency
- Unit tests for typed Get<T>/Set<T> methods
