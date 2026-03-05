namespace Agibuild.Fulora;

/// <summary>
/// Bridge service implementation that exposes drag-and-drop events to JavaScript.
/// </summary>
public sealed class DragDropBridgeService : IDragDropBridgeService
{
    private DragDropPayload? _lastPayload;
    private readonly bool _isSupported;

    /// <summary>
    /// Creates a new <see cref="DragDropBridgeService"/> that subscribes to the given core's drop events.
    /// </summary>
    public DragDropBridgeService(WebViewCore core)
    {
        ArgumentNullException.ThrowIfNull(core);
        _isSupported = core.HasDragDropSupport;
        core.DropCompleted += (_, e) => _lastPayload = e.Payload;
    }

    /// <inheritdoc />
    public Task<DragDropPayload?> GetLastDropPayloadAsync(CancellationToken ct = default)
        => Task.FromResult(_lastPayload);

    /// <inheritdoc />
    public Task<bool> IsDragDropSupportedAsync(CancellationToken ct = default)
        => Task.FromResult(_isSupported);
}
