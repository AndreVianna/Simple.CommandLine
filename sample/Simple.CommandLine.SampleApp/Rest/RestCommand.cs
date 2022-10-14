namespace Simple.CommandLine.SampleApp.Rest;

internal sealed class RestCommand : Command
{
    public RestCommand() : base("rest", "Manages restful api projects.") {
        AddCommand(new RestCreateCommand());
    }
}
