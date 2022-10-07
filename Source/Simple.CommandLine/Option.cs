namespace Simple.CommandLine;

public abstract class Option : Token
{
    protected Option(IReadOnlyCollection<string> names, string? description = null, bool isShared = false)
        : base(GetFirstName(names), description)
    {
        if (names.Any(string.IsNullOrWhiteSpace)) throw new ArgumentException("Value cannot contain null or empty values.", nameof(names));
        IsShared = isShared;
        Names = names.Select(i => i.Trim().TrimStart('-')).ToArray();
    }

    protected Option(string name, string? description = null, bool isShared = false)
        : this(new[] { name }, description, isShared)
    {
    }

    internal bool IsShared { get; }

    internal ICollection<string> Names { get; }
    internal bool IsAlias(string alias) => Names.Contains(alias.Trim().TrimStart('-'), StringComparer.InvariantCultureIgnoreCase);

    internal virtual void Read(Command caller, ref Span<string> arguments, out bool terminate) => OnRead(caller, out terminate);

    protected virtual void OnRead(Command caller, out bool terminate) => terminate = false;

    private static string GetFirstName(IReadOnlyCollection<string>? names) =>
        names is null || names.Count == 0
            ? throw new ArgumentException("Value cannot be null or empty.", nameof(names))
            : names.First();
}

public abstract class Option<TValue> : Option
{
    protected Option(IReadOnlyCollection<string> names, string? description = null, bool isShared = false)
        : base(names, description, isShared)
    {
    }

    protected Option(string name, string? description = null, bool isShared = false)
        : base(name, description, isShared)
    {
    }

    public TValue Value { get; private set; } = default!;

    internal sealed override void Read(Command caller, ref Span<string> arguments, out bool terminate)
    {
        try
        {
            Value = Convert.ChangeType(arguments[0], typeof(TValue)) is TValue value ? value : default!;
            arguments = arguments[1..];
            base.Read(caller, ref arguments, out terminate);
        }
        catch (Exception ex)
        {
            Writer.WriteErrorLine($"An error occurred reading option '{Name}'.", ex);
            terminate = true;
        }
    }
}
