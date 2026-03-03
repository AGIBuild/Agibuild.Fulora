namespace Agibuild.Fulora;

/// <summary>
/// Provides application auto-update lifecycle: check, download, verify, and apply updates.
/// Registered via <c>Bridge.Expose&lt;IAutoUpdateService&gt;(impl)</c>.
/// </summary>
[JsExport]
public interface IAutoUpdateService
{
    /// <summary>Checks the update feed for a newer version.</summary>
    Task<UpdateResult> CheckForUpdate();

    /// <summary>Downloads the update package identified by a previous <see cref="CheckForUpdate"/> call.</summary>
    Task<UpdateResult> DownloadUpdate();

    /// <summary>
    /// Applies the downloaded update and restarts the application.
    /// Returns <see cref="UpdateStatus.Error"/> if no update has been downloaded.
    /// </summary>
    Task<UpdateResult> ApplyUpdate();

    /// <summary>Returns the currently running application version.</summary>
    Task<string> GetCurrentVersion();

    /// <summary>Push event fired when an update check discovers a new version.</summary>
    IBridgeEvent<UpdateInfo> UpdateAvailable { get; }

    /// <summary>Push event fired during download to report progress.</summary>
    IBridgeEvent<UpdateDownloadProgress> DownloadProgress { get; }
}
