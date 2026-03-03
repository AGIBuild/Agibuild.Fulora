using Agibuild.Fulora;

namespace Agibuild.Fulora.Plugin.HttpClient;

/// <summary>
/// Bridge service for host-routed HTTP requests.
/// Wraps System.Net.Http.HttpClient with base URL, timeout, and interceptor pipeline.
/// </summary>
[JsExport]
public interface IHttpClientService
{
    Task<HttpBridgeResponse> Get(string url, Dictionary<string, string>? headers = null);
    Task<HttpBridgeResponse> Post(string url, string? body = null, Dictionary<string, string>? headers = null);
    Task<HttpBridgeResponse> Put(string url, string? body = null, Dictionary<string, string>? headers = null);
    Task<HttpBridgeResponse> Delete(string url, Dictionary<string, string>? headers = null);
    Task<HttpBridgeResponse> Patch(string url, string? body = null, Dictionary<string, string>? headers = null);
}
