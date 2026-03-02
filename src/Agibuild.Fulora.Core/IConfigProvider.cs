namespace Agibuild.Fulora;

/// <summary>
/// Provides remote configuration and feature flag values.
/// </summary>
public interface IConfigProvider
{
    /// <summary>Gets a raw string value for the given key, or null if missing.</summary>
    Task<string?> GetValueAsync(string key, CancellationToken ct = default);

    /// <summary>Gets and deserializes a value for the given key, or null if missing.</summary>
    Task<T?> GetValueAsync<T>(string key, CancellationToken ct = default);

    /// <summary>Returns true if the feature is enabled (truthy value in config).</summary>
    Task<bool> IsFeatureEnabledAsync(string featureKey, CancellationToken ct = default);

    /// <summary>Gets a section (nested object) as key-value pairs, or null if the key is missing or not an object.</summary>
    Task<IReadOnlyDictionary<string, string>?> GetSectionAsync(string key, CancellationToken ct = default);

    /// <summary>Refreshes the configuration from the underlying source.</summary>
    Task RefreshAsync(CancellationToken ct = default);
}
