## Purpose

Define a policy-governed, platform-aware auto-update lifecycle for Fulora applications, with typed bridge service, integrity verification, and progress reporting.

## Requirements

### Requirement: IAutoUpdateService bridge interface
Runtime SHALL provide a `[JsExport]` `IAutoUpdateService` interface with methods for the full update lifecycle.

#### Scenario: CheckForUpdate returns update info when newer version exists
- **WHEN** C# or JS calls `CheckForUpdate()` and a newer version is available in the feed
- **THEN** the service SHALL return `UpdateResult` with `Status = UpdateAvailable` and populated `Update` info
- **AND** the `UpdateAvailable` bridge event SHALL be emitted

#### Scenario: CheckForUpdate returns UpToDate when current version is latest
- **WHEN** C# or JS calls `CheckForUpdate()` and no newer version exists
- **THEN** the service SHALL return `UpdateResult` with `Status = UpToDate`

#### Scenario: CheckForUpdate returns Error on failure
- **WHEN** the update feed is unreachable or returns invalid data
- **THEN** the service SHALL return `UpdateResult` with `Status = Error` and descriptive `ErrorMessage`

### Requirement: DownloadUpdate with progress and integrity verification
The service SHALL download the update package with progress reporting and verify integrity before marking as ready.

#### Scenario: DownloadUpdate reports progress via bridge event
- **WHEN** `DownloadUpdate()` is called after a successful check
- **THEN** `DownloadProgress` bridge events SHALL be emitted with `BytesDownloaded`, `TotalBytes`, and `ProgressPercent`

#### Scenario: DownloadUpdate verifies package integrity
- **WHEN** the download completes
- **THEN** the service SHALL verify the package (e.g. SHA-256 hash) before returning `ReadyToInstall`
- **AND** if verification fails, SHALL return `Status = Error` with integrity failure message

#### Scenario: DownloadUpdate fails without prior check
- **WHEN** `DownloadUpdate()` is called without a prior `CheckForUpdate()`
- **THEN** the service SHALL return `Status = Error` with descriptive message

### Requirement: ApplyUpdate triggers installation
The service SHALL apply a downloaded update, which may restart the application.

#### Scenario: ApplyUpdate succeeds after download
- **WHEN** `ApplyUpdate()` is called after a successful download
- **THEN** the platform provider SHALL apply the update (extract, replace, restart)

#### Scenario: ApplyUpdate fails without download
- **WHEN** `ApplyUpdate()` is called without a downloaded package
- **THEN** the service SHALL return `Status = Error`

### Requirement: Platform provider abstraction
The update service SHALL delegate platform-specific operations to an `IAutoUpdatePlatformProvider`.

#### Scenario: Provider handles check, download, verify, apply
- **WHEN** the auto-update service is constructed with a platform provider
- **THEN** all lifecycle operations (check, download, verify, apply) SHALL be delegated to the provider
- **AND** the service SHALL handle provider exceptions gracefully

### Requirement: Automatic periodic checks
The service SHALL support optional automatic periodic update checks.

#### Scenario: Timer triggers periodic checks
- **WHEN** `AutoUpdateOptions.CheckInterval` is set to a non-null positive value
- **THEN** the service SHALL schedule periodic `CheckForUpdate()` calls at the specified interval
- **AND** exceptions during auto-checks SHALL be swallowed to not crash the application

#### Scenario: AutoDownload triggers download after check
- **WHEN** `AutoUpdateOptions.AutoDownload` is true and `CheckForUpdate()` finds an update
- **THEN** `DownloadUpdate()` SHALL be called automatically in the background

### Requirement: DI registration
The auto-update service SHALL be registerable via the DI builder pattern.

#### Scenario: AddAutoUpdate registers service
- **WHEN** `services.AddFulora().AddAutoUpdate(options, provider)` is called
- **THEN** `IAutoUpdateService` SHALL be resolvable from the service provider
