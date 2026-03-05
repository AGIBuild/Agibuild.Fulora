using System.Runtime.Versioning;

namespace Agibuild.Fulora.NativeOverlay;

/// <summary>
/// macOS native overlay using NSPanel via ObjC runtime interop.
/// The panel is borderless, non-activating, and transparent.
/// </summary>
[SupportedOSPlatform("macos")]
internal sealed class MacOsNativeOverlayProvider : INativeOverlayProvider
{
    private IntPtr _panelHandle;
    private IntPtr _parentWindowHandle;
    private bool _isVisible;

    public IntPtr OverlayHandle => _panelHandle;
    public bool IsVisible => _isVisible;

    public void CreateOverlay(IntPtr parentHandle)
    {
        if (_panelHandle != IntPtr.Zero) return;
        _parentWindowHandle = parentHandle;
        // NSPanel creation via ObjC runtime:
        // NSPanel *panel = [[NSPanel alloc] initWithContentRect:NSZeroRect
        //     styleMask:NSWindowStyleMaskBorderless | NSWindowStyleMaskNonactivatingPanel
        //     backing:NSBackingStoreBuffered defer:YES];
        // [panel setOpaque:NO];
        // [panel setBackgroundColor:[NSColor clearColor]];
        // [panel setIgnoresMouseEvents:YES];
        // [panel setLevel:NSFloatingWindowLevel];
        // [parentWindow addChildWindow:panel ordered:NSWindowAbove];
        //
        // This requires ObjC runtime interop which is best done in the native shim.
        // Stub — native integration pending.
    }

    public void DestroyOverlay()
    {
        if (_panelHandle != IntPtr.Zero)
        {
            // [panel orderOut:nil];
            // [parentWindow removeChildWindow:panel];
            _panelHandle = IntPtr.Zero;
        }
        _isVisible = false;
    }

    public void UpdateBounds(double x, double y, double width, double height, double dpiScale)
    {
        if (_panelHandle == IntPtr.Zero) return;
        // [panel setFrame:NSMakeRect(x*dpi, y*dpi, w*dpi, h*dpi) display:YES];
    }

    public void Show()
    {
        if (_panelHandle == IntPtr.Zero) return;
        // [panel orderFront:nil];
        _isVisible = true;
    }

    public void Hide()
    {
        if (_panelHandle == IntPtr.Zero) return;
        // [panel orderOut:nil];
        _isVisible = false;
    }

    public void Dispose() => DestroyOverlay();
}
