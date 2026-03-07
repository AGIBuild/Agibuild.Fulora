## Tasks

### Task 1: Create project and models

- [x] Create `Agibuild.Fulora.Auth.OAuth` project (net10.0)
- [x] Create `OAuthPkceOptions`
- [x] Create `OAuthTokenResponse`
- [x] Create `OAuthException`

### Task 2: Implement PkceHelper

- [x] `GenerateCodeVerifier()` — RFC 7636 compliant
- [x] `ComputeCodeChallenge(verifier)` — S256 base64url

### Task 3: Implement OAuthPkceClient

- [x] Constructor with options validation
- [x] `BuildAuthorizationUrl(codeChallenge, state)`
- [x] `ExchangeCodeAsync(code, codeVerifier)`
- [x] `RefreshTokenAsync(refreshToken)`

### Task 4: Unit tests

- [x] PkceHelper tests (verifier format, challenge computation)
- [x] Authorization URL tests
- [x] Token exchange tests with mock HttpClient
- [x] Token refresh tests with mock HttpClient
- [x] Error handling tests

### Task 5: Verification

- [x] All tests pass (23/23)
- [x] Solution builds without errors
