namespace SampleApp.Rest;

internal sealed class RestCommand : Command<RestCommand> {
    public RestCommand() : base("rest", "Manages restful api projects.") {
        Add(new RestCreateCommand());
    }
}
