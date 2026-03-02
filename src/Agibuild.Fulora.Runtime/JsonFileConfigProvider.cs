using System.Collections.Concurrent;
using System.Text.Json;

namespace Agibuild.Fulora;

/// <summary>
/// An <see cref="IConfigProvider"/> that reads configuration from a local JSON file.
/// Supports refresh to reload the file. Thread-safe.
/// </summary>
public sealed class JsonFileConfigProvider : IConfigProvider
{
    private readonly string _filePath;
    private readonly object _lock = new();
    private Dictionary<string, JsonElement> _cache = new();

    /// <summary>Creates a provider that reads from the specified file path.</summary>
    /// <param name="filePath">Absolute or relative path to the JSON config file.</param>
    /// <exception cref="FileNotFoundException">Thrown when the file does not exist.</exception>
    public JsonFileConfigProvider(string filePath)
    {
        _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        if (!File.Exists(_filePath))
            throw new FileNotFoundException("Config file not found.", _filePath);
        LoadFile();
    }

    /// <inheritdoc />
    public Task<string?> GetValueAsync(string key, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        lock (_lock)
        {
            if (_cache.TryGetValue(key, out var el))
            {
                if (el.ValueKind == JsonValueKind.String)
                    return Task.FromResult<string?>(el.GetString());
                return Task.FromResult<string?>(el.GetRawText());
            }
        }
        return Task.FromResult<string?>(null);
    }

    /// <inheritdoc />
    public Task<T?> GetValueAsync<T>(string key, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        lock (_lock)
        {
            if (_cache.TryGetValue(key, out var el))
            {
                try
                {
                    var value = JsonSerializer.Deserialize<T>(el.GetRawText());
                    return Task.FromResult(value);
                }
                catch (JsonException)
                {
                    return Task.FromResult<T?>(default);
                }
            }
        }
        return Task.FromResult<T?>(default);
    }

    /// <inheritdoc />
    public Task<bool> IsFeatureEnabledAsync(string featureKey, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        lock (_lock)
        {
            if (!_cache.TryGetValue(featureKey, out var el))
                return Task.FromResult(false);

            return Task.FromResult(IsTruthy(el));
        }
    }

    /// <inheritdoc />
    public Task<IReadOnlyDictionary<string, string>?> GetSectionAsync(string key, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        lock (_lock)
        {
            if (!_cache.TryGetValue(key, out var el) || el.ValueKind != JsonValueKind.Object)
                return Task.FromResult<IReadOnlyDictionary<string, string>?>(null);

            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var prop in el.EnumerateObject())
                dict[prop.Name] = JsonElementToString(prop.Value);
            return Task.FromResult<IReadOnlyDictionary<string, string>?>(dict);
        }
    }

    /// <inheritdoc />
    public Task RefreshAsync(CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        if (!File.Exists(_filePath))
            throw new FileNotFoundException("Config file not found.", _filePath);
        LoadFile();
        return Task.CompletedTask;
    }

    private void LoadFile()
    {
        var json = File.ReadAllText(_filePath);
        using var doc = JsonDocument.Parse(json);
        var dict = new Dictionary<string, JsonElement>(StringComparer.OrdinalIgnoreCase);
        foreach (var prop in doc.RootElement.EnumerateObject())
            dict[prop.Name] = prop.Value.Clone();
        lock (_lock)
        {
            _cache = dict;
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
