namespace Simple.CommandLine.Defaults;

internal sealed class DefaultHelpOption : TerminalOption
{
    public DefaultHelpOption(IOutputWriter? writer = null)
        : base("help", 'h', "Show this help information and exit.", true, writer: writer)
    {
    }

    protected override void OnRead(Command caller) => Writer.WriteHelp(caller);
}