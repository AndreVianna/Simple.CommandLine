namespace Simple.CommandLine;

public abstract class Token
{
    private readonly string _description;

    protected Token(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
        Name = name.Trim().TrimStart('-');
        _description = description ?? string.Empty;
    }

    internal string Name { get; }

    internal virtual bool Is(string? name) => name is not null && Name == name.Trim().ToLower();

    protected IOutputWriter Writer { get; set; } = new ConsoleOutputWriter();

    internal string Describe(int indent = 0, int padding = 0)
    {
        var builder = new StringBuilder();
        if (indent == 0) builder.Append('[').Append(GetType().Name).Append("] ");
        else builder.Append(' ', indent);

        switch (this)
        {
            case Argument when indent != 0:
                builder.Append('<').Append(Name).Append('>');
                break;
            case Parameter option when indent != 0:
                builder.Append("--").Append(option.Name);
                if (option.Alias is not null) builder.Append("|-").Append(option.Alias);
                break;
            default:
                builder.Append(Name);
                break;
        }

        if (_description.Length == 0) return builder.ToString();

        if (indent == 0) {
            builder.Append(": ");
        }
        else
        {
            var length = builder.Length;
            if (padding <= length) padding = length + 1;
            builder.Append(' ', padding - length);
        }

        builder.Append(_description);
        return builder.ToString();
    }

    public override string ToString() => Describe();
}
