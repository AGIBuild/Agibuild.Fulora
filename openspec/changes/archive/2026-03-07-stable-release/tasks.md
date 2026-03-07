## Tasks

### Task 1: Determine release version ✅
- [x] Decision: `v1.1.0` (minor, backward compatible) — confirmed by user
- [x] Verify `MinVerMinimumMajorMinor` in `Directory.Build.props` is compatible

### Task 2: Update CHANGELOG
- [x] Document adapter shared logic extraction
- [x] Document service layer refactoring
- [x] Document mutation testing infrastructure
- [x] Document quality hardening (template fixes, coverage)
- [x] Document Avalonia 12 requirement

### Task 3: Create git tag
- [ ] `git tag v1.1.0` on main branch (after commit + CI pass)
- [ ] Push tag to trigger release pipeline

### Task 4: Verify release pipeline
- [ ] CI/CiPublish workflow triggered
- [ ] NuGet packages published
- [ ] npm package published
- [ ] GitHub Release created

### Task 5: Post-release
- [ ] Verify NuGet packages are available
- [ ] Verify template references correct version
- [x] Update ROADMAP.md to reflect completion
