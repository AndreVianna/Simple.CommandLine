namespace Simple.CommandLine;

public sealed class CommandBuilder
{
    private readonly bool _isDefaultRoot;
    private readonly string? _name;
    private readonly string? _description;

    private IOutputWriter _writer;
    private Action<Command>? _onExecute;
    private Action<Command>? _onBeforeExecuteChild;

    private readonly ICollection<Func<Command, Command>> _steps = new List<Func<Command, Command>>();

    private CommandBuilder(bool isDefaultRoot, string? name, string? description)
    {
        _writer = new ConsoleOutputWriter();
        _isDefaultRoot = isDefaultRoot;
        _name = name;
        _description = description;
    }

    public static CommandBuilder FromRoot() => new(false, null, null);
    public static CommandBuilder FromDefaultRoot() => new(true, null, null);

    public CommandBuilder WithWriter(IOutputWriter writer) {
        _writer = writer ?? throw new ArgumentNullException(nameof(writer));
        return this;
    }

    public CommandBuilder OnExecute(Action<Command> onExecute) {
        _onExecute = onExecute ?? throw new ArgumentNullException(nameof(onExecute));
        return this;
    }

    public CommandBuilder OnBeforeExecuteChild(Action<Command> onBeforeExecuteChild) {
        _onBeforeExecuteChild = onBeforeExecuteChild ?? throw new ArgumentNullException(nameof(onBeforeExecuteChild));
        return this;
    }

    public CommandBuilder AddFlag(string name, string? description = null, bool isInheritable = false, Action<Command>? onRead = null) =>
        AddFlag(name, '\0', description, isInheritable, onRead);

    public CommandBuilder AddFlag(string name, char alias, string? description = null, bool isInheritable = false, Action<Command>? onRead = null)
        => AddFlag(new(name, alias, description, isInheritable, onRead));

    public CommandBuilder AddFlag(Flag flag) {
        _steps.Add(c => {
            c.AddFlag(flag);
            return c;
        });
        return this;
    }

    public CommandBuilder AddMultiOption<T>(string name, string? description = null, bool isInheritable = false, Action<Command>? onRead = null) =>
        AddMultiOption<T>(name, '\0', description, isInheritable, onRead);

    public CommandBuilder AddMultiOption<T>(string name, char alias, string? description = null, bool isInheritable = false, Action<Command>? onRead = null) =>
        AddMultiOption<T>(new(name, alias, description, isInheritable, onRead));

    public CommandBuilder AddMultiOption<T>(MultiOption<T> option) {
        _steps.Add(c => {
            c.AddMultiOption(option);
            return c;
        });
        return this;
    }

    public CommandBuilder AddTerminalOption(string name, string? description = null, bool isInheritable = false, Action<Command>? onRead = null) =>
        AddTerminalOption(name, '\0', description, isInheritable, onRead);

    public CommandBuilder AddTerminalOption(string name, char alias, string? description = null, bool isInheritable = false, Action<Command>? onRead = null) =>
        AddTerminalOption(new(name, alias, description, isInheritable, onRead));

    public CommandBuilder AddTerminalOption(TerminalOption option) {
        _steps.Add(c => {
            c.AddTerminalOption(option);
            return c;
        });
        return this;
    }

    public CommandBuilder AddOption<T>(string name, string? description = null, bool isInheritable = false, Action<Command>? onRead = null) =>
        AddOption<T>(name, '\0', description, isInheritable, onRead);

    public CommandBuilder AddOption<T>(string name, char alias, string? description = null, bool isInheritable = false, Action<Command>? onRead = null) =>
        AddOption(new Option<T>(name, alias, description, isInheritable, onRead));

    public CommandBuilder AddOption<T>(Option<T> option) {
        _steps.Add(c => {
            c.AddOption(option);
            return c;
        });
        return this;
    }

    public CommandBuilder AddParameter<T>(string name, string? description = null, Action<Command>? onRead = null) =>
        AddParameter(new Parameter<T>(name, description, onRead));

    public CommandBuilder AddParameter<T>(Parameter<T> parameter) {
        _steps.Add(c => {
            c.AddParameter(parameter);
            return c;
        });
        return this;
    }

    public CommandBuilder AddCommand(string name, string? description = null, Action<CommandBuilder>? setup = null) {
        var builder = new CommandBuilder(false, name, description);
        builder.WithWriter(_writer);
        setup?.Invoke(builder);
        AddCommand(builder.Build());
        return this;
    }

    public CommandBuilder AddCommand(Command command) {
        _steps.Add(parent => {
            parent.AddCommand(command);
            return parent;
        });
        return this;
    }

    public Command Build()
    {
        var command = _name is null
            ? _isDefaultRoot
                ? new DefaultRootCommand(_onExecute, _onBeforeExecuteChild)
                : new RootCommand(_onExecute, _onBeforeExecuteChild)
            : new Command(_name, _description, _onExecute, _onBeforeExecuteChild);
        command.Writer = _writer;
        return _steps.Aggregate(command, (current, step) => step(current));
    }
}
