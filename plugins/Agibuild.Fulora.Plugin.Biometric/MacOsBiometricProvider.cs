namespace Agibuild.Fulora.Plugin.Biometric;

/// <summary>
/// macOS biometric provider using LocalAuthentication framework (LAContext).
/// Requires ObjCRuntime interop — stub until native binding is added.
/// </summary>
public sealed class MacOsBiometricProvider : IBiometricPlatformProvider
{
    public Task<BiometricAvailability> CheckAvailabilityAsync(CancellationToken ct = default)
        => Task.FromResult(new BiometricAvailability(
            OperatingSystem.IsMacOS(), "touchid", OperatingSystem.IsMacOS() ? null : "wrong_platform"));

    public Task<BiometricResult> AuthenticateAsync(string reason, CancellationToken ct = default)
        => Task.FromResult(new BiometricResult(false, "not_implemented",
            "macOS LAContext integration pending — requires ObjCRuntime native binding"));
}
