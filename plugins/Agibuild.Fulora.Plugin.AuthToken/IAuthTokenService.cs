using Agibuild.Fulora;

namespace Agibuild.Fulora.Plugin.AuthToken;

/// <summary>
/// Bridge service for secure token storage with expiry and scope metadata.
/// </summary>
[JsExport]
public interface IAuthTokenService
{
    Task<string?> GetToken(string key);
    Task SetToken(string key, string value, TokenOptions? options = null);
    Task RemoveToken(string key);
    Task<string[]> ListKeys();
}
