using Agibuild.Fulora.Plugin.AuthToken;
using Xunit;

namespace Agibuild.Fulora.UnitTests.Plugins;

public class AuthTokenServiceTests
{
    private static AuthTokenService CreateService() =>
        new(new InMemorySecureStorageProvider());

    [Fact]
    public async Task GetToken_NonExistentKey_ReturnsNull()
    {
        var svc = CreateService();
        Assert.Null(await svc.GetToken("unknown"));
    }

    [Fact]
    public async Task SetToken_GetToken_Roundtrip()
    {
        var svc = CreateService();
        await svc.SetToken("access_token", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9");
        var token = await svc.GetToken("access_token");
        Assert.Equal("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9", token);
    }

    [Fact]
    public async Task RemoveToken_RemovesKey()
    {
        var svc = CreateService();
        await svc.SetToken("k", "v");
        await svc.RemoveToken("k");
        Assert.Null(await svc.GetToken("k"));
    }

    [Fact]
    public async Task ListKeys_ReturnsAllKeys()
    {
        var svc = CreateService();
        await svc.SetToken("a", "1");
        await svc.SetToken("b", "2");
        await svc.SetToken("c", "3");
        var keys = await svc.ListKeys();
        Assert.Equal(3, keys.Length);
        Assert.Contains("a", keys);
        Assert.Contains("b", keys);
        Assert.Contains("c", keys);
    }

    [Fact]
    public async Task ExpiredToken_ReturnsNull()
    {
        var svc = CreateService();
        var pastExpiry = DateTimeOffset.UtcNow.AddMinutes(-1);
        await svc.SetToken("expired", "secret", new TokenOptions { ExpiresAt = pastExpiry });
        var token = await svc.GetToken("expired");
        Assert.Null(token);
    }

    [Fact]
    public async Task NonExpiredToken_ReturnsValue()
    {
        var svc = CreateService();
        var futureExpiry = DateTimeOffset.UtcNow.AddHours(1);
        await svc.SetToken("valid", "secret", new TokenOptions { ExpiresAt = futureExpiry });
        var token = await svc.GetToken("valid");
        Assert.Equal("secret", token);
    }
}
