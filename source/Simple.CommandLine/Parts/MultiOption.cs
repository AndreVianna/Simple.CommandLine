namespace Simple.CommandLine.Parts;

public abstract class MultiOption : Option {
    protected MultiOption(string name, char alias, string? description = null, bool isInheritable = false, Action<Command>? onRead = null)
        : base(name, alias, description, isInheritable, onRead) {
    }
}

public class MultiOption<TValue> : MultiOption, IHasValue {
    private readonly ICollection<TValue> _values = new List<TValue>();

    public MultiOption(string name, char alias, string? description = null, bool isInheritable = false, Action<Command>? onRead = null)
        : base(name, alias, description, isInheritable, onRead) {
    }

    public MultiOption(string name, string? description = null, bool isInheritable = false, Action<Command>? onRead = null)
        : this(name, '\0', description, isInheritable, onRead) {
    }

    public IReadOnlyList<TValue> Values => (IReadOnlyList<TValue>)_values;

    protected sealed override void Read(ref Span<string> arguments) {
        var item = (TValue)Convert.ChangeType(arguments[0], typeof(TValue));
        ((ICollection<TValue>)Values).Add(item);
        arguments = arguments[1..];
    }
}
