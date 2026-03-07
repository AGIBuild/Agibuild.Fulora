namespace Agibuild.Fulora.Telemetry;

/// <summary>
/// Configuration options for the Fulora Sentry telemetry integration.
/// </summary>
public sealed class SentryFuloraOptions
{
    /// <summary>
    /// Whether to include bridge method parameters in Sentry breadcrumbs.
    /// Default is <c>false</c> to prevent sensitive data leakage.
    /// </summary>
    public bool CaptureBridgeParams { get; set; }

    /// <summary>
    /// Maximum character length for parameter JSON in breadcrumbs. Longer values are truncated.
    /// Only applies when <see cref="CaptureBridgeParams"/> is <c>true</c>.
    /// </summary>
    public int MaxBreadcrumbParamsLength { get; set; } = 512;

    /// <summary>
    /// Timeout for <see cref="ITelemetryProvider.Flush"/> calls.
    /// </summary>
    public TimeSpan FlushTimeout { get; set; } = TimeSpan.FromSeconds(2);
}
