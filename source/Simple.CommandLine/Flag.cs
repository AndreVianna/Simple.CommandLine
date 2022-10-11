namespace Simple.CommandLine;

public class Flag : Parameter
{
    private readonly Action<Command>? _onRead;

    public Flag(string name, char alias, string? description = null, bool isAvailableToChildren = false, Action<Command>? onRead = null)
        : base(name, alias, description, isAvailableToChildren) {
        _onRead = onRead;
    }

    public Flag(string name, string? description = null, bool availableToSubCommands = false, Action<Command>? onRead = null)
        : this(name, '\0', description, availableToSubCommands, onRead)
    {
    }

    public bool IsEnable { get; private set; }

    internal sealed override void Read(Command caller, ref Span<string> arguments, out bool terminate) {
        try {
            terminate = false;
            IsEnable = true;
            OnRead(caller);
        }
        catch (Exception ex) {
            terminate = true;
            Writer.WriteError($"An error occurred reading option '{Name}'.", ex);
        }
    }

    protected virtual void OnRead(Command caller) => _onRead?.Invoke(caller);
}
