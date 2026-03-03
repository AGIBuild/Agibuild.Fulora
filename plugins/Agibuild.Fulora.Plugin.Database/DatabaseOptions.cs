namespace Agibuild.Fulora.Plugin.Database;

/// <summary>
/// Options for configuring the database plugin.
/// </summary>
public sealed class DatabaseOptions
{
    public string DatabasePath { get; init; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "Fulora", "fulora.db");
    public string[]? MigrationScripts { get; init; }
}
