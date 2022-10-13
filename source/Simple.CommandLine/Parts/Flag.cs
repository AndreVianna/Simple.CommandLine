namespace Simple.CommandLine.Parts;

public class Flag : Option
{
    public Flag(string name, char alias, string? description = null, bool isInheritable = false, Action<Command>? onRead = null, IOutputWriter? writer = null)
        : base(name, alias, description, isInheritable, onRead, writer)
    {
    }

    public Flag(string name, string? description = null, bool availableToSubCommands = false, Action<Command>? onRead = null, IOutputWriter? writer = null)
        : this(name, '\0', description, availableToSubCommands, onRead, writer)
    {
    }

    public bool IsEnable { get; private set; }

    protected sealed override void Read(ref Span<string> _) => IsEnable = true;
}
