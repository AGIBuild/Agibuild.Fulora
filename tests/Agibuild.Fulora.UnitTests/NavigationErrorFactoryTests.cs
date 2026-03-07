using Agibuild.Fulora.Adapters.Abstractions;
using Xunit;

namespace Agibuild.Fulora.UnitTests;

public class NavigationErrorFactoryTests
{
    private static readonly Guid NavId = Guid.NewGuid();
    private static readonly Uri RequestUri = new("https://example.com/page");

    [Fact]
    public void Create_Timeout_ReturnsWebViewTimeoutException()
    {
        var ex = NavigationErrorFactory.Create(NavigationErrorCategory.Timeout, "timed out", NavId, RequestUri);

        Assert.IsType<WebViewTimeoutException>(ex);
        Assert.Equal("timed out", ex.Message);
        var typed = (WebViewTimeoutException)ex;
        Assert.Equal(NavId, typed.NavigationId);
        Assert.Equal(RequestUri, typed.RequestUri);
    }

    [Fact]
    public void Create_Network_ReturnsWebViewNetworkException()
    {
        var ex = NavigationErrorFactory.Create(NavigationErrorCategory.Network, "disconnected", NavId, RequestUri);

        Assert.IsType<WebViewNetworkException>(ex);
        Assert.Equal(NavId, ((WebViewNetworkException)ex).NavigationId);
    }

    [Fact]
    public void Create_Ssl_ReturnsWebViewSslException()
    {
        var ex = NavigationErrorFactory.Create(NavigationErrorCategory.Ssl, "cert expired", NavId, RequestUri);

        Assert.IsType<WebViewSslException>(ex);
    }

    [Fact]
    public void Create_Other_ReturnsWebViewNavigationException()
    {
        var ex = NavigationErrorFactory.Create(NavigationErrorCategory.Other, "unknown", NavId, RequestUri);

        Assert.IsType<WebViewNavigationException>(ex);
        Assert.IsNotType<WebViewTimeoutException>(ex);
        Assert.IsNotType<WebViewNetworkException>(ex);
        Assert.IsNotType<WebViewSslException>(ex);
    }

    [Theory]
    [InlineData(0, "Timeout")]
    [InlineData(1, "Network")]
    [InlineData(2, "Ssl")]
    [InlineData(3, "Other")]
    public void Create_AllCategories_PreserveMessageAndIds(int categoryInt, string label)
    {
        var category = (NavigationErrorCategory)categoryInt;
        var msg = $"error-{label}";
        var ex = NavigationErrorFactory.Create(category, msg, NavId, RequestUri);

        Assert.Equal(msg, ex.Message);
        var navEx = Assert.IsAssignableFrom<WebViewNavigationException>(ex);
        Assert.Equal(NavId, navEx.NavigationId);
        Assert.Equal(RequestUri, navEx.RequestUri);
    }

    [Fact]
    public void Create_UndefinedEnumValue_ReturnsWebViewNavigationException()
    {
        var ex = NavigationErrorFactory.Create((NavigationErrorCategory)999, "fallback", NavId, RequestUri);

        Assert.IsType<WebViewNavigationException>(ex);
    }
}
