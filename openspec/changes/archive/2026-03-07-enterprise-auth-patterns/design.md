## Context

The AuthToken plugin handles secure token storage. M12.3 adds the OAuth PKCE flow layer on top: generating PKCE codes, building authorization URLs, exchanging codes for tokens, and refreshing expired tokens. This enables enterprise SSO integration patterns.

## Decisions

### D1: Separate package for OAuth

`Agibuild.Fulora.Auth.OAuth` as a standalone package. Not bundled into AuthToken plugin because OAuth is optional and adds `System.Net.Http` dependency for token exchange. Apps that use simple API keys don't need it.

### D2: OAuthPkceClient with HttpClient injection

`OAuthPkceClient` accepts `HttpClient` and `OAuthPkceOptions` via constructor. Uses RFC 7636 PKCE with S256 challenge method. No static state — fully testable via injected HttpClient.

### D3: OAuthPkceOptions for configuration

```csharp
public sealed class OAuthPkceOptions
{
    public string Authority { get; set; }      // e.g. https://login.microsoftonline.com/tenant
    public string ClientId { get; set; }
    public string RedirectUri { get; set; }     // e.g. myapp://auth/callback
    public string[] Scopes { get; set; }
    public string? TokenEndpoint { get; set; }  // override if non-standard
    public string? AuthorizationEndpoint { get; set; }
}
```

### D4: Pure HTTP token exchange — no browser automation

The client builds the authorization URL and returns it. The app is responsible for presenting it (system browser, embedded WebView). The client provides `ExchangeCodeAsync(code, codeVerifier)` after the redirect captures the authorization code.

### D5: Token refresh via standard refresh_token grant

`RefreshTokenAsync(refreshToken)` performs a standard OAuth refresh. Returns new `OAuthTokenResponse` with access_token, refresh_token, expires_in.

### D6: Static PKCE helper for code generation

`PkceHelper.GenerateCodeVerifier()` and `PkceHelper.ComputeCodeChallenge(verifier)` as static methods for apps that want to customize the flow.

## Testing Strategy

- Unit tests for PkceHelper (verifier length, challenge computation, S256)
- Unit tests for authorization URL building
- Unit tests for token exchange with mock HttpClient
- Unit tests for token refresh with mock HttpClient
- Unit tests for error handling (bad response, network error)
