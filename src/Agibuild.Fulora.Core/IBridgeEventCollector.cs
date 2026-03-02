namespace Agibuild.Fulora;

/// <summary>
/// Collects bridge call events in a bounded buffer for DevTools consumption.
/// </summary>
public interface IBridgeEventCollector
{
    /// <summary>Current number of events in the buffer.</summary>
    int Count { get; }

    /// <summary>Number of events dropped due to buffer overflow.</summary>
    long DroppedCount { get; }

    /// <summary>Maximum number of events the buffer can hold.</summary>
    int Capacity { get; }

    /// <summary>Returns a snapshot of all buffered events, oldest first.</summary>
    IReadOnlyList<BridgeDevToolsEvent> GetEvents();

    /// <summary>Clears all buffered events and resets the dropped count.</summary>
    void Clear();

    /// <summary>
    /// Registers a callback invoked whenever a new event is added.
    /// Returns a disposable that unregisters the callback.
    /// </summary>
    IDisposable Subscribe(Action<BridgeDevToolsEvent> onEvent);
}
