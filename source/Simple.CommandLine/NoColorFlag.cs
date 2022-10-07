namespace Simple.CommandLine;

internal sealed class NoColorFlag : Flag
{
    public NoColorFlag()
        : base("--no-color", "Don't colorize output.", true)
    {
    }

    protected override void OnRead(Command caller, out bool terminate)
    {
        WriterUtilities.Colorize = false;
        base.OnRead(caller, out terminate);
    }
}
