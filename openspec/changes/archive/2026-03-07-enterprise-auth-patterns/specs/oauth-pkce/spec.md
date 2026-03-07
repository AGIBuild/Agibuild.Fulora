## Purpose

Define requirements for the OAuth PKCE client for enterprise authentication patterns.

## Requirements

### Requirement: PKCE code generation

#### Scenario: Generate code verifier
- **WHEN** `PkceHelper.GenerateCodeVerifier()` is called
- **THEN** it SHALL return a URL-safe string between 43 and 128 characters (RFC 7636)

#### Scenario: Compute code challenge
- **WHEN** `PkceHelper.ComputeCodeChallenge(verifier)` is called
- **THEN** it SHALL return the Base64URL-encoded SHA-256 hash of the verifier

### Requirement: Authorization URL building

#### Scenario: Build authorization URL
- **GIVEN** `OAuthPkceOptions` with authority, client_id, scopes, redirect_uri
- **WHEN** `BuildAuthorizationUrl(codeChallenge, state)` is called
- **THEN** the URL SHALL include `response_type=code`, `client_id`, `redirect_uri`, `scope`, `code_challenge`, `code_challenge_method=S256`, and `state`

### Requirement: Token exchange

#### Scenario: Exchange authorization code for tokens
- **GIVEN** a valid authorization code and code verifier
- **WHEN** `ExchangeCodeAsync(code, codeVerifier)` is called
- **THEN** it SHALL POST to the token endpoint with `grant_type=authorization_code`
- **AND** include `code`, `code_verifier`, `client_id`, `redirect_uri`
- **AND** return `OAuthTokenResponse` with access_token, refresh_token, expires_in

#### Scenario: Token exchange failure
- **GIVEN** an invalid authorization code
- **WHEN** `ExchangeCodeAsync` is called
- **THEN** it SHALL throw `OAuthException` with error details

### Requirement: Token refresh

#### Scenario: Refresh expired token
- **GIVEN** a valid refresh token
- **WHEN** `RefreshTokenAsync(refreshToken)` is called
- **THEN** it SHALL POST to the token endpoint with `grant_type=refresh_token`
- **AND** return a new `OAuthTokenResponse`

### Requirement: Configuration validation

#### Scenario: Missing required options
- **WHEN** `OAuthPkceClient` is constructed with missing Authority or ClientId
- **THEN** it SHALL throw `ArgumentException`
