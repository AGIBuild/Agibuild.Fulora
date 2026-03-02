using System.Text.Json.Serialization;

namespace Agibuild.Fulora;

/// <summary>
/// Represents a single bridge call event captured for the DevTools panel.
/// </summary>
public sealed record BridgeDevToolsEvent
{
    /// <summary>Monotonically increasing event ID within the collector session.</summary>
    public long Id { get; init; }

    /// <summary>UTC timestamp when the event was recorded.</summary>
    public DateTimeOffset Timestamp { get; init; }

    /// <summary>Call direction: Export (JS→C#) or Import (C#→JS).</summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public BridgeCallDirection Direction { get; init; }

    /// <summary>Bridge lifecycle phase.</summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public BridgeCallPhase Phase { get; init; }

    /// <summary>Service name (e.g. "AppService").</summary>
    public string ServiceName { get; init; } = "";

    /// <summary>Method name (e.g. "getCurrentUser").</summary>
    public string MethodName { get; init; } = "";

    /// <summary>Serialized params JSON (truncated if larger than threshold).</summary>
    public string? ParamsJson { get; init; }

    /// <summary>Result type name or serialized result (truncated).</summary>
    public string? ResultJson { get; init; }

    /// <summary>Error message if the call failed.</summary>
    public string? ErrorMessage { get; init; }

    /// <summary>Elapsed time in milliseconds (only for End/Error phases).</summary>
    public long? ElapsedMs { get; init; }

    /// <summary>True if the payload was truncated to fit the buffer threshold.</summary>
    public bool Truncated { get; init; }
}

/// <summary>Direction of a bridge call.</summary>
public enum BridgeCallDirection
{
    /// <summary>JS → C# (exported service called from JavaScript).</summary>
    Export,

    /// <summary>C# → JS (imported service called from C#).</summary>
    Import,

    /// <summary>Lifecycle event (service exposed/removed).</summary>
    Lifecycle,
}

/// <summary>Phase of a bridge call lifecycle.</summary>
public enum BridgeCallPhase
{
    /// <summary>Call started.</summary>
    Start,

    /// <summary>Call completed successfully.</summary>
    End,

    /// <summary>Call failed with an error.</summary>
    Error,

    /// <summary>Service was exposed.</summary>
    ServiceExposed,

    /// <summary>Service was removed.</summary>
    ServiceRemoved,
}
