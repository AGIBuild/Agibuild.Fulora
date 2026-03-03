using Agibuild.Fulora;

namespace Agibuild.Fulora.Plugin.FileSystem;

/// <summary>
/// Bridge service for sandboxed file system access.
/// All paths are resolved relative to a configurable root directory with path traversal prevention.
/// </summary>
[JsExport]
public interface IFileSystemService
{
    Task<string> ReadText(string path);
    Task WriteText(string path, string content);
    Task<byte[]> ReadBinary(string path);
    Task WriteBinary(string path, byte[] data);
    Task<FileEntry[]> List(string path);
    Task Delete(string path);
    Task<bool> Exists(string path);
    Task CreateDirectory(string path);
}
