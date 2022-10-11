namespace Simple.CommandLine;

public class RootCommand : Command
{
    private static readonly string _executableName = System.IO.Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]);

    public RootCommand() : base(null, _executableName)
    {
        AddTerminalOption(new VersionOption());
        AddTerminalOption(new HelpOption());
        AddFlag(new VerboseFlag());
        AddFlag(new NoColorFlag());
    }

    public void Execute(string[] arguments) => Execute(arguments.AsSpan());
}
