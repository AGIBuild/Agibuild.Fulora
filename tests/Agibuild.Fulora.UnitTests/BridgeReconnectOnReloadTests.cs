using Agibuild.Fulora.Testing;
using Xunit;

namespace Agibuild.Fulora.UnitTests;

/// <summary>
/// Verifies that bridge JS stubs are re-injected after page navigation/reload
/// so the frontend can reconnect without an application restart.
/// </summary>
public sealed class BridgeReconnectOnReloadTests
{
    private readonly TestDispatcher _dispatcher = new();

    private (WebViewCore Core, MockWebViewAdapter Adapter, List<string> Scripts) CreateCore(bool enableBridge = true)
    {
        var adapter = MockWebViewAdapter.Create();
        var core = new WebViewCore(adapter, _dispatcher);
        var scripts = new List<string>();
        adapter.ScriptCallback = script => { scripts.Add(script); return null; };

        if (enableBridge)
        {
            core.EnableWebMessageBridge(new WebMessageBridgeOptions
            {
                AllowedOrigins = new HashSet<string> { "*" }
            });
        }

        return (core, adapter, scripts);
    }

    [Fact]
    public async Task Successful_navigation_reinjects_base_rpc_stub_and_service_stubs()
    {
        var (core, adapter, scripts) = CreateCore();

        core.Bridge.Expose<IAppService>(new FakeAppService());
        _dispatcher.RunAll();

        scripts.Clear();

        var navTask = core.NavigateAsync(new Uri("https://example.test/page2"));
        _dispatcher.RunAll();
        adapter.RaiseNavigationCompleted(NavigationCompletedStatus.Success);
        _dispatcher.RunAll();
        await navTask;

        Assert.Contains(scripts, s => s.Contains("window.agWebView"));
        Assert.Contains(scripts, s => s.Contains("AppService"));
    }

    [Fact]
    public async Task Failed_navigation_does_not_reinject_stubs()
    {
        var (core, adapter, scripts) = CreateCore();

        core.Bridge.Expose<IAppService>(new FakeAppService());
        _dispatcher.RunAll();

        scripts.Clear();

        var navTask = core.NavigateAsync(new Uri("https://example.test/fail"));
        _dispatcher.RunAll();
        adapter.RaiseNavigationCompleted(NavigationCompletedStatus.Failure, new Exception("network error"));
        _dispatcher.RunAll();

        await Assert.ThrowsAsync<WebViewNavigationException>(() => navTask);

        Assert.DoesNotContain(scripts, s => s.Contains("window.agWebView"));
    }

    [Fact]
    public async Task Bridge_disabled_navigation_does_not_inject_stubs()
    {
        var (core, adapter, scripts) = CreateCore(enableBridge: false);

        scripts.Clear();

        var navTask = core.NavigateAsync(new Uri("https://example.test/page"));
        _dispatcher.RunAll();
        adapter.RaiseNavigationCompleted(NavigationCompletedStatus.Success);
        _dispatcher.RunAll();
        await navTask;

        Assert.DoesNotContain(scripts, s => s.Contains("window.agWebView"));
    }

    [Fact]
    public async Task Multiple_services_all_reinjected_after_reload()
    {
        var (core, adapter, scripts) = CreateCore();

        core.Bridge.Expose<IAppService>(new FakeAppService());
        core.Bridge.Expose<ICustomNameService>(new FakeCustomNameService());
        _dispatcher.RunAll();

        scripts.Clear();

        var navTask = core.NavigateAsync(new Uri("https://example.test/reload"));
        _dispatcher.RunAll();
        adapter.RaiseNavigationCompleted(NavigationCompletedStatus.Success);
        _dispatcher.RunAll();
        await navTask;

        Assert.Contains(scripts, s => s.Contains("AppService"));
        Assert.Contains(scripts, s => s.Contains("api"));
    }

    [Fact]
    public async Task Canceled_navigation_does_not_reinject_stubs()
    {
        var (core, adapter, scripts) = CreateCore();

        core.Bridge.Expose<IAppService>(new FakeAppService());
        _dispatcher.RunAll();

        scripts.Clear();

        core.NavigationStarted += (_, args) => args.Cancel = true;
        var navTask = core.NavigateAsync(new Uri("https://example.test/cancel"));
        _dispatcher.RunAll();
        await navTask;

        Assert.DoesNotContain(scripts, s => s.Contains("AppService"));
    }
}
