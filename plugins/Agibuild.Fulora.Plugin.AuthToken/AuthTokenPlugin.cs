using Agibuild.Fulora;

namespace Agibuild.Fulora.Plugin.AuthToken;

/// <summary>
/// Bridge plugin manifest for the AuthToken service.
/// Register with: <c>bridge.UsePlugin&lt;AuthTokenPlugin&gt;();</c>
/// Uses <see cref="InMemorySecureStorageProvider"/> as the default storage.
/// </summary>
public sealed class AuthTokenPlugin : IBridgePlugin
{
    public static IEnumerable<BridgePluginServiceDescriptor> GetServices()
    {
        yield return BridgePluginServiceDescriptor.Create<IAuthTokenService>(
            _ => new AuthTokenService(new InMemorySecureStorageProvider()));
    }
}
