# Native ↔ Web Drag and Drop — Tasks

## 1. Contracts & Abstractions

- [x] 1.1 Define `IDragDropAdapter` interface in `Agibuild.Fulora.Adapters.Abstractions`
- [x] 1.2 Define `DragDropPayload`, `FileDropInfo`, `DragDropEffects` types in Core
- [x] 1.3 Define `DragEventArgs`, `DropEventArgs` event types
- [x] 1.4 Add `IDragDropAdapter` as optional adapter facet
- [x] 1.5 CT: DragDropPayload, FileDropInfo, DragEventArgs tests
- [x] 1.6 CT: DragDropEffects flags combine correctly
- [x] 1.7 Add `CreateWithDragDrop()` mock adapter variant

## 2. WebViewCore Integration

- [x] 2.1 Add drag-drop event forwarding in `WebViewCore` — subscribe to adapter events
- [x] 2.2 Expose `DragEntered`, `DragOver`, `DragLeft`, `DropCompleted` events on `WebViewCore`
- [x] 2.3 Wire to `WebView.cs` Avalonia control
- [x] 2.4 CT: WebViewCore fires DragEntered when adapter raises
- [x] 2.5 CT: WebViewCore fires DropCompleted when adapter raises
- [x] 2.6 CT: Drag events fire without subscribers
- [x] 2.7 CT: WebViewCore without drag adapter does not crash

## 3. Windows Adapter (Phase 1: IDropTarget)

- [x] 3.1 Spike: validate `IDropTarget`/`RegisterDragDrop` compatibility with WebView2
- [x] 3.2 Implement `IDropTarget` on the WebView2 HWND
- [x] 3.3 Extract `IDataObject` contents: files, text, HTML
- [x] 3.4 Map to `DragDropPayload` and raise adapter events

## 4. macOS Adapter

- [x] 4.1 Extend `WkWebViewShim.mm`: register for dragged types
- [x] 4.2 Implement `NSDraggingDestination` methods
- [x] 4.3 Extract file URLs and text from pasteboard
- [x] 4.4 Forward to C# adapter

## 5. Bridge Service

- [x] 5.1 Define `IDragDropBridgeService` (`[JsExport]`) with event-based API
- [x] 5.2 Implement `DragDropBridgeService` consuming `WebViewCore` drag events
- [x] 5.3 Add JS helpers to `@agibuild/bridge`
- [x] 5.4 Add TypeScript types

## 6. Tests

- [x] 6.1 CT: Windows `IDropTarget` mock with file data → payload extraction
- [x] 6.2 CT: macOS pasteboard mock → payload extraction
- [x] 6.3 CT: Bridge service delivers events to JS handler
- [ ] 6.4 Manual IT: drag file from Finder/Explorer onto WebView
