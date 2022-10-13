namespace Simple.CommandLine.Parts;

public abstract class Parameter : Argument, IHasValue
{
    protected Parameter(string name, string? description = null, Action<Command>? onRead = null, IOutputWriter? writer = null)
        : base(name, description, false, onRead, writer)
    {
    }
}

public class Parameter<TValue> : Parameter
{
    public Parameter(string name, string? description = null, Action<Command>? onRead = null, IOutputWriter? writer = null)
    : base(name, description, onRead, writer)
    {
    }

    internal TValue Value { get; private set; } = default!;

    protected sealed override void Read(ref Span<string> arguments) =>
        Value = (TValue)Convert.ChangeType(arguments[0], typeof(TValue));
}
