using Agibuild.Fulora.Adapters.Abstractions;
using Xunit;

namespace Agibuild.Fulora.UnitTests;

public class AdapterCookieParserTests
{
    [Fact]
    public void ParseCookiesJson_Null_ReturnsEmpty()
    {
        var result = AdapterCookieParser.ParseCookiesJson(null);
        Assert.Empty(result);
    }

    [Fact]
    public void ParseCookiesJson_EmptyString_ReturnsEmpty()
    {
        var result = AdapterCookieParser.ParseCookiesJson("");
        Assert.Empty(result);
    }

    [Fact]
    public void ParseCookiesJson_EmptyArray_ReturnsEmpty()
    {
        var result = AdapterCookieParser.ParseCookiesJson("[]");
        Assert.Empty(result);
    }

    [Fact]
    public void ParseCookiesJson_SingleCookie_ParsesAllFields()
    {
        var json = """[{"name":"session","value":"abc123","domain":".example.com","path":"/","expires":1700000000.000,"isSecure":true,"isHttpOnly":false}]""";

        var result = AdapterCookieParser.ParseCookiesJson(json);

        Assert.Single(result);
        var cookie = result[0];
        Assert.Equal("session", cookie.Name);
        Assert.Equal("abc123", cookie.Value);
        Assert.Equal(".example.com", cookie.Domain);
        Assert.Equal("/", cookie.Path);
        Assert.True(cookie.IsSecure);
        Assert.False(cookie.IsHttpOnly);
        Assert.NotNull(cookie.Expires);
        Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1700000000), cookie.Expires);
    }

    [Fact]
    public void ParseCookiesJson_NegativeExpires_ReturnsNullExpiry()
    {
        var json = """[{"name":"temp","value":"x","domain":"a.com","path":"/","expires":-1.0,"isSecure":false,"isHttpOnly":false}]""";

        var result = AdapterCookieParser.ParseCookiesJson(json);

        Assert.Single(result);
        Assert.Null(result[0].Expires);
    }

    [Fact]
    public void ParseCookiesJson_MultipleCookies_ParsesAll()
    {
        var json = """[{"name":"a","value":"1","domain":"x.com","path":"/","expires":-1.0,"isSecure":false,"isHttpOnly":false},{"name":"b","value":"2","domain":"y.com","path":"/api","expires":1800000000.000,"isSecure":true,"isHttpOnly":true}]""";

        var result = AdapterCookieParser.ParseCookiesJson(json);

        Assert.Equal(2, result.Count);
        Assert.Equal("a", result[0].Name);
        Assert.Equal("b", result[1].Name);
        Assert.True(result[1].IsSecure);
        Assert.True(result[1].IsHttpOnly);
    }

    [Fact]
    public void ParseCookiesJson_EscapedQuotes_HandledCorrectly()
    {
        // Native shim produces: {"value":"val\"ue"} — C# raw string needs \\\" for \"
        var json = "[{\"name\":\"key\",\"value\":\"val\\\"ue\",\"domain\":\"d.com\",\"path\":\"/\",\"expires\":-1.0,\"isSecure\":false,\"isHttpOnly\":false}]";

        var result = AdapterCookieParser.ParseCookiesJson(json);

        Assert.Single(result);
        Assert.Equal("val\"ue", result[0].Value);
    }

    [Fact]
    public void ParseCookiesJson_EscapedBackslash_HandledCorrectly()
    {
        // Native shim produces: {"value":"a\\b"} — C# needs \\\\ for \\
        var json = "[{\"name\":\"key\",\"value\":\"a\\\\b\",\"domain\":\"d.com\",\"path\":\"/\",\"expires\":-1.0,\"isSecure\":false,\"isHttpOnly\":false}]";

        var result = AdapterCookieParser.ParseCookiesJson(json);

        Assert.Single(result);
        Assert.Equal("a\\b", result[0].Value);
    }

    [Fact]
    public void ExtractJsonString_MissingKey_ReturnsEmpty()
    {
        var result = AdapterCookieParser.ExtractJsonString("""{"name":"val"}""", "missing");
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void ExtractJsonRaw_MissingKey_ReturnsEmpty()
    {
        var result = AdapterCookieParser.ExtractJsonRaw("""{"name":"val"}""", "missing");
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void ExtractJsonRaw_BooleanValue_ReturnsTrimmed()
    {
        var result = AdapterCookieParser.ExtractJsonRaw("""{"isSecure":true,"other":"x"}""", "isSecure");
        Assert.Equal("true", result);
    }

    [Fact]
    public void ExtractJsonRaw_LastFieldBeforeClosingBrace_ReturnsTrimmed()
    {
        var result = AdapterCookieParser.ExtractJsonRaw("""{"isHttpOnly":false}""", "isHttpOnly");
        Assert.Equal("false", result);
    }

    [Fact]
    public void ParseCookiesJson_WhitespaceOnly_ReturnsEmpty()
    {
        var result = AdapterCookieParser.ParseCookiesJson("   ");
        Assert.Empty(result);
    }

    [Fact]
    public void ParseCookiesJson_ZeroExpires_ReturnsNullExpiry()
    {
        var json = """[{"name":"z","value":"v","domain":"d.com","path":"/","expires":0,"isSecure":false,"isHttpOnly":false}]""";

        var result = AdapterCookieParser.ParseCookiesJson(json);

        Assert.Single(result);
        Assert.Null(result[0].Expires);
    }
}
