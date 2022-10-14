namespace Simple.CommandLine.Defaults;

internal sealed class DefaultNoColorFlag : Flag
{
    public DefaultNoColorFlag()
        : base("no-color", "Don't colorize output.", true)
    {
    }

    protected override void OnRead(Command caller) =>
        Writer.UseColors = false;
}
