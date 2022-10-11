namespace Simple.CommandLine;

public class TerminalOption : Parameter
{
    private readonly Action<Command>? _onRead;

    public TerminalOption(string name, char alias, string? description = null, bool isAvailableToChildren = false, Action<Command>? onRead = null)
        : base(name, alias, description, isAvailableToChildren) {
        _onRead = onRead;
    }

    protected TerminalOption(string name, string? description = null, bool availableToSubCommands = false, Action<Command>? onRead = null)
        : this(name, '\0', description, availableToSubCommands, onRead) {
    }

    internal sealed override void Read(Command caller, ref Span<string> arguments, out bool terminate) {
        terminate = true;
        try {
            OnRead(caller);
        }
        catch (Exception ex) {
            Writer.WriteError($"An error occurred reading option '{Name}'.", ex);
        }
    }

    protected virtual void OnRead(Command caller) => _onRead?.Invoke(caller);
}