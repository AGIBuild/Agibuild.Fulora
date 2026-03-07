## Context

The project has 97%+ line coverage and 93%+ branch coverage, but these metrics don't measure test effectiveness. Mutation testing introduces small code changes and verifies tests detect them.

## Goals / Non-Goals

**Goals:**
- Automated mutation testing of non-UI C# code via Stryker.NET
- Global mutation score threshold of 80% (break build if below)
- Weekly scheduled CI runs + manual trigger

**Non-Goals:**
- Mutation testing for UI layer or platform adapter native interop
- Mutation testing for Bridge.Generator (Roslyn source generator)
- Immediate 80% score achievement (infrastructure first, score improvement in Change 4)

## Decisions

### D1: Stryker.NET as the mutation testing tool

Industry standard for .NET. Active development, supports .NET 10 preview, integrates with CI via JSON/HTML reports.

### D2: Single test project scope

All testable code is already exercised by `Agibuild.Fulora.UnitTests`. Stryker runs mutations on source projects and verifies via this test project. No need to configure multiple test projects.

### D3: Weekly scheduled runs, not per-PR

Mutation testing is CPU-intensive (30min-2hrs). Running per-PR would block development. Weekly schedule catches regressions; manual trigger for on-demand.

### D4: Exclude UI and native-interop projects

Platform adapters (Windows, macOS, GTK, iOS, Android) contain native interop code that requires real platform runtimes. Avalonia UI project depends on UI framework. Both are excluded.

## Testing Strategy

- Verify `dotnet stryker --config-file stryker-config.json` runs without error
- Verify HTML/JSON reports are generated in `artifacts/mutation-report/`
- Verify the `break: 80` threshold is enforced

## Risks / Trade-offs

- **[Risk]** Initial mutation score may be below 80% → **Mitigation**: First run establishes baseline; threshold enforcement deferred until Change 4 brings score above threshold. May temporarily set `break: 0` for first run.
