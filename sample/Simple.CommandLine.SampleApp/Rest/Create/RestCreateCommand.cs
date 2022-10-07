namespace Simple.CommandLine.SampleApp.Rest.Create;

internal sealed class RestCreateCommand : Command
{
    public RestCreateCommand(Command parent) : base(parent, "create", "Create the initial api project.") {
        AddArgument(new NameArgument());
        AddOption(new OutputOption());
    }

    protected override void Execute()
    {
        var name = GetArgumentOrDefault<string>("NAME");
        if (name is null)
        {
            Writer.WriteErrorLine("The name of the project is required.");
            return;
        }

        var outputPath = GetOptionOrDefault<string>("output");

        var isVerbose = GetFlagOrDefault("verbose");
        if (isVerbose) {
            Writer.WriteLine($"Setting project output to '{outputPath}'...");
            Writer.WriteLine($"Creating project '{name}'...");
        }

        Writer.WriteLine("Done.");
    }
}
