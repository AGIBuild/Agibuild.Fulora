namespace Agibuild.Fulora;

/// <summary>
/// Specific bridge call error codes with actionable diagnostic information.
/// </summary>
public enum BridgeErrorCode
{
    Unknown = -1,
    ServiceNotFound = 1001,
    MethodNotFound = 1002,
    ParameterMismatch = 1003,
    SerializationError = 1004,
    InvocationError = 1005,
    TimeoutError = 1006,
    CancellationError = 1007,
    PermissionDenied = 1008,
    RateLimitExceeded = 1009,
}

/// <summary>
/// Rich error diagnostic with error code, message, and optional actionable hint.
/// </summary>
public sealed record BridgeErrorDiagnostic(
    BridgeErrorCode Code,
    string Message,
    string? Hint = null)
{
    /// <summary>
    /// Creates a diagnostic for a service not found error.
    /// </summary>
    public static BridgeErrorDiagnostic ServiceNotFound(string serviceName) => new(
        BridgeErrorCode.ServiceNotFound,
        $"Service '{serviceName}' is not registered.",
        $"Did you forget to call bridge.Expose<I{serviceName}>() or bridge.UsePlugin<...>()?");

    public static BridgeErrorDiagnostic MethodNotFound(string serviceName, string methodName) => new(
        BridgeErrorCode.MethodNotFound,
        $"Method '{methodName}' not found on service '{serviceName}'.",
        $"Check that '{methodName}' is declared on the [JsExport] interface for {serviceName} and the service is re-exposed after changes.");

    public static BridgeErrorDiagnostic ParameterMismatch(string serviceName, string methodName, string details) => new(
        BridgeErrorCode.ParameterMismatch,
        $"Parameter mismatch calling '{serviceName}.{methodName}': {details}",
        "Verify the TypeScript call signature matches the C# interface. Re-run 'fulora generate' to update types.");

    public static BridgeErrorDiagnostic SerializationError(string serviceName, string methodName, string details) => new(
        BridgeErrorCode.SerializationError,
        $"Serialization error for '{serviceName}.{methodName}': {details}",
        $"For {serviceName}.{methodName}: check that all parameter and return types are JSON-serializable. Ensure the [JsonSerializable] context includes these types.");

    public static BridgeErrorDiagnostic InvocationError(string serviceName, string methodName, string exceptionMessage) => new(
        BridgeErrorCode.InvocationError,
        $"Error invoking '{serviceName}.{methodName}': {exceptionMessage}",
        null);

    public static BridgeErrorDiagnostic Timeout(string serviceName, string methodName) => new(
        BridgeErrorCode.TimeoutError,
        $"Call to '{serviceName}.{methodName}' timed out.",
        $"For {serviceName}.{methodName}: consider increasing the timeout or investigating why the method is taking too long.");

    public static BridgeErrorDiagnostic Cancellation(string serviceName, string methodName) => new(
        BridgeErrorCode.CancellationError,
        $"Call to '{serviceName}.{methodName}' was cancelled.",
        null);

    /// <summary>
    /// Maps to JSON-RPC error code in the range -32000 to -32099 for server implementation-defined errors.
    /// </summary>
    internal static int ToJsonRpcCode(BridgeErrorCode code) => code switch
    {
        BridgeErrorCode.ServiceNotFound => -32601,
        BridgeErrorCode.MethodNotFound => -32601,
        BridgeErrorCode.ParameterMismatch => -32602,
        BridgeErrorCode.SerializationError => -32602,
        BridgeErrorCode.InvocationError => -32603,
        BridgeErrorCode.TimeoutError => -32603,
        BridgeErrorCode.CancellationError => -32603,
        BridgeErrorCode.PermissionDenied => -32603,
        BridgeErrorCode.RateLimitExceeded => -32029,
        _ => -32603,
    };
}
