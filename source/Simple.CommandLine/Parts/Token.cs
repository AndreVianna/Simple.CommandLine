namespace Simple.CommandLine.Parts;

public abstract class Token {
    private static readonly Regex _validName = new("^[a-z0-9]+(-?[a-z0-9]+)*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private readonly string _description;

    protected Token(string name, string? description = null, IOutputWriter? writer = null) {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be null or whitespace.", nameof(name));
        Name = name.Trim().ToLower();
        if (!_validName.IsMatch(Name)) throw new ArgumentException($"The value '{name}' is not a valid name. A name must be in the 'kebab lower case' form. Examples: 'name', 'address2' or 'full-name'.", nameof(name));
        _description = description ?? string.Empty;
        Writer = writer ?? new ConsoleOutputWriter();
    }

    public IOutputWriter Writer { get; internal set; }

    internal string Name { get; }

    internal virtual bool Is(string candidate) => Name == candidate.ToLower();

    internal string Describe() {
        var builder = new StringBuilder();
        builder.Append(' ', 2);

        switch (this) {
            case Parameter:
                builder.Append('<').Append(Name).Append('>');
                break;
            case Option option:
                builder.Append("--").Append(Name);
                if (option.Alias is not null)
                    builder.Append("|-").Append(option.Alias);
                if (option is IHasValue)
                    builder.Append(" <").Append(Name).Append('>');
                break;
            default:
                builder.Append(Name);
                break;
        }

        if (_description.Length == 0)
            return builder.ToString();

        var padding = 25;
        var length = builder.Length;
        if (padding <= length)
            padding = length + 1;
        builder.Append(' ', padding - length);

        builder.Append(_description);
        return builder.ToString();
    }

    public override string ToString() {
        var builder = new StringBuilder();
        builder.Append(Name);
        return builder.ToString();
    }
}