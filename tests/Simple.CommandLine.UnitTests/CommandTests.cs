using System;

using Newtonsoft.Json.Linq;
using Simple.CommandLine.Defaults;

namespace Simple.CommandLine;

public class CommandTests {
    [Fact]
    public void Command_Execute_ExecutesDelegate() {
        var writer = new InMemoryOutputWriter();
        var subject = new Command("Command", "Command description.", c => c.Writer.WriteLine("Executing command..."), writer);

        subject.ExecuteInternally(Array.Empty<string>());

        subject.Describe().Should().BeEquivalentTo("  command                Command description.");
        subject.GetPartDescriptions(nameof(Option)).Should().BeEmpty();
        subject.GetPartDescriptions(nameof(Parameter)).Should().BeEmpty();
        subject.GetPartDescriptions(nameof(Command)).Should().BeEmpty();

        writer.Output.Should().Be("Executing command...\n");
    }

    [Fact]
    public void Command_Execute_WithTerminalOption_ExecutesDelegate() {
        var writer = new InMemoryOutputWriter();
        var subject = new Command("Command", onExecuting: c => c.Writer.WriteLine("You should not be here!"), writer: writer);
        subject.AddTerminalOption(new("option", onRead: c => c.Writer.WriteLine("Stop here!")));

        subject.ExecuteInternally(new[] { "--option" });

        writer.Output.Should().Be("Stop here!\n");
    }

    [Fact]
    public void Command_Execute_WithChildCommand_ExecutesDelegate() {
        var writer = new InMemoryOutputWriter();
        var subject = new Command("command", "Command description.", c => c.Writer.WriteLine("You should not be here!"), writer);
        subject.AddSubCommand(new("child", onExecuting: c => c.Writer.WriteLine("Executing child...")));

        subject.ExecuteInternally(new[] { "child" });

        writer.Output.Should().Be("Executing child...\n");
    }

    [Fact]
    public void Command_Execute_WithExceptionDuringExecution_ShowError() {
        var writer = new InMemoryOutputWriter();
        var subject = new Command("command", onExecuting: _ => throw new Exception("Some exception."), writer: writer);

        subject.ExecuteInternally(Array.Empty<string>());

        writer.Output.Should().Be("An error occurred while executing command 'command'.\n");
    }

    [Fact]
    public void Command_Execute_WithExceptionDuringRead_ShowError() {
        var writer = new InMemoryOutputWriter();
        var subject = new Command("command", onExecuting: c => c.Writer.WriteLine("You should not see this!"), writer: writer);
        subject.AddOption<string>(new("option", onRead: _ => throw new Exception("Some exception.")));

        subject.ExecuteInternally(new[] { "--option", "abc" });

        writer.Output.Should().Be("An error occurred while reading argument 'option'.\nAn error occurred while executing command 'command'.\n");
    }

