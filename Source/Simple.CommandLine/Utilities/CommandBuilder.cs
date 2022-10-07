namespace Simple.CommandLine.Utilities;

public class CommandBuilder
{
    private readonly Command _command;

    public CommandBuilder(Command command)
    {
        _command = command;
    }

    public CommandBuilder AddArgument(Argument argument)
    {
        _command.AddArgument(argument);
        return this;
    }

    public CommandBuilder AddOption(Option option)
    {
        _command.AddOption(option);
        return this;
    }

    public CommandBuilder AddSubCommand(Command command)
    {
        _command.AddSubCommand(command);
        return this;
    }
}
