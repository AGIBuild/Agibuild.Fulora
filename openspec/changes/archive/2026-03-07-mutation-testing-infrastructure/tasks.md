## Tasks

### Task 1: Install Stryker.NET ✅
- [x] `dotnet tool install dotnet-stryker --local` → v4.13.0
- [x] `.config/dotnet-tools.json` updated

### Task 2: Create Stryker configuration ✅
- [x] `stryker-config.json` at repo root
- [x] Configured for Core project (expandable to other projects via `--project` flag)
- [x] Thresholds: high=90, low=80, break=0 (for initial baseline)
- [x] Reporters: html, json, progress, cleartext

### Task 3: Add Nuke MutationTest target ✅
- [x] `MutationTest` target in `Build.Testing.cs`
- [x] Runs from test project directory for proper project discovery
- [x] Build compiles clean

### Task 4: Add CI workflow ✅
- [x] `.github/workflows/mutation-testing.yml`
- [x] Weekly Monday 4am UTC schedule + manual trigger
- [x] Runs from test project directory
- [x] Uploads reports as artifacts

### Task 5: Run initial baseline ✅
- [x] Core project: **7.53% mutation score** (3 killed, 86 survived, 4 timeout)
- [x] Low score expected: Core is primarily interfaces, contracts, and data types
- [x] HTML + JSON reports generated at `artifacts/mutation-report/`
- [x] Infrastructure validated: Stryker runs end-to-end successfully
- [x] Break threshold set to 0 for initial period; to be raised after test hardening