    [Fact]
    public void Command_Execute_WithException_AndVerboseFlagAndNoColor_ShowError() {
        var writer = new InMemoryOutputWriter();
        var subject = new Command("command", onExecuting: _ => throw new Exception("Some exception."), writer: writer);
        subject.AddFlag(new DefaultVerboseFlag());
        subject.AddFlag(new DefaultNoColorFlag());

        subject.ExecuteInternally(new[] { "-v", "--no-color" });

        writer.Output.Should().Be(
            @"An error occurred while executing command 'command'.
System.Exception: Some exception.
   at Simple.CommandLine.CommandTests.<>c.<Command_Execute_WithException_AndVerboseFlagAndNoColor_ShowError>b__5_0(Command _) in C:\Projects\Simple.CommandLine\tests\Simple.CommandLine.UnitTests\CommandTests.cs:line 70
   at Simple.CommandLine.Command.Execute() in C:\Projects\Simple.CommandLine\source\Simple.CommandLine\Command.cs:line 60
   at Simple.CommandLine.Command.ExecuteInternally(Span`1 arguments) in C:\Projects\Simple.CommandLine\source\Simple.CommandLine\Command.cs:line 169
".Replace("\r", ""));
    }

    [Fact]
    public void Command_Execute_WithException_AndVerboseFlag_ShowError() {
        var writer = new InMemoryOutputWriter();
        var subject = new Command("command", onExecuting: _ => throw new Exception("Some exception."), writer: writer);
        subject.AddFlag(new DefaultVerboseFlag());

        subject.ExecuteInternally(new[] { "-v" });

        writer.Output.Should().Be(
@"An error occurred while executing command 'command'.
System.Exception: Some exception.
   at Simple.CommandLine.CommandTests.<>c.<Command_Execute_WithException_AndVerboseFlag_ShowError>b__6_0(Command _) in C:\Projects\Simple.CommandLine\tests\Simple.CommandLine.UnitTests\CommandTests.cs:line 88
   at Simple.CommandLine.Command.Execute() in C:\Projects\Simple.CommandLine\source\Simple.CommandLine\Command.cs:line 60
   at Simple.CommandLine.Command.ExecuteInternally(Span`1 arguments) in C:\Projects\Simple.CommandLine\source\Simple.CommandLine\Command.cs:line 169
".Replace("\r", ""));
    }

    [Fact]
    public void Command_Execute_WithRootHelp_ShowsHelp() {
        var writer = new InMemoryOutputWriter();
        var subject = new RootCommand(writer: writer);

        subject.ExecuteInternally(Array.Empty<string>());

        writer.Output.Should().Be(@"
Simple.CommandLine 0.1.0-rc1

This package provides tools for creating a simple CLI (Command-Line Interface) console application.

Usage: testhost

".Replace("\r", ""));
    }

    [Fact]
    public void Command_Execute_WithChildCommand_ShowsHelp() {
        var writer = new InMemoryOutputWriter();
        var subject = new RootCommand(writer: writer);
        subject.AddSubCommand(new("child", onExecuting: c => c.Writer.WriteHelp(c)));

        subject.ExecuteInternally(new [] { "child" });

        writer.Output.Should().Be(@"
Usage: testhost child

".Replace("\r", ""));
    }

    [Fact]
    public void Command_Execute_DefaultRoot_ShowsHelp() {
        var writer = new InMemoryOutputWriter();
        var subject = new DefaultRootCommand(writer);

        subject.ExecuteInternally(new[] { "--version" });

        writer.Output.Should().Be("Simple.CommandLine\n0.1.0-rc1\n");
    }

    [Fact]
    public void Command_Execute_WithHelp_ShowsHelp() {
        var writer = new InMemoryOutputWriter();
        var subject = new Command("command", "Command description.", c => c.Writer.WriteLine("You should not be here!"), writer);
        subject.AddTerminalOption(new DefaultHelpOption());
        subject.AddOption(new Option<string>("options"));
        subject.AddOption(new Option<string>("very-long-name", 'v',"Some description"));
        subject.AddSubCommand(new("child", onExecuting: c => c.Writer.WriteLine("Executing child...")));
        subject.AddParameter(new Parameter<string>("param"));

        subject.ExecuteInternally(new[] { "-h" });

        writer.Output.Should().Be(@"
Usage: command [arguments] [options] [command]

Arguments:
  <param>

Options:
  --help|-h              Show this help information and exit.
  --options <options>
  --very-long-name|-v <very-long-name> Some description

Commands:
  child

Use ""command [command] --help"" for more information about a command.

".Replace("\r", ""));
    }

    [Fact]
    public void Command_AddOption_AddsOption() {
        var subject = new Command("Command", "Command description.");
        subject.AddOption<string>(new("option", 'o', description: "Option description."));

        subject.GetPartDescriptions(nameof(Option)).Should().BeEquivalentTo("  --option|-o <option>   Option description.");
        subject.GetPartDescriptions(nameof(Parameter)).Should().BeEmpty();
        subject.GetPartDescriptions(nameof(Command)).Should().BeEmpty();
    }

    [Fact]
    public void Command_InheritsParentInheritableOptions() {
        var parent = new Command("Command", "Command description.");
        parent.AddOption<string>(new("option", 'o', description: "Option description.", isInheritable: true));
        var subject = new Command("Command", "Command description.");
        parent.AddSubCommand(subject);

        subject.GetPartDescriptions(nameof(Option)).Should().BeEquivalentTo("  --option|-o <option>   Option description.");
        subject.GetPartDescriptions(nameof(Parameter)).Should().BeEmpty();
        subject.GetPartDescriptions(nameof(Command)).Should().BeEmpty();
    }

    [Fact]
    public void Command_AddOption_WithEmptyName_Throws() {
        var subject = new Command("Command", "Command description.");
        var action = () => subject.AddOption<string>(new(""));
        action.Should().Throw<ArgumentException>().WithMessage("Name cannot be null or whitespace. (Parameter 'name')");
    }

    [Fact]
    public void Command_AddOption_WithEmptyAlias_Throws() {
        var subject = new Command("Command", "Command description.");
        var action = () => subject.AddOption<string>(new("option", '!'));
        action.Should().Throw<ArgumentException>().WithMessage("The value '!' is not a valid alias. An alias must be a letter or number. (Parameter 'alias')");
    }

    [Fact]
    public void Command_AddOption_InvalidEmptyName_Throws() {
        var subject = new Command("Command", "Command description.");
        var action = () => subject.AddOption<string>(new("!123"));
        action.Should().Throw<ArgumentException>().WithMessage("The value '!123' is not a valid name. A name must be in the 'kebab lower case' form. Examples: 'name', 'address2' or 'full-name'. (Parameter 'name')");
    }

    [Fact]
    public void Command_AddOption_WithNameUsedByChild_Throws() {
        var subject = new Command("Command", "Command description.");
        subject.AddSubCommand(new("option", description: "Option description."));
        var action = () => subject.AddOption<string>(new("option"));
        action.Should().Throw<ArgumentException>().WithMessage("A sub-command with name 'option' already exists. (Parameter 'option')");
    }

    [Fact]
    public void Command_AddOption_WithNameUsedByParameter_Throws() {
        var subject = new Command("Command", "Command description.");
        subject.AddParameter<string>(new("option", description: "Option description."));
        var action = () => subject.AddOption<string>(new("option"));
        action.Should().Throw<ArgumentException>().WithMessage("A parameter with name 'option' already exists. (Parameter 'option')");
    }

    [Fact]
    public void Command_AddOption_WithDuplicatedName_Throws() {
        var subject = new Command("Command", "Command description.");
        subject.AddOption<string>(new("option", 'o', description: "Option description."));
        var action = () => subject.AddOption<string>(new("option"));
        action.Should().Throw<ArgumentException>().WithMessage("A flag or option with name 'option' already exists. (Parameter 'option')");
    }

    [Fact]
    public void Command_AddOption_WithDuplicatedAlias_Throws() {
        var subject = new Command("Command", "Command description.");
        subject.AddOption<string>(new("option", 'o', description: "Option description."));
        var action = () => subject.AddOption<string>(new("output", 'o'));
        action.Should().Throw<ArgumentException>().WithMessage("A flag or option with alias 'o' already exists. (Parameter 'option')");
    }

    [Fact]
    public void Command_AddMultiOption_AddsOption() {
        var subject = new Command("Command", "Command description.");
        subject.AddMultiOption<string>(new("option", 'o', "Option description."));

        subject.GetPartDescriptions(nameof(Option)).Should().BeEquivalentTo("  --option|-o <option>   Option description.");
        subject.GetPartDescriptions(nameof(Parameter)).Should().BeEmpty();
        subject.GetPartDescriptions(nameof(Command)).Should().BeEmpty();
    }

    [Fact]
    public void Command_AddTerminalOption_AddsOption() {
        var subject = new Command("Command", "Command description.");
        subject.AddTerminalOption(new("option", 'o', "Option description."));

        subject.GetPartDescriptions(nameof(Option)).Should().BeEquivalentTo("  --option|-o            Option description.");
        subject.GetPartDescriptions(nameof(Parameter)).Should().BeEmpty();
        subject.GetPartDescriptions(nameof(Command)).Should().BeEmpty();
    }

    [Fact]
    public void Command_AddFlag_WithFlag_AddsOption() {
        var subject = new Command("Command", "Command description.");
        subject.AddFlag(new("flag", 'f', "Flag description."));

        subject.GetPartDescriptions(nameof(Option)).Should().BeEquivalentTo("  --flag|-f              Flag description.");
        subject.GetPartDescriptions(nameof(Parameter)).Should().BeEmpty();
        subject.GetPartDescriptions(nameof(Command)).Should().BeEmpty();
    }

    [Fact]
    public void Command_AddParameter_AddsParameter() {
        var subject = new Command("Command", "Command description.");
        subject.AddParameter<int>(new("argument", "Parameter description."));

        subject.GetPartDescriptions(nameof(Parameter)).Should().BeEquivalentTo("  <argument>             Parameter description.");
        subject.GetPartDescriptions(nameof(Option)).Should().BeEmpty();
        subject.GetPartDescriptions(nameof(Command)).Should().BeEmpty();
    }

    [Fact]
    public void Command_AddSubCommand_AddsSubCommand() {
        var subject = new Command("Command", "Command description.");
        subject.AddSubCommand(new("sub-command", "Sub command description."));

        subject.GetPartDescriptions(nameof(Command)).Should().BeEquivalentTo("  sub-command            Sub command description.");
        subject.GetPartDescriptions(nameof(Parameter)).Should().BeEmpty();
        subject.GetPartDescriptions(nameof(Option)).Should().BeEmpty();
    }

    [Fact]
    public void Command_GetValueOrDefault_BeforeExecute_ReturnsDefault() {
        var subject = new Command("Command", "Command description.");
        subject.AddOption<string>(new("option"));

        var value = subject.GetValueOrDefault<string>("option");

        value.Should().BeNull();
    }

    [Fact]
    public void Command_GetValueOrDefault_WithEmptyName_Throws() {
        var subject = new Command("Command", "Command description.");

        subject.ExecuteInternally(Array.Empty<string>());

        var action = () => subject.GetValueOrDefault<string>("");

        action.Should().Throw<ArgumentException>().WithMessage("Name cannot be null or whitespace. (Parameter 'name')");
    }

    [Fact]
    public void Command_GetValueOrDefault_ReturnsValue() {
        var subject = new Command("Command", "Command description.");
        subject.AddOption<string>(new("option"));

        subject.ExecuteInternally(new[] { "--option", "some value" });

        var value = subject.GetValueOrDefault<string>("option");

        value.Should().Be("some value");
    }

    [Fact]
    public void Command_GetValueOrDefault_ForParameter_WhenNotSet_ReturnsDefault() {
        var subject = new Command("Command", "Command description.");
        subject.AddParameter<string>(new("option"));

        subject.ExecuteInternally(Array.Empty<string>());

        var value = subject.GetValueOrDefault<string>("option");

        value.Should().BeNull();
    }

    [Fact]
    public void Command_GetValueOrDefault_WithWrongType_Throws() {
        var subject = new Command("Command", "Command description.");
        subject.AddOption<string>(new("option"));

        subject.ExecuteInternally(new[] { "--option", "some value" });

        var action = () => subject.GetValueOrDefault<int>("option");

        action.Should().Throw<InvalidCastException>().WithMessage("Cannot cast value 'option' to 'Int32'.");
    }

    [Fact]
    public void Command_GetValueOrDefault_ForParameter_WithWrongType_Throws() {
        var subject = new Command("Command", "Command description.");
        subject.AddParameter<string>(new("option"));

        subject.ExecuteInternally(new[] { "--option", "some value" });

        var action = () => subject.GetValueOrDefault<int>("option");

        action.Should().Throw<InvalidCastException>().WithMessage("Cannot cast value 'option' to 'Int32'.");
    }

    [Fact]
    public void Command_GetValueOrDefault_WhenSetByAlias_ReturnsValue() {
        var subject = new Command("Command", "Command description.");
        subject.AddOption<string>(new("option", 'o'));

        subject.ExecuteInternally(new[] { "-o", "some value" });

        var value = subject.GetValueOrDefault<string>("option");

        value.Should().Be("some value");
    }

    [Fact]
    public void Command_GetValueOrDefault_WhenNotSet_ReturnsDefault() {
        var subject = new Command("Command", "Command description.");
        subject.AddOption<string>(new("option"));

        subject.ExecuteInternally(Array.Empty<string>());

        var value = subject.GetValueOrDefault<string>("option");

        value.Should().BeNull();
    }

    [Fact]
    public void Command_GetValueOrDefault_WhenNotAdded_ReturnsDefault() {
        var subject = new Command("Command", "Command description.");

        subject.ExecuteInternally(Array.Empty<string>());

        var value = subject.GetValueOrDefault<string>("option");

        value.Should().BeNull();
    }

    [Fact]
    public void Command_GetRequiredValue_BeforeExecute_Throws() {
        var subject = new Command("Command", "Command description.");
        subject.AddOption<string>(new("option"));

        var action = () => subject.GetRequiredValue<string>("option");

        action.Should().Throw<InvalidOperationException>().WithMessage("Missing required value 'option'.");
    }

    [Fact]
    public void Command_GetRequiredValue_WithEmptyName_Throws() {
        var subject = new Command("Command", "Command description.");

        subject.ExecuteInternally(Array.Empty<string>());

        var action = () => subject.GetRequiredValue<string>("");

        action.Should().Throw<ArgumentException>().WithMessage("Name cannot be null or whitespace. (Parameter 'name')");
    }

    [Fact]
    public void Command_GetRequiredValue_ReturnsValue() {
        var subject = new Command("Command", "Command description.");
        subject.AddOption<string>(new("option"));

        subject.ExecuteInternally(new[] { "--option", "some value" });

        var value = subject.GetRequiredValue<string>("option");

        value.Should().Be("some value");
    }

    [Fact]
    public void Command_GetRequiredValue_ForParameter_ReturnsValue() {
        var subject = new Command("Command", "Command description.");
        subject.AddParameter<string>(new("option"));

        subject.ExecuteInternally(new[] { "some value" });

        var value = subject.GetRequiredValue<string>("option");

        value.Should().Be("some value");
    }

    [Fact]
    public void Command_GetRequiredValue_WithWrongType_ReturnsValue() {
        var subject = new Command("Command", "Command description.");
        subject.AddOption<string>(new("option"));

        subject.ExecuteInternally(new[] { "--option", "some value" });

        var action = () => subject.GetRequiredValue<int>("option");

        action.Should().Throw<InvalidCastException>().WithMessage("Cannot cast value 'option' to 'Int32'.");
    }

    [Fact]
    public void Command_GetRequiredValue_ForParameter_WithWrongType_ReturnsValue() {
        var subject = new Command("Command", "Command description.");
        subject.AddParameter<string>(new("option"));

        subject.ExecuteInternally(new[] { "some value" });

        var action = () => subject.GetRequiredValue<int>("option");

        action.Should().Throw<InvalidCastException>().WithMessage("Cannot cast value 'option' to 'Int32'.");
    }

    [Fact]
    public void Command_GetRequiredValue_WhenSetByAlias_ReturnsValue() {
        var subject = new Command("Command", "Command description.");
        subject.AddOption<string>(new("option", 'o'));

        subject.ExecuteInternally(new[] { "-o", "some value" });

        var value = subject.GetRequiredValue<string>("option");

        value.Should().Be("some value");
    }

    [Fact]
    public void Command_GetRequiredValue_WhenNotSet_Throws() {
        var subject = new Command("Command", "Command description.");
        subject.AddOption<string>(new("option"));

        subject.ExecuteInternally(Array.Empty<string>());

        var action = () => subject.GetRequiredValue<string>("option");

        action.Should().Throw<InvalidOperationException>().WithMessage("Missing required value 'option'.");
    }

    [Fact]
    public void Command_GetRequiredValue_ForParameter_WhenNotSet_ReturnsValue() {
        var subject = new Command("Command", "Command description.");
        subject.AddParameter<string>(new("option"));

        subject.ExecuteInternally(Array.Empty<string>());

        var action = () => subject.GetRequiredValue<string>("option");

        action.Should().Throw<InvalidOperationException>().WithMessage("Missing required value 'option'.");
    }

    [Fact]
    public void Command_GetRequiredValue_WhenNotAdded_Throws() {
        var subject = new Command("Command", "Command description.");

        subject.ExecuteInternally(Array.Empty<string>());

        var action = () => subject.GetRequiredValue<string>("option");

        action.Should().Throw<InvalidOperationException>().WithMessage("Required value 'option' not defined.");
    }

    [Fact]
    public void Command_GetValueOrDefault_ByIndex_BeforeExecute_ReturnsDefault() {
        var subject = new Command("Command", "Command description.");
        subject.AddParameter<string>(new("argument"));

        var value = subject.GetValueOrDefault<string>(0);

        value.Should().BeNull();
    }

    [Fact]
    public void Command_GetValueOrDefault_ByIndex_ReturnsValue() {
        var subject = new Command("Command", "Command description.");
        subject.AddParameter<string>(new("argument"));

        subject.ExecuteInternally(new[] { "some value" });

        var value = subject.GetValueOrDefault<string>(0);

        value.Should().Be("some value");
    }

    [Fact]
    public void Command_GetValueOrDefault_ByIndex_WithIndexOutOfRange_ReturnsDefault() {
        var subject = new Command("Command", "Command description.");

        subject.ExecuteInternally(Array.Empty<string>());

        var value = subject.GetValueOrDefault<string>(0);

        value.Should().BeNull();
    }

    [Fact]
    public void Command_GetValueOrDefault_ByIndex_WhenNotSet_ReturnsDefault() {
        var subject = new Command("Command", "Command description.");
        subject.AddParameter<string>(new("argument"));

        subject.ExecuteInternally(Array.Empty<string>());

        var value = subject.GetValueOrDefault<string>(0);

        value.Should().BeNull();
    }

    [Fact]
    public void Command_GetValueOrDefault_ByIndex_WithWrongType_Throws() {
        var subject = new Command("Command", "Command description.");
        subject.AddParameter<string>(new("argument"));

        subject.ExecuteInternally(new[] { "some value" });

        var action = () => subject.GetValueOrDefault<int>(0);

        action.Should().Throw<InvalidCastException>().WithMessage("Cannot cast parameter at index 0 to 'Int32'.");
    }

    [Fact]
    public void Command_GetFlagOrDefault_BeforeExecute_ReturnsDefault() {
        var subject = new Command("Command", "Command description.");
        subject.AddParameter<string>(new("argument"));

        var value = subject.GetFlagOrDefault("flag");

        value.Should().BeFalse();
    }

    [Fact]
    public void Command_GetRequiredValue_ByIndex_BeforeExecute_Throws() {
        var subject = new Command("Command", "Command description.");
        subject.AddParameter<string>(new("option"));

        var action = () => subject.GetRequiredValue<string>(0);

        action.Should().Throw<InvalidOperationException>().WithMessage("Missing required parameter at index 0.");
    }

    [Fact]
    public void Command_GetRequiredValue_ByIndex_ReturnsValue() {
        var subject = new Command("Command", "Command description.");
        subject.AddParameter<string>(new("option"));

        subject.ExecuteInternally(new[] { "some value" });

        var value = subject.GetRequiredValue<string>(0);

        value.Should().Be("some value");
    }

    [Fact]
    public void Command_GetRequiredValue_ByIndex_WithWrongType_Throws() {
        var subject = new Command("Command", "Command description.");
        subject.AddParameter<string>(new("option"));

        subject.ExecuteInternally(new[] { "some value" });

        var action = () => subject.GetRequiredValue<int>(0);

        action.Should().Throw<InvalidCastException>().WithMessage("Cannot cast parameter at index 0 to 'Int32'.");
    }

    [Fact]
    public void Command_GetRequiredValue_ByIndex_WhenNotSet_Throws() {
        var subject = new Command("Command", "Command description.");
        subject.AddParameter<string>(new("option"));

        subject.ExecuteInternally(Array.Empty<string>());

        var action = () => subject.GetRequiredValue<string>(0);

        action.Should().Throw<InvalidOperationException>().WithMessage("Missing required parameter at index 0.");
    }

    [Fact]
    public void Command_GetRequiredValue_ByIndex_WhenNotAdded_Throws() {
        var subject = new Command("Command", "Command description.");

        subject.ExecuteInternally(Array.Empty<string>());

        var action = () => subject.GetRequiredValue<string>(0);

        action.Should().Throw<ArgumentOutOfRangeException>().WithMessage("Parameter at index 0 not found. Parameter count is 0. (Parameter 'index')");
    }

    [Fact]
    public void Command_GetFlagOrDefault_WithEmptyName_Throws() {
        var subject = new Command("Command", "Command description.");

        subject.ExecuteInternally(Array.Empty<string>());

        var action = () => subject.GetFlagOrDefault("");

        action.Should().Throw<ArgumentException>().WithMessage("Name cannot be null or whitespace. (Parameter 'name')");
    }

    [Fact]
    public void Command_GetFlagOrDefault_ReturnsValue() {
        var subject = new Command("Command", "Command description.");
        subject.AddFlag(new("flag"));

        subject.ExecuteInternally(new[] { "--flag" });

        var value = subject.GetFlagOrDefault("flag");

        value.Should().BeTrue();
    }

    [Fact]
    public void Command_GetFlagOrDefault_WhenSetByAlias_ReturnsValue() {
        var subject = new Command("Command", "Command description.");
        subject.AddFlag(new("flag", 'f'));

        subject.ExecuteInternally(new[] { "-f" });

        var value = subject.GetFlagOrDefault("flag");

        value.Should().BeTrue();
    }

    [Fact]
    public void Command_GetFlagOrDefault_WhenNotSet_ReturnsDefault() {
        var subject = new Command("Command", "Command description.");
        subject.AddFlag(new("flag"));

        subject.ExecuteInternally(Array.Empty<string>());

        var value = subject.GetFlagOrDefault("flag");

        value.Should().BeFalse();
    }

    [Fact]
    public void Command_GetRequiredFlagOrDefault_WithEmptyName_Throws() {
        var subject = new Command("Command", "Command description.");

        subject.ExecuteInternally(Array.Empty<string>());

        var action = () => subject.GetRequiredFlag("");

        action.Should().Throw<ArgumentException>().WithMessage("Name cannot be null or whitespace. (Parameter 'name')");
    }

    [Fact]
    public void Command_GetRequiredFlagOrDefault_ReturnsValue() {
        var subject = new Command("Command", "Command description.");
        subject.AddFlag(new("flag"));

        subject.ExecuteInternally(new[] { "--flag" });

        var value = subject.GetRequiredFlag("flag");

        value.Should().BeTrue();
    }

    [Fact]
    public void Command_GetRequiredFlagOrDefault_WhenSetByAlias_ReturnsValue() {
        var subject = new Command("Command", "Command description.");
        subject.AddFlag(new("flag", 'f'));

        subject.ExecuteInternally(new[] { "-f" });

        var value = subject.GetRequiredFlag("flag");

        value.Should().BeTrue();
    }

    [Fact]
    public void Command_GetRequiredFlagOrDefault_WhenNotSet_Throws() {
        var subject = new Command("Command", "Command description.");
        subject.AddFlag(new("flag"));

        subject.ExecuteInternally(Array.Empty<string>());

        var action = () => subject.GetRequiredFlag("flag");

        action.Should().Throw<InvalidOperationException>().WithMessage("Missing required flag 'flag'.");
    }

    [Fact]
    public void Command_GetRequiredFlagOrDefault_WhenNotAdd_Throws() {
        var subject = new Command("Command", "Command description.");

        subject.ExecuteInternally(Array.Empty<string>());

        var action = () => subject.GetRequiredFlag("flag");

        action.Should().Throw<InvalidOperationException>().WithMessage("Required flag 'flag' not defined.");
    }

    [Fact]
    public void Command_GetCollectionOrDefault_WithEmptyName_Throws() {
        var subject = new Command("Command", "Command description.");

        subject.ExecuteInternally(Array.Empty<string>());

        var action = () => subject.GetCollectionOrDefault<string>("");

        action.Should().Throw<ArgumentException>().WithMessage("Name cannot be null or whitespace. (Parameter 'name')");
    }

    [Fact]
    public void Command_GetCollectionOrDefault_ReturnsValue() {
        var subject = new Command("Command", "Command description.");
        subject.AddMultiOption<string>(new("option", 'o'));

        subject.ExecuteInternally(new[] { "--option", "value1", "-o", "value2" });

        var value = subject.GetCollectionOrDefault<string>("option");

        value.Should().BeEquivalentTo("value1", "value2");
    }

    [Fact]
    public void Command_GetCollectionOrDefault_WhenNotSet_ReturnsEmpty() {
        var subject = new Command("Command", "Command description.");
        subject.AddMultiOption<string>(new("option"));

        subject.ExecuteInternally(Array.Empty<string>());

        var value = subject.GetCollectionOrDefault<string>("option");

        value.Should().BeEmpty();
    }

    [Fact]
    public void Command_GetCollectionOrDefault_WithWrongType_Throws() {
        var subject = new Command("Command", "Command description.");
        subject.AddMultiOption<string>(new("option"));

        subject.ExecuteInternally(new[] { "--option", "value1", "-o", "value2" });

        var action = () => subject.GetCollectionOrDefault<int>("option");

        action.Should().Throw<InvalidCastException>().WithMessage("Cannot cast value 'option' to a collection of 'Int32'.");
    }

    [Fact]
    public void Command_GetCollectionOrDefault_WhenNotAdded_ReturnsEmpty() {
        var subject = new Command("Command", "Command description.");

        subject.ExecuteInternally(Array.Empty<string>());

        var value = subject.GetCollectionOrDefault<string>("option");

        value.Should().BeEmpty();
    }

    [Fact]
    public void Command_GetRequiredCollection_WithEmptyName_Throws() {
        var subject = new Command("Command", "Command description.");

        subject.ExecuteInternally(Array.Empty<string>());

        var action = () => subject.GetRequiredCollection<string>("");

        action.Should().Throw<ArgumentException>().WithMessage("Name cannot be null or whitespace. (Parameter 'name')");
    }

    [Fact]
    public void Command_GetRequiredCollection_ReturnsValue() {
        var subject = new Command("Command", "Command description.");
        subject.AddMultiOption<string>(new("option", 'o'));

        subject.ExecuteInternally(new[] { "--option", "value1", "-o", "value2" });

        var value = subject.GetRequiredCollection<string>("option");

        value.Should().BeEquivalentTo("value1", "value2");
    }

    [Fact]
    public void Command_GetRequiredCollection_WithWrongType_Throws() {
        var subject = new Command("Command", "Command description.");
        subject.AddMultiOption<string>(new("option"));

        subject.ExecuteInternally(new[] { "--option", "value1", "-o", "value2" });

        var action = () => subject.GetRequiredCollection<int>("option");

        action.Should().Throw<InvalidCastException>().WithMessage("Cannot cast value 'option' to a collection of 'Int32'.");
    }

    [Fact]
    public void Command_GetRequiredCollection_WhenNotSet_ReturnsEmpty() {
        var subject = new Command("Command", "Command description.");
        subject.AddMultiOption<string>(new("option"));

        subject.ExecuteInternally(Array.Empty<string>());

        var value = subject.GetRequiredCollection<string>("option");

        value.Should().BeEmpty();
    }

    [Fact]
    public void Command_GetRequiredCollection_WhenNotAdd_Throws() {
        var subject = new Command("Command", "Command description.");

        subject.ExecuteInternally(Array.Empty<string>());

        var action = () => subject.GetRequiredCollection<string>("option");

        action.Should().Throw<InvalidOperationException>().WithMessage("Required option 'option' not defined.");
    }
}
