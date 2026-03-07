## Purpose

Define requirements for the cross-WebView shared state store.

## Requirements

### Requirement: Key-value state operations

#### Scenario: Set and Get value
- **WHEN** `Set("theme", "{\"dark\":true}")` is called
- **THEN** `Get("theme")` SHALL return `"{\"dark\":true}"`

#### Scenario: Get non-existent key returns null
- **WHEN** `Get("missing")` is called for a key that has not been set
- **THEN** it SHALL return `null`

#### Scenario: TryGet existing key returns true
- **WHEN** `TryGet("theme", out var value)` is called for an existing key
- **THEN** it SHALL return `true` with the value

#### Scenario: Remove key
- **WHEN** `Remove("theme")` is called for an existing key
- **THEN** `Get("theme")` SHALL return `null`
- **AND** a `StateChanged` event SHALL fire with new value `null`

### Requirement: Last-writer-wins conflict resolution

#### Scenario: Later write wins
- **GIVEN** key "x" is set with timestamp T1
- **WHEN** key "x" is set again with timestamp T2 > T1
- **THEN** the value from T2 SHALL be stored

#### Scenario: Stale write rejected
- **GIVEN** key "x" is set with timestamp T2
- **WHEN** a delayed write arrives for key "x" with timestamp T1 < T2
- **THEN** the T2 value SHALL be retained (stale write ignored)

### Requirement: Change notifications

#### Scenario: StateChanged fires on Set
- **WHEN** `Set("key", "value")` is called
- **THEN** the `StateChanged` event SHALL fire with key, old value, and new value

#### Scenario: StateChanged fires on Remove
- **WHEN** `Remove("key")` is called for an existing key
- **THEN** the `StateChanged` event SHALL fire with key, old value, and null new value

#### Scenario: No event for same value
- **GIVEN** key "x" has value "v"
- **WHEN** `Set("x", "v")` is called with the same value
- **THEN** the `StateChanged` event SHALL NOT fire

### Requirement: Typed access

#### Scenario: Set<T> and Get<T> with complex object
- **WHEN** `Set<UserPrefs>("prefs", new UserPrefs { Theme = "dark" })` is called
- **THEN** `Get<UserPrefs>("prefs")` SHALL return a deserialized `UserPrefs` with `Theme == "dark"`

### Requirement: Snapshot

#### Scenario: GetSnapshot returns current state
- **GIVEN** keys "a", "b", "c" have been set
- **WHEN** `GetSnapshot()` is called
- **THEN** it SHALL return a read-only dictionary with all 3 entries
