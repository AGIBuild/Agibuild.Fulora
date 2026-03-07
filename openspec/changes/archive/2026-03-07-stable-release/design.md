## Context

The codebase has been stabilized through 4 prior changes and is ready for a formal stable release. Current tags are on preview versions (latest: `v1.0.12-preview`). The project uses MinVer for version derivation from git tags, Nuke Build for CI automation, and GitHub Actions for the release pipeline.

## Goals / Non-Goals

**Goals:**
- Release `v1.1.0` as the first stable version post-stabilization
- Document all changes since the last release in CHANGELOG
- Trigger the automated release pipeline (NuGet + npm + GitHub Release)

**Non-Goals:**
- Introduce new features or API changes
- Modify the release pipeline or build system
- Plan Phase 12 (separate post-release activity)

## Decisions

### D1: Version number — v1.1.0

Chosen by the user. Rationale: minor version bump indicates backward-compatible improvements (adapter extraction, service relocation, mutation testing infra, quality hardening). Avalonia 12.0.0-preview1 is an internal dependency upgrade, not a public API breaking change for consumers.

### D2: MinVer tag-based versioning

The project uses [MinVer](https://github.com/adamralph/minver) to derive package versions from git tags. Creating a `v1.1.0` tag will cause all packages to be versioned `1.1.0`. No `Directory.Build.props` changes needed — MinVer reads the tag directly.

### D3: CHANGELOG scope

Document all changes from the stabilization track:
1. **Adapter shared utilities** (NavigationErrorFactory, AdapterCookieParser)
2. **Runtime service relocation** (GlobalShortcutService, ThemeService moved from UI to Runtime)
3. **Mutation testing infrastructure** (Stryker.NET integration, Nuke target, CI workflow)
4. **Quality hardening** (Avalonia 12 version alignment in samples/templates, test validation)

### D4: Release pipeline — existing workflow

The existing GitHub Actions workflow (`ci-publish.yml` or equivalent) is triggered by pushing a version tag. No pipeline modifications needed.

### D5: npm @agibuild/bridge package

The JavaScript bridge package should be published alongside the NuGet packages to maintain version parity.

## Testing Strategy

- All tests must be green before tagging (validated in Change 4)
- Post-release: verify NuGet packages are available on nuget.org
- Post-release: verify npm package is published
- Post-release: verify GitHub Release page is created with correct tag

## Risks / Trade-offs

- **[Risk]** Tag pushed before all changes are committed → **Mitigation**: Verify `git status` is clean before tagging
- **[Risk]** Pipeline failure during publish → **Mitigation**: Tags can be deleted and re-pushed; NuGet packages are unlisted (not deleted)
- **[Trade-off]** v1.1.0 vs v2.0.0 — Avalonia 12 is a preview dependency but not a public API break for this library's consumers → v1.1.0 is appropriate
