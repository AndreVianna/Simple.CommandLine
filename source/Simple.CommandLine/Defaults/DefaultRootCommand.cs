namespace Simple.CommandLine.Defaults;

public class DefaultRootCommand : RootCommand
{
    public DefaultRootCommand(IOutputWriter? writer = null) : base(writer: writer)
    {
        AddTerminalOption(new DefaultVersionOption());
        AddTerminalOption(new DefaultHelpOption());
        AddFlag(new DefaultVerboseFlag());
        AddFlag(new DefaultNoColorFlag());
    }
}
