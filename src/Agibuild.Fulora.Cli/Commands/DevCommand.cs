using System.CommandLine;
using System.Diagnostics;

namespace Agibuild.Fulora.Cli.Commands;

internal static class DevCommand
{
    internal static Command Create()
    {
        var cmd = new Command("dev", "Start Vite dev server and Avalonia desktop app together");

        cmd.SetAction(async (_, ct) =>
        {
            var webDir = DetectWebProject();
            var desktopCsproj = DetectDesktopProject();

            if (webDir is null)
            {
                Console.Error.WriteLine("Could not find a web project (no package.json with vite). Use the project root directory.");
                return;
            }

            if (desktopCsproj is null)
            {
                Console.Error.WriteLine("Could not find a Desktop .csproj project.");
                return;
            }

            Console.WriteLine($"Starting dev server in {Path.GetFileName(webDir)}...");
            Console.WriteLine($"Starting desktop app from {Path.GetFileName(desktopCsproj)}...");

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };

            var viteTask = RunProcessAsync("npm", "run dev", webDir, cts.Token);
            var dotnetTask = RunProcessAsync("dotnet", $"run --project \"{desktopCsproj}\"", Directory.GetCurrentDirectory(), cts.Token);

            try
            {
                await Task.WhenAll(viteTask, dotnetTask);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("\nShutting down...");
            }
        });

        return cmd;
    }

    private static async Task RunProcessAsync(string command, string args, string workingDirectory, CancellationToken ct)
    {
        var psi = new ProcessStartInfo(command, args)
        {
            WorkingDirectory = workingDirectory,
            UseShellExecute = false,
        };

        using var process = Process.Start(psi)
            ?? throw new InvalidOperationException($"Failed to start {command}");

        try
        {
            await process.WaitForExitAsync(ct);
        }
        catch (OperationCanceledException)
        {
            if (!process.HasExited)
            {
                process.Kill(entireProcessTree: true);
                await process.WaitForExitAsync();
            }
        }
    }

    private static string? DetectWebProject()
    {
        var dir = Directory.GetCurrentDirectory();
        var packageJsonFiles = Directory.GetFiles(dir, "package.json", SearchOption.AllDirectories)
            .Where(f => !f.Contains("node_modules") && !f.Contains("packages"))
            .ToArray();

        return packageJsonFiles
            .Select(Path.GetDirectoryName)
            .FirstOrDefault(d => d is not null && File.Exists(Path.Combine(d, "vite.config.ts")));
    }

    private static string? DetectDesktopProject()
    {
        var dir = Directory.GetCurrentDirectory();
        var desktopProjects = Directory.GetFiles(dir, "*.Desktop.csproj", SearchOption.AllDirectories);
        return desktopProjects.Length == 1 ? desktopProjects[0] : null;
    }
}
