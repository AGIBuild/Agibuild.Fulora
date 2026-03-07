using System.Text.Json;
using System.Text.Json.Serialization;

namespace Agibuild.Fulora;

/// <summary>
/// Represents the <c>fulora-plugin.json</c> manifest embedded in Fulora plugin NuGet packages.
/// Provides machine-readable metadata for compatibility checking and plugin discovery.
/// </summary>
public sealed class PluginManifest
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        ReadCommentHandling = JsonCommentHandling.Skip,
    };

    [JsonPropertyName("id")]
    public string Id { get; set; } = "";

    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; } = "";

    [JsonPropertyName("services")]
    public string[] Services { get; set; } = [];

    [JsonPropertyName("npmPackage")]
    public string? NpmPackage { get; set; }

    [JsonPropertyName("minFuloraVersion")]
    public string MinFuloraVersion { get; set; } = "0.0.0";

    [JsonPropertyName("platforms")]
    public string[]? Platforms { get; set; }

    /// <summary>
    /// Checks whether this plugin is compatible with the given Fulora framework version.
    /// </summary>
    public bool IsCompatibleWith(Version fuloraVersion)
    {
        ArgumentNullException.ThrowIfNull(fuloraVersion);
        return Version.TryParse(MinFuloraVersion, out var minVersion)
            && fuloraVersion >= minVersion;
    }

    /// <summary>Parses a manifest from JSON string.</summary>
    public static PluginManifest? Parse(string json)
    {
        if (string.IsNullOrWhiteSpace(json)) return null;
        return JsonSerializer.Deserialize<PluginManifest>(json, JsonOptions);
    }

    /// <summary>Parses a manifest from a file path.</summary>
    public static PluginManifest? LoadFromFile(string path)
    {
        if (!File.Exists(path)) return null;
        var json = File.ReadAllText(path);
        return Parse(json);
    }
}
