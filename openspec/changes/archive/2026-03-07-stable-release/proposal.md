## Why

The codebase has been stabilized through Changes 1-4:
- Adapter duplicate logic extracted (Change 1)
- Framework services properly layered (Change 2)
- Mutation testing infrastructure in place (Change 3)
- All quality gates passing: 1606 tests, 97.56% line / 93.13% branch coverage, 0 errors (Change 4)
- Template Avalonia version fixed to 12.0.0-preview1

The codebase is ready for a formal release. Current tags are on preview versions (latest: v1.0.12-preview). A stable release needs a deliberate version bump and changelog.

Traces to ROADMAP Phase 9 (GA Release) and stabilization track.

## What Changes

- Finalize release version number (discussion: `v1.1.0` or `v2.0.0` given Avalonia 12 upgrade)
- Update CHANGELOG.md with all changes since v1.0.0
- Tag and trigger the release pipeline
- Publish NuGet packages and npm @agibuild/bridge package

## Non-goals

- New feature development
- Phase 12 planning (done after release)

## Capabilities

### New Capabilities
(none)

### Modified Capabilities
(none)

## Impact

- **Version**: MinVer version bump via git tag
- **Packages**: NuGet + npm publication
- **Docs**: CHANGELOG update
