namespace Simple.CommandLine.Parts;

public abstract class Argument : Token {
    private readonly Action<Command>? _onRead;
    private static readonly Regex _validAlias = new("^[a-z0-9]$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    protected Argument(string name, char alias, string? description = null, bool isInheritable = false, Action<Command>? onRead = null, IOutputWriter? writer = null)
        : base(name, description, writer) {
        Alias = alias == '\0' ? null : alias.ToString();
        if (Alias is not null && !_validAlias.IsMatch(Alias))
            throw new ArgumentException($"The value '{alias}' is not a valid alias. An alias must be a letter or number.", nameof(alias));
        IsInheritable = isInheritable;
        _onRead = onRead;
    }

    protected Argument(string name, string? description = null, bool isInheritable = false, Action<Command>? onRead = null, IOutputWriter? writer = null)
        : this(name, '\0', description, isInheritable, onRead, writer) {
    }

    internal bool IsSet { get; private set; }
    internal bool IsInheritable { get; }
    internal string? Alias { get; }

    internal override bool Is(string? candidate) =>
        candidate is not null && (Name == candidate.ToLower() || (Alias is not null && Alias == candidate));

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
