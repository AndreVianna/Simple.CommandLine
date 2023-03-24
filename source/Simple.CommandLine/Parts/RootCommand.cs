namespace Simple.CommandLine.Parts;

public class RootCommand : Command
{
    private static readonly string _executableName = System.IO.Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]);

    public RootCommand(IOutputWriter? writer = null, Action<Command>? onExecute = null, Action<Command, Command>? onBeforeSubCommand = null, Action<Command, Command>? onAfterSubCommand = null)
        : base(_executableName, null, onExecute, onBeforeSubCommand, onAfterSubCommand)
    {
        Writer = writer ?? new ConsoleOutputWriter();
        Add(new VersionFlag());
        Add(new HelpFlag());
        Add(new VerboseLevelOption());
        Add(new NoColorFlag());
    }
}