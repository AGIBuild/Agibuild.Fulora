using Agibuild.Fulora;

namespace ShowcaseTodo.Bridge;

[JsExport]
public interface ITodoService
{
    Task<TodoItem[]> GetAll();
    Task<TodoItem> Create(string title);
    Task<TodoItem> Update(int id, string? title = null, bool? completed = null);
    Task Delete(int id);
}

public class TodoItem
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public bool Completed { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
