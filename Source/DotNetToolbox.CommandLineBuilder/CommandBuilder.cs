namespace DotNetToolbox.CommandLineBuilder;

public sealed class CommandBuilder {
    private readonly bool _isRoot;
    private readonly string? _name;
    private readonly string? _description;
    private readonly IOutputWriter? _writer;

    private Action<Command>? _onExecute;
    private Action<Command, Command>? _onBeforeSubCommand;
    private Action<Command, Command>? _onAfterSubCommand;

    private readonly ICollection<Func<Command, Command>> _steps = new List<Func<Command, Command>>();

    private CommandBuilder(IOutputWriter? writer = null) {
        _isRoot = true;
        _writer = writer ?? new ConsoleOutputWriter();
    }

    private CommandBuilder(string name, string? description = null) {
        _isRoot = false;
        _name = name;
        _description = description;
    }

    public static CommandBuilder FromRoot(IOutputWriter? writer = null) => new(writer);

    public CommandBuilder OnExecute(Action<Command> onExecute) {
        _onExecute = onExecute ?? throw new ArgumentNullException(nameof(onExecute));
        return this;
    }

    public CommandBuilder OnBeforeSubCommand(Action<Command, Command> onBeforeExecuteChild) {
        _onBeforeSubCommand = onBeforeExecuteChild ?? throw new ArgumentNullException(nameof(onBeforeExecuteChild));
        return this;
    }

    public CommandBuilder OnAfterSubCommand(Action<Command, Command> onAfterExecuteChild) {
        _onAfterSubCommand = onAfterExecuteChild ?? throw new ArgumentNullException(nameof(onAfterExecuteChild));
        return this;
    }

    public CommandBuilder AddFlag(string name, string? description = null, bool existsIfSet = false, Action<Token>? onRead = null)
        => AddFlag(name, '\0', description, existsIfSet, onRead);

    public CommandBuilder AddFlag(string name, char alias, string? description = null, bool existsIfSet = false, Action<Token>? onRead = null)
        => Add(new Flag(name, alias, description, existsIfSet, onRead));

    public CommandBuilder AddOptions<T>(string name, string? description = null, Action<Token>? onRead = null)
        => AddOptions<T>(name, '\0', description, onRead);

    public CommandBuilder AddOptions<T>(string name, char alias, string? description = null, Action<Token>? onRead = null)
        => Add(new Options<T>(name, alias, description, onRead));

    public CommandBuilder AddOption<T>(string name, string? description = null, Action<Token>? onRead = null)
        => AddOption<T>(name, '\0', description, onRead);

    public CommandBuilder AddOption<T>(string name, char alias, string? description = null, Action<Token>? onRead = null)
        => Add(new Option<T>(name, alias, description, onRead));

    public CommandBuilder AddParameter<T>(string name, string? description = null, Action<Token>? onRead = null)
        => Add(new Parameter<T>(name, description, onRead));

    public CommandBuilder AddSubCommand(string name, string? description = null, Action<CommandBuilder>? setup = null) {
        CommandBuilder builder = new(name, description);
        setup?.Invoke(builder);
        _ = Add(builder.Build());
        return this;
    }

    public CommandBuilder Add(Token token) {
        _steps.Add(parent => {
            parent.Add(token);
            return parent;
        });
        return this;
    }

    public Command Build() {
        Command command = _isRoot
            ? new RootCommand(_writer, _onExecute, _onBeforeSubCommand, _onAfterSubCommand)
            : new SubCommand(_name!, _description, _onExecute, _onBeforeSubCommand, _onAfterSubCommand);
        return _steps.Aggregate(command, (current, step) => step(current));
    }
}
