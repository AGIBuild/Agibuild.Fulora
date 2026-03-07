## Tasks

### Task 1: Move GlobalShortcutService to Runtime ✅

**Files**:
- Remove: `src/Agibuild.Fulora.Avalonia/Shell/GlobalShortcutService.cs`
- Add: `src/Agibuild.Fulora.Runtime/Shell/GlobalShortcutService.cs`

- [x] Copy `GlobalShortcutService.cs` to `Runtime/Shell/` with same namespace (`Agibuild.Fulora`)
- [x] Delete original file from `Avalonia/Shell/`
- [x] Verify `InternalsVisibleTo Include="Agibuild.Fulora"` exists in `Runtime.csproj` (needed for `WebViewShortcutRouter` access to `SuppressNextActivation`/`FindIdByChord`)
- [x] Build solution — verify 0 errors

### Task 2: Move ThemeService to Runtime ✅

**Files**:
- Remove: `src/Agibuild.Fulora.Avalonia/Shell/ThemeService.cs`
- Add: `src/Agibuild.Fulora.Runtime/Shell/ThemeService.cs`

- [x] Copy `ThemeService.cs` to `Runtime/Shell/` with same namespace (`Agibuild.Fulora.Shell`)
- [x] Delete original file from `Avalonia/Shell/`
- [x] Build solution — verify 0 errors

### Task 3: Verify test suite ✅

- [x] Run `dotnet test` — all unit tests pass (including `GlobalShortcutServiceTests`, `ThemeServiceTests`)
- [x] Run integration tests — all pass (including `GlobalShortcutServiceIntegrationTests`, `ThemeServiceIntegrationTests`)
- [x] No test code changes required (tests reference via Core interfaces)

### Task 4: Verify cross-assembly internal access ✅

- [x] `WebViewShortcutRouter` in Avalonia compiles with calls to `GlobalShortcutService.SuppressNextActivation()` and `FindIdByChord()`
- [x] Confirmed via `InternalsVisibleTo Include="Agibuild.Fulora"` in `Agibuild.Fulora.Runtime.csproj` line 28

### Task 5: Verify mutation testing scope

- [x] Confirm `GlobalShortcutService` and `ThemeService` are now in `Agibuild.Fulora.Runtime` (included in Stryker mutation scope)
- [x] Stryker found and targeted `Agibuild.Fulora.Runtime.csproj`, created 3983 mutants (includes both migrated services)
