namespace Simple.CommandLine.SampleApp.Rest.Create;

internal sealed class RestCreateCommand : Command
{
    public RestCreateCommand() : base("create", "Create the initial api project.") {
        AddParameter(new NameParameter());
        AddOption(new OutputOption());
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

        var isVerbose = GetFlagOrDefault("verbose");
        if (isVerbose) {
            if (outputPath is not null) Writer.WriteLine($"Setting project output to '{outputPath}'...");
            Writer.WriteLine($"Creating project '{name}'...");
        }

        Writer.WriteLine("Done.");
    }
}
