namespace Agibuild.Fulora.Plugin.Database;

/// <summary>
/// Result of a SQL query containing column names and rows as dictionaries.
/// </summary>
public sealed class QueryResult
{
    public string[] Columns { get; init; } = [];
    public List<Dictionary<string, object?>> Rows { get; init; } = [];
    public int RowCount { get; init; }
}
