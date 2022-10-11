namespace Simple.CommandLine;

public abstract class Argument : Token {
    private readonly Action<Command>? _onRead;

    protected Argument(string name, string? description = null, Action<Command>? onRead = null)
        : base(name, description) {
        _onRead = onRead;
    }

    internal virtual void Read(Command caller, string argument, out bool terminate) {
        terminate = false;
        OnRead(caller);
    }

    protected virtual void OnRead(Command caller) => _onRead?.Invoke(caller);
}

public class Argument<TValue> : Argument
{
    public Argument(string name, string? description = null)
    : base(name, description)
    {
    }

    internal TValue Value { get; private set; } = default!;

    internal sealed override void Read(Command caller, string argument, out bool terminate)
    {
        try {
            Value = Convert.ChangeType(argument, typeof(TValue)) is TValue value ? value : default!;
            base.Read(caller, argument, out terminate);
        }
        catch (Exception ex)
        {
            terminate = true;
            Writer.WriteError($"An error occurred processing argument '{Name}'.", ex);
        }
    }
}
