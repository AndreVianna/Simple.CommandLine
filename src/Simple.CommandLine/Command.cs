namespace Simple.CommandLine;

public abstract class Command : Token
{
    private readonly Command? _parent;
    private readonly IList<Argument> _arguments = new List<Argument>();
    private readonly ICollection<Option> _options = new List<Option>();
    private readonly ICollection<Command> _commands = new List<Command>();

    protected Command(Command? parent, string name, string? description = null) : base(name, description)
    {
        _parent = parent;
        if (_parent is null) return;
        foreach (var sharedOption in _parent._options.Where(i => i.IsShared).ToArray())
        {
            AddOption(sharedOption);
        }
    }

    public string Path => (_parent is null ? "" : _parent.Path + " ") + Name;

    public void AddArgument(Argument argument) => _arguments.Add(argument);

    public void AddOption(Option option) => _options.Add(option);

    public void AddSubCommand(Command command) => _commands.Add(command);

    protected void Execute(Span<string> arguments)
    {
        try
        {
            var parameterIndex = 0;
            while (arguments.Length > 0)
            {
                var argument = arguments[0];
                arguments = arguments[1..];
                if (TryReadOption(argument, ref arguments, out var terminate)) continue;
                if (terminate) return;
                if (TryExecuteChild(argument, arguments)) return;
                if (TryReadArgument(parameterIndex++, argument, out terminate)) continue;
                if (terminate) return;

                Writer.WriteErrorLine($"Unknown option or command: '{argument}'.");
                return;
            }

            Execute();
        }
        catch (Exception ex)
        {
            Writer.WriteErrorLine("An error occurred while executing command.", ex);
        }
    }

    protected T? GetOptionOrDefault<T>(string name) {
        var option = _options.OfType<Option<T>>().FirstOrDefault(i => i.Names.Contains(name.Trim().TrimStart('-'), StringComparer.InvariantCultureIgnoreCase));
        return option is null ? default : option.Value;
    }

    protected T? GetArgumentOrDefault<T>(string name)
    {
        var argument = _arguments.OfType<Argument<T>>().FirstOrDefault(i => i.Name.Equals(name.Trim(), StringComparison.InvariantCultureIgnoreCase));
        return argument is null ? default : argument.Value;
    }

    protected bool GetFlagOrDefault(string name) {
        var flag = _options.OfType<Flag>().FirstOrDefault(i => i.Names.Contains(name.Trim().TrimStart('-'), StringComparer.InvariantCultureIgnoreCase));
        return flag?.IsEnable ?? false;
    }

    protected virtual void Execute() => Writer.WriteHelp(this);

    internal string[] GetTokenDescriptions(string type) => type switch {
        nameof(Argument) => _arguments.Select(c => c.Describe(2, 20)).ToArray(),
        nameof(Option) => _options.Select(c => c.Describe(2, 20)).ToArray(),
        _ => _commands.Select(c => c.Describe(2, 20)).ToArray()
    };

    private bool TryReadOption(string name, ref Span<string> arguments, out bool terminate)
    {
        terminate = false;
        var option = _options.FirstOrDefault(o => o.IsAlias(name));
        if (option is null) return false;
        option.Read(this, ref arguments, out terminate);
        return true;
    }

    private bool TryReadArgument(int index, string argument, out bool terminate)
    {
        if (index >= _arguments.Count)
        {
            Writer.WriteErrorLine($"Invalid number of arguments for command '{Name}'.");
            terminate = true;
            return false;
        }

        _arguments[index].Read(this, argument, out terminate);
        return !terminate;
    }

    private bool TryExecuteChild(string name, Span<string> arguments)
    {
        var command = _commands.FirstOrDefault(o => o.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        if (command is null) return false;
        command.Execute(arguments);
        return true;
    }
}