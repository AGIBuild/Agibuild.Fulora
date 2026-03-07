## Purpose

Define shared utility types in `Adapters.Abstractions` that eliminate duplicated cross-cutting logic across platform adapters: error-to-exception mapping, cookie JSON parsing, and navigation correlation state tracking.

## Requirements

### Requirement: NavigationErrorFactory maps error categories to typed exceptions

`NavigationErrorFactory` SHALL provide a static method that accepts a `NavigationErrorCategory` enum value, a message string, a navigation ID, and a request URI, and returns the corresponding typed exception.

#### Scenario: Timeout category produces WebViewTimeoutException

- **GIVEN** a `NavigationErrorCategory.Timeout` value
- **WHEN** `NavigationErrorFactory.Create(category, message, navId, uri)` is called
- **THEN** the result SHALL be a `WebViewTimeoutException` with the provided message, navigation ID, and URI

#### Scenario: Network category produces WebViewNetworkException

- **GIVEN** a `NavigationErrorCategory.Network` value
- **WHEN** `NavigationErrorFactory.Create(...)` is called
- **THEN** the result SHALL be a `WebViewNetworkException`

#### Scenario: Ssl category produces WebViewSslException

- **GIVEN** a `NavigationErrorCategory.Ssl` value
- **WHEN** `NavigationErrorFactory.Create(...)` is called
- **THEN** the result SHALL be a `WebViewSslException`

#### Scenario: Other category produces WebViewNavigationException

- **GIVEN** a `NavigationErrorCategory.Other` value
- **WHEN** `NavigationErrorFactory.Create(...)` is called
- **THEN** the result SHALL be a `WebViewNavigationException`

### Requirement: AdapterCookieParser parses native cookie JSON arrays

`AdapterCookieParser.ParseCookiesJson(string json)` SHALL parse the JSON array format produced by macOS/GTK/iOS native shims and return a list of `WebViewCookie` instances.

#### Scenario: Empty or null input returns empty list

- **WHEN** input is `null`, empty string, or `"[]"`
- **THEN** result SHALL be an empty list

#### Scenario: Valid single-cookie JSON

- **WHEN** input is a JSON array with one cookie object containing name, value, domain, path, expires, isSecure, isHttpOnly
- **THEN** result SHALL contain exactly one `WebViewCookie` with all fields correctly populated

#### Scenario: Expires field with valid unix timestamp

- **WHEN** a cookie has `"expires": 1700000000.000`
- **THEN** the parsed `WebViewCookie.Expires` SHALL be a `DateTimeOffset` corresponding to that unix timestamp

#### Scenario: Expires field with negative value means no expiry

- **WHEN** a cookie has `"expires": -1.0`
- **THEN** the parsed `WebViewCookie.Expires` SHALL be `null`

### Requirement: NavigationCorrelationTracker manages navigation lifecycle state

`NavigationCorrelationTracker` SHALL encapsulate the state machine for correlating API-initiated navigations with platform-reported navigation events, deduplicating completion notifications, and tracking redirects.

#### Scenario: API navigation registers correlation ID

- **WHEN** `BeginApiNavigation(navigationId)` is called
- **THEN** the tracker SHALL record the navigation ID as pending

#### Scenario: Duplicate completion is suppressed

- **WHEN** a navigation ID has already been completed
- **AND** `TryComplete(navigationId)` is called again
- **THEN** the second call SHALL return `false`

#### Scenario: Platform navigation gets or creates correlation ID

- **WHEN** a platform-reported navigation event arrives with a platform-specific key
- **THEN** `GetOrCreateCorrelationId(platformKey)` SHALL return the existing API navigation ID if one is pending, or create a new one
