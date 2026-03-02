namespace Agibuild.Fulora;

/// <summary>
/// Provides telemetry and crash reporting capabilities.
/// </summary>
public interface ITelemetryProvider
{
    /// <summary>Tracks a named event with optional properties.</summary>
    void TrackEvent(string name, IDictionary<string, string>? properties = null);

    /// <summary>Tracks a metric with optional dimensions.</summary>
    void TrackMetric(string name, double value, IDictionary<string, string>? dimensions = null);

    /// <summary>Tracks an exception with optional properties.</summary>
    void TrackException(Exception exception, IDictionary<string, string>? properties = null);

    /// <summary>Flushes any buffered telemetry.</summary>
    void Flush();
}

/// <summary>
/// A no-op telemetry provider that discards all events.
/// </summary>
public sealed class NullTelemetryProvider : ITelemetryProvider
{
    /// <summary>Singleton instance.</summary>
    public static readonly NullTelemetryProvider Instance = new();

    /// <inheritdoc />
    public void TrackEvent(string name, IDictionary<string, string>? properties = null) { }

    /// <inheritdoc />
    public void TrackMetric(string name, double value, IDictionary<string, string>? dimensions = null) { }

    /// <inheritdoc />
    public void TrackException(Exception exception, IDictionary<string, string>? properties = null) { }

    /// <inheritdoc />
    public void Flush() { }
}
