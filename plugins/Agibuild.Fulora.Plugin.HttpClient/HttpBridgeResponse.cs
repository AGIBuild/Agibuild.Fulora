namespace Agibuild.Fulora.Plugin.HttpClient;

/// <summary>
/// HTTP response DTO returned by <see cref="IHttpClientService"/> methods.
/// </summary>
public sealed class HttpBridgeResponse
{
    public int StatusCode { get; init; }
    public string? Body { get; init; }
    public Dictionary<string, string> Headers { get; init; } = new();
    public bool IsSuccess { get; init; }
}
