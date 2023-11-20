namespace SampleApp.Rest.Create;

internal sealed class RestCreateCommand : Command<RestCreateCommand> {
    public RestCreateCommand() : base("create", "Create the initial api project.") {
        Add(new NameParameter());
        Add(new OutputOption());
        OnExecute += (_, _) => Execute();
    }

    private Task Execute() {
        var name = GetValueOrDefault<string>("NAME");
        if (name is null) {
            Writer.WriteError("The name of the project is required.");
            return Task.CompletedTask;
        }

        var outputPath = GetValueOrDefault<string>("output");

        var isVerbose = IsFlagSet("verbose");
        if (isVerbose) {
            if (outputPath is not null) Writer.WriteLine($"Setting project output to '{outputPath}'...");
            Writer.WriteLine($"Creating project '{name}'...");
        }

        Writer.WriteLine("Done.");
        return Task.CompletedTask;
    }
}
