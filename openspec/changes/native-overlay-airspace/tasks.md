# Native Overlay / Airspace — Tasks

## 1. Core Overlay Infrastructure

- [x] 1.1 Create `WebViewOverlayHost` class in `Agibuild.Fulora.Avalonia`
- [x] 1.2 Add `OverlayContent` styled property to `WebView` control
- [x] 1.3 Implement overlay lifecycle: create on `OverlayContent` set, destroy on null
- [x] 1.4 CT: WebViewOverlayHost construction, Content get/set, Dispose

## 2. Position & Size Synchronization

- [x] 2.1 Implement `UpdatePosition(Rect, Point, double)` with DPI scaling
- [x] 2.2 Implement `Show()` / `Hide()` visibility toggling
- [x] 2.3 Implement DPI scaling — overlay matches WebView DPI
- [x] 2.4 Add `SyncVisibilityWith(bool isVisible)` for WebView visibility sync
- [x] 2.5 Subscribe to `LayoutUpdated` in WebView for position tracking
- [x] 2.6 CT: UpdatePosition stores DPI-scaled bounds correctly
- [x] 2.7 CT: SyncVisibilityWith shows/hides correctly
- [x] 2.8 CT: Bounds returns correct value after UpdatePosition

## 3. Input Routing

- [x] 3.1 Implement hit-test: overlay receives input → test against Avalonia visual tree
- [x] 3.2 If hit on Avalonia control → handle in overlay
- [x] 3.3 If hit on transparent area → forward input to WebView
- [x] 3.4 Handle keyboard focus routing between overlay and WebView

## 4. Windows Platform Implementation

- [x] 4.1 Create overlay as child window with `WS_EX_LAYERED | WS_EX_TOOLWINDOW`
- [x] 4.2 Use `SetLayeredWindowAttributes` for transparency
- [x] 4.3 Use `DeferWindowPos` for flicker-free position updates
- [x] 4.4 Handle `WM_NCHITTEST` for input passthrough

## 5. macOS Platform Implementation

- [x] 5.1 Create overlay as `NSPanel` with transparency
- [x] 5.2 Set panel level and parent window relationship
- [x] 5.3 Implement `ignoresMouseEvents` toggling for input passthrough

## 6. Linux Platform Implementation

- [x] 6.1 Create overlay using GTK + RGBA visual
- [x] 6.2 Position tracking via GTK signals
- [x] 6.3 Input passthrough via shape regions

## 7. Integration Tests

- [ ] 7.1 Manual IT: overlay button clickable, transparent area passes through (Windows)
- [ ] 7.2 Manual IT: same on macOS
- [ ] 7.3 Manual IT: overlay tracks WebView during resize/move
- [ ] 7.4 Manual IT: multi-monitor DPI transition
