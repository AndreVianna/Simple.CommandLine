namespace Simple.CommandLine.Defaults;

internal sealed class DefaultVersionOption : TerminalOption
{
    public DefaultVersionOption()
        : base("version", "Show version information and exit.")
    {
    }

    protected override void OnRead(Command caller) => Writer.WriteVersion(caller);
}