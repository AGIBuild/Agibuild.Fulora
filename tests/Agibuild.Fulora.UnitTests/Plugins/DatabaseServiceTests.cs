using Agibuild.Fulora.Plugin.Database;
using Xunit;

namespace Agibuild.Fulora.UnitTests.Plugins;

public sealed class DatabaseServiceTests
{
    private static DatabaseService CreateInMemoryService()
    {
        return new DatabaseService(new DatabaseOptions { DatabasePath = ":memory:" });
    }

    [Fact]
    public async Task Query_ReturnsCorrectColumnsAndRows()
    {
        var db = CreateInMemoryService();
        await db.Execute("CREATE TABLE t (id INTEGER, name TEXT)", null);
        await db.Execute("INSERT INTO t (id, name) VALUES (1, 'a'), (2, 'b')", null);

        var result = await db.Query("SELECT id, name FROM t ORDER BY id");

        Assert.Equal(["id", "name"], result.Columns);
        Assert.Equal(2, result.RowCount);
        Assert.Equal(2, result.Rows.Count);
        Assert.Equal(1L, result.Rows[0]["id"]);
        Assert.Equal("a", result.Rows[0]["name"]);
        Assert.Equal(2L, result.Rows[1]["id"]);
        Assert.Equal("b", result.Rows[1]["name"]);
    }

    [Fact]
    public async Task Execute_ReturnsAffectedRowCount()
    {
        var db = CreateInMemoryService();
        await db.Execute("CREATE TABLE t (x INTEGER)", null);

        var n = await db.Execute("INSERT INTO t (x) VALUES (1), (2), (3)", null);
        Assert.Equal(3, n);

        n = await db.Execute("UPDATE t SET x = 0 WHERE x = 2", null);
        Assert.Equal(1, n);

        n = await db.Execute("DELETE FROM t WHERE x = 0", null);
        Assert.Equal(1, n);
    }

    [Fact]
    public async Task ParameterizedQueries_Work()
    {
        var db = CreateInMemoryService();
        await db.Execute("CREATE TABLE t (id INTEGER, name TEXT)", null);
        await db.Execute("INSERT INTO t (id, name) VALUES (@id, @name)", new Dictionary<string, object?> { ["id"] = 42, ["name"] = "test" });

        var result = await db.Query("SELECT id, name FROM t WHERE id = @id", new Dictionary<string, object?> { ["id"] = 42 });

        Assert.Single(result.Rows);
        Assert.Equal(42L, result.Rows[0]["id"]);
        Assert.Equal("test", result.Rows[0]["name"]);
    }

    [Fact]
    public async Task Transaction_Commit_Persists()
    {
        var db = CreateInMemoryService();
        await db.Execute("CREATE TABLE t (x INTEGER)", null);

        await db.BeginTransaction();
        await db.Execute("INSERT INTO t (x) VALUES (1)", null);
        await db.CommitTransaction();

        var result = await db.Query("SELECT x FROM t");
        Assert.Single(result.Rows);
        Assert.Equal(1L, result.Rows[0]["x"]);
    }

    [Fact]
    public async Task Transaction_Rollback_Reverts()
    {
        var db = CreateInMemoryService();
        await db.Execute("CREATE TABLE t (x INTEGER)", null);
        await db.Execute("INSERT INTO t (x) VALUES (99)", null);

        await db.BeginTransaction();
        await db.Execute("INSERT INTO t (x) VALUES (1), (2)", null);
        await db.RollbackTransaction();

        var result = await db.Query("SELECT x FROM t");
        Assert.Single(result.Rows);
        Assert.Equal(99L, result.Rows[0]["x"]);
    }

    [Fact]
    public async Task GetSchemaVersion_Returns0_Initially()
    {
        var db = CreateInMemoryService();
        var version = await db.GetSchemaVersion();
        Assert.Equal(0, version);
    }

    [Fact]
    public async Task ExecuteBatch_RunsAllStatements()
    {
        var db = CreateInMemoryService();
        var statements = new[]
        {
            "CREATE TABLE t (id INTEGER PRIMARY KEY, name TEXT)",
            "INSERT INTO t (id, name) VALUES (1, 'a')",
            "INSERT INTO t (id, name) VALUES (2, 'b')",
            "INSERT INTO t (id, name) VALUES (3, 'c')",
        };

        var total = await db.ExecuteBatch(statements);
        Assert.Equal(3, total);

        var result = await db.Query("SELECT COUNT(*) as cnt FROM t");
        Assert.Equal(1, result.Rows.Count);
        Assert.Equal(3L, result.Rows[0]["cnt"]);
    }

    [Fact]
    public async Task Query_EmptyResult_ReturnsColumnsAndEmptyRows()
    {
        var db = CreateInMemoryService();
        await db.Execute("CREATE TABLE t (x INTEGER)", null);

        var result = await db.Query("SELECT x FROM t WHERE 1=0");

        Assert.Equal(["x"], result.Columns);
        Assert.Empty(result.Rows);
        Assert.Equal(0, result.RowCount);
    }

    [Fact]
    public async Task GetSchemaVersion_AfterSchemaVersionTable_Returns0()
    {
        var db = CreateInMemoryService();
        await db.GetSchemaVersion();
        var version = await db.GetSchemaVersion();
        Assert.Equal(0, version);
    }
}
