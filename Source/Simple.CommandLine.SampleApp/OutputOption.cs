namespace Simple.CommandLine.SampleApp;

internal sealed class OutputOption : Option<string>
{
    public OutputOption()
        : base(new[] { "--output", "-o" }, "Destination folder for the api. If not specified, the current directory is used.")
    {
    }
}
