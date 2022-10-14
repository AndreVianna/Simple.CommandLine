namespace Simple.CommandLine.Parts;

public class TerminalOption : Option
{
    public TerminalOption(string name, char alias, string? description = null, bool isInheritable = false, Action<Command>? onRead = null)
        : base(name, alias, description, isInheritable, onRead)
    {
    }

    public TerminalOption(string name, string? description = null, bool isInheritable = false, Action<Command>? onRead = null)
        : this(name, '\0', description, isInheritable, onRead)
    {
    }

    protected sealed override void Read(ref Span<string> _) { }
}