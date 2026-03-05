namespace Agibuild.Fulora.NativeOverlay;

public static class NativeOverlayProviderFactory
{
    public static INativeOverlayProvider Create()
    {
        if (OperatingSystem.IsWindows())
            return new WindowsNativeOverlayProvider();
        if (OperatingSystem.IsMacOS())
            return new MacOsNativeOverlayProvider();
        if (OperatingSystem.IsLinux())
            return new LinuxNativeOverlayProvider();

        throw new PlatformNotSupportedException("Native overlay is not supported on this platform.");
    }
}
