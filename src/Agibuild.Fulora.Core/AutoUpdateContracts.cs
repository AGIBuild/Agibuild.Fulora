namespace Agibuild.Fulora;

/// <summary>
/// Describes an available application update.
/// </summary>
public sealed class UpdateInfo
{
    /// <summary>Version string of the available update (e.g. "1.2.0").</summary>
    public required string Version { get; init; }

    /// <summary>Release notes or changelog summary, or <c>null</c> if unavailable.</summary>
    public string? ReleaseNotes { get; init; }

    /// <summary>Download URL for the update package.</summary>
    public required string DownloadUrl { get; init; }

    /// <summary>Size in bytes of the update package, or <c>null</c> if unknown.</summary>
    public long? SizeBytes { get; init; }

    /// <summary>Whether this update is mandatory (user cannot skip).</summary>
    public bool IsMandatory { get; init; }

    /// <summary>SHA-256 hash of the update package for integrity verification, or <c>null</c> if unavailable.</summary>
    public string? Sha256 { get; init; }
}

/// <summary>
/// Result of an update check or lifecycle operation.
/// </summary>
public sealed class UpdateResult
{
    /// <summary>Operation status.</summary>
    public required UpdateStatus Status { get; init; }

    /// <summary>Information about the available update, or <c>null</c> when no update is available.</summary>
    public UpdateInfo? Update { get; init; }

    /// <summary>Error message when <see cref="Status"/> is <see cref="UpdateStatus.Error"/>.</summary>
    public string? ErrorMessage { get; init; }
}

/// <summary>
/// Status of an auto-update operation.
/// </summary>
public enum UpdateStatus
{
    /// <summary>No update available; current version is the latest.</summary>
    UpToDate,
    /// <summary>An update is available for download.</summary>
    UpdateAvailable,
    /// <summary>Update download is in progress.</summary>
    Downloading,
    /// <summary>Update has been downloaded and is ready to apply.</summary>
    ReadyToInstall,
    /// <summary>The operation failed.</summary>
    Error
}

/// <summary>
/// Progress information for an ongoing download.
/// </summary>
public sealed class UpdateDownloadProgress
{
    /// <summary>Bytes downloaded so far.</summary>
    public long BytesDownloaded { get; init; }

    /// <summary>Total bytes to download, or <c>null</c> if unknown.</summary>
    public long? TotalBytes { get; init; }

    /// <summary>Download progress as a percentage (0–100), or <c>null</c> if total is unknown.</summary>
    public double? ProgressPercent { get; init; }
}

/// <summary>
/// Configuration options for the auto-update service.
/// </summary>
public sealed class AutoUpdateOptions
{
    /// <summary>Base URL of the update feed endpoint.</summary>
    public required string FeedUrl { get; init; }

    /// <summary>
    /// Interval between automatic update checks. Set to <c>null</c> to disable automatic checks.
    /// Default is 1 hour.
    /// </summary>
    public TimeSpan? CheckInterval { get; init; } = TimeSpan.FromHours(1);

    /// <summary>Whether to automatically download updates after detection. Default <c>false</c>.</summary>
    public bool AutoDownload { get; init; }

    /// <summary>
    /// Custom HTTP headers included in update feed requests (e.g. authorization tokens).
    /// </summary>
    public IDictionary<string, string>? Headers { get; init; }
}
