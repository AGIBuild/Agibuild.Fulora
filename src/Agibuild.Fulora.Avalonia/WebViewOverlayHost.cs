using Avalonia;

namespace Agibuild.Fulora;

/// <summary>
/// Manages a companion overlay for rendering Avalonia controls above WebView.
/// Core behavior: tracks WebView bounds and renders OverlayContent.
/// </summary>
public sealed class WebViewOverlayHost : IDisposable
{
    private readonly WebView _owner;
    private bool _isVisible;
    private bool _disposed;
    private Rect _bounds;
    private double _dpiScale;

    internal WebViewOverlayHost(WebView owner)
    {
        _owner = owner ?? throw new ArgumentNullException(nameof(owner));
    }

    /// <summary>
    /// The Avalonia visual to render in the overlay.
    /// </summary>
    public object? Content { get; set; }

    /// <summary>
    /// Whether the overlay is currently visible.
    /// </summary>
    public bool IsVisible => _isVisible;

    /// <summary>
    /// DPI-adjusted bounds of the overlay.
    /// </summary>
    public Rect Bounds => _bounds;

    /// <summary>
    /// Current DPI scale factor applied to bounds.
    /// </summary>
    public double DpiScale => _dpiScale;

    /// <summary>
    /// Updates the overlay position to match WebView bounds.
    /// </summary>
    internal void UpdatePosition(Rect webViewBounds, Point screenOffset, double dpiScale)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _bounds = new Rect(
            screenOffset.X + webViewBounds.X * dpiScale,
            screenOffset.Y + webViewBounds.Y * dpiScale,
            webViewBounds.Width * dpiScale,
            webViewBounds.Height * dpiScale);
        _dpiScale = dpiScale;
    }

    /// <summary>
    /// Syncs overlay visibility with the WebView visibility.
    /// </summary>
    public void SyncVisibilityWith(bool isVisible)
    {
        if (isVisible && Content is not null)
            Show();
        else
            Hide();
    }

    internal void Show()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _isVisible = true;
    }

    internal void Hide()
    {
        _isVisible = false;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        Content = null;
        _isVisible = false;
    }
}
