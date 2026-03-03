namespace Agibuild.Fulora.Plugin.AuthToken;

/// <summary>
/// Abstraction for platform-secure storage. Implementations may use Keychain,
/// Credential Manager, Keystore, or Secret Service per platform.
/// </summary>
public interface ISecureStorageProvider
{
    Task<string?> GetAsync(string key);
    Task SetAsync(string key, string value);
    Task RemoveAsync(string key);
    Task<string[]> ListKeysAsync();
}
