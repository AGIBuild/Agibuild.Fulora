## Why

Multi-WebView apps (split panels, popup dialogs) need shared state without manual synchronization. Currently each WebView has its own JS context with no shared state mechanism. Developers must hand-roll message bus publish/subscribe to keep state in sync, which is error-prone.

Traces to ROADMAP Phase 12 M12.2.

## What Changes

- Add `ISharedStateStore` interface in Core: reactive key-value store with change notifications
- Implement `SharedStateStore` in Runtime using last-writer-wins conflict resolution
- Integrate with `IWebViewMessageBus` for automatic cross-WebView state synchronization
- Provide DI registration via `AddSharedState()` extension
- Full unit test coverage

## Non-goals

- JS-side API (belongs to separate cross-webview-bridge change)
- CRDT-based merge (overkill for typical hybrid app scenarios)
- Persistent state across app restarts (local storage plugin handles that)

## Capabilities

### New Capabilities
- `shared-state`: Cross-WebView reactive state store with LWW conflict resolution

### Modified Capabilities
- `dependency-injection`: New `AddSharedState()` extension method

## Impact

- **Code**: New `ISharedStateStore` in Core, `SharedStateStore` in Runtime, DI extension
- **Tests**: Comprehensive unit tests for store operations, conflict resolution, notifications
- **Packages**: Runtime and DI packages updated
