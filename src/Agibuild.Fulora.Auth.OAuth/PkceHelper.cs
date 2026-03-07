using System.Security.Cryptography;
using System.Text;

namespace Agibuild.Fulora.Auth;

/// <summary>
/// RFC 7636 PKCE (Proof Key for Code Exchange) helper for generating
/// code verifiers and S256 code challenges.
/// </summary>
public static class PkceHelper
{
    private const int DefaultVerifierLength = 64;

    /// <summary>
    /// Generates a cryptographically random code verifier (43–128 characters, URL-safe).
    /// </summary>
    public static string GenerateCodeVerifier(int length = DefaultVerifierLength)
    {
        if (length < 43 || length > 128)
            throw new ArgumentOutOfRangeException(nameof(length), "Code verifier length must be between 43 and 128.");

        Span<byte> buffer = stackalloc byte[length];
        RandomNumberGenerator.Fill(buffer);
        return Base64UrlEncode(buffer)[..length];
    }

    /// <summary>
    /// Computes the S256 code challenge from a code verifier: BASE64URL(SHA256(verifier)).
    /// </summary>
    public static string ComputeCodeChallenge(string codeVerifier)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(codeVerifier);
        var hash = SHA256.HashData(Encoding.ASCII.GetBytes(codeVerifier));
        return Base64UrlEncode(hash);
    }

    private static string Base64UrlEncode(ReadOnlySpan<byte> data) =>
        Convert.ToBase64String(data)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
}
