namespace Simple.CommandLine;

public sealed class CommandBuilder
{
    private readonly IOutputWriter? _writer;
    private readonly string? _name;
    private readonly string? _description;
    private readonly Action<Command>? _onExecuting;
    private readonly ICollection<Func<Command, Command>> _steps = new List<Func<Command, Command>>();

    private CommandBuilder(IOutputWriter? writer, string? name = null, string? description = null, Action<Command> ? onExecuting = null)
    {
        _writer = writer ?? new ConsoleOutputWriter();
        _name = name;
        _description = description;
        _onExecuting = onExecuting;
    }

    public static CommandBuilder FromRoot(Action<Command>? onExecuting = null, IOutputWriter ? writer = null) => new(writer, onExecuting : onExecuting);

    public CommandBuilder AddFlag(string name, string? description = null, bool isInheritable = false, Action<Command>? onRead = null) =>
        AddFlag(name, '\0', description, isInheritable, onRead);

    public CommandBuilder AddFlag(string name, char alias, string? description = null, bool isInheritable = false, Action<Command>? onRead = null)
        => AddFlag(new(name, alias, description, isInheritable, onRead, _writer));

    public CommandBuilder AddFlag(Flag flag) {
        _steps.Add(c => {
            c.AddFlag(flag);
            return c;
        });
        return this;
    }

    public CommandBuilder AddListOption<T>(string name, string? description = null, bool isInheritable = false, Action<Command>? onRead = null) =>
        AddListOption<T>(name, '\0', description, isInheritable, onRead);

    public CommandBuilder AddListOption<T>(string name, char alias, string? description = null, bool isInheritable = false, Action<Command>? onRead = null) =>
        AddListOption<T>(new(name, alias, description, isInheritable, onRead, _writer));

    public CommandBuilder AddListOption<T>(MultiOption<T> option) {
        _steps.Add(c => {
            c.AddMultiOption(option);
            return c;
        });
        return this;
    }

    public CommandBuilder AddTerminalOption(string name, string? description = null, bool isInheritable = false, Action<Command>? onRead = null) =>
        AddTerminalOption(name, '\0', description, isInheritable, onRead);

    public CommandBuilder AddTerminalOption(string name, char alias, string? description = null, bool isInheritable = false, Action<Command>? onRead = null) =>
        AddTerminalOption(new(name, alias, description, isInheritable, onRead, _writer));

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
        AddOption(new Option<T>(name, alias, description, isInheritable, onRead, _writer));

    public CommandBuilder AddOption<T>(Option<T> option) {
        _steps.Add(c => {
            c.AddOption(option);
            return c;
        });
        return this;
    }

    public CommandBuilder AddArgument<T>(string name, string? description = null, Action<Command>? onRead = null) =>
        AddArgument(new Parameter<T>(name, description, onRead, _writer));

    public CommandBuilder AddArgument<T>(Parameter<T> parameter) {
        _steps.Add(c => {
            c.AddParameter(parameter);
            return c;
        });
        return this;
    }

    public CommandBuilder AddSubCommand(string name, Action<CommandBuilder> setup, Action<Command>? onExecuting = null) =>
        AddSubCommand(name, null!, setup, onExecuting);

    public CommandBuilder AddSubCommand(string name, string description, Action<CommandBuilder> setup, Action<Command>? onExecuting = null) {
        _steps.Add(parent => {
            var builder = new CommandBuilder(_writer, name, description, onExecuting);
            setup(builder);
            parent.AddSubCommand(builder.Build());
            return parent;
        });
        return this;
    }

    public CommandBuilder AddSubCommand(Command command) {
        _steps.Add(c => {
            c.AddSubCommand(command);
            return c;
        });
        return this;
    }

    public Command Build()
    {
        var command = _name is null ? new RootCommand(_onExecuting, _writer) : new Command(_name, _description, _onExecuting);
        return _steps.Aggregate(command, (current, config) => config(current));
    }
}
