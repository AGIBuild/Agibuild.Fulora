namespace Agibuild.Fulora;

/// <summary>
/// Options for the Bridge Debug Server (WebSocket streaming of bridge events).
/// </summary>
public sealed class BridgeDebugServerOptions
{
    /// <summary>
    /// When true, the debug server starts and streams bridge events to connected clients.
    /// Default is false (opt-in).
    /// </summary>
    public bool Enabled { get; init; }

    /// <summary>
    /// Port to listen on. Default is 9229.
    /// </summary>
    public int Port { get; init; } = 9229;
}
