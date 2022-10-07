namespace Simple.CommandLine;

public abstract class Parameter : Token {
    protected Parameter(IReadOnlyCollection<string> names, string? description = null, bool isInheritable = false)
        : base(GetFirstName(names), description) {
        if (names.Any(string.IsNullOrWhiteSpace))
            throw new ArgumentException("Value cannot contain null or empty values.", nameof(names));
        Names = names.Select(i => i.Trim().TrimStart('-')).ToArray();
        IsInheritable = isInheritable;
    }

    protected Parameter(string name, string? description = null, bool isInheritable = false)
        : this(new[] { name }, description, isInheritable) {
    }

    internal bool IsInheritable { get; }

    internal ICollection<string> Names { get; }
    internal bool IsAlias(string alias) => Names.Contains(alias.Trim().TrimStart('-'), StringComparer.InvariantCultureIgnoreCase);

    internal abstract void Read(Command caller, ref Span<string> arguments, out bool terminate);

    private static string GetFirstName(IReadOnlyCollection<string>? names) =>
        names is null || names.Count == 0
            ? throw new ArgumentException("Value cannot be null or empty.", nameof(names))
            : names.First();
}