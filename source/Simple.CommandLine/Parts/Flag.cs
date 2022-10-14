namespace Simple.CommandLine.Parts;

public class Flag : Option
{
    public Flag(string name, char alias, string? description = null, bool isInheritable = false, Action<Command>? onRead = null)
        : base(name, alias, description, isInheritable, onRead)
    {
    }

    public Flag(string name, string? description = null, bool availableToSubCommands = false, Action<Command>? onRead = null)
        : this(name, '\0', description, availableToSubCommands, onRead)
    {
    }

    public bool IsEnable { get; private set; }

    protected sealed override void Read(ref Span<string> _) => IsEnable = true;
}
