namespace Simple.CommandLine.Parts;

public abstract class Token {
    private static readonly Regex _validName = new("^[a-zA-Z0-9]+(-?[a-zA-Z0-9]+)*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private Token(TokenType type, string name, string? description, bool isPrivate)
    {
        TokenType = type;
        Name = name.Trim();
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("The name cannot be null or whitespace.", nameof(name));

        if (!_validName.IsMatch(Name))
            throw new ArgumentException($"'{name}' is not a valid name. A name must be in the 'kebab case' form. Examples: 'name', 'address2' or 'full-name'.", nameof(name));
        Description = description ?? string.Empty;
    }

    protected Token(TokenType type, string name, string? description = null)
        : this(type, name, description, true) {
    }

    private IOutputWriter _writer;
    public IOutputWriter Writer {
        get => _writer;
        internal set {
            _writer = value;
            if (this is not Command command) return;
            foreach (var token in command.Tokens) token.Writer = value;
        }
    }

    public Command? Parent { get; set; }
    internal TokenType TokenType { get; set; }
    internal string Name { get; set; }
    internal string Description { get; set; }

    internal bool Is(string? name) => Name.Equals(name, StringComparison.InvariantCultureIgnoreCase);

    public override string ToString()
    {
        return $"{TokenType} '{(this is Argument ? "--" : string.Empty)}{Name}'{(this is Argument a && a.Alias != '\0' ? $" | '-{a.Alias}'" : string.Empty)}";
    }
}