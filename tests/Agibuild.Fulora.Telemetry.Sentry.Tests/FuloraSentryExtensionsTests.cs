using Agibuild.Fulora.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Agibuild.Fulora.Telemetry.Sentry.Tests;

public class FuloraSentryExtensionsTests
{
    [Fact]
    public void AddSentry_registers_telemetry_provider()
    {
        var services = new ServiceCollection();
        services.AddFulora().AddSentry();
        var sp = services.BuildServiceProvider();

        var provider = sp.GetRequiredService<ITelemetryProvider>();
        Assert.IsType<SentryTelemetryProvider>(provider);
    }

    [Fact]
    public void AddSentry_registers_bridge_tracer()
    {
        var services = new ServiceCollection();
        services.AddFulora().AddSentry();
        var sp = services.BuildServiceProvider();

        var tracer = sp.GetRequiredService<IBridgeTracer>();
        Assert.IsType<SentryBridgeTracer>(tracer);
    }

    [Fact]
    public void AddSentry_with_options_configures_tracer()
    {
        var services = new ServiceCollection();
        services.AddFulora().AddSentry(o => o.CaptureBridgeParams = true);
        var sp = services.BuildServiceProvider();

        var tracer = sp.GetRequiredService<IBridgeTracer>();
        Assert.IsType<SentryBridgeTracer>(tracer);
    }

    [Fact]
    public void AddSentry_throws_on_null_builder()
    {
        Assert.Throws<ArgumentNullException>(() =>
            FuloraSentryExtensions.AddSentry(null!));
    }

    [Fact]
    public void AddSentry_throws_on_null_configure()
    {
        var services = new ServiceCollection();
        var builder = services.AddFulora();
        Assert.Throws<ArgumentNullException>(() =>
            builder.AddSentry(null!));
    }
}
