using Agibuild.Fulora.Plugin.FileSystem;
using Xunit;

namespace Agibuild.Fulora.UnitTests.Plugins;

public class FileSystemServiceTests : IDisposable
{
    private readonly string _tempDir;

    public FileSystemServiceTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "fulora-fs-test-" + Guid.NewGuid().ToString("N")[..8]);
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, recursive: true);
    }

    private FileSystemService CreateService(bool allowWrite = true) =>
        new(new FileSystemOptions { RootDirectory = _tempDir, AllowWrite = allowWrite });

    [Fact]
    public async Task ReadText_WriteText_Roundtrip()
    {
        var svc = CreateService();
        await svc.WriteText("test.txt", "hello world");
        var text = await svc.ReadText("test.txt");
        Assert.Equal("hello world", text);
    }

    [Fact]
    public async Task ReadBinary_WriteBinary_Roundtrip()
    {
        var svc = CreateService();
        var data = new byte[] { 1, 2, 3, 255, 0 };
        await svc.WriteBinary("data.bin", data);
        var result = await svc.ReadBinary("data.bin");
        Assert.Equal(data, result);
    }

    [Fact]
    public async Task List_ReturnsEntries()
    {
        var svc = CreateService();
        await svc.WriteText("a.txt", "a");
        await svc.WriteText("b.txt", "b");
        await svc.CreateDirectory("subdir");

        var entries = await svc.List(".");
        Assert.Equal(3, entries.Length);
        var names = entries.Select(e => e.Name).OrderBy(x => x).ToArray();
        Assert.Equal(["a.txt", "b.txt", "subdir"], names);

        var fileEntry = entries.First(e => e.Name == "a.txt");
        Assert.False(fileEntry.IsDirectory);
        Assert.Equal(1, fileEntry.Size);

        var dirEntry = entries.First(e => e.Name == "subdir");
        Assert.True(dirEntry.IsDirectory);
    }

    [Fact]
    public async Task Delete_RemovesFile()
    {
        var svc = CreateService();
        await svc.WriteText("to-delete.txt", "x");
        Assert.True(await svc.Exists("to-delete.txt"));
        await svc.Delete("to-delete.txt");
        Assert.False(await svc.Exists("to-delete.txt"));
    }

    [Fact]
    public async Task Exists_ReturnsTrue_WhenFileExists()
    {
        var svc = CreateService();
        await svc.WriteText("exists.txt", "x");
        Assert.True(await svc.Exists("exists.txt"));
    }

    [Fact]
    public async Task Exists_ReturnsFalse_WhenFileMissing()
    {
        var svc = CreateService();
        Assert.False(await svc.Exists("missing.txt"));
    }

    [Fact]
    public async Task CreateDirectory_CreatesDir()
    {
        var svc = CreateService();
        await svc.CreateDirectory("newdir");
        Assert.True(await svc.Exists("newdir"));
        var entries = await svc.List(".");
        Assert.Contains(entries, e => e.Name == "newdir" && e.IsDirectory);
    }

    [Fact]
    public async Task PathTraversal_Rejected()
    {
        var svc = CreateService();
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => svc.ReadText("../other/file.txt"));
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => svc.WriteText("..\\escape.txt", "x"));
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => svc.ReadText("sub/../../etc/passwd"));
    }

    [Fact]
    public async Task Write_WhenAllowWriteFalse_Throws()
    {
        var svc = CreateService(allowWrite: false);
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => svc.WriteText("x.txt", "x"));
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => svc.WriteBinary("x.bin", [1, 2, 3]));
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => svc.Delete("x.txt"));
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => svc.CreateDirectory("dir"));
    }

    [Fact]
    public async Task Read_WhenAllowWriteFalse_Succeeds()
    {
        var writableSvc = CreateService(allowWrite: true);
        await writableSvc.WriteText("read-only.txt", "ok");
        var readOnlySvc = CreateService(allowWrite: false);
        var text = await readOnlySvc.ReadText("read-only.txt");
        Assert.Equal("ok", text);
    }
}
