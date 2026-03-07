using Xunit;

namespace Agibuild.Fulora.Telemetry.Sentry.Tests;

public class SentryFuloraOptionsTests
{
    [Fact]
    public void Default_CaptureBridgeParams_is_false()
    {
        var options = new SentryFuloraOptions();
        Assert.False(options.CaptureBridgeParams);
    }

    [Fact]
    public void Default_MaxBreadcrumbParamsLength_is_512()
    {
        var options = new SentryFuloraOptions();
        Assert.Equal(512, options.MaxBreadcrumbParamsLength);
    }

    [Fact]
    public void Default_FlushTimeout_is_2_seconds()
    {
        var options = new SentryFuloraOptions();
        Assert.Equal(TimeSpan.FromSeconds(2), options.FlushTimeout);
    }
}
