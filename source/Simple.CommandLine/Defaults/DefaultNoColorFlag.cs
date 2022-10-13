namespace Simple.CommandLine.Defaults;

internal sealed class DefaultNoColorFlag : Flag
{
    public DefaultNoColorFlag(IOutputWriter? writer = null)
        : base("no-color", "Don't colorize output.", true, writer: writer)
    {
    }

    protected override void OnRead(Command caller) =>
        Writer.UseColors = false;
}
