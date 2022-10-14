namespace Simple.CommandLine;

public class RootCommand : Command
{
    private static readonly string _executableName = System.IO.Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]);

    public RootCommand(Action<Command>? onExecute = null, Action<Command>? onBeforeExecuteChild = null)
        : base(_executableName, null, onExecute, onBeforeExecuteChild)
    {
    }
}