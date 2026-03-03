using Agibuild.Fulora;

namespace Agibuild.Fulora.Plugin.FileSystem;

/// <summary>
/// Bridge plugin manifest for the FileSystem service.
/// Register with: <c>bridge.UsePlugin&lt;FileSystemPlugin&gt;();</c>
/// </summary>
public sealed class FileSystemPlugin : IBridgePlugin
{
    public static IEnumerable<BridgePluginServiceDescriptor> GetServices()
    {
        yield return BridgePluginServiceDescriptor.Create<IFileSystemService>(
            _ => new FileSystemService());
    }
}
