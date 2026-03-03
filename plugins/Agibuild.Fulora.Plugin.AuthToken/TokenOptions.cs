namespace Agibuild.Fulora.Plugin.AuthToken;

/// <summary>
/// Optional metadata for stored tokens (expiry, scope).
/// </summary>
public sealed class TokenOptions
{
    public DateTimeOffset? ExpiresAt { get; init; }
    public string? Scope { get; init; }
}
