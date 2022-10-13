namespace Simple.CommandLine.Parts;

public class TerminalOption : Option
{
    public TerminalOption(string name, char alias, string? description = null, bool isInheritable = false, Action<Command>? onRead = null, IOutputWriter? writer = null)
        : base(name, alias, description, isInheritable, onRead, writer)
    {
    }

    public TerminalOption(string name, string? description = null, bool isInheritable = false, Action<Command>? onRead = null, IOutputWriter? writer = null)
        : this(name, '\0', description, isInheritable, onRead, writer)
    {
    }

    protected sealed override void Read(ref Span<string> _) { }
}