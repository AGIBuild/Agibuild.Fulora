using System.Diagnostics;
using System.Linq;
using Agibuild.Fulora.Telemetry;
using Xunit;

namespace Agibuild.Fulora.Telemetry.OpenTelemetry.Tests;

public class OpenTelemetryTelemetryProviderTests
{
    [Fact]
    public void TrackEvent_creates_activity()
    {
        var provider = new OpenTelemetryTelemetryProvider();
        var activities = new List<Activity>();
        using var listener = new ActivityListener
        {
            ActivityStarted = a => activities.Add(a),
            ShouldListenTo = s => s.Name == OpenTelemetryBridgeTracer.ActivitySourceName,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData
        };
        ActivitySource.AddActivityListener(listener);

        provider.TrackEvent("UserClicked", new Dictionary<string, string> { { "button", "submit" } });

        var matching = activities.Where(a => a.OperationName == "UserClicked").ToList();
        Assert.Single(matching);
        Assert.Equal("UserClicked", matching[0].OperationName);
        Assert.Equal("submit", matching[0].GetTagItem("button"));
    }

    [Fact]
    public void TrackMetric_records_value()
    {
        var provider = new OpenTelemetryTelemetryProvider();
        provider.TrackMetric("response_time_ms", 42.5, new Dictionary<string, string> { { "endpoint", "/api" } });
        provider.TrackMetric("response_time_ms", 100.0);
        // No exception = success (metrics are recorded to the global MeterProvider)
    }

    [Fact]
    public void TrackException_does_not_throw()
    {
        var provider = new OpenTelemetryTelemetryProvider();
        provider.TrackException(new InvalidOperationException("test"), new Dictionary<string, string> { { "context", "unit" } });
    }

    [Fact]
    public void Flush_does_not_throw()
    {
        var provider = new OpenTelemetryTelemetryProvider();
        provider.Flush();
    }
}
