using Agibuild.Fulora.Auth;
using Xunit;

namespace Agibuild.Fulora.Auth.OAuth.Tests;

public class PkceHelperTests
{
    [Fact]
    public void GenerateCodeVerifier_DefaultLength_Returns64Chars()
    {
        var verifier = PkceHelper.GenerateCodeVerifier();
        Assert.Equal(64, verifier.Length);
    }

    [Fact]
    public void GenerateCodeVerifier_CustomLength_ReturnsCorrectLength()
    {
        var verifier = PkceHelper.GenerateCodeVerifier(43);
        Assert.Equal(43, verifier.Length);
    }

    [Fact]
    public void GenerateCodeVerifier_MaxLength_Returns128Chars()
    {
        var verifier = PkceHelper.GenerateCodeVerifier(128);
        Assert.Equal(128, verifier.Length);
    }

    [Fact]
    public void GenerateCodeVerifier_IsUrlSafe()
    {
        var verifier = PkceHelper.GenerateCodeVerifier();
        Assert.DoesNotContain("+", verifier);
        Assert.DoesNotContain("/", verifier);
        Assert.DoesNotContain("=", verifier);
    }

    [Fact]
    public void GenerateCodeVerifier_TooShort_ThrowsArgument()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => PkceHelper.GenerateCodeVerifier(42));
    }

    [Fact]
    public void GenerateCodeVerifier_TooLong_ThrowsArgument()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => PkceHelper.GenerateCodeVerifier(129));
    }

    [Fact]
    public void GenerateCodeVerifier_IsCryptographicallyRandom()
    {
        var v1 = PkceHelper.GenerateCodeVerifier();
        var v2 = PkceHelper.GenerateCodeVerifier();
        Assert.NotEqual(v1, v2);
    }

    [Fact]
    public void ComputeCodeChallenge_KnownInput_ProducesExpectedHash()
    {
        // RFC 7636 Appendix B test vector
        var verifier = "dBjftJeZ4CVP-mB92K27uhbUJU1p1r_wW1gFWFOEjXk";
        var challenge = PkceHelper.ComputeCodeChallenge(verifier);
        Assert.Equal("E9Melhoa2OwvFrEMTJguCHaoeK1t8URWbuGJSstw-cM", challenge);
    }

    [Fact]
    public void ComputeCodeChallenge_IsUrlSafe()
    {
        var verifier = PkceHelper.GenerateCodeVerifier();
        var challenge = PkceHelper.ComputeCodeChallenge(verifier);
        Assert.DoesNotContain("+", challenge);
        Assert.DoesNotContain("/", challenge);
        Assert.DoesNotContain("=", challenge);
    }

    [Fact]
    public void ComputeCodeChallenge_NullInput_ThrowsArgument()
    {
        Assert.Throws<ArgumentNullException>(() => PkceHelper.ComputeCodeChallenge(null!));
    }
}
