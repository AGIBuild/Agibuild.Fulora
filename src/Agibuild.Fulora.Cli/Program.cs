using System.CommandLine;
using Agibuild.Fulora.Cli.Commands;

var root = new RootCommand("Agibuild.Fulora CLI — hybrid app development toolkit")
{
    NewCommand.Create(),
    GenerateCommand.Create(),
    DevCommand.Create(),
    AddCommand.Create()
};

return await root.Parse(args).InvokeAsync();
