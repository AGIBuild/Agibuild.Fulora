namespace Agibuild.Fulora;

/// <summary>
/// Contract for hosting an Agibuild.Fulora WebView in a .NET MAUI application.
/// </summary>
public interface IMauiWebViewHost
{
    IWebView WebView { get; }
    IBridgeService Bridge { get; }
    Task InitializeAsync(MauiWebViewHostOptions options, CancellationToken ct = default);
    Task NavigateAsync(Uri uri, CancellationToken ct = default);
}

/// <summary>
/// Options for configuring a MAUI WebView host.
/// </summary>
public sealed class MauiWebViewHostOptions
{
    /// <summary>When true, enables browser developer tools.</summary>
    public bool EnableDevTools { get; set; }

    /// <summary>When true, enables bridge-specific DevTools panel.</summary>
    public bool EnableBridgeDevTools { get; set; }

    /// <summary>Optional SPA hosting configuration for embedded or dev-server assets.</summary>
    public SpaHostingOptions? SpaHosting { get; set; }

    /// <summary>Optional tracer for bridge call observation.</summary>
    public IBridgeTracer? BridgeTracer { get; set; }
}
