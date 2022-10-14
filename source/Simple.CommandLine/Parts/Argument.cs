namespace Simple.CommandLine.Parts;

public abstract class Argument : Token {
    private readonly Action<Command>? _onRead;
    private static readonly Regex _validAlias = new("^[a-zA-Z0-9]$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    protected Argument(string name, char alias, string? description = null, bool isInheritable = false, Action<Command>? onRead = null)
        : base(name, description) {
        Alias = alias;
        if (Alias != '\0' && !_validAlias.IsMatch(Alias.ToString()))
            throw new ArgumentException($"The value '{alias}' is not a valid alias. An alias must be a letter or number.", nameof(alias));
        IsInheritable = isInheritable;
        _onRead = onRead;
    }

    protected Argument(string name, string? description = null, bool isInheritable = false, Action<Command>? onRead = null)
        : this(name, '\0', description, isInheritable, onRead) {
    }

    internal bool IsSet { get; private set; }
    internal bool IsInheritable { get; }
    internal char Alias { get; }

    internal bool IsAlias(char candidate) =>
        Alias != '\0' && candidate != '\0' && Alias == candidate;

    internal void Read(Command caller, ref Span<string> arguments) {
        try {
            Read(ref arguments);
            OnRead(caller);
            IsSet = true;
        }
        catch (Exception ex) {
            Writer.WriteError($"An error occurred while reading argument '{this}'.", ex);
            throw;
        }
    }

    protected abstract void Read(ref Span<string> arguments);

    protected virtual void OnRead(Command caller) => _onRead?.Invoke(caller);
}
