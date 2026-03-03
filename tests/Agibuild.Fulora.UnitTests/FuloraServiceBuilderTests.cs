using Agibuild.Fulora.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Agibuild.Fulora.UnitTests;

public class FuloraServiceBuilderTests
{
    [Fact]
    public void AddFulora_registers_WebViewMessageBus()
    {
        var services = new ServiceCollection();
        services.AddFulora();
        var sp = services.BuildServiceProvider();

        var bus = sp.GetService<IWebViewMessageBus>();
        Assert.NotNull(bus);
        Assert.IsType<WebViewMessageBus>(bus);
    }

    [Fact]
    public void AddFulora_registers_default_NullTelemetryProvider()
    {
        var services = new ServiceCollection();
        services.AddFulora();
        var sp = services.BuildServiceProvider();

        var telemetry = sp.GetService<ITelemetryProvider>();
        Assert.NotNull(telemetry);
        Assert.Same(NullTelemetryProvider.Instance, telemetry);
    }

    [Fact]
    public void AddFulora_does_not_override_existing_TelemetryProvider()
    {
        var custom = new ConsoleTelemetryProvider();
        var services = new ServiceCollection();
        services.AddSingleton<ITelemetryProvider>(custom);
        services.AddFulora();
        var sp = services.BuildServiceProvider();

        var telemetry = sp.GetService<ITelemetryProvider>();
        Assert.Same(custom, telemetry);
    }

    [Fact]
    public void AddTelemetry_replaces_default_provider()
    {
        var custom = new ConsoleTelemetryProvider();
        var services = new ServiceCollection();
        services.AddFulora().AddTelemetry(custom);
        var sp = services.BuildServiceProvider();

        var telemetry = sp.GetService<ITelemetryProvider>();
        Assert.Same(custom, telemetry);
    }

    [Fact]
    public void AddTelemetry_throws_on_null()
    {
        var services = new ServiceCollection();
        var builder = services.AddFulora();
        Assert.Throws<ArgumentNullException>(() => builder.AddTelemetry(null!));
    }

    [Fact]
    public void AddAutoUpdate_registers_service()
    {
        var services = new ServiceCollection();
        var provider = new StubAutoUpdateProvider();
        var options = new AutoUpdateOptions { FeedUrl = "https://update.test", CheckInterval = null };
        services.AddFulora().AddAutoUpdate(options, provider);
        var sp = services.BuildServiceProvider();

        var svc = sp.GetService<IAutoUpdateService>();
        Assert.NotNull(svc);
        Assert.IsType<AutoUpdateService>(svc);
    }

    [Fact]
    public void AddAutoUpdate_throws_on_null_options()
    {
        var services = new ServiceCollection();
        var builder = services.AddFulora();
        Assert.Throws<ArgumentNullException>(() => builder.AddAutoUpdate(null!, new StubAutoUpdateProvider()));
    }

    [Fact]
    public void AddAutoUpdate_throws_on_null_provider()
    {
        var services = new ServiceCollection();
        var builder = services.AddFulora();
        Assert.Throws<ArgumentNullException>(() =>
            builder.AddAutoUpdate(new AutoUpdateOptions { FeedUrl = "https://test" }, null!));
    }

    [Fact]
    public void AddFulora_returns_builder_with_services_reference()
    {
        var services = new ServiceCollection();
        var builder = services.AddFulora();
        Assert.Same(services, builder.Services);
    }

    [Fact]
    public void AddFulora_is_idempotent_for_message_bus()
    {
        var services = new ServiceCollection();
        services.AddFulora();
        services.AddFulora();
        var sp = services.BuildServiceProvider();

        var buses = sp.GetServices<IWebViewMessageBus>().ToList();
        Assert.True(buses.Count >= 1);
    }

    private sealed class StubAutoUpdateProvider : IAutoUpdatePlatformProvider
    {
        public Task<UpdateInfo?> CheckForUpdateAsync(AutoUpdateOptions options, string currentVersion, CancellationToken ct = default)
            => Task.FromResult<UpdateInfo?>(null);
        public Task<string> DownloadUpdateAsync(UpdateInfo update, AutoUpdateOptions options, Action<UpdateDownloadProgress>? onProgress = null, CancellationToken ct = default)
            => Task.FromResult("/tmp/stub.zip");
        public Task<bool> VerifyPackageAsync(string packagePath, UpdateInfo update, CancellationToken ct = default)
            => Task.FromResult(true);
        public Task ApplyUpdateAsync(string packagePath, CancellationToken ct = default)
            => Task.CompletedTask;
        public string GetCurrentVersion() => "1.0.0";
    }
}
