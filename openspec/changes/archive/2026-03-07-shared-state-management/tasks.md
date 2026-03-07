## Tasks

### Task 1: ISharedStateStore interface in Core

- [x] Create `ISharedStateStore.cs` with Set/Get/TryGet/Remove/GetSnapshot/Set<T>/Get<T>
- [x] Create `StateChangedEventArgs` record

### Task 2: SharedStateStore implementation in Runtime

- [x] Implement `SharedStateStore` with ConcurrentDictionary and LWW timestamps
- [x] Implement change notification via `StateChanged` event
- [x] Implement typed access methods
- [x] Implement snapshot support

### Task 3: DI registration

- [x] Add `AddSharedState()` extension to `FuloraServiceBuilder`

### Task 4: Unit tests

- [x] Tests for Set/Get/Remove/TryGet operations
- [x] Tests for LWW conflict resolution
- [x] Tests for StateChanged event delivery
- [x] Tests for no-op on same value
- [x] Tests for typed Get<T>/Set<T>
- [x] Tests for snapshot

### Task 5: Verification

- [x] All tests pass (21/21)
- [x] Solution builds without errors
