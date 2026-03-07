using Agibuild.Fulora;
using Xunit;

namespace Agibuild.Fulora.UnitTests;

public class SharedStateStoreTests
{
    private readonly SharedStateStore _store = new();

    [Fact]
    public void Set_And_Get_ReturnsValue()
    {
        _store.Set("theme", "{\"dark\":true}");
        Assert.Equal("{\"dark\":true}", _store.Get("theme"));
    }

    [Fact]
    public void Get_NonExistentKey_ReturnsNull()
    {
        Assert.Null(_store.Get("missing"));
    }

    [Fact]
    public void TryGet_ExistingKey_ReturnsTrueWithValue()
    {
        _store.Set("key", "value");
        Assert.True(_store.TryGet("key", out var value));
        Assert.Equal("value", value);
    }

    [Fact]
    public void TryGet_NonExistentKey_ReturnsFalse()
    {
        Assert.False(_store.TryGet("missing", out var value));
        Assert.Null(value);
    }

    [Fact]
    public void Remove_ExistingKey_ReturnsTrue()
    {
        _store.Set("key", "value");
        Assert.True(_store.Remove("key"));
        Assert.Null(_store.Get("key"));
    }

    [Fact]
    public void Remove_NonExistentKey_ReturnsFalse()
    {
        Assert.False(_store.Remove("missing"));
    }

    [Fact]
    public void Remove_FiresStateChanged()
    {
        _store.Set("key", "value");
        StateChangedEventArgs? args = null;
        _store.StateChanged += (_, e) => args = e;

        _store.Remove("key");

        Assert.NotNull(args);
        Assert.Equal("key", args.Key);
        Assert.Equal("value", args.OldValue);
        Assert.Null(args.NewValue);
    }

    [Fact]
    public void Set_FiresStateChanged()
    {
        StateChangedEventArgs? args = null;
        _store.StateChanged += (_, e) => args = e;

        _store.Set("key", "value");

        Assert.NotNull(args);
        Assert.Equal("key", args.Key);
        Assert.Null(args.OldValue);
        Assert.Equal("value", args.NewValue);
    }

    [Fact]
    public void Set_SameValue_DoesNotFireStateChanged()
    {
        _store.Set("key", "value");
        var eventCount = 0;
        _store.StateChanged += (_, _) => eventCount++;

        _store.Set("key", "value");

        Assert.Equal(0, eventCount);
    }

    [Fact]
    public void Set_DifferentValue_FiresStateChanged()
    {
        _store.Set("key", "old");
        StateChangedEventArgs? args = null;
        _store.StateChanged += (_, e) => args = e;

        _store.Set("key", "new");

        Assert.NotNull(args);
        Assert.Equal("old", args.OldValue);
        Assert.Equal("new", args.NewValue);
    }

    [Fact]
    public void LWW_LaterTimestampWins()
    {
        var t1 = DateTimeOffset.UtcNow;
        var t2 = t1.AddSeconds(1);

        _store.Set("key", "first", t1);
        _store.Set("key", "second", t2);

        Assert.Equal("second", _store.Get("key"));
    }

    [Fact]
    public void LWW_StaleWriteIgnored()
    {
        var t1 = DateTimeOffset.UtcNow;
        var t2 = t1.AddSeconds(-1);

        _store.Set("key", "fresh", t1);
        _store.Set("key", "stale", t2);

        Assert.Equal("fresh", _store.Get("key"));
    }

    [Fact]
    public void LWW_StaleWrite_DoesNotFireStateChanged()
    {
        var t1 = DateTimeOffset.UtcNow;
        _store.Set("key", "fresh", t1);

        var eventCount = 0;
        _store.StateChanged += (_, _) => eventCount++;

        _store.Set("key", "stale", t1.AddSeconds(-1));

        Assert.Equal(0, eventCount);
    }

    [Fact]
    public void GetSnapshot_ReturnsAllEntries()
    {
        _store.Set("a", "1");
        _store.Set("b", "2");
        _store.Set("c", "3");

        var snapshot = _store.GetSnapshot();

        Assert.Equal(3, snapshot.Count);
        Assert.Equal("1", snapshot["a"]);
        Assert.Equal("2", snapshot["b"]);
        Assert.Equal("3", snapshot["c"]);
    }

    [Fact]
    public void GetSnapshot_IsImmutable()
    {
        _store.Set("key", "before");
        var snapshot = _store.GetSnapshot();

        _store.Set("key", "after");

        Assert.Equal("before", snapshot["key"]);
    }

    [Fact]
    public void SetGeneric_And_GetGeneric_RoundTrips()
    {
        var prefs = new TestPrefs { Theme = "dark", FontSize = 14 };
        _store.Set("prefs", prefs);

        var result = _store.Get<TestPrefs>("prefs");

        Assert.NotNull(result);
        Assert.Equal("dark", result.Theme);
        Assert.Equal(14, result.FontSize);
    }

    [Fact]
    public void GetGeneric_NonExistentKey_ReturnsDefault()
    {
        var result = _store.Get<TestPrefs>("missing");
        Assert.Null(result);
    }

    [Fact]
    public void Set_NullKey_ThrowsArgument()
    {
        Assert.Throws<ArgumentNullException>(() => _store.Set(null!, "value"));
    }

    [Fact]
    public void Get_NullKey_ThrowsArgument()
    {
        Assert.Throws<ArgumentNullException>(() => _store.Get(null!));
    }

    [Fact]
    public void Remove_NullKey_ThrowsArgument()
    {
        Assert.Throws<ArgumentNullException>(() => _store.Remove(null!));
    }

    [Fact]
    public void Set_NullValue_Allowed()
    {
        _store.Set("key", (string?)null);
        Assert.True(_store.TryGet("key", out var val));
        Assert.Null(val);
    }

    private sealed class TestPrefs
    {
        public string? Theme { get; set; }
        public int FontSize { get; set; }
    }
}
