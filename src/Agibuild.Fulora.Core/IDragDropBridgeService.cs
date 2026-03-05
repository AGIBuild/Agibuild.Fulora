namespace Agibuild.Fulora;

/// <summary>
/// Bridge service that exposes drag-and-drop events to JavaScript.
/// Events are delivered as streaming callbacks via bridge import proxies.
/// </summary>
[JsExport]
public interface IDragDropBridgeService
{
    Task<DragDropPayload?> GetLastDropPayloadAsync(CancellationToken ct = default);
    Task<bool> IsDragDropSupportedAsync(CancellationToken ct = default);
}
