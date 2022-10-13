namespace Simple.CommandLine.Defaults;

internal sealed class DefaultVersionOption : TerminalOption
{
    public DefaultVersionOption(IOutputWriter? writer = null)
        : base("version", "Show version information and exit.", writer: writer)
    {
    }

    protected override void OnRead(Command caller) => Writer.WriteVersion(caller);
}