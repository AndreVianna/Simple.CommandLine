using Simple.CommandLine.Parts;

using Parameter = Simple.CommandLine.Parts.Parameter;

namespace Simple.CommandLine;

public class Command : Token {
    private Command? _parent;
    private readonly Action<Command>? _onExecuting;
    private readonly IList<Parameter> _parameters = new List<Parameter>();
    private readonly ICollection<Option> _options = new List<Option>();
    private readonly ICollection<Command> _commands = new List<Command>();

    public Command(string name, string? description = null, Action<Command>? onExecuting = null, IOutputWriter? writer = null) : base(name, description, writer) {
        _onExecuting = onExecuting;
    }

    public void AddParameter<T>(Parameter<T> parameter) {
        EnsureUniqueness(parameter, nameof(parameter));
        parameter.Writer = Writer;
        _parameters.Add(parameter);
    }

    public void AddMultiOption<T>(MultiOption<T> option) {
        EnsureUniqueness(option, nameof(option));
        option.Writer = Writer;
        _options.Add(option);
    }

    public void AddOption<T>(Option<T> option) {
        EnsureUniqueness(option, nameof(option));
        option.Writer = Writer;
        _options.Add(option);
    }

    public void AddTerminalOption(TerminalOption option) {
        EnsureUniqueness(option, nameof(option));
        option.Writer = Writer;
        _options.Add(option);
    }

    public void AddFlag(Flag flag) {
        EnsureUniqueness(flag, nameof(flag));
        flag.Writer = Writer;
        _options.Add(flag);
    }

    public void AddSubCommand(Command command) {
        command._parent = this;
        command.Writer = Writer;
        foreach (var inheritedOption in _options.Where(i => i.IsInheritable).ToArray()) {
            command._options.Add(inheritedOption);
        }

        _commands.Add(command);
    }

    protected virtual void Execute() {
        if (_onExecuting == null) Writer.WriteHelp(this);
        else _onExecuting.Invoke(this);
    }

    public IReadOnlyList<T> GetCollectionOrDefault<T>(string name) {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be null or whitespace.", nameof(name));
        var option = _options.FirstOrDefault(i => i.Is(name.Trim()));
        return option switch {
            null => Array.Empty<T>(),
            MultiOption<T> typedOption => typedOption.Values,
            _ => throw new InvalidCastException($"Cannot cast value '{name}' to a collection of '{typeof(T).Name}'.")
        };
    }

    public IReadOnlyList<T> GetRequiredCollection<T>(string name) {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be null or whitespace.", nameof(name));
        var option = _options.FirstOrDefault(i => i.Is(name.Trim()));
        return option switch {
            null => throw new InvalidOperationException($"Required option '{name}' not defined."),
            MultiOption<T> typedOption => typedOption.Values,
            _ => throw new InvalidCastException($"Cannot cast value '{name}' to a collection of '{typeof(T).Name}'.")
        };
    }

    public T? GetValueOrDefault<T>(string name, T? defaultValue = default) {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be null or whitespace.", nameof(name));
        var option = _options.FirstOrDefault(i => i.Is(name.Trim()));
        if (option is not null) {
            return option switch {
                Option<T> { IsSet: false } => defaultValue,
                Option<T> typedArgument => typedArgument.Value,
                _ => throw new InvalidCastException($"Cannot cast value '{name}' to '{typeof(T).Name}'.")
            };
        }

        var parameter = _parameters.FirstOrDefault(i => i.Is(name.Trim()));
        return parameter switch {
            null => defaultValue,
            Parameter<T> { IsSet: false } => defaultValue,
            Parameter<T> typedArgument => typedArgument.Value,
            _ => throw new InvalidCastException($"Cannot cast value '{name}' to '{typeof(T).Name}'.")
        };
    }

    public T? GetValueOrDefault<T>(uint index, T? defaultValue = default) {
        if (index >= _parameters.Count) return defaultValue;
        var parameter = _parameters[(int)index];
        return parameter switch {
            Parameter<T> { IsSet: false } => defaultValue,
            Parameter<T> typedArgument => typedArgument.Value,
            _ => throw new InvalidCastException($"Cannot cast parameter at index {index} to '{typeof(T).Name}'.")
        };
    }

    public T GetRequiredValue<T>(string name) {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be null or whitespace.", nameof(name));
        var option = _options.FirstOrDefault(i => i.Is(name.Trim()));
        if (option is not null) {
            return option switch {
                Option<T> { IsSet: false } => throw new InvalidOperationException($"Missing required value '{name}'."),
                Option<T> typedArgument => typedArgument.Value,
                _ => throw new InvalidCastException($"Cannot cast value '{name}' to '{typeof(T).Name}'.")
            };
        }

        var parameter = _parameters.FirstOrDefault(i => i.Is(name.Trim()));
        return parameter switch {
            null => throw new InvalidOperationException($"Required value '{name}' not defined."),
            Parameter<T> { IsSet: false } => throw new InvalidOperationException($"Missing required value '{name}'."),
            Parameter<T> typedArgument => typedArgument.Value,
            _ => throw new InvalidCastException($"Cannot cast value '{name}' to '{typeof(T).Name}'.")
        };
    }

    public T GetRequiredValue<T>(uint index) {
        if (index >= _parameters.Count) throw new ArgumentOutOfRangeException(nameof(index), $"Parameter at index {index} not found. Parameter count is {_parameters.Count}.");
        var parameter = _parameters[(int)index];
        return parameter switch {
            Parameter<T> { IsSet: false } => throw new InvalidOperationException($"Missing required parameter at index {index}."),
            Parameter<T> typedArgument => typedArgument.Value,
            _ => throw new InvalidCastException($"Cannot cast parameter at index {index} to '{typeof(T).Name}'.")
        };
    }

    public bool GetFlagOrDefault(string name, bool defaultValue = false) {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be null or whitespace.", nameof(name));
        var flag = _options.OfType<Flag>().FirstOrDefault(i => i.Is(name.Trim()));
        return flag is null || !flag.IsSet ? defaultValue : flag.IsEnable;
    }

    public bool GetRequiredFlag(string name) {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be null or whitespace.", nameof(name));
        var flag = _options.OfType<Flag>().FirstOrDefault(i => i.Is(name.Trim()));
        return flag switch {
            null => throw new InvalidOperationException($"Required flag '{name}' not defined."),
            { IsSet: false } => throw new InvalidOperationException($"Missing required flag '{name}'."),
            _ => flag.IsEnable,
        };
    }

    internal void ExecuteInternally(Span<string> arguments) {
        try {
            while (arguments.Length > 0) {
                if (TryReadOption(ref arguments, out var terminate)) continue;
                if (terminate) return;
                if (TryExecuteChildCommand(arguments)) return;
                ReadCommandParameters(ref arguments);
                break;
            }

            Execute();
        }
        catch (Exception ex) {
            Writer.WriteError($"An error occurred while executing command '{Name}'.", ex);
        }
    }

    internal string Path => (_parent is null ? "" : _parent.Path + " ") + Name;

    internal string[] GetPartDescriptions(string type) => type switch {
        nameof(Parameter) => _parameters.Select(c => c.Describe()).ToArray(),
        nameof(Option) => _options.Select(c => c.Describe()).ToArray(),
        _ => _commands.Select(c => c.Describe()).ToArray()
    };

    private void EnsureUniqueness(Argument commandPart, string name) {
        if (_commands.Any(i => i.Is(commandPart.Name)))
            throw new ArgumentException($"A sub-command with name '{commandPart.Name}' already exists.", name);
        if (_parameters.Any(i => i.Is(commandPart.Name)))
            throw new ArgumentException($"A parameter with name '{commandPart.Name}' already exists.", name);
        if (_options.Any(i => i.Is(commandPart.Name)))
            throw new ArgumentException($"A flag or option with name '{commandPart.Name}' already exists.", name);
        if (_options.Any(i => i.Is(commandPart.Alias)))
            throw new ArgumentException($"A flag or option with alias '{commandPart.Alias}' already exists.", name);
    }

    private bool TryReadOption(ref Span<string> arguments, out bool terminate) {
        terminate = false;

        var candidate = arguments[0];
        if (candidate[0] != '-') return false;

        candidate = candidate.TrimStart('-');
        var option = _options.FirstOrDefault(o => o.Is(candidate));
        if (option is null) return false;

        arguments = arguments[1..];
        option.Read(this, ref arguments);

        terminate = option is TerminalOption;
        return !terminate;
    }

    private void ReadCommandParameters(ref Span<string> arguments) {
        foreach (var parameter in _parameters) {
            parameter.Read(this, ref arguments);
            arguments = arguments[1..];
        }
    }
    private bool TryExecuteChildCommand(Span<string> arguments) {
        var name = arguments[0].Trim();
        var command = _commands.FirstOrDefault(o => o.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        if (command is null) return false;
        command.ExecuteInternally(arguments);
        return true;
    }
}