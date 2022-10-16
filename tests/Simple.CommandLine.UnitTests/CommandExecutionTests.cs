namespace Simple.CommandLine;

public class CommandExecutionTests {
    private readonly InMemoryOutputWriter _writer = new();

    [Fact]
    public void Command_Execute_WithException_AndVerboseFlagAndNoColor_ShowError() {
        var projectRootFolder = Directory.GetCurrentDirectory().Replace("\\Simple.CommandLine\\tests\\Simple.CommandLine.UnitTests\\bin\\Debug\\net6.0".Replace('\\', Path.DirectorySeparatorChar), "");
        var subject = CommandBuilder.FromRoot().WithWriter(_writer)
            .OnExecute(_ => throw new("Some exception."))
            .AddFlag(new DefaultVerboseFlag())
            .AddFlag(new DefaultNoColorFlag())
            .Build();

        subject.Execute("-v", "--no-color");

        _writer.Output.Should().Be(
@"An error occurred while executing command 'testhost'.
System.Exception: Some exception.
   at Simple.CommandLine.CommandExecutionTests.<>c.<Command_Execute_WithException_AndVerboseFlagAndNoColor_ShowError>b__1_0(Command _) in C:\Projects\Simple.CommandLine\tests\Simple.CommandLine.UnitTests\CommandExecutionTests.cs:line 10
   at Simple.CommandLine.Command.OnExecute() in C:\Projects\Simple.CommandLine\source\Simple.CommandLine\Command.cs:line 166
   at Simple.CommandLine.Command.Execute(Span`1 arguments) in C:\Projects\Simple.CommandLine\source\Simple.CommandLine\Command.cs:line 181
".Replace("\r", "").Replace("C:\\Projects", projectRootFolder).Replace('\\', Path.DirectorySeparatorChar));
    }

    [Fact]
    public void Command_Execute_WithException_AndVerboseFlag_ShowError() {
        var projectRootFolder = Directory.GetCurrentDirectory().Replace("\\Simple.CommandLine\\tests\\Simple.CommandLine.UnitTests\\bin\\Debug\\net6.0".Replace('\\', Path.DirectorySeparatorChar), "");
        var subject = CommandBuilder.FromRoot().WithWriter(_writer)
            .OnExecute(_ => throw new("Some exception."))
            .AddFlag(new DefaultVerboseFlag())
            .Build();

        subject.Execute("-v");

        _writer.Output.Should().Be(
@"An error occurred while executing command 'testhost'.
System.Exception: Some exception.
   at Simple.CommandLine.CommandExecutionTests.<>c.<Command_Execute_WithException_AndVerboseFlag_ShowError>b__2_0(Command _) in C:\Projects\Simple.CommandLine\tests\Simple.CommandLine.UnitTests\CommandExecutionTests.cs:line 30
   at Simple.CommandLine.Command.OnExecute() in C:\Projects\Simple.CommandLine\source\Simple.CommandLine\Command.cs:line 166
   at Simple.CommandLine.Command.Execute(Span`1 arguments) in C:\Projects\Simple.CommandLine\source\Simple.CommandLine\Command.cs:line 181
".Replace("\r", "").Replace("C:\\Projects", projectRootFolder).Replace('\\', Path.DirectorySeparatorChar));
    }

    [Fact]
    public void Command_Execute_ExecutesDelegate() {
        var subject = new Command("Command", "Command description.", c => c.Writer.WriteLine("Executing command...")) {
            Writer = _writer
        };

        subject.Execute();

        _writer.Output.Should().Be("Executing command...\n");
    }

    [Fact]
    public void Command_Execute_WithTerminalOption_ExecutesDelegate() {
        var subject = CommandBuilder.FromRoot().WithWriter(_writer)
            .OnExecute(r => r.Writer.WriteLine("You should not be here!"))
            .AddTerminalOption("option", onRead: c => c.Writer.WriteLine("Stop here!"))
            .Build();

        subject.Execute("--option");

        _writer.Output.Should().Be("Stop here!\n");
    }

    [Fact]
    public void Command_Execute_WithUnknownOption_ExecutesDelegate() {
        var subject = CommandBuilder.FromRoot().WithWriter(_writer)
            .OnExecute(r => r.Writer.WriteLine("Executing command..."))
            .Build();

        subject.Execute("--option");

        _writer.Output.Should().Be("Executing command...\n");
    }

    [Fact]
    public void Command_Execute_WithEmptyArgument_ExecutesDelegate() {
        var subject = CommandBuilder.FromRoot().WithWriter(_writer)
            .OnExecute(r => r.Writer.WriteLine("Executing command..."))
            .Build();

        subject.Execute("  ");

        _writer.Output.Should().Be("Executing command...\n");
    }

    [Fact]
    public void Command_Execute_WithEmptyName_ExecutesDelegate() {
        var subject = CommandBuilder.FromRoot().WithWriter(_writer)
            .OnExecute(r => r.Writer.WriteLine("Executing command..."))
            .Build();

        subject.Execute("--");

        _writer.Output.Should().Be("Executing command...\n");
    }

    [Fact]
    public void Command_Execute_WithEmptyAlias_ExecutesDelegate() {
        var subject = CommandBuilder.FromRoot().WithWriter(_writer)
            .OnExecute(r => r.Writer.WriteLine("Executing command..."))
            .Build();

        subject.Execute("-");

        _writer.Output.Should().Be("Executing command...\n");
    }

    [Fact]
    public void Command_Execute_WithChildCommand_ExecutesDelegate() {
        var subject = CommandBuilder.FromRoot().WithWriter(_writer)
            .OnExecute(r => r.Writer.WriteLine("You should not be here!"))
            .OnBeforeExecuteChild(r => r.Writer.WriteLine("Before execute child!"))
            .AddCommand("child", setup: b =>
                b.OnExecute(c => c.Writer.WriteLine("Executing child...")))
            .Build();

        subject.Execute("child");

        _writer.Output.Should().Be("Before execute child!\nExecuting child...\n");
    }

    [Fact]
    public void Command_Execute_WithChildCommandAndContinuation_ExecutesDelegate() {
        var subject = CommandBuilder.FromRoot().WithWriter(_writer)
            .OnExecute(r => r.Writer.WriteLine("Now you can see me!"))
            .OnBeforeExecuteChild(r => r.Writer.WriteLine("Before execute child!"))
            .AddCommand("child", setup: b =>
                b.OnExecute(c => {
                    c.Writer.WriteLine("Executing child...");
                    c.TerminateExecution = false;
                }))
            .Build();

        subject.Execute("child");

        _writer.Output.Should().Be("Before execute child!\nExecuting child...\nNow you can see me!\n");
    }

    [Fact]
    public void Command_Execute_WithExceptionDuringExecution_ShowError() {
        var subject = CommandBuilder.FromRoot().WithWriter(_writer)
            .OnExecute(_ => throw new("Some exception."))
            .Build();

        subject.Execute();

        _writer.Output.Should().Be("An error occurred while executing command 'testhost'.\n");
    }

    [Fact]
    public void Command_Execute_WithExceptionDuringRead_ShowError() {
        var subject = CommandBuilder.FromRoot().WithWriter(_writer)
            .OnExecute(r => r.Writer.WriteLine("You should not be here!"))
            .AddOption<string>("option", onRead: _ => throw new("Some exception."))
            .Build();

        subject.Execute("--option", "abc");

        _writer.Output.Should().Be("An error occurred while reading argument 'option'.\nAn error occurred while executing command 'testhost'.\n");
    }

    [Fact]
    public void Command_Execute_WithRootHelp_ShowsHelp() {
        var subject = CommandBuilder.FromRoot().WithWriter(_writer).Build();

        subject.Execute();

        _writer.Output.Should().Be(@"
Simple.CommandLine 0.1.0-rc1

This package provides tools for creating a simple CLI (Command-Line Interface) console application.

Usage: testhost

".Replace("\r", ""));
    }

    [Fact]
    public void Command_Execute_WithChildCommand_ShowsHelp() {
        var subject = CommandBuilder.FromRoot().WithWriter(_writer)
            .OnExecute(r => r.Writer.WriteLine("You should not be here!"))
            .AddCommand("child")
            .Build();

        subject.Execute("child");

        _writer.Output.Should().Be(@"
Usage: testhost child

".Replace("\r", ""));
    }

    [Fact]
    public void Command_Execute_DefaultRoot_ShowsHelp() {
        var subject = CommandBuilder.FromDefaultRoot().WithWriter(_writer).Build();

        subject.Execute("--version");

        _writer.Output.Should().Be("Simple.CommandLine\n0.1.0-rc1\n");
    }

    [Fact]
    public void Command_Execute_WithHelpLongCommandName_ShowsHelp() {
        var subject = new Command("command", "Command description.", c => c.Writer.WriteLine("You should not be here!"));
        subject.AddTerminalOption(new DefaultHelpOption());
        subject.AddOption(new Option<string>("options"));
        subject.AddOption(new Option<string>("very-long-name", 'v', "Some description"));
        subject.AddCommand(new("child", onExecute: c => c.Writer.WriteLine("Executing child...")));
        subject.AddParameter(new Parameter<string>("param"));
        subject.Writer = _writer;

        subject.Execute("-h");

        _writer.Output.Should().Be(@"
Usage: command [parameters] [options]
Usage: command [options] [command]

Options:
  --help|-h              Show this help information and exit.
  --options <options>
  --very-long-name|-v <very-long-name> Some description

Parameters:
  <param>

Commands:
  child

Use ""command [command] --help"" for more information about a command.

".Replace("\r", ""));
    }
}