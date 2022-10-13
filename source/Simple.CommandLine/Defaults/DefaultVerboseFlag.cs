namespace Simple.CommandLine.Defaults;

internal sealed class DefaultVerboseFlag : Flag
{
    public DefaultVerboseFlag(IOutputWriter? writer = null)
        : base("verbose", 'v', "Show verbose output.", true, writer: writer)
    {
    }

    protected override void OnRead(Command caller) =>
        Writer.IsVerbose = true;
}