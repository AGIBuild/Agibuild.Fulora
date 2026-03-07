## Why

Enterprise apps commonly require SSO/OAuth PKCE authentication. Desktop/mobile hybrid apps have unique challenges: no server-side callback handling, need for system browser or embedded WebView redirect capture, and secure token persistence. Currently Fulora provides token storage (AuthToken plugin) but no OAuth flow helper.

Traces to ROADMAP Phase 12 M12.3.

## What Changes

- Add `Agibuild.Fulora.Auth.OAuth` package with `OAuthPkceClient`
- PKCE flow implementation: code_verifier/challenge generation, authorization URL building, token exchange, token refresh
- Integration with `IAuthTokenService` for token persistence
- Configurable via `OAuthPkceOptions` (authority, client_id, scopes, redirect_uri)
- Unit tests for PKCE generation, URL building, token exchange, refresh logic

## Non-goals

- Embedded browser OAuth popup (framework-level WebView navigation handles this separately)
- SAML/WS-Federation support
- Multi-provider federation orchestration
- Custom identity server implementation

## Capabilities

### New Capabilities
- `oauth-pkce`: Reusable OAuth PKCE client for desktop/mobile hybrid apps

## Impact

- **Code**: New `Agibuild.Fulora.Auth.OAuth` project (5-6 files)
- **Tests**: New test project with comprehensive PKCE flow tests
- **Packages**: New NuGet package `Agibuild.Fulora.Auth.OAuth`
