namespace Simple.CommandLine;

public class Option : Parameter
{
    private readonly Action<Command>? _onRead;

    public Option(string name, char alias, string? description = null, bool isAvailableToChildren = false, Action<Command>? onRead = null)
        : base(name, alias, description, isAvailableToChildren) {
        _onRead = onRead;
    }

    protected Option(string name, string? description = null, bool availableToSubCommands = false, Action<Command>? onRead = null)
        : this(name, '\0', description, availableToSubCommands, onRead) {
    }

    internal sealed override void Read(Command caller, ref Span<string> arguments, out bool terminate) {
        terminate = true;
        try {
            terminate = false;
            OnRead(caller);
        }
        catch (Exception ex) {
            Writer.WriteErrorLine($"An error occurred reading option '{Name}'.", ex);
        }
    }

    protected virtual void OnRead(Command caller) => _onRead?.Invoke(caller);
}

public class Option<TValue> : Parameter {
    private readonly Action<Command>? _onRead;

    public Option(string name, char alias, string? description = null, bool isAvailableToChildren = false, Action<Command>? onRead = null)
        : base(name, alias, description, isAvailableToChildren) {
        _onRead = onRead;
    }

    protected Option(string name, string? description = null, bool availableToSubCommands = false, Action<Command>? onRead = null)
        : this(name, '\0', description, availableToSubCommands, onRead) {
    }

    public TValue Value { get; private set; } = default!;

    internal sealed override void Read(Command caller, ref Span<string> arguments, out bool terminate) {
        try {
            terminate = false;
            Value = Convert.ChangeType(arguments[0], typeof(TValue)) is TValue value ? value : default!;
            arguments = arguments[1..];
            OnRead(caller);
        }
        catch (Exception ex) {
            terminate = true;
            Writer.WriteErrorLine($"An error occurred reading option '{Name}'.", ex);
        }
    }

    protected virtual void OnRead(Command caller) => _onRead?.Invoke(caller);
}