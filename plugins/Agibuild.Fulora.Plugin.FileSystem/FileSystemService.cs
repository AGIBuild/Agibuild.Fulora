namespace Agibuild.Fulora.Plugin.FileSystem;

/// <summary>
/// Sandboxed implementation of <see cref="IFileSystemService"/>.
/// All paths are resolved relative to RootDirectory with path traversal prevention.
/// </summary>
public sealed class FileSystemService : IFileSystemService
{
    private readonly string _rootDirectory;
    private readonly bool _allowWrite;

    public FileSystemService(FileSystemOptions? options = null)
    {
        var opts = options ?? new FileSystemOptions();
        _rootDirectory = Path.GetFullPath(opts.RootDirectory);
        _allowWrite = opts.AllowWrite;
    }

    public async Task<string> ReadText(string path)
    {
        var fullPath = ResolveAndValidate(path, allowWrite: false);
        return await File.ReadAllTextAsync(fullPath);
    }

    public async Task WriteText(string path, string content)
    {
        EnsureWriteAllowed();
        var fullPath = ResolveAndValidate(path, allowWrite: true);
        var dir = Path.GetDirectoryName(fullPath);
        if (dir is not null && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);
        await File.WriteAllTextAsync(fullPath, content);
    }

    public async Task<byte[]> ReadBinary(string path)
    {
        var fullPath = ResolveAndValidate(path, allowWrite: false);
        return await File.ReadAllBytesAsync(fullPath);
    }

    public async Task WriteBinary(string path, byte[] data)
    {
        EnsureWriteAllowed();
        var fullPath = ResolveAndValidate(path, allowWrite: true);
        var dir = Path.GetDirectoryName(fullPath);
        if (dir is not null && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);
        await File.WriteAllBytesAsync(fullPath, data);
    }

    public Task<FileEntry[]> List(string path)
    {
        var fullPath = ResolveAndValidate(path, allowWrite: false);
        if (!Directory.Exists(fullPath))
            throw new DirectoryNotFoundException($"Directory not found: {path}");

        var entries = System.IO.Directory.GetFileSystemEntries(fullPath);
        var results = new List<FileEntry>();

        foreach (var entry in entries)
        {
            var name = Path.GetFileName(entry);
            var isDir = Directory.Exists(entry);
            long size = 0;
            DateTime lastWrite = default;
            if (isDir)
            {
                var dirInfo = new DirectoryInfo(entry);
                lastWrite = dirInfo.LastWriteTimeUtc;
            }
            else
            {
                var fileInfo = new FileInfo(entry);
                size = fileInfo.Length;
                lastWrite = fileInfo.LastWriteTimeUtc;
            }
            var relativePath = Path.GetRelativePath(_rootDirectory, entry).Replace('\\', '/');
            results.Add(new FileEntry
            {
                Name = name,
                Path = relativePath,
                IsDirectory = isDir,
                Size = size,
                LastModified = lastWrite
            });
        }

        return Task.FromResult(results.ToArray());
    }

    public Task Delete(string path)
    {
        EnsureWriteAllowed();
        var fullPath = ResolveAndValidate(path, allowWrite: true);
        if (File.Exists(fullPath))
            File.Delete(fullPath);
        else if (Directory.Exists(fullPath))
            Directory.Delete(fullPath, recursive: true);
        else
            throw new FileNotFoundException($"Path not found: {path}");
        return Task.CompletedTask;
    }

    public Task<bool> Exists(string path)
    {
        var fullPath = ResolveAndValidate(path, allowWrite: false);
        return Task.FromResult(File.Exists(fullPath) || Directory.Exists(fullPath));
    }

    public Task CreateDirectory(string path)
    {
        EnsureWriteAllowed();
        var fullPath = ResolveAndValidate(path, allowWrite: true);
        Directory.CreateDirectory(fullPath);
        return Task.CompletedTask;
    }

    private string ResolveAndValidate(string path, bool allowWrite)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Path cannot be null or empty.", nameof(path));

        var normalized = path.Replace('\\', '/').TrimStart('/');
        if (normalized.Contains("..", StringComparison.Ordinal))
            throw new UnauthorizedAccessException("Path traversal (..) is not allowed.");

        var fullPath = Path.GetFullPath(Path.Combine(_rootDirectory, normalized));
        if (!fullPath.StartsWith(_rootDirectory, StringComparison.OrdinalIgnoreCase))
            throw new UnauthorizedAccessException("Path is outside the allowed root directory.");

        return fullPath;
    }

    private void EnsureWriteAllowed()
    {
        if (!_allowWrite)
            throw new UnauthorizedAccessException("Write operations are not allowed. Set AllowWrite=true in FileSystemOptions.");
    }
}
