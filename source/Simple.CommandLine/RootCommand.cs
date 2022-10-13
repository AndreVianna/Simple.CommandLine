namespace Simple.CommandLine;

public class RootCommand : Command
{
    private static readonly string _executableName = System.IO.Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]);

    public RootCommand(Action<Command>? onExecuting = null, IOutputWriter? writer = null) : base(_executableName, null, onExecuting, writer)
    {
    }

    public void Execute(params string[] arguments) => ExecuteInternally(arguments);
}