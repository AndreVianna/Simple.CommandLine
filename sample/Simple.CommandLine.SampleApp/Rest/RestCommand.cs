namespace Simple.CommandLine.SampleApp.Rest;

internal sealed class RestCommand : SubCommand
{
    public RestCommand() : base("rest", "Manages restful api projects.") {
        Add(new RestCreateCommand());
    }
}
