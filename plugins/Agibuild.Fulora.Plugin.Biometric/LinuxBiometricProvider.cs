namespace Agibuild.Fulora.Plugin.Biometric;

public sealed class LinuxBiometricProvider : IBiometricPlatformProvider
{
    public Task<BiometricAvailability> CheckAvailabilityAsync(CancellationToken ct = default)
        => Task.FromResult(new BiometricAvailability(false, null, "platform_not_supported"));

    public Task<BiometricResult> AuthenticateAsync(string reason, CancellationToken ct = default)
        => Task.FromResult(new BiometricResult(false, "not_available", "Biometric authentication is not supported on Linux"));
}
