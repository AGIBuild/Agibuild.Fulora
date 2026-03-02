using System.Collections.Concurrent;

namespace Agibuild.Fulora;

/// <summary>
/// Thread-safe bounded ring buffer that collects <see cref="BridgeDevToolsEvent"/> instances.
/// When the buffer is full, the oldest events are dropped.
/// </summary>
public sealed class BridgeEventCollector : IBridgeEventCollector
{
    private readonly object _lock = new();
    private readonly BridgeDevToolsEvent[] _buffer;
    private int _head;
    private int _count;
    private long _nextId;
    private long _droppedCount;
    private readonly ConcurrentBag<Action<BridgeDevToolsEvent>> _subscribers = [];

    public BridgeEventCollector(int capacity = 500)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(capacity, 1);
        _buffer = new BridgeDevToolsEvent[capacity];
    }

    public int Capacity => _buffer.Length;

    public int Count
    {
        get { lock (_lock) return _count; }
    }

    public long DroppedCount
    {
        get { lock (_lock) return _droppedCount; }
    }

    internal void Add(BridgeDevToolsEvent evt)
    {
        BridgeDevToolsEvent stamped;
        lock (_lock)
        {
            stamped = evt with { Id = _nextId++ };

            var index = (_head + _count) % _buffer.Length;
            if (_count == _buffer.Length)
            {
                _head = (_head + 1) % _buffer.Length;
                _droppedCount++;
            }
            else
            {
                _count++;
            }
            _buffer[index] = stamped;
        }

        foreach (var subscriber in _subscribers)
        {
            try { subscriber(stamped); }
            catch { /* subscriber failures must not break the collector */ }
        }
    }

    public IReadOnlyList<BridgeDevToolsEvent> GetEvents()
    {
        lock (_lock)
        {
            var result = new BridgeDevToolsEvent[_count];
            for (var i = 0; i < _count; i++)
                result[i] = _buffer[(_head + i) % _buffer.Length];
            return result;
        }
    }

    public void Clear()
    {
        lock (_lock)
        {
            Array.Clear(_buffer, 0, _buffer.Length);
            _head = 0;
            _count = 0;
            _droppedCount = 0;
        }
    }

    public IDisposable Subscribe(Action<BridgeDevToolsEvent> onEvent)
    {
        ArgumentNullException.ThrowIfNull(onEvent);
        _subscribers.Add(onEvent);
        return new Subscription(this, onEvent);
    }

    private sealed class Subscription(BridgeEventCollector collector, Action<BridgeDevToolsEvent> callback) : IDisposable
    {
        public void Dispose()
        {
            var bag = collector._subscribers;
            var remaining = new ConcurrentBag<Action<BridgeDevToolsEvent>>();
            while (bag.TryTake(out var item))
            {
                if (!ReferenceEquals(item, callback))
                    remaining.Add(item);
            }
            while (remaining.TryTake(out var item))
                bag.Add(item);
        }
    }
}
