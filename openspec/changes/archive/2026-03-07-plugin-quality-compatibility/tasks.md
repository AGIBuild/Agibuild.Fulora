## Tasks

### Task 1: Add PluginManifest model to Core

- [x] Create `PluginManifest.cs` in `Agibuild.Fulora.Core` with STJ deserialization
- [x] Add `IsCompatibleWith(Version fuloraVersion)` method
- [x] Add unit tests for deserialization and compatibility

### Task 2: Create fulora-plugin.json for all 7 plugins

- [x] Database plugin manifest
- [x] HttpClient plugin manifest
- [x] FileSystem plugin manifest
- [x] Notifications plugin manifest
- [x] AuthToken plugin manifest
- [x] LocalStorage plugin manifest
- [x] Biometric plugin manifest
- [x] Embed manifests in .csproj as NuGet content

### Task 3: Fix LocalStorage missing fulora-plugin tag

- [x] Add `fulora-plugin` to `PackageTags` in LocalStorage `.csproj`

### Task 4: Enhance CLI list plugins --check

- [x] Add `--check` option to `fulora list plugins`
- [x] Read manifest from plugin project/NuGet cache
- [x] Report compatibility status
- [x] Add unit tests for CLI command

### Task 5: Verification

- [x] All new tests pass (14/14)
- [x] All plugins have valid fulora-plugin.json
- [x] Solution builds without errors
