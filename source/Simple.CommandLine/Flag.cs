namespace Simple.CommandLine;

public class Flag : Parameter
{
    private readonly Action<Command>? _onRead;

    public Flag(IReadOnlyCollection<string> names, string? description = null, bool isInheritable = false, Action<Command>? onRead = null)
        : base(names, description, isInheritable) {
        _onRead = onRead;
    }

    public Flag(string name, string? description = null, bool isInheritable = false, Action<Command>? onRead = null)
        : this(new []{ name }, description, isInheritable, onRead)
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
            Writer.WriteErrorLine($"An error occurred reading option '{Name}'.", ex);
        }
    }

    protected virtual void OnRead(Command caller) => _onRead?.Invoke(caller);
}
