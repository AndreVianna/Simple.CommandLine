namespace Simple.CommandLine;

internal sealed class VersionOption : Option
{
    public VersionOption()
        : base("--version", "Show version information and exit.", false)
    {
    }

    protected override void OnRead(Command caller, out bool terminate)
    {
        var assembly = Assembly.GetEntryAssembly()!;
        var name = assembly.GetName().Name;
        var description = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description;
        var version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion;
        Writer.WriteLine(description ?? name);
        Writer.WriteLine(version);
        terminate = true;
    }
}