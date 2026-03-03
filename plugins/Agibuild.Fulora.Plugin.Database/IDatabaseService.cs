using Agibuild.Fulora;

namespace Agibuild.Fulora.Plugin.Database;

/// <summary>
/// Bridge service for SQLite database access.
/// Provides query, execute, and transaction operations backed by Microsoft.Data.Sqlite.
/// </summary>
[JsExport]
public interface IDatabaseService
{
    Task<QueryResult> Query(string sql, Dictionary<string, object?>? parameters = null);
    Task<int> Execute(string sql, Dictionary<string, object?>? parameters = null);
    Task<int> ExecuteBatch(string[] statements);
    Task BeginTransaction();
    Task CommitTransaction();
    Task RollbackTransaction();
    Task<int> GetSchemaVersion();
}
