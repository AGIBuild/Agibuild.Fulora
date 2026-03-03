namespace Agibuild.Fulora.Plugin.AuthToken;

/// <summary>
/// In-memory implementation of <see cref="ISecureStorageProvider"/> for testing
/// and default fallback when platform secure storage is unavailable.
/// </summary>
public sealed class InMemorySecureStorageProvider : ISecureStorageProvider
{
    private readonly Dictionary<string, string> _store = new();
    private readonly object _lock = new();

    public Task<string?> GetAsync(string key)
    {
        lock (_lock)
        {
            return Task.FromResult(_store.GetValueOrDefault(key));
        }
    }

    public Task SetAsync(string key, string value)
    {
        lock (_lock)
        {
            _store[key] = value;
        }
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key)
    {
        lock (_lock)
        {
            _store.Remove(key);
        }
        return Task.CompletedTask;
    }

    public Task<string[]> ListKeysAsync()
    {
        lock (_lock)
        {
            return Task.FromResult(_store.Keys.ToArray());
        }
    }
}
