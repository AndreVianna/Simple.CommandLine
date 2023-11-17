using DotNetToolbox.CommandLineBuilder.Extensions;

namespace DotNetToolbox.CommandLineBuilder;

public class CommandExecutionTests {
    private readonly InMemoryOutputWriter _writer = new();

    [Fact]
    public void Command_Execute_WithException_AndVerboseFlagAndNoColor_ShowError() {
        var subject = CommandBuilder.FromRoot(_writer)
            .OnExecute(_ => throw new("Some exception."))
            .Build();

        subject.Execute("-v", "2", "--no-color");

        _ = _writer.Output.Should().Be("An error occurred while executing command 'testhost'.\n");
    }

    [Fact]
    public void Command_Execute_WithException_AndVerboseLevel_Detailed_ShowError() {
        var subject = CommandBuilder.FromRoot(_writer)
            .OnExecute(_ => throw new("Some exception."))
            .Build();

        subject.Execute("-v", "2");

        _ = _writer.Output.Should().Be("An error occurred while executing command 'testhost'.\n");
    }

    [Fact]
    public void Command_Execute_WithException_AndVerboseLevel_Silent_ShowNotShowError() {
        var subject = CommandBuilder.FromRoot(_writer)
            .OnExecute(_ => throw new("Some exception."))
            .Build();

        subject.Execute("-v", "6");

        _ = _writer.Output.Should().BeEmpty();
    }

    [Fact]
    public void Command_Execute_WithException_AndVerboseLevel_Debug_ShowErrorWithException() {
        var subject = CommandBuilder.FromRoot(_writer)
            .OnExecute(_ => throw new("Some exception."))
            .Build();

        subject.Execute("-v", "1");

        _ = _writer.Output.Should().Contain("An error occurred while executing command 'testhost'.\nSystem.Exception: Some exception.\n");
    }

    [Fact]
    public void Command_Execute_WriteError_WithoutException_AndVerboseLevel_Silent_ShowNoError() {
        var subject = CommandBuilder.FromRoot(_writer)
            .OnExecute(c => c.Writer.WriteError("Some error."))
            .Build();

        subject.Execute("-v", "6");

        _ = _writer.Output.Should().BeEmpty();
    }

    [Fact]
    public void Command_Execute_ExecutesDelegate() {
        var subject = new SubCommand("Command", "Command description.", c => c.Writer.WriteLine("Executing command...")) {
            Writer = _writer
        };

        subject.Execute();

        _ = _writer.Output.Should().Be("Executing command...\n");
    }

    [Fact]
    public void Command_Execute_WithTerminalOption_ExecutesDelegate() {
        var subject = CommandBuilder.FromRoot(_writer)
            .OnExecute(r => r.Writer.WriteLine("You should not be here!"))
            .AddFlag("option", onRead: c => c.Writer.WriteLine("Stop here!"), existsIfSet: true)
            .Build();

        subject.Execute("--option");

        _ = _writer.Output.Should().Be("Stop here!\n");
    }

    [Fact]
    public void Command_Execute_WithUnknownOption_ExecutesDelegate() {
        var subject = CommandBuilder.FromRoot(_writer)
            .OnExecute(r => r.Writer.WriteLine("Executing command..."))
            .Build();

        subject.Execute("--option");

        _ = _writer.Output.Should().Be("Executing command...\n");
    }

    [Fact]
    public void Command_Execute_WithEmptyArgument_ExecutesDelegate() {
        var subject = CommandBuilder.FromRoot(_writer)
            .OnExecute(r => r.Writer.WriteLine("Executing command..."))
            .Build();

        subject.Execute("  ");

        _ = _writer.Output.Should().Be("Executing command...\n");
    }

    [Fact]
    public void Command_Execute_WithEmptyName_ExecutesDelegate() {
        var subject = CommandBuilder.FromRoot(_writer)
            .OnExecute(r => r.Writer.WriteLine("Executing command..."))
            .Build();

        subject.Execute("--");

        _ = _writer.Output.Should().Be("Executing command...\n");
    }

    [Fact]
    public void Command_Execute_WithEmptyAlias_ExecutesDelegate() {
        var subject = CommandBuilder.FromRoot(_writer)
            .OnExecute(r => r.Writer.WriteLine("Executing command..."))
            .Build();

        subject.Execute("-");

        _ = _writer.Output.Should().Be("Executing command...\n");
    }

