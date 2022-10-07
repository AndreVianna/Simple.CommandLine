namespace Simple.CommandLine;

public abstract class Argument : Token
{
    protected Argument(string name, string? description = null)
        : base(name, description)
    {
    }

    internal virtual void Read(Command caller, string argument, out bool terminate) => OnRead(caller, out terminate);
    protected virtual void OnRead(Command caller, out bool terminate) => terminate = false;
}

public abstract class Argument<TValue> : Argument
{
    protected Argument(string name, string? description = null)
    : base(name, description)
    {
    }

    internal TValue Value { get; private set; } = default!;

    internal override void Read(Command caller, string argument, out bool terminate)
    {
        try
        {
            Value = Convert.ChangeType(argument, typeof(TValue)) is TValue value ? value : default!;
            base.Read(caller, argument, out terminate);
        }
        catch (Exception ex)
        {
            Writer.WriteErrorLine($"An error occurred reading argument '{Name}'.", ex);
            terminate = true;
        }
    }
}
