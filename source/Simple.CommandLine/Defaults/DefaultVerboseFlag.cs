namespace Simple.CommandLine.Defaults;

internal sealed class DefaultVerboseFlag : Flag
{
    public DefaultVerboseFlag()
        : base("verbose", 'v', "Show verbose output.", true)
    {
    }

    protected override void OnRead(Command caller) =>
        Writer.IsVerbose = true;
}