## Context

Before releasing a stable version, the codebase must pass all quality gates established during the project. Changes 1-3 (adapter extraction, runtime service relocation, mutation testing infrastructure) introduced structural modifications. This change validates that all quality metrics are maintained and that all sample/template projects build against the current dependency graph.

## Goals / Non-Goals

**Goals:**
- Verify all unit and integration tests pass
- Verify code coverage meets thresholds (line ≥ 96%, branch ≥ 93%)
- Review compiler warnings (no new warnings from recent changes)
- Ensure all sample/template projects build successfully with Avalonia 12.0.0-preview1

**Non-Goals:**
- Add new features or capabilities
- Achieve mutation testing score targets (infrastructure was established in Change 3; score improvement is iterative)
- Refactor or optimize existing code

## Decisions

### D1: Coverage thresholds are existing gates, not new requirements

Line coverage ≥ 96% and branch coverage ≥ 93% were pre-established by the CI pipeline. This change validates they are still met after structural changes from Changes 1-3, not setting new thresholds.

### D2: Compiler warnings reviewed, not all eliminated

Pre-existing XML doc comment warnings (5 total, CS1591/CS1572) are non-critical and present before this stabilization effort. No new warnings were introduced. Eliminating all pre-existing warnings is deferred as a separate hygiene task.

### D3: Sample Avalonia version alignment

All sample and template projects must reference Avalonia `12.0.0-preview1` to match the core framework dependency. Mixed version references (11.x with 12.x) cause `NU1605` package downgrade errors.

### D4: Web frontend build is a prerequisite, not a project issue

Samples with web frontends (`avalonia-react`, `avalonia-vue`, `avalonia-ai-chat`, `showcase-todo`) require `npm install && npm run build` before the .NET build succeeds. This is by design — the .NET project embeds pre-built web assets.

## Testing Strategy

- Run full unit test suite (1606 tests) — zero failures, zero skips
- Run full integration test suite (209 tests) — zero failures, zero skips
- Build each sample/template project individually to surface version conflicts
- Review `dotnet build` warnings output for new entries

## Risks / Trade-offs

- **[Risk]** Future Avalonia version updates will require re-aligning all sample .csproj files → **Mitigation**: Consider centralizing Avalonia version via `Directory.Packages.props` in Phase 12
- **[Trade-off]** Pre-existing warnings left unfixed → Acceptable: they are non-critical documentation warnings, not behavioral issues
