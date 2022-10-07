namespace Simple.CommandLine.SampleApp.Rest;

internal sealed class RestCommand : Command
{
    public RestCommand(Command parent) : base(parent, "rest", "Manages restful api projects.") {
        AddSubCommand(new RestCreateCommand(this));
    }
}
