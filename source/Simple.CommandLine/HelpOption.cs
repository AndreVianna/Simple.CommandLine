namespace Simple.CommandLine;

internal sealed class HelpOption : Option
{
    public HelpOption() : base(new[] { "--help", "-h" }, "Show this help information and exit.", true)
    {
    }

    protected override void OnRead(Command caller) => Writer.WriteHelp(caller);
}