namespace Simple.CommandLine.SampleApp.Rest.Create;

internal sealed class RestCreateCommand : SubCommand
{
    public RestCreateCommand() : base("create", "Create the initial api project.") {
        Add(new NameParameter());
        Add(new OutputOption());
    }

    protected override void OnExecute()
    {
        var name = GetValueOrDefault<string>("NAME");
        if (name is null)
        {
            Writer.WriteError("The name of the project is required.");
            return;
        }

        var outputPath = GetValueOrDefault<string>("output");

        var isVerbose = IsFlagSet("verbose");
        if (isVerbose) {
            if (outputPath is not null) Writer.WriteLine($"Setting project output to '{outputPath}'...");
            Writer.WriteLine($"Creating project '{name}'...");
        }

        Writer.WriteLine("Done.");
    }
}
