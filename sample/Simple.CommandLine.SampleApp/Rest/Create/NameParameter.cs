namespace Simple.CommandLine.SampleApp.Rest.Create;

internal sealed class NameParameter : Parameter<string>
{
    public NameParameter()
        : base("NAME", "Defines the name of the project.")
    {
    }
}