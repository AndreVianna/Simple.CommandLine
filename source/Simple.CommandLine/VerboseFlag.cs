namespace Simple.CommandLine;

internal sealed class VerboseFlag : Flag
{
    public VerboseFlag()
        : base(new[] { "--verbose", "-v" }, "Show verbose output.", true)
    {
    }

    protected override void OnRead(Command caller, out bool terminate)
    {
        WriterUtilities.IsVerbose = true;
        base.OnRead(caller, out terminate);
    }
}