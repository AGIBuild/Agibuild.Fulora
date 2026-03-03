namespace Agibuild.Fulora.Plugin.HttpClient;

/// <summary>
/// Configuration options for <see cref="HttpClientService"/>.
/// </summary>
public sealed class HttpClientOptions
{
    public string? BaseUrl { get; init; }
    public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(30);
    public Dictionary<string, string> DefaultHeaders { get; init; } = new();
    public IReadOnlyList<IHttpRequestInterceptor> Interceptors { get; init; } = [];
}

/// <summary>
/// Request interceptor that can modify outgoing HTTP requests.
/// </summary>
public interface IHttpRequestInterceptor
{
    Task<HttpRequestMessage> InterceptAsync(HttpRequestMessage request);
}

/// <summary>
/// Provides auth tokens for request header injection.
/// </summary>
public interface IAuthTokenProvider
{
    Task<string?> GetTokenAsync();
}
