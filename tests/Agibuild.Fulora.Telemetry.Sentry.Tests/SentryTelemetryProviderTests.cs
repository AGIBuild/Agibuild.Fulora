using Sentry;
using Xunit;

namespace Agibuild.Fulora.Telemetry.Sentry.Tests;

public class SentryTelemetryProviderTests
{
    private readonly RecordingHub _hub = new();
    private readonly SentryFuloraOptions _options = new();

    private SentryTelemetryProvider CreateProvider() => new(_hub, _options);

    [Fact]
    public void TrackEvent_adds_breadcrumb_with_event_category()
    {
        var provider = CreateProvider();
        var props = new Dictionary<string, string> { ["button"] = "submit" };

        provider.TrackEvent("user.login", props);

        var crumbs = _hub.GetScopeBreadcrumbs();
        Assert.Single(crumbs);
        Assert.Equal("user.login", crumbs[0].Message);
        Assert.Equal("fulora.event", crumbs[0].Category);
    }

    [Fact]
    public void TrackMetric_adds_breadcrumb_with_metric_category()
    {
        var provider = CreateProvider();
        var dims = new Dictionary<string, string> { ["endpoint"] = "/api" };

        provider.TrackMetric("latency_ms", 42.5, dims);

        var crumbs = _hub.GetScopeBreadcrumbs();
        Assert.Single(crumbs);
        Assert.Equal("latency_ms", crumbs[0].Message);
        Assert.Equal("fulora.metric", crumbs[0].Category);
    }

    [Fact]
    public void TrackException_captures_sentry_event()
    {
        var provider = CreateProvider();
        var ex = new InvalidOperationException("test error");

        provider.TrackException(ex, new Dictionary<string, string> { ["ctx"] = "unit" });

        Assert.Single(_hub.CapturedExceptions);
        Assert.Same(ex, _hub.CapturedExceptions[0].Exception);
    }

    [Fact]
    public void TrackException_with_null_properties_does_not_throw()
    {
        var provider = CreateProvider();
        provider.TrackException(new InvalidOperationException("test"));
        Assert.Single(_hub.CapturedExceptions);
    }

    [Fact]
    public void Flush_calls_hub_flush()
    {
        var provider = CreateProvider();

        provider.Flush();

        Assert.Equal(1, _hub.FlushCount);
    }

    [Fact]
    public void Constructor_throws_on_null_hub()
    {
        Assert.Throws<ArgumentNullException>(() => new SentryTelemetryProvider(null!, _options));
    }

    [Fact]
    public void Constructor_throws_on_null_options()
    {
        Assert.Throws<ArgumentNullException>(() => new SentryTelemetryProvider(_hub, null!));
    }
}
