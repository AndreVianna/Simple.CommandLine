namespace Simple.CommandLine;

public class ListOption<TValue> : Parameter {
    private readonly Action<Command>? _onRead;

    public ListOption(string name, char alias, string? description = null, bool isAvailableToChildren = false, Action<Command>? onRead = null)
        : base(name, alias, description, isAvailableToChildren) {
        _onRead = onRead;
    }

    protected ListOption(string name, string? description = null, bool availableToSubCommands = false, Action<Command>? onRead = null)
        : this(name, '\0', description, availableToSubCommands, onRead) {
    }

    public IReadOnlyList<TValue> Values { get; } = new List<TValue>();

    internal sealed override void Read(Command caller, ref Span<string> arguments, out bool terminate) {
        try {
            terminate = false;
            var item = Convert.ChangeType(arguments[0], typeof(TValue)) is TValue value ? value : default!;
            ((ICollection<TValue>)Values).Add(item);
            arguments = arguments[1..];
            OnRead(caller);
        }
        catch (Exception ex) {
            terminate = true;
            Writer.WriteError($"An error occurred reading option '{Name}'.", ex);
        }
    }

    protected virtual void OnRead(Command caller) => _onRead?.Invoke(caller);
}
