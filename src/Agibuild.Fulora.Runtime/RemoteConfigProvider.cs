using System.Collections.Concurrent;
using System.Text.Json;

namespace Agibuild.Fulora;

/// <summary>
/// Config provider that fetches from a remote HTTP endpoint and merges with a local fallback.
/// Remote values override local values.
/// </summary>
public sealed class RemoteConfigProvider : IConfigProvider
{
    private readonly HttpClient _httpClient;
    private readonly Uri _remoteUri;
    private readonly IConfigProvider? _localFallback;
    private readonly object _lock = new();
    private Dictionary<string, JsonElement> _remoteCache = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Creates a provider that fetches config from a remote URI and optionally falls back to a local provider.
    /// </summary>
    /// <param name="httpClient">The HTTP client used to fetch remote config.</param>
    /// <param name="remoteUri">The URI to fetch JSON config from.</param>
    /// <param name="localFallback">Optional local provider for keys not present in remote config.</param>
    public RemoteConfigProvider(HttpClient httpClient, Uri remoteUri, IConfigProvider? localFallback = null)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _remoteUri = remoteUri ?? throw new ArgumentNullException(nameof(remoteUri));
        _localFallback = localFallback;
    }

    /// <inheritdoc />
    public async Task<string?> GetValueAsync(string key, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        lock (_lock)
        {
            if (_remoteCache.TryGetValue(key, out var el))
            {
                if (el.ValueKind == JsonValueKind.String)
                    return el.GetString();
                return el.GetRawText();
            }
        }
        return _localFallback != null ? await _localFallback.GetValueAsync(key, ct) : null;
    }

    /// <inheritdoc />
    public async Task<T?> GetValueAsync<T>(string key, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        lock (_lock)
        {
            if (_remoteCache.TryGetValue(key, out var el))
            {
                try
                {
                    var value = JsonSerializer.Deserialize<T>(el.GetRawText());
                    return value;
                }
                catch (JsonException)
                {
                    return default;
                }
            }
        }
        return _localFallback != null ? await _localFallback.GetValueAsync<T>(key, ct) : default;
    }

    /// <inheritdoc />
    public async Task<bool> IsFeatureEnabledAsync(string featureKey, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        lock (_lock)
        {
            if (_remoteCache.TryGetValue(featureKey, out var el))
                return IsTruthy(el);
        }
        return _localFallback != null && await _localFallback.IsFeatureEnabledAsync(featureKey, ct);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyDictionary<string, string>?> GetSectionAsync(string key, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        lock (_lock)
        {
            if (_remoteCache.TryGetValue(key, out var el) && el.ValueKind == JsonValueKind.Object)
            {
                var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (var prop in el.EnumerateObject())
                    dict[prop.Name] = JsonElementToString(prop.Value);
                return dict;
            }
        }
        return _localFallback != null ? await _localFallback.GetSectionAsync(key, ct) : null;
    }

    /// <inheritdoc />
    public async Task RefreshAsync(CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        var response = await _httpClient.GetAsync(_remoteUri, ct);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync(ct);
        using var doc = JsonDocument.Parse(json);
        var dict = new Dictionary<string, JsonElement>(StringComparer.OrdinalIgnoreCase);
        foreach (var prop in doc.RootElement.EnumerateObject())
            dict[prop.Name] = prop.Value.Clone();
        lock (_lock)
        {
            _remoteCache = dict;
        }
    }

    private static bool IsTruthy(JsonElement el)
    {
        return el.ValueKind switch
        {
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Number => el.TryGetDouble(out var d) && Math.Abs(d - 1) < 0.001,
            JsonValueKind.String => IsTruthyString(el.GetString()),
            _ => false,
        };
    }

    private static bool IsTruthyString(string? s)
    {
        if (string.IsNullOrWhiteSpace(s)) return false;
        return s.Equals("true", StringComparison.OrdinalIgnoreCase)
            || s.Equals("1", StringComparison.Ordinal)
            || s.Equals("yes", StringComparison.OrdinalIgnoreCase)
            || s.Equals("on", StringComparison.OrdinalIgnoreCase);
    }

    private static string JsonElementToString(JsonElement el)
    {
        return el.ValueKind switch
        {
            JsonValueKind.String => el.GetString() ?? "",
            _ => el.GetRawText(),
        };
    }
}
