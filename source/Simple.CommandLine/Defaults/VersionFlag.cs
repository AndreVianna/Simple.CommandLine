namespace Simple.CommandLine.Defaults;

internal sealed class VersionFlag : Flag
{
    public VersionFlag()
        : base("version", "Show version information and exit.", true, t => t.Writer.WriteVersion(t.Parent!))
    {
    }
}