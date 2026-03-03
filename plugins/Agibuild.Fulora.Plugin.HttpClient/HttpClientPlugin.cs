using Agibuild.Fulora;

namespace Agibuild.Fulora.Plugin.HttpClient;

/// <summary>
/// Bridge plugin manifest for the HTTP client service.
/// Register with: <c>bridge.UsePlugin&lt;HttpClientPlugin&gt;();</c>
/// </summary>
public sealed class HttpClientPlugin : IBridgePlugin
{
    public static IEnumerable<BridgePluginServiceDescriptor> GetServices()
    {
        yield return BridgePluginServiceDescriptor.Create<IHttpClientService>(sp =>
            new HttpClientService(sp?.GetService(typeof(HttpClientOptions)) as HttpClientOptions));
    }
}
