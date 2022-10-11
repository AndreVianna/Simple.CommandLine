using System.Text.RegularExpressions;

namespace Simple.CommandLine;

public abstract class Token
{
    private static readonly Regex _validName = new("^[a-z0-9]+(-?[a-z0-9]+)*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private readonly string _description;

    protected Token(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
        Name = name.Trim().ToLower();
        if (!_validName.IsMatch(Name))
            throw new ArgumentException($"Invalid value '{name}'. Name must be in the 'kebab lower case' form. Examples: 'name', 'address2' or 'full-name'.", nameof(name));
        _description = description ?? string.Empty;
    }

    internal string Name { get; }

    internal virtual bool Is(string candidate) => Name == candidate.ToLower();

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
