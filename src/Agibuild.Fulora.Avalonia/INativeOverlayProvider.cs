namespace Agibuild.Fulora.NativeOverlay;

/// <summary>
/// Platform-specific provider for creating and managing native overlay windows above WebView.
/// </summary>
public interface INativeOverlayProvider : IDisposable
{
    void CreateOverlay(IntPtr parentHandle);
    void DestroyOverlay();
    void UpdateBounds(double x, double y, double width, double height, double dpiScale);
    void Show();
    void Hide();
    bool IsVisible { get; }
    IntPtr OverlayHandle { get; }
}