    [Fact]
    public void Command_Execute_WithChildCommand_ExecutesDelegate() {
        var subject = CommandBuilder.FromRoot(_writer)
            .OnExecute(r => r.Writer.WriteLine("You should not be here!"))
            .OnBeforeSubCommand((_, r) => r.Writer.WriteLine("Before execute sub-Command!"))
            .AddSubCommand("sub", setup: b =>
                b.OnExecute(c => c.Writer.WriteLine("Executing sub-Command...")))
            .OnAfterSubCommand((_, r) => r.Writer.WriteLine("Sub-Command executed!"))
            .Build();

        subject.Execute("sub");

        _ = _writer.Output.Should().Be("Before execute sub-Command!\nExecuting sub-Command...\nSub-Command executed!\n");
    }

    [Fact]
    public void Command_Execute_WithExceptionDuringExecution_ShowError() {
        var subject = CommandBuilder.FromRoot(_writer)
            .OnExecute(_ => throw new("Some exception."))
            .Build();

        subject.Execute();

        _ = _writer.Output.Should().Be("An error occurred while executing command 'testhost'.\n");
    }

    [Fact]
    public void Command_Execute_WithExceptionDuringRead_ShowError() {
        var subject = CommandBuilder.FromRoot(_writer)
            .OnExecute(r => r.Writer.WriteLine("You should not be here!"))
            .AddOption<string>("option", onRead: _ => throw new("Some exception."))
            .Build();

        subject.Execute("--option", "abc");

        _ = _writer.Output.Should().Be("An error occurred while reading option 'option'.\nAn error occurred while executing command 'testhost'.\n");
    }

    [Fact]
    public void Command_Execute_WithRootHelp_ShowsHelp() {
        var subject = CommandBuilder.FromRoot(_writer).Build();

        subject.Execute();

        _ = _writer.Output.Should().Be("""

                                        DotNetToolbox.CommandLineBuilder 7.0.0

                                        This package provides tools for creating a simple CLI (Command-Line Interface) console application.

                                        Usage: testhost [options]

                                        Options:
                                          -h, --help                    Show this help information and exit.
                                          --no-color                    Don't colorize output.
                                          -v, --verbose <verbose>       Show verbose output.
                                          --version                     Show version information and exit.


                                        """.Replace("\r", ""));
    }

    [Fact]
    public void Command_Execute_WithChildCommand_ShowsHelp() {
        var subject = CommandBuilder.FromRoot(_writer)
            .OnExecute(r => r.Writer.WriteLine("You should not be here!"))
            .AddSubCommand("sub-Command")
            .Build();

        subject.Execute("sub-Command");

        _ = _writer.Output.Should().Be("""

                                        Usage: testhost sub-Command

                                        Options:
                                          -h, --help                    Show this help information and exit.


                                        """.Replace("\r", ""));
    }

    [Fact]
    public void Command_Execute_DefaultRoot_ShowsHelp() {
        var subject = CommandBuilder.FromRoot(_writer).Build();

        subject.Execute("--version");

        _ = _writer.Output.Should().Be("DotNetToolbox.CommandLineBuilder\n7.0.0\n");
    }

    [Fact]
    public void Command_Execute_WithHelpLongCommandName_ShowsHelp() {
        var subject = new SubCommand("command", "Command description.", c => c.Writer.WriteLine("You should not be here!"));
        subject.Add(new Option<string>("options"));
        subject.Add(new Option<string>("very-long-name", 'v', "Some description"));
        subject.Add(new SubCommand("sub-Command", onExecute: c => c.Writer.WriteLine("Executing sub-Command...")));
        subject.Add(new Parameter<string>("param"));
        subject.Writer = _writer;

        subject.Execute("-h");

        _ = _writer.Output.Should().Be("""

                                   Usage: command [parameters] [options]
                                   Usage: command [options] [command]

                                   Options:
                                     -h, --help                    Show this help information and exit.
                                     --options <options>
                                     -v, --very-long-name <very-long-name> Some description

                                   Parameters:
                                     <param>

                                   Commands:
                                     sub-Command

                                   Use "command [command] --help" for more information about a command.


                                   """.Replace("\r", ""));
    }
}
