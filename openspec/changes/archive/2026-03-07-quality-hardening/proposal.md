## Why

Before releasing a stable version, the codebase needs to pass all quality gates: full CI pipeline green, coverage thresholds met, compiler warnings reviewed, and recently added features (overlay windows, bridge reconnect, drag-drop, DevTools, biometric stubs) need integration test validation.

Traces to all Goals (G1-G4) and stabilization before Phase 12.

## What Changes

- Ensure all unit tests pass (1606+ tests)
- Ensure coverage thresholds: line ≥ 96%, branch ≥ 93%
- Review and resolve compiler warnings
- Validate CI pipeline end-to-end (all governance checks)
- Smoke-test sample apps build

## Non-goals

- Adding new features
- Refactoring existing architecture (done in Changes 1-2)
- Achieving 80% mutation score (mutation infrastructure is in place, score improvement is iterative)

## Capabilities

### New Capabilities
(none)

### Modified Capabilities
(none)

## Impact

- **Tests**: Potential test fixes/additions to restore coverage
- **Code**: Minor fixes for warnings and edge cases
- **CI**: Pipeline validation
