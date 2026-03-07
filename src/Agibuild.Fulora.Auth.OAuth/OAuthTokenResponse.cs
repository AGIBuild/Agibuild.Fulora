using System.Text.Json.Serialization;

namespace Agibuild.Fulora.Auth;

/// <summary>
/// Represents the response from an OAuth token endpoint.
/// </summary>
public sealed class OAuthTokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = "";

    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = "Bearer";

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; set; }

    [JsonPropertyName("scope")]
    public string? Scope { get; set; }

    [JsonPropertyName("id_token")]
    public string? IdToken { get; set; }
}
