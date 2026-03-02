using System.CommandLine;
using System.Diagnostics;

namespace Agibuild.Fulora.Cli.Commands;

internal static class GenerateCommand
{
    internal static Command Create()
    {
        var projectOpt = new Option<string?>("--project", "-p")
        {
            Description = "Path to the Bridge .csproj file (auto-detected if omitted)"
        };

        var typesCmd = new Command("types", "Generate TypeScript declarations from C# bridge interfaces")
        {
            projectOpt
        };

        typesCmd.SetAction(async (parseResult, ct) =>
        {
            var project = parseResult.GetValue(projectOpt);
            var csproj = project ?? DetectBridgeProject();
            if (csproj is null)
            {
                Console.Error.WriteLine("Could not find a Bridge project. Use --project to specify one.");
                return;
            }

            Console.WriteLine($"Building {Path.GetFileName(csproj)} to generate TypeScript types...");

            var psi = new ProcessStartInfo("dotnet", $"build \"{csproj}\" --no-restore")
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

            using var process = Process.Start(psi);
            if (process is null)
            {
                Console.Error.WriteLine("Failed to start dotnet build.");
                return;
            }

            var stdout = await process.StandardOutput.ReadToEndAsync(ct);
            var stderr = await process.StandardError.ReadToEndAsync(ct);
            await process.WaitForExitAsync(ct);

            if (process.ExitCode != 0)
            {
                Console.Error.WriteLine("Build failed:");
                if (!string.IsNullOrWhiteSpace(stderr)) Console.Error.Write(stderr);
                return;
            }

            // After successful build, find generated .d.ts files and copy to web project
            var fullCsproj = Path.GetFullPath(csproj);
            var bridgeDir = Path.GetDirectoryName(fullCsproj)!;
            var solutionDir = Path.GetDirectoryName(bridgeDir);
            if (!string.IsNullOrEmpty(solutionDir))
            {
                var webProjects = Directory.GetDirectories(solutionDir, "*.Web", SearchOption.TopDirectoryOnly);
                if (webProjects.Length == 1)
                {
                    var outputDir = Path.Combine(bridgeDir, "bin", "Debug");
                    if (Directory.Exists(outputDir))
                    {
                        var dtsFiles = Directory.GetFiles(outputDir, "*.d.ts", SearchOption.AllDirectories);
                        foreach (var dts in dtsFiles)
                        {
                            var dest = Path.Combine(webProjects[0], Path.GetFileName(dts));
                            File.Copy(dts, dest, overwrite: true);
                            Console.WriteLine($"  Copied {Path.GetFileName(dts)} -> {Path.GetRelativePath(Directory.GetCurrentDirectory(), dest)}");
                        }
                    }
                    // Bridge.Generator default output is project directory; also check there
                    var projectDtsFiles = Directory.GetFiles(bridgeDir, "*.d.ts", SearchOption.TopDirectoryOnly);
                    foreach (var dts in projectDtsFiles)
                    {
                        var dest = Path.Combine(webProjects[0], Path.GetFileName(dts));
                        File.Copy(dts, dest, overwrite: true);
                        Console.WriteLine($"  Copied {Path.GetFileName(dts)} -> {Path.GetRelativePath(Directory.GetCurrentDirectory(), dest)}");
                    }
                }
            }

            Console.WriteLine("TypeScript type generation complete.");
            Console.WriteLine("Hint: Types are emitted by the Bridge.Generator at compile time.");
        });

        var cmd = new Command("generate", "Code generation commands") { typesCmd };
        return cmd;
    }

    private static string? DetectBridgeProject()
    {
        var dir = Directory.GetCurrentDirectory();
        var bridgeProjects = Directory.GetFiles(dir, "*.Bridge.csproj", SearchOption.AllDirectories);
        return bridgeProjects.Length == 1 ? bridgeProjects[0] : null;
    }
}
