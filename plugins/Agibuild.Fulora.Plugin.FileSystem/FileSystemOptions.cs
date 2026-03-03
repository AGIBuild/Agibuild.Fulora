namespace Agibuild.Fulora.Plugin.FileSystem;

/// <summary>
/// Configuration options for the file system plugin.
/// </summary>
public sealed class FileSystemOptions
{
    /// <summary>
    /// Root directory for all file operations. All paths are resolved relative to this.
    /// Defaults to <c>{AppData}/Fulora/files</c>.
    /// </summary>
    public string RootDirectory { get; init; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "Fulora", "files");

    /// <summary>
    /// When false, write operations (WriteText, WriteBinary, Delete, CreateDirectory) throw <see cref="UnauthorizedAccessException"/>.
    /// </summary>
    public bool AllowWrite { get; init; } = true;
}
