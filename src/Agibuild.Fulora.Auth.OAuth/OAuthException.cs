namespace Agibuild.Fulora.Auth;

/// <summary>
/// Exception thrown when an OAuth operation fails.
/// </summary>
public sealed class OAuthException : Exception
{
    /// <summary>OAuth error code (e.g. "invalid_grant").</summary>
    public string? ErrorCode { get; }

    /// <summary>OAuth error description from the identity provider.</summary>
    public string? ErrorDescription { get; }

    public OAuthException(string message) : base(message) { }

    public OAuthException(string message, Exception innerException) : base(message, innerException) { }

    public OAuthException(string? errorCode, string? errorDescription)
        : base($"OAuth error: {errorCode} — {errorDescription}")
    {
        ErrorCode = errorCode;
        ErrorDescription = errorDescription;
    }
}
