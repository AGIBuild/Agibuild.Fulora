namespace Agibuild.Fulora;

/// <summary>
/// A telemetry provider that forwards all calls to multiple inner providers.
/// Exceptions from one provider do not affect others.
/// </summary>
public sealed class CompositeTelemetryProvider : ITelemetryProvider
{
    private readonly ITelemetryProvider[] _providers;

    /// <summary>Creates a composite with the given providers.</summary>
    public CompositeTelemetryProvider(params ITelemetryProvider[] providers)
    {
        _providers = providers ?? [];
    }

    /// <inheritdoc />
    public void TrackEvent(string name, IDictionary<string, string>? properties = null)
    {
        foreach (var p in _providers)
        {
            try { p.TrackEvent(name, properties); }
            catch { /* swallow to not affect others */ }
        }
    }

    /// <inheritdoc />
    public void TrackMetric(string name, double value, IDictionary<string, string>? dimensions = null)
    {
        foreach (var p in _providers)
        {
            try { p.TrackMetric(name, value, dimensions); }
            catch { /* swallow to not affect others */ }
        }
    }

    /// <inheritdoc />
    public void TrackException(Exception exception, IDictionary<string, string>? properties = null)
    {
        foreach (var p in _providers)
        {
            try { p.TrackException(exception, properties); }
            catch { /* swallow to not affect others */ }
        }
    }

    /// <inheritdoc />
    public void Flush()
    {
        foreach (var p in _providers)
        {
            try { p.Flush(); }
            catch { /* swallow to not affect others */ }
        }
    }
}
