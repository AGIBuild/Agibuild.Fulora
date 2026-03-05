export interface BiometricAvailability {
  isAvailable: boolean;
  biometricType: string | null;
  errorReason: string | null;
}

export interface BiometricResult {
  success: boolean;
  errorCode: string | null;
  errorMessage: string | null;
}

export interface IBiometricService {
  checkAvailabilityAsync(signal?: AbortSignal): Promise<BiometricAvailability>;
  authenticateAsync(reason: string, signal?: AbortSignal): Promise<BiometricResult>;
}
