namespace Simple.CommandLine;

public abstract class Flag : Option
{
    protected Flag(IReadOnlyCollection<string> names, string? description = null, bool isShared = false)
        : base(names, description, isShared)
    {
    }

    protected Flag(string name, string? description = null, bool isShared = false)
        : base(name, description, isShared)
    {
    }

    public bool IsEnable { get; private set; }

    internal sealed override void Read(Command caller, ref Span<string> arguments, out bool terminate)
    {
        IsEnable = true;
        base.Read(caller, ref arguments, out terminate);
    }
}
