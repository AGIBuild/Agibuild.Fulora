using System.Collections.Concurrent;

namespace ShowcaseTodo.Bridge;

public sealed class TodoService : ITodoService
{
    private readonly ConcurrentDictionary<int, TodoItem> _items = new();
    private int _nextId = 1;

    public Task<TodoItem[]> GetAll()
    {
        var items = _items.Values.OrderBy(x => x.CreatedAt).ToArray();
        return Task.FromResult(items);
    }

    public Task<TodoItem> Create(string title)
    {
        var item = new TodoItem
        {
            Id = Interlocked.Increment(ref _nextId),
            Title = title ?? "",
            Completed = false,
            CreatedAt = DateTimeOffset.UtcNow
        };
        _items[item.Id] = item;
        return Task.FromResult(item);
    }

    public Task<TodoItem> Update(int id, string? title = null, bool? completed = null)
    {
        if (!_items.TryGetValue(id, out var item))
            throw new KeyNotFoundException($"Todo item {id} not found.");

        if (title is not null)
            item.Title = title;
        if (completed is not null)
            item.Completed = completed.Value;

        return Task.FromResult(item);
    }

    public Task Delete(int id)
    {
        _items.TryRemove(id, out _);
        return Task.CompletedTask;
    }
}
