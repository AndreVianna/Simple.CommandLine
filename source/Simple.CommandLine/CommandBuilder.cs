namespace Simple.CommandLine;

public sealed class CommandBuilder
{
    private readonly Command? _parent;
    private readonly string? _name;
    private readonly string? _description;
    private readonly ICollection<Func<Command, Command>> _steps = new List<Func<Command, Command>>();

    private CommandBuilder(Command? parent = null, string? name = null, string? description = null) {
        _parent = parent;
        _name = name;
        _description = description;
    }

    public static CommandBuilder FromRoot() => new();

    public CommandBuilder AddFlag(string name, string? description = null, bool isInheritable = false) =>
        AddFlag(name, '\0', description, isInheritable);

    public CommandBuilder AddFlag(string name, char alias, string? description = null, bool isInheritable = false)
        => AddFlag(new(name, alias, description, isInheritable));

    public CommandBuilder AddFlag(Flag flag) {
        _steps.Add(c => {
            c.AddFlag(flag);
            return c;
        });
        return this;
    }

    public CommandBuilder AddListOption<T>(string name, string? description = null, bool isInheritable = false) =>
        AddListOption<T>(name, '\0', description, isInheritable);

    public CommandBuilder AddListOption<T>(string name, char alias, string? description = null, bool isInheritable = false) =>
        AddListOption<T>(new(name, alias, description, isInheritable));

    public CommandBuilder AddListOption<T>(ListOption<T> option) {
        _steps.Add(c => {
            c.AddListOption(option);
            return c;
        });
        return this;
    }

    public CommandBuilder AddTerminalOption(string name, string? description = null, bool isInheritable = false) =>
        AddTerminalOption(name, '\0', description, isInheritable);

    public CommandBuilder AddTerminalOption(string name, char alias, string? description = null, bool isInheritable = false) =>
        AddTerminalOption(new(name, alias, description, isInheritable));

    public CommandBuilder AddTerminalOption(TerminalOption option) {
        _steps.Add(c => {
            c.AddTerminalOption(option);
            return c;
        });
        return this;
    }

    public CommandBuilder AddOption<T>(string name, string? description = null, bool isInheritable = false) =>
        AddOption<T>(name, '\0', description, isInheritable);

    public CommandBuilder AddOption<T>(string name, char alias, string? description = null, bool isInheritable = false) =>
        AddOption(new Option<T>(name, alias, description, isInheritable));

    public CommandBuilder AddOption<T>(Option<T> option) {
        _steps.Add(c => {
            c.AddOption(option);
            return c;
        });
        return this;
    }

    public CommandBuilder AddArgument<T>(string name, string? description = null) =>
        AddArgument(new Argument<T>(name, description));

    public CommandBuilder AddArgument<T>(Argument<T> argument) {
        _steps.Add(c => {
            c.AddArgument(argument);
            return c;
        });
        return this;
    }

    public CommandBuilder AddSubCommand(string name, Action<CommandBuilder> setup) =>
        AddSubCommand(name, null!, setup);

    public CommandBuilder AddSubCommand(string name, string description, Action<CommandBuilder> setup) {
        _steps.Add(c => {
            var builder = new CommandBuilder(c, name, description);
            setup(builder);
            c.AddSubCommand(builder.Build());
            return c;
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

    public Command Build() {
        var command = _name is null ? new RootCommand() : new Command(_parent, _name, _description);
        return _steps.Aggregate(command, (current, config) => config(current));
    }
}
