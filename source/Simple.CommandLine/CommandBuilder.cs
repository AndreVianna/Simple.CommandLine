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

    public CommandBuilder AddFlag(Flag flag) {
        _steps.Add(c => {
            c.AddFlag(flag);
            return c;
        });
        return this;
    }

    public CommandBuilder AddFlag(IReadOnlyCollection<string> names, string? description = null, bool isInheritable = false) {
        _steps.Add(c => {
            var flag = new Flag(names, description, isInheritable);
            c.AddFlag(flag);
            return c;
        });
        return this;
    }

    public CommandBuilder AddFlag(string name, string? description = null, bool isInheritable = false) =>
        AddFlag(new[] { name }, description, isInheritable);

    public CommandBuilder AddOption(IReadOnlyCollection<string> names, string? description = null, bool isInheritable = false) {
        _steps.Add(c => {
            var option = new Option(names, description, isInheritable);
            c.AddOption(option);
            return c;
        });
        return this;
    }

    public CommandBuilder AddOption(string name, string? description = null, bool isInheritable = false) =>
        AddOption(new[] { name }, description, isInheritable);

    public CommandBuilder AddOption(Option option) {
        _steps.Add(c => {
            c.AddOption(option);
            return c;
        });
        return this;
    }

    public CommandBuilder AddOption<T>(IReadOnlyCollection<string> names, string? description = null, bool isInheritable = false) {
        _steps.Add(c => {
            var option = new Option<T>(names, description, isInheritable);
            c.AddOption(option);
            return c;
        });
        return this;
    }

    public CommandBuilder AddOption<T>(string name, string? description = null, bool isInheritable = false) =>
        AddOption<T>(new[] { name }, description, isInheritable);

    public CommandBuilder AddOption<T>(Option<T> option) {
        _steps.Add(c => {
            c.AddOption(option);
            return c;
        });
        return this;
    }

    public CommandBuilder AddArgument<T>(string name, string? description = null) {
        _steps.Add(c => {
            var argument = new Argument<T>(name, description);
            c.AddArgument(argument);
            return c;
        });
        return this;
    }

    public CommandBuilder AddArgument<T>(Argument<T> argument) {
        _steps.Add(c => {
            c.AddArgument<T>(argument);
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

    public CommandBuilder AddSubCommand(string name, Action<CommandBuilder> setup) {
        _steps.Add(c => {
            var builder = new CommandBuilder(c, name);
            setup(builder);
            c.AddSubCommand(builder.Build());
            return c;
        });
        return this;
    }

    public CommandBuilder AddSubCommand(string name, string description, Action<CommandBuilder> setup) {
        _steps.Add(c => {
            var builder = new CommandBuilder(c, name, description);
            setup(builder);
            c.AddSubCommand(builder.Build());
            return c;
        });
        return this;
    }

    public Command Build() {
        var command = _name is null ? new RootCommand() : new Command(_parent, _name, _description);
        return _steps.Aggregate(command, (current, config) => config(current));
    }
}
