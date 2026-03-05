using Agibuild.Fulora;
using Agibuild.Fulora.NativeOverlay;
using Avalonia;
using Xunit;

namespace Agibuild.Fulora.UnitTests;

public sealed class OverlayHostTests
{
    [Fact]
    public void WebViewOverlayHost_construction()
    {
        var webView = new WebView();
        var host = new WebViewOverlayHost(webView);

        Assert.NotNull(host);
    }

    [Fact]
    public void WebViewOverlayHost_Content_get_set()
    {
        var webView = new WebView();
        var host = new WebViewOverlayHost(webView);

        Assert.Null(host.Content);

        var content = new object();
        host.Content = content;
        Assert.Same(content, host.Content);

        host.Content = null;
        Assert.Null(host.Content);
    }

    [Fact]
    public void WebViewOverlayHost_Dispose_cleans_up()
    {
        var webView = new WebView();
        var host = new WebViewOverlayHost(webView);
        host.Content = new object();

        host.Dispose();

        Assert.Null(host.Content);
        Assert.False(host.IsVisible);

        host.Dispose();
    }

    [Fact]
    public void UpdatePosition_stores_DPI_scaled_bounds_correctly()
    {
        var webView = new WebView();
        var host = new WebViewOverlayHost(webView);
        var webViewBounds = new Rect(10, 20, 100, 50);
        var screenOffset = new Point(5, 15);
        var dpiScale = 2.0;

        host.UpdatePosition(webViewBounds, screenOffset, dpiScale);

        var expected = new Rect(
            screenOffset.X + webViewBounds.X * dpiScale,
            screenOffset.Y + webViewBounds.Y * dpiScale,
            webViewBounds.Width * dpiScale,
            webViewBounds.Height * dpiScale);
        Assert.Equal(expected, host.Bounds);
        Assert.Equal(dpiScale, host.DpiScale);
    }

    [Fact]
    public void SyncVisibilityWith_shows_when_visible_and_content_set()
    {
        var webView = new WebView();
        var host = new WebViewOverlayHost(webView);
        host.Content = new object();

        host.SyncVisibilityWith(isVisible: true);

        Assert.True(host.IsVisible);
    }

    [Fact]
    public void SyncVisibilityWith_hides_when_not_visible()
    {
        var webView = new WebView();
        var host = new WebViewOverlayHost(webView);
        host.Content = new object();
        host.SyncVisibilityWith(isVisible: true);

        host.SyncVisibilityWith(isVisible: false);

        Assert.False(host.IsVisible);
    }

    [Fact]
    public void Bounds_returns_correct_value_after_UpdatePosition()
    {
        var webView = new WebView();
        var host = new WebViewOverlayHost(webView);
        var webViewBounds = new Rect(0, 0, 200, 100);
        var screenOffset = new Point(50, 25);
        var dpiScale = 1.5;

        host.UpdatePosition(webViewBounds, screenOffset, dpiScale);

        Assert.Equal(50, host.Bounds.X);
        Assert.Equal(25, host.Bounds.Y);
        Assert.Equal(300, host.Bounds.Width);
        Assert.Equal(150, host.Bounds.Height);
    }

    [Fact]
    public void HitTest_returns_Passthrough_when_no_content()
    {
        var webView = new WebView();
        var host = new WebViewOverlayHost(webView);
        host.UpdatePosition(new Rect(0, 0, 100, 100), new Point(0, 0), 1.0);

        var result = host.HitTest(50, 50);

        Assert.Equal(OverlayHitTestResult.Passthrough, result);
    }

    [Fact]
    public void HitTest_returns_Overlay_when_point_is_within_bounds()
    {
        var webView = new WebView();
        var host = new WebViewOverlayHost(webView);
        host.Content = new object();
        host.SyncVisibilityWith(isVisible: true);
        host.UpdatePosition(new Rect(0, 0, 100, 100), new Point(50, 25), 1.0);

        var result = host.HitTest(75, 50);

        Assert.Equal(OverlayHitTestResult.Overlay, result);
    }

    [Fact]
    public void HitTest_returns_Passthrough_when_point_is_outside_bounds()
    {
        var webView = new WebView();
        var host = new WebViewOverlayHost(webView);
        host.Content = new object();
        host.SyncVisibilityWith(isVisible: true);
        host.UpdatePosition(new Rect(0, 0, 100, 100), new Point(50, 25), 1.0);

        var result = host.HitTest(10, 10);

        Assert.Equal(OverlayHitTestResult.Passthrough, result);
    }

    [Fact]
    public void Focus_transfer_toggles_HasKeyboardFocus()
    {
        var webView = new WebView();
        var host = new WebViewOverlayHost(webView);

        Assert.False(host.HasKeyboardFocus);

        host.TransferFocusToOverlay();
        Assert.True(host.HasKeyboardFocus);

        host.TransferFocusToWebView();
        Assert.False(host.HasKeyboardFocus);
    }

    [Fact]
    public void NativeOverlayProviderFactory_returns_provider_for_current_platform()
    {
        var provider = NativeOverlayProviderFactory.Create();
        Assert.NotNull(provider);
        provider.Dispose();
    }

    [Fact]
    public void NativeOverlayProvider_initial_state_is_not_visible()
    {
        var provider = NativeOverlayProviderFactory.Create();
        Assert.False(provider.IsVisible);
        Assert.Equal(IntPtr.Zero, provider.OverlayHandle);
        provider.Dispose();
    }

    [Fact]
    public void NativeOverlayProvider_dispose_is_safe_when_no_overlay()
    {
        var provider = NativeOverlayProviderFactory.Create();
        provider.Dispose(); // Should not throw
        provider.Dispose(); // Double dispose should be safe
    }
}
