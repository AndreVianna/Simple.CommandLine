namespace Simple.CommandLine.Parts;

public abstract class Option : Argument
{
    protected Option(string name, char alias, string? description = null, bool isInheritable = false, Action<Command>? onRead = null)
        : base(name, alias, description, isInheritable, onRead)
    {
    }
}

public class Option<TValue> : Option, IHasValue {
    public Option(string name, char alias, string? description = null, bool isInheritable = false, Action<Command>? onRead = null)
        : base(name, alias, description, isInheritable, onRead) {
    }

    public Option(string name, string? description = null, bool isInheritable = false, Action<Command>? onRead = null)
        : this(name, '\0', description, isInheritable, onRead) {
    }

    public TValue Value { get; private set; } = default!;

    protected sealed override void Read(ref Span<string> arguments) {
        Value = (TValue)Convert.ChangeType(arguments[0], typeof(TValue));
        arguments = arguments[1..];
    }
}
