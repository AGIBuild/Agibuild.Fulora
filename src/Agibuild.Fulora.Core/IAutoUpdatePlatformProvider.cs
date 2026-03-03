namespace Agibuild.Fulora;

/// <summary>
/// Platform-specific provider for auto-update operations.
/// Implement this interface to supply update mechanics for each target platform.
/// </summary>
public interface IAutoUpdatePlatformProvider
{
    /// <summary>
    /// Fetches update metadata from the feed URL.
    /// Returns <c>null</c> if the current version is already the latest.
    /// </summary>
    Task<UpdateInfo?> CheckForUpdateAsync(AutoUpdateOptions options, string currentVersion, CancellationToken ct = default);

    /// <summary>
    /// Downloads the update package to a staging location.
    /// </summary>
    /// <param name="update">The update to download.</param>
    /// <param name="options">Update configuration.</param>
    /// <param name="onProgress">Progress callback invoked during download.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Path to the downloaded package file.</returns>
    Task<string> DownloadUpdateAsync(UpdateInfo update, AutoUpdateOptions options, Action<UpdateDownloadProgress>? onProgress = null, CancellationToken ct = default);

    /// <summary>
    /// Verifies the integrity of a downloaded update package.
    /// Returns <c>true</c> if the package is valid.
    /// </summary>
    Task<bool> VerifyPackageAsync(string packagePath, UpdateInfo update, CancellationToken ct = default);

    /// <summary>
    /// Applies the downloaded update and restarts the application.
    /// This method may not return if a restart is triggered.
    /// </summary>
    Task ApplyUpdateAsync(string packagePath, CancellationToken ct = default);

    /// <summary>
    /// Returns the currently running application version string.
    /// </summary>
    string GetCurrentVersion();
}
