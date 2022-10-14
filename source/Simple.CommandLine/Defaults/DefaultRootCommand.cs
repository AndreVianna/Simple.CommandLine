namespace Simple.CommandLine.Defaults;

public class DefaultRootCommand : RootCommand
{
    public DefaultRootCommand(Action<Command>? onExecute = null, Action<Command>? onBeforeExecuteChild = null) : base(onExecute, onBeforeExecuteChild)
    {
        AddTerminalOption(new DefaultVersionOption());
        AddTerminalOption(new DefaultHelpOption());
        AddFlag(new DefaultVerboseFlag());
        AddFlag(new DefaultNoColorFlag());
    }
}
