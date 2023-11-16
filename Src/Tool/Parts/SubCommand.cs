using DotNetToolbox.CommandLineBuilder.Defaults;

namespace DotNetToolbox.CommandLineBuilder.Parts;

public class SubCommand : Command
{
    public SubCommand(string name, string? description = null, Action<Command>? onExecute = null, Action<Command, Command>? onBeforeSubCommand = null, Action<Command, Command>? onAfterSubCommand = null)
        : base(name, description, onExecute, onBeforeSubCommand, onAfterSubCommand)
    {
        Add(new HelpFlag());
    }
}
