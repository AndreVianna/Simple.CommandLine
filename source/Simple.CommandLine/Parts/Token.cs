namespace Simple.CommandLine.Parts;

public abstract class Token {
    private static readonly Regex _validName = new("^[a-zA-Z0-9]+(-?[a-zA-Z0-9]+)*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    protected Token(string name, string? description = null) {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be null or whitespace.", nameof(name));
        Name = name.Trim();
        if (!_validName.IsMatch(Name)) throw new ArgumentException($"The value '{name}' is not a valid name. A name must be in the 'kebab lower case' form. Examples: 'name', 'address2' or 'full-name'.", nameof(name));
        Description = description ?? string.Empty;
        _writer = new ConsoleOutputWriter();
    }

    private IOutputWriter _writer;
    public IOutputWriter Writer {
        get => _writer;
        internal set {
            _writer = value;
            if (this is not Command command) return;
            foreach (var child in command.Commands) child.Writer = value;
            foreach (var parameter in command.Parameters) parameter.Writer = value;
            foreach (var option in command.Options) option.Writer = value;
        }
    }

    internal string Name { get; }
    internal string Description { get; }

    internal bool Is(string? candidate) => Name.Equals(candidate, StringComparison.InvariantCultureIgnoreCase);

    public override string ToString() => Name;
}