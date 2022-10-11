﻿namespace Simple.CommandLine;

public class Command : Token
{
    private readonly Command? _parent;
    private readonly Action<Command>? _onExecuting;
    private readonly IList<Argument> _arguments = new List<Argument>();
    private readonly ICollection<Parameter> _options = new List<Parameter>();
    private readonly ICollection<Command> _commands = new List<Command>();

    public Command(Command? parent, string name, string? description = null, Action<Command>? onExecuting = null) : base(name, description)
    {
        _parent = parent;
        _onExecuting = onExecuting;
        if (_parent is null) return;
        foreach (var inheritedOption in _parent._options.Where(i => i.IsAvailableToChildren).ToArray()) {
            _options.Add(inheritedOption);
        }
    }

    internal string Path => (_parent is null ? "" : _parent.Path + " ") + Name;

    public void AddArgument<T>(Argument<T> argument) {
        EnsureUniqueness(argument, nameof(argument));
        _arguments.Add(argument);
    }

    public void AddListOption<T>(ListOption<T> option) {
        EnsureUniqueness(option, nameof(option));
        _options.Add(option);
    }

    public void AddOption<T>(Option<T> option) {
        EnsureUniqueness(option, nameof(option));
        _options.Add(option);
    }

    public void AddTerminalOption(TerminalOption option) {
        EnsureUniqueness(option, nameof(option));
        _options.Add(option);
    }

    public void AddFlag(Flag flag) {
        EnsureUniqueness(flag, nameof(flag));
        _options.Add(flag);
    }

    public void AddSubCommand(Command command) => _commands.Add(command);

    protected void Execute(Span<string> arguments)
    {
        try
        {
            var argumentCount = 0;
            var parameterIndex = 0;
            while (arguments.Length > 0)
            {
                argumentCount++;
                var argument = arguments[0].Trim();
                if (!string.IsNullOrWhiteSpace(argument)) {
                    arguments = arguments[1..];
                    if (TryReadOption(argument, ref arguments, out var terminate)) continue;
                    if (terminate) return;
                    if (TryExecuteChildCommand(argument, arguments)) return;
                    if (TryReadArgument(parameterIndex++, argument, out terminate)) continue;
                    if (terminate) return;
                }

                Writer.WriteError($"Unknown option or command: '{argument}'.");
                Writer.WriteHelp(this);
                return;
            }

            Execute();
        }
        catch (Exception ex)
        {
            Writer.WriteError("An error occurred while executing command.", ex);
        }
    }

    protected virtual void Execute() {
        if (_onExecuting == null) Writer.WriteHelp(this);
        else _onExecuting.Invoke(this);
    }

    protected IReadOnlyCollection<T> GetListOptionOrDefault<T>(string name) {
        ArgumentNullException.ThrowIfNull(name, nameof(name));
        var option = _options.OfType<ListOption<T>>().FirstOrDefault(i => i.Is(name.Trim()));
        return option is null ? new List<T>() : option.Values;
    }

    protected T? GetOptionOrDefault<T>(string name) {
        ArgumentNullException.ThrowIfNull(name, nameof(name));
        var option = _options.OfType<Option<T>>().FirstOrDefault(i => i.Is(name.Trim()));
        return option is null ? default : option.Value;
    }

    protected T? GetArgumentOrDefault<T>(string name)
    {
        ArgumentNullException.ThrowIfNull(name, nameof(name));
        var argument = _arguments.OfType<Argument<T>>().FirstOrDefault(i => i.Is(name.Trim()));
        return argument is null ? default : argument.Value;
    }

    protected bool GetFlagOrDefault(string name) {
        ArgumentNullException.ThrowIfNull(name, nameof(name));
        var flag = _options.OfType<Flag>().FirstOrDefault(i => i.Is(name.Trim()));
        return flag?.IsEnable ?? false;
    }

    internal string[] GetTokenDescriptions(string type) => type switch {
        nameof(Argument) => _arguments.Select(c => c.Describe(2, 20)).ToArray(),
        nameof(Parameter) => _options.Select(c => c.Describe(2, 20)).ToArray(),
        _ => _commands.Select(c => c.Describe(2, 20)).ToArray()
    };

    private void EnsureUniqueness(Token token, string name) {
        if (_options.Any(i => i.Is(token.Name)))
            throw new ArgumentException($"An argument, flag or option with the candidate '{token.Name}' already exists.", name);
        if (token is Parameter parameter && parameter.Alias is not null && _options.Any(i => i.IsAlias(parameter.Alias)))
            throw new ArgumentException($"A flag or option with the alias '{parameter.Alias}' already exists.", name);
    }

    private bool TryReadOption(string candidate, ref Span<string> arguments, out bool terminate)
    {
        terminate = false;
        if (candidate[0] != '-') return false;
        var isName = candidate.Length > 1 && candidate[1] == '-';
        candidate = candidate.TrimStart('-');
        var option = _options.FirstOrDefault(o => isName ? o.Is(candidate) : o.IsAlias(candidate));
        if (option is null) return false;
        option.Read(this, ref arguments, out terminate);
        return !terminate;
    }

    private bool TryReadArgument(int index, string argument, out bool terminate)
    {
        if (index >= _arguments.Count)
        {
            terminate = false;
            if (_arguments.Count == 0) return false;

            terminate = true;
            Writer.WriteError($"Invalid number of arguments for command '{Name}'.");
            return false;
        }

        _arguments[index].Read(this, argument, out terminate);
        return !terminate;
    }

    private bool TryExecuteChildCommand(string candidate, Span<string> arguments)
    {
        var command = _commands.FirstOrDefault(o => o.Name.Equals(candidate, StringComparison.InvariantCultureIgnoreCase));
        if (command is null) return false;
        command.Execute(arguments);
        return true;
    }
}