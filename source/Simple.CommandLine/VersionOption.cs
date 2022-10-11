namespace Simple.CommandLine;

internal sealed class VersionOption : TerminalOption
{
    public VersionOption()
        : base("version", "Show version information and exit.")
    {
    }

    protected override void OnRead(Command caller)
    {
        var assembly = Assembly.GetEntryAssembly()!;
        var name = assembly.GetName().Name;
        var description = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description;
        var version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion;
        Writer.WriteLine(description ?? name);
        Writer.WriteLine(version);
    }
}