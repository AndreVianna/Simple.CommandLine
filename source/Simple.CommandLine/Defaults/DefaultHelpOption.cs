namespace Simple.CommandLine.Defaults;

internal sealed class DefaultHelpOption : TerminalOption
{
    public DefaultHelpOption()
        : base("help", 'h', "Show this help information and exit.", true)
    {
    }

    protected override void OnRead(Command caller) => Writer.WriteHelp(caller);
}