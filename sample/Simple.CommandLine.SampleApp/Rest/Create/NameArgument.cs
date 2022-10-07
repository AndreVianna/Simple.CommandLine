namespace Simple.CommandLine.SampleApp.Rest.Create;

internal sealed class NameArgument : Argument<string>
{
    public NameArgument()
        : base("NAME", "Defines the name of the project.")
    {
    }
}