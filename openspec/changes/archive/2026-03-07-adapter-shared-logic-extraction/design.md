## Context

Five platform adapters implement cross-cutting concerns independently. After code audit:

- **Error mapping**: All 5 adapters map platform-specific error codes to 4 exception types (`WebViewTimeoutException`, `WebViewNetworkException`, `WebViewSslException`, `WebViewNavigationException`). Input types differ per platform (Windows: `CoreWebView2WebErrorStatus` enum; macOS/iOS/GTK: int status codes; Android: `ClientError` enum), but the output pattern is identical.
- **Cookie JSON parsing**: macOS, GTK, and iOS share byte-identical `ParseCookiesJson`/`ExtractJsonString`/`ExtractJsonRaw` (manual JSON parser). Windows uses WebView2 COM API; Android uses `CookieManager` string format. Only 3 of 5 adapters share this.
- **Navigation correlation**: All 5 adapters implement `_navLock`, `_completedNavIds`, `BeginApiNavigation`, `GetOrCreateNativeCorrelationId` with the same state machine pattern for API-vs-native navigation tracking and redirect correlation.
- **Permission mapping**: All 5 adapters map platform permission kinds to `WebViewPermissionKind`. Input types differ; output enum is shared.

## Goals / Non-Goals

**Goals:**
- Single maintenance point for each cross-cutting concern
- Enable unit testing of shared logic (currently embedded in untestable adapter code)
- Prepare clean boundaries for mutation testing scope

**Non-Goals:**
- Change adapter public API or observable behavior
- Introduce new abstractions or interfaces for consumers
- Modify native shim code (Objective-C, C, Java)

## Decisions

### D1: Error category intermediate enum

Each adapter maps platform codes to a `NavigationErrorCategory` enum (`Timeout | Network | Ssl | Other`). A shared `NavigationErrorFactory.Create(category, message, navId, requestUri)` produces the correct exception.

**Why not a single mapping function?** Platform error code types are incompatible — Windows uses a COM enum, macOS uses ints, Android uses a Java-bridged enum. The 2-step approach (platform → category → exception) gives single maintenance for the category→exception mapping while keeping platform-specific code in each adapter.

### D2: Cookie parsing as static utility class

`AdapterCookieParser.ParseCookiesJson(string json)` in Abstractions. Only macOS, GTK, and iOS use it. Windows and Android are unaffected.

### D3: Navigation correlation tracker as composable component

`NavigationCorrelationTracker` class encapsulating `_navLock`, `_completedNavIds`, `_navigationIdMap`, `BeginApiNavigation()`, `TryMapAndCompleteNavigation()`. Each adapter creates an instance and delegates to it, removing ~50-80 lines of duplicated state machine code per adapter.

**Alternative considered:** Base class with shared logic. Rejected because adapters don't share a common base (different constructor signatures, platform initialization patterns). Composition over inheritance.

### D4: Permission mapping stays per-adapter

Permission mapping is ~10 lines per adapter with platform-specific input types. Extracting it would require either generics or object boxing, adding complexity for minimal deduplication. The maintenance burden is low. **Keep as-is.**

## Testing Strategy

- **Unit tests for `NavigationErrorFactory`**: All 4 categories → correct exception types
- **Unit tests for `AdapterCookieParser`**: Empty JSON, single cookie, multiple cookies, malformed JSON, special characters, edge cases
- **Unit tests for `NavigationCorrelationTracker`**: API navigation lifecycle, native navigation, redirect correlation, duplicate completion guard, concurrent access
- **Existing adapter integration tests**: Must continue passing unchanged (pure refactor)

## Risks / Trade-offs

- **[Risk]** NavigationCorrelationTracker may have subtle per-platform differences → **Mitigation**: Compare all 5 adapter implementations line-by-line before extracting; flag any platform-specific branches
- **[Risk]** Cookie parser changes could affect all 3 adapters at once → **Mitigation**: This is the goal; single fix point is the benefit, not the risk. Comprehensive unit tests prevent regressions.
