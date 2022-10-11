namespace Simple.CommandLine;

internal sealed class HelpOption : TerminalOption
{
    public HelpOption() : base("help", 'h', "Show this help information and exit.", true)
    {
    }

    protected override void OnRead(Command caller) => Writer.WriteHelp(caller);
}