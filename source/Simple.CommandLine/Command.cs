using Parameter = Simple.CommandLine.Parts.Parameter;

namespace Simple.CommandLine;

public class Command : Token {
    private readonly Action<Command> _defaultExecution = c => c.Writer.WriteHelp(c);
    private readonly Action<Command> _onExecute;
    private readonly Action<Command>? _onBeforeExecuteChild;

    private Command? _parent;

    public Command(string name, string? description = null, Action<Command>? onExecute = null, Action<Command>? onBeforeExecuteChild = null)
        : base(name, description) {
        _onExecute = onExecute ?? _defaultExecution;
        _onBeforeExecuteChild = onBeforeExecuteChild;
    }

    internal ICollection<Option> Options { get; } = new List<Option>();
    internal IList<Parameter> Parameters { get; } = new List<Parameter>();
    internal ICollection<Command> Commands { get; } = new List<Command>();

    public bool TerminateExecution { get; set; }

    public void AddParameter<T>(Parameter<T> parameter) {
        EnsureUniqueness(parameter, nameof(parameter));
        parameter.Writer = Writer;
        Parameters.Add(parameter);
    }

    public void AddMultiOption<T>(MultiOption<T> option) {
        EnsureUniqueness(option, nameof(option));
        option.Writer = Writer;
        Options.Add(option);
    }

    public void AddOption<T>(Option<T> option) {
        EnsureUniqueness(option, nameof(option));
        option.Writer = Writer;
        Options.Add(option);
    }

    public void AddTerminalOption(TerminalOption option) {
        EnsureUniqueness(option, nameof(option));
        option.Writer = Writer;
        Options.Add(option);
    }

    public void AddFlag(Flag flag) {
        EnsureUniqueness(flag, nameof(flag));
        flag.Writer = Writer;
        Options.Add(flag);
    }

    public void AddCommand(Command command) {
        command._parent = this;
        command.Writer = Writer;
        foreach (var inheritedOption in Options.Where(i => i.IsInheritable).ToArray()) {
            command.Options.Add(inheritedOption);
        }

        Commands.Add(command);
    }

    public void Execute(params string[] arguments) => Execute(arguments.AsSpan());

    public IReadOnlyList<T> GetCollectionOrDefault<T>(string name) {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be null or whitespace.", nameof(name));
        var option = Options.FirstOrDefault(i => i.Is(name.Trim()));
        return option switch {
            null => Array.Empty<T>(),
            MultiOption<T> typedOption => typedOption.Values,
            _ => throw new InvalidCastException($"Cannot cast value '{name}' to a collection of '{typeof(T).Name}'.")
        };
    }

    public IReadOnlyList<T> GetRequiredCollection<T>(string name) {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be null or whitespace.", nameof(name));
        var option = Options.FirstOrDefault(i => i.Is(name.Trim()));
        return option switch {
            null => throw new InvalidOperationException($"Required option '{name}' not defined."),
            MultiOption<T> typedOption => typedOption.Values,
            _ => throw new InvalidCastException($"Cannot cast value '{name}' to a collection of '{typeof(T).Name}'.")
        };
    }

    public T? GetValueOrDefault<T>(string name, T? defaultValue = default) {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be null or whitespace.", nameof(name));
        var option = Options.FirstOrDefault(i => i.Is(name.Trim()));
        if (option is not null) {
            return option switch {
                Option<T> { IsSet: false } => defaultValue,
                Option<T> typedArgument => typedArgument.Value,
                _ => throw new InvalidCastException($"Cannot cast value '{name}' to '{typeof(T).Name}'.")
            };
        }

        var parameter = Parameters.FirstOrDefault(i => i.Is(name.Trim()));
        return parameter switch {
            null => defaultValue,
            Parameter<T> { IsSet: false } => defaultValue,
            Parameter<T> typedArgument => typedArgument.Value,
            _ => throw new InvalidCastException($"Cannot cast value '{name}' to '{typeof(T).Name}'.")
        };
    }

    public T? GetValueOrDefault<T>(uint index, T? defaultValue = default) {
        if (index >= Parameters.Count) return defaultValue;
        var parameter = Parameters[(int)index];
        return parameter switch {
            Parameter<T> { IsSet: false } => defaultValue,
            Parameter<T> typedArgument => typedArgument.Value,
            _ => throw new InvalidCastException($"Cannot cast parameter at index {index} to '{typeof(T).Name}'.")
        };
    }

    public T GetRequiredValue<T>(string name) {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be null or whitespace.", nameof(name));
        var option = Options.FirstOrDefault(i => i.Is(name.Trim()));
        if (option is not null) {
            return option switch {
                Option<T> { IsSet: false } => throw new InvalidOperationException($"Missing required value '{name}'."),
                Option<T> typedArgument => typedArgument.Value,
                _ => throw new InvalidCastException($"Cannot cast value '{name}' to '{typeof(T).Name}'.")
            };
        }

        var parameter = Parameters.FirstOrDefault(i => i.Is(name.Trim()));
        return parameter switch {
            null => throw new InvalidOperationException($"Required value '{name}' not defined."),
            Parameter<T> { IsSet: false } => throw new InvalidOperationException($"Missing required value '{name}'."),
            Parameter<T> typedArgument => typedArgument.Value,
            _ => throw new InvalidCastException($"Cannot cast value '{name}' to '{typeof(T).Name}'.")
        };
    }

    public T GetRequiredValue<T>(uint index) {
        if (index >= Parameters.Count) throw new ArgumentOutOfRangeException(nameof(index), $"Parameter at index {index} not found. Parameter count is {Parameters.Count}.");
        var parameter = Parameters[(int)index];
        return parameter switch {
            Parameter<T> { IsSet: false } => throw new InvalidOperationException($"Missing required parameter at index {index}."),
            Parameter<T> typedArgument => typedArgument.Value,
            _ => throw new InvalidCastException($"Cannot cast parameter at index {index} to '{typeof(T).Name}'.")
        };
    }

    public bool GetFlagOrDefault(string name, bool defaultValue = false) {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be null or whitespace.", nameof(name));
        var flag = Options.OfType<Flag>().FirstOrDefault(i => i.Is(name.Trim()));
        return flag is null || !flag.IsSet ? defaultValue : flag.IsEnable;
    }

    public bool GetRequiredFlag(string name) {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be null or whitespace.", nameof(name));
        var flag = Options.OfType<Flag>().FirstOrDefault(i => i.Is(name.Trim()));
        return flag switch {
            null => throw new InvalidOperationException($"Required flag '{name}' not defined."),
            { IsSet: false } => throw new InvalidOperationException($"Missing required flag '{name}'."),
            _ => flag.IsEnable,
        };
    }

    protected virtual void OnBeforeExecuteChild()  => _onBeforeExecuteChild?.Invoke(this);

    protected virtual void OnExecute() {
        TerminateExecution = true;
        _onExecute.Invoke(this);
    }

    private void Execute(Span<string> arguments) {
        try {
            while (arguments.Length > 0) {
                if (TryReadOption(ref arguments, out var terminate)) continue;
                if (terminate) return;
                var result = TryExecuteChildCommand(arguments);
                if (result == ExecutionResult.Terminate) return;
                if (result == ExecutionResult.Continue) break;
                ReadCommandParameters(ref arguments);
                break;
            }

            OnExecute();
        }
        catch (Exception ex) {
            Writer.WriteError($"An error occurred while executing command '{Name}'.", ex);
        }
    }

    internal string Path => (_parent is null ? "" : _parent.Path + " ") + Name;

    private void EnsureUniqueness(Argument commandPart, string name) {
        if (Commands.Any(i => i.Is(commandPart.Name)))
            throw new ArgumentException($"A sub-command with name '{commandPart.Name}' already exists.", name);
        if (Parameters.Any(i => i.Is(commandPart.Name)))
            throw new ArgumentException($"A parameter with name '{commandPart.Name}' already exists.", name);
        if (Options.Any(i => i.Is(commandPart.Name)))
            throw new ArgumentException($"A flag or option with name '{commandPart.Name}' already exists.", name);
        if (Options.Any(i => i.IsAlias(commandPart.Alias)))
            throw new ArgumentException($"A flag or option with alias '{commandPart.Alias}' already exists.", name);
    }

    private bool TryReadOption(ref Span<string> arguments, out bool terminate) {
        terminate = false;

        var candidate = arguments[0];
        if (candidate[0] != '-') return false;

        candidate = candidate.TrimStart('-');
        if (candidate.Length == 0) return false;
        var option = Options.FirstOrDefault(o => o.Is(candidate)) ?? Options.FirstOrDefault(o => o.IsAlias(candidate[0]));
        if (option is null) return false;

        arguments = arguments[1..];
        option.Read(this, ref arguments);

        terminate = option is TerminalOption;
        return !terminate;
    }

    private void ReadCommandParameters(ref Span<string> arguments) {
        foreach (var parameter in Parameters) {
            parameter.Read(this, ref arguments);
            arguments = arguments[1..];
        }
    }
    private ExecutionResult TryExecuteChildCommand(Span<string> arguments) {
        var name = arguments[0].Trim();
        var command = Commands.FirstOrDefault(o => o.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        if (command is null) return ExecutionResult.NotFound;
        OnBeforeExecuteChild();
        command.Execute(arguments);
        return command.TerminateExecution ? ExecutionResult.Terminate : ExecutionResult.Continue;
    }

    private enum ExecutionResult {
        NotFound,
        Continue,
        Terminate
    }
}