using System.Runtime.Versioning;

namespace Agibuild.Fulora.NativeOverlay;

/// <summary>
/// Linux native overlay using GTK RGBA window.
/// The window uses RGBA visual for transparency and is positioned
/// to track the WebView widget.
/// </summary>
[SupportedOSPlatform("linux")]
internal sealed class LinuxNativeOverlayProvider : INativeOverlayProvider
{
    private IntPtr _windowHandle;
    private bool _isVisible;

    public IntPtr OverlayHandle => _windowHandle;
    public bool IsVisible => _isVisible;

    public void CreateOverlay(IntPtr parentHandle)
    {
        if (_windowHandle != IntPtr.Zero) return;
        // GTK overlay creation:
        // GtkWidget *win = gtk_window_new(GTK_WINDOW_POPUP);
        // GdkScreen *screen = gtk_widget_get_screen(win);
        // GdkVisual *visual = gdk_screen_get_rgba_visual(screen);
        // if (visual) gtk_widget_set_visual(win, visual);
        // gtk_widget_set_app_paintable(win, TRUE);
        // gtk_window_set_transient_for(GTK_WINDOW(win), GTK_WINDOW(parent));
        //
        // Stub — native GTK shim integration pending.
    }

    public void DestroyOverlay()
    {
        if (_windowHandle != IntPtr.Zero)
        {
            // gtk_widget_destroy(_windowHandle);
            _windowHandle = IntPtr.Zero;
        }
        _isVisible = false;
    }

    public void UpdateBounds(double x, double y, double width, double height, double dpiScale)
    {
        if (_windowHandle == IntPtr.Zero) return;
        // gtk_window_move(GTK_WINDOW(win), (int)(x*dpi), (int)(y*dpi));
        // gtk_window_resize(GTK_WINDOW(win), (int)(w*dpi), (int)(h*dpi));
    }

    public void Show()
    {
        if (_windowHandle == IntPtr.Zero) return;
        // gtk_widget_show_all(win);
        _isVisible = true;
    }

    public void Hide()
    {
        if (_windowHandle == IntPtr.Zero) return;
        // gtk_widget_hide(win);
        _isVisible = false;
    }

    public void Dispose() => DestroyOverlay();
}
