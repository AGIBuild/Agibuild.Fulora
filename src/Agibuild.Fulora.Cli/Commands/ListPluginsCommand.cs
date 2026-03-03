using System.CommandLine;
using System.Xml.Linq;

namespace Agibuild.Fulora.Cli.Commands;

internal static class ListPluginsCommand
{
    private const string PluginPrefix = "Agibuild.Fulora.Plugin.";

    public static Command Create()
    {
        var pluginsCommand = new Command("plugins") { Description = "List installed Fulora plugins from the project" };

        pluginsCommand.SetAction((parseResult) =>
        {
            var result = Execute();
            return result;
        });

        var listCommand = new Command("list") { Description = "List project resources" };
        listCommand.Subcommands.Add(pluginsCommand);
        return listCommand;
    }

    internal static int Execute()
    {
        var cwd = Directory.GetCurrentDirectory();
        var csprojFiles = Directory.GetFiles(cwd, "*.csproj");

        if (csprojFiles.Length == 0)
        {
            Console.WriteLine("No .csproj file found in the current directory.");
            return 0;
        }

        var allPlugins = new List<(string Project, string PackageId, string Version)>();

        foreach (var csprojPath in csprojFiles)
        {
            var plugins = GetFuloraPluginsFromCsproj(csprojPath);
            var projectName = Path.GetFileNameWithoutExtension(csprojPath);
            foreach (var (pkgId, version) in plugins)
                allPlugins.Add((projectName, pkgId, version));
        }

        if (allPlugins.Count == 0)
        {
            Console.WriteLine("No Fulora plugins installed.");
            return 0;
        }

        var idWidth = Math.Max(6, allPlugins.Max(p => p.PackageId.Length));
        var verWidth = Math.Max(7, allPlugins.Max(p => p.Version.Length));
        var projWidth = Math.Max(7, allPlugins.Max(p => p.Project.Length));

        var headerFmt = string.Format("{0,-" + projWidth + "} {1,-" + idWidth + "} {2,-" + verWidth + "}", "Project", "Package", "Version");
        Console.WriteLine(headerFmt);
        Console.WriteLine(new string('-', projWidth + idWidth + verWidth + 4));

        var rowFmt = "{0,-" + projWidth + "} {1,-" + idWidth + "} {2,-" + verWidth + "}";
        foreach (var (project, pkgId, version) in allPlugins.OrderBy(p => p.Project).ThenBy(p => p.PackageId))
            Console.WriteLine(string.Format(rowFmt, project, pkgId, version));

        return 0;
    }

    internal static List<(string PackageId, string Version)> GetFuloraPluginsFromCsproj(string csprojPath)
    {
        var result = new List<(string, string)>();
        if (!File.Exists(csprojPath))
            return result;

        try
        {
            var doc = XDocument.Load(csprojPath);
            var packageRefs = doc.Descendants().Where(e => e.Name.LocalName == "PackageReference");

            foreach (var pr in packageRefs)
            {
                var include = pr.Attribute("Include")?.Value;
                if (string.IsNullOrEmpty(include) || !include.StartsWith(PluginPrefix, StringComparison.OrdinalIgnoreCase))
                    continue;

                var version = pr.Attribute("Version")?.Value
                    ?? pr.Elements().FirstOrDefault(e => e.Name.LocalName == "Version")?.Value
                    ?? "(unknown)";

                result.Add((include, version));
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Failed to parse {csprojPath}: {ex.Message}");
        }

        return result;
    }
}
