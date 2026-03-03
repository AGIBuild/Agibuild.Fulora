namespace Agibuild.Fulora.Plugin.FileSystem;

/// <summary>
/// Represents a file or directory entry returned by <see cref="IFileSystemService.List"/>.
/// </summary>
public sealed class FileEntry
{
    public string Name { get; init; } = "";
    public string Path { get; init; } = "";
    public bool IsDirectory { get; init; }
    public long Size { get; init; }
    public DateTimeOffset LastModified { get; init; }
}
