namespace DotNetToolbox.CommandLineBuilder.Defaults;

internal sealed class VerboseLevelOption : Option<int>
{
    public VerboseLevelOption()
        : base("verbose", 'v', "Show verbose output.", t => t.Writer.VerboseLevel = (VerboseLevel)((Option<int>)t).Value)
    {
    }
}