namespace Agibuild.Fulora.Auth;

/// <summary>
/// Configuration for OAuth PKCE authentication flow.
/// </summary>
public sealed class OAuthPkceOptions
{
    /// <summary>OAuth authority base URL (e.g. https://login.microsoftonline.com/tenant).</summary>
    public string Authority { get; set; } = "";

    /// <summary>OAuth client identifier.</summary>
    public string ClientId { get; set; } = "";

    /// <summary>Redirect URI registered with the identity provider.</summary>
    public string RedirectUri { get; set; } = "";

    /// <summary>Requested OAuth scopes.</summary>
    public string[] Scopes { get; set; } = ["openid", "profile"];

    /// <summary>Override token endpoint (defaults to {Authority}/oauth2/v2.0/token).</summary>
    public string? TokenEndpoint { get; set; }

    /// <summary>Override authorization endpoint (defaults to {Authority}/oauth2/v2.0/authorize).</summary>
    public string? AuthorizationEndpoint { get; set; }

    internal string GetTokenEndpoint() =>
        TokenEndpoint ?? $"{Authority.TrimEnd('/')}/oauth2/v2.0/token";

    internal string GetAuthorizationEndpoint() =>
        AuthorizationEndpoint ?? $"{Authority.TrimEnd('/')}/oauth2/v2.0/authorize";

    internal void Validate()
    {
        if (string.IsNullOrWhiteSpace(Authority))
            throw new ArgumentException("Authority is required.", nameof(Authority));
        if (string.IsNullOrWhiteSpace(ClientId))
            throw new ArgumentException("ClientId is required.", nameof(ClientId));
        if (string.IsNullOrWhiteSpace(RedirectUri))
            throw new ArgumentException("RedirectUri is required.", nameof(RedirectUri));
    }
}
