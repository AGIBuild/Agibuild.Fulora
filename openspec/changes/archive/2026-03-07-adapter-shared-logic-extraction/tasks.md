## Tasks

### Task 1: Add NavigationErrorCategory enum and NavigationErrorFactory ✅

**File**: `src/Agibuild.Fulora.Adapters.Abstractions/NavigationErrorFactory.cs`

- [x] Create `NavigationErrorCategory` enum: `Timeout`, `Network`, `Ssl`, `Other`
- [x] Create `NavigationErrorFactory.Create(NavigationErrorCategory, string message, Guid navigationId, Uri requestUri)` returning the corresponding `WebView*Exception`
- [x] Add unit tests covering all 4 categories

### Task 2: Add AdapterCookieParser ✅

**File**: `src/Agibuild.Fulora.Adapters.Abstractions/AdapterCookieParser.cs`

- [x] Move `ParseCookiesJson`, `ExtractJsonString`, `ExtractJsonRaw` from any of the 3 duplicated adapters (macOS/GTK/iOS) into `AdapterCookieParser` as static methods
- [x] Add unit tests: null/empty input, single cookie, multiple cookies, negative expires, special characters in values

### Task 3: Add NavigationCorrelationTracker — DEFERRED

**File**: `src/Agibuild.Fulora.Adapters.Abstractions/NavigationCorrelationTracker.cs`

> Deferred per design decision D3: navigation correlation is deeply coupled to platform-specific navigation ID types, redirect handling, and lock contention patterns. Extraction requires per-platform analysis that is out of scope for stabilization. Revisit in Phase 12.

- [ ] ~~Analyze all 5 adapters' navigation correlation implementations and identify the common state machine~~
- [ ] ~~Extract shared state: `_navLock`, `_completedNavIds`, `_pendingApiNavigationId`, navigation ID mapping~~
- [ ] ~~Implement: `BeginApiNavigation(Guid)`, `TryComplete(Guid) → bool`, `GetOrCreateCorrelationId(TKey) → Guid`~~
- [ ] ~~Add unit tests: API lifecycle, duplicate completion suppression, concurrent access, redirect correlation~~

### Task 4: Refactor Windows adapter to use shared utilities ✅

**File**: `src/Agibuild.Fulora.Adapters.Windows/WindowsWebViewAdapter.cs`

- [x] Replace `MapWebErrorStatus` with `NavigationErrorFactory.Create(category, ...)` + platform-to-category mapping
- [ ] ~~Replace navigation correlation inline code with `NavigationCorrelationTracker` instance~~ (deferred with Task 3)
- [x] Verify all existing tests still pass

### Task 5: Refactor macOS adapter to use shared utilities ✅

**File**: `src/Agibuild.Fulora.Adapters.MacOS/MacOSWebViewAdapter.PInvoke.cs`

- [x] Replace `ParseCookiesJson`/`ExtractJsonString`/`ExtractJsonRaw` with `AdapterCookieParser` calls
- [x] Replace error mapping (int status → exception) with `NavigationErrorFactory.Create(category, ...)`
- [ ] ~~Replace navigation correlation code with `NavigationCorrelationTracker`~~ (deferred with Task 3)
- [x] Verify all existing tests still pass

### Task 6: Refactor GTK adapter to use shared utilities ✅

**File**: `src/Agibuild.Fulora.Adapters.Gtk/GtkWebViewAdapter.cs`

- [x] Same changes as macOS: cookie parser, error factory ~~, navigation tracker~~ (tracker deferred)
- [x] Verify all existing tests still pass

### Task 7: Refactor iOS adapter to use shared utilities ✅

**File**: `src/Agibuild.Fulora.Adapters.iOS/iOSWebViewAdapter.cs`

- [x] Same changes as macOS: cookie parser, error factory ~~, navigation tracker~~ (tracker deferred)
- [x] Verify all existing tests still pass

### Task 8: Refactor Android adapter to use shared utilities ✅

**File**: `src/Agibuild.Fulora.Adapters.Android/AndroidWebViewAdapter.cs`

- [x] Replace error mapping with `NavigationErrorFactory.Create(category, ...)`
- [ ] ~~Replace navigation correlation code with `NavigationCorrelationTracker`~~ (deferred with Task 3)
- [x] Android does not use `AdapterCookieParser` (uses `CookieManager` string API)
- [x] Verify all existing tests still pass

### Task 9: Final verification ✅

- [x] Run `nuke UnitTests` — all 1606 pass
- [x] Run `nuke IntegrationTests` — all 209 pass
- [x] Run `nuke Coverage` — line 97.56%, branch 93.13%
- [x] Confirm no duplicated `ParseCookiesJson`/`ExtractJsonString` remain in adapter projects
- [ ] ~~Confirm no duplicated `_completedNavIds`/`BeginApiNavigation` remain in adapter projects~~ (navigation correlation not extracted — deferred)
