using System.CommandLine;
using System.Diagnostics;

namespace Agibuild.Fulora.Cli.Commands;

internal static class NewCommand
{
    internal static Command Create()
    {
        var nameArg = new Argument<string>("name") { Description = "Project name" };
        var frontendOpt = new Option<string>("--frontend", "-f")
        {
            Description = "Frontend framework (react, vue, svelte)",
            Required = true
        };
        frontendOpt.AcceptOnlyFromAmong("react", "vue", "svelte");

        var cmd = new Command("new", "Create a new Agibuild.Fulora hybrid project")
        {
            nameArg,
            frontendOpt
        };

        cmd.SetAction(async (parseResult, ct) =>
        {
            var name = parseResult.GetValue(nameArg);
            var frontend = parseResult.GetValue(frontendOpt);

            Console.WriteLine($"Creating project '{name}' with {frontend} frontend...");

            var psi = new ProcessStartInfo("dotnet",
                $"new agibuild-hybrid -n {name} --framework {frontend}")
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

            using var process = Process.Start(psi);
            if (process is null)
            {
                Console.Error.WriteLine("Failed to start dotnet new.");
                return;
            }

            var stdout = await process.StandardOutput.ReadToEndAsync(ct);
            var stderr = await process.StandardError.ReadToEndAsync(ct);
            await process.WaitForExitAsync(ct);

            if (!string.IsNullOrWhiteSpace(stdout)) Console.Write(stdout);
            if (!string.IsNullOrWhiteSpace(stderr)) Console.Error.Write(stderr);

            if (process.ExitCode != 0)
            {
                Console.Error.WriteLine($"dotnet new failed with exit code {process.ExitCode}.");
                return;
            }

            Console.WriteLine($"Project '{name}' created successfully.");
            Console.WriteLine($"  cd {name}");
            Console.WriteLine("  agibuild dev");
        });

        return cmd;
    }
}
