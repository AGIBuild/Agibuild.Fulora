using System.Reflection;
using Xunit;

namespace Agibuild.Fulora.UnitTests;

public sealed class MauiIntegrationTests
{
    [Fact]
    public void MauiWebViewHostOptions_defaults_are_correct()
    {
        var options = new MauiWebViewHostOptions();
        Assert.False(options.EnableDevTools);
        Assert.False(options.EnableBridgeDevTools);
        Assert.Null(options.SpaHosting);
        Assert.Null(options.BridgeTracer);
    }

    [Fact]
    public void MauiWebViewHostOptions_can_be_configured()
    {
        var tracer = NullBridgeTracer.Instance;
        var spaOptions = new SpaHostingOptions { Scheme = "maui", Host = "app" };

        var options = new MauiWebViewHostOptions
        {
            EnableDevTools = true,
            EnableBridgeDevTools = true,
            SpaHosting = spaOptions,
            BridgeTracer = tracer
        };

        Assert.True(options.EnableDevTools);
        Assert.True(options.EnableBridgeDevTools);
        Assert.Same(spaOptions, options.SpaHosting);
        Assert.Same(tracer, options.BridgeTracer);
    }

    [Fact]
    public void IMauiWebViewHost_interface_has_expected_members()
    {
        var iface = typeof(IMauiWebViewHost);
        Assert.NotNull(iface);

        var webViewProp = iface.GetProperty(nameof(IMauiWebViewHost.WebView));
        Assert.NotNull(webViewProp);
        Assert.Equal(typeof(IWebView), webViewProp!.PropertyType);

        var bridgeProp = iface.GetProperty(nameof(IMauiWebViewHost.Bridge));
        Assert.NotNull(bridgeProp);
        Assert.Equal(typeof(IBridgeService), bridgeProp!.PropertyType);

        var initMethod = iface.GetMethod(nameof(IMauiWebViewHost.InitializeAsync));
        Assert.NotNull(initMethod);
        Assert.Equal(typeof(Task), initMethod!.ReturnType);
        var initParams = initMethod.GetParameters();
        Assert.Equal(2, initParams.Length);
        Assert.Equal(typeof(MauiWebViewHostOptions), initParams[0].ParameterType);
        Assert.Equal(typeof(CancellationToken), initParams[1].ParameterType);

        var navMethod = iface.GetMethod(nameof(IMauiWebViewHost.NavigateAsync));
        Assert.NotNull(navMethod);
        Assert.Equal(typeof(Task), navMethod!.ReturnType);
        var navParams = navMethod.GetParameters();
        Assert.Equal(2, navParams.Length);
        Assert.Equal(typeof(Uri), navParams[0].ParameterType);
        Assert.Equal(typeof(CancellationToken), navParams[1].ParameterType);
    }

    [Fact]
    public void MauiWebViewHostOptions_SpaHosting_integration()
    {
        var spaOptions = new SpaHostingOptions
        {
            Scheme = "maui",
            Host = "app",
            FallbackDocument = "index.html",
            EmbeddedResourcePrefix = "wwwroot"
        };

        var hostOptions = new MauiWebViewHostOptions { SpaHosting = spaOptions };

        Assert.NotNull(hostOptions.SpaHosting);
        Assert.Equal("maui", hostOptions.SpaHosting.Scheme);
        Assert.Equal("app", hostOptions.SpaHosting.Host);
        Assert.Equal("index.html", hostOptions.SpaHosting.FallbackDocument);
        Assert.Equal("wwwroot", hostOptions.SpaHosting.EmbeddedResourcePrefix);
        Assert.Equal(new Uri("maui://app/index.html"), hostOptions.SpaHosting.EntryPointUri);
    }
}
