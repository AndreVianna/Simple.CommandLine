namespace Simple.CommandLine;

public class CommandConfigurationTests {
    [Fact]
    public void Command_AddOption_AddsOption() {
        var subject = new Command("command");

        subject.AddOption<string>(new("option", 'o', description: "Option description."));

        subject.Options.Should().HaveCount(1).And.Contain(i => i.Name == "option");
        subject.Parameters.Should().BeEmpty();
        subject.Commands.Should().BeEmpty();
    }

    [Fact]
    public void Command_AddOption_WithEmptyName_Throws() {
        var subject = new Command("command");

        var action = () => subject.AddOption<string>(new(""));

        action.Should().Throw<ArgumentException>().WithMessage("Name cannot be null or whitespace. (Parameter 'name')");
    }

    [Fact]
    public void Command_AddOption_WithEmptyAlias_Throws() {
        var subject = new Command("command");

        var action = () => subject.AddOption<string>(new("option", '!'));

        action.Should().Throw<ArgumentException>().WithMessage("The value '!' is not a valid alias. An alias must be a letter or number. (Parameter 'alias')");
    }

    [Fact]
    public void Command_AddOption_InvalidEmptyName_Throws() {
        var subject = new Command("command");

        var action = () => subject.AddOption<string>(new("!123"));

        action.Should().Throw<ArgumentException>().WithMessage("The value '!123' is not a valid name. A name must be in the 'kebab lower case' form. Examples: 'name', 'address2' or 'full-name'. (Parameter 'name')");
    }

    [Fact]
    public void Command_AddOption_WithNameUsedByChild_Throws() {
        var subject = new Command("command");
        subject.AddCommand(new("option", description: "Option description."));

        var action = () => subject.AddOption<string>(new("option"));

        action.Should().Throw<ArgumentException>().WithMessage("A sub-command with name 'option' already exists. (Parameter 'option')");
    }

    [Fact]
    public void Command_AddOption_WithNameUsedByParameter_Throws() {
        var subject = new Command("command");
        subject.AddParameter<string>(new("option", description: "Option description."));

        var action = () => subject.AddOption<string>(new("option"));

        action.Should().Throw<ArgumentException>().WithMessage("A parameter with name 'option' already exists. (Parameter 'option')");
    }

    [Fact]
    public void Command_AddOption_WithDuplicatedName_Throws() {
        var subject = new Command("command");
        subject.AddOption<string>(new("option", 'o', description: "Option description."));

        var action = () => subject.AddOption<string>(new("option"));

        action.Should().Throw<ArgumentException>().WithMessage("A flag or option with name 'option' already exists. (Parameter 'option')");
    }

    [Fact]
    public void Command_AddOption_WithDuplicatedAlias_Throws() {
        var subject = new Command("command");
        subject.AddOption<string>(new("option", 'o', description: "Option description."));

        var action = () => subject.AddOption<string>(new("output", 'o'));

        action.Should().Throw<ArgumentException>().WithMessage("A flag or option with alias 'o' already exists. (Parameter 'option')");
    }

    [Fact]
    public void Command_AddMultiOption_AddsOption() {
        var subject = new Command("command");

        subject.AddMultiOption<string>(new("option", 'o', "Option description."));

        subject.Options.Should().HaveCount(1).And.Contain(i => i.Name == "option");
        subject.Parameters.Should().BeEmpty();
        subject.Commands.Should().BeEmpty();
    }

    [Fact]
    public void Command_AddTerminalOption_AddsOption() {
        var subject = new Command("command");

        subject.AddTerminalOption(new("option", 'o', "Option description."));

        subject.Options.Should().HaveCount(1).And.Contain(i => i.Name == "option");
        subject.Parameters.Should().BeEmpty();
        subject.Commands.Should().BeEmpty();
    }

    [Fact]
    public void Command_AddFlag_WithFlag_AddsOption() {
        var subject = new Command("command");

        subject.AddFlag(new("flag", 'f', "Flag description."));

        subject.Options.Should().HaveCount(1).And.Contain(i => i.Name == "flag");
        subject.Parameters.Should().BeEmpty();
        subject.Commands.Should().BeEmpty();
    }

    [Fact]
    public void Command_AddParameter_AddsParameter() {
        var subject = new Command("command");

        subject.AddParameter<int>(new("parameter", "Parameter description."));

        subject.Options.Should().BeEmpty();
        subject.Parameters.Should().HaveCount(1).And.Contain(i => i.Name == "parameter");
        subject.Commands.Should().BeEmpty();
    }

    [Fact]
    public void Command_AddCommand_AddsSubCommand() {
        var subject = new Command("command");

        subject.AddCommand(new("sub-command", "Sub command description."));

        subject.Options.Should().BeEmpty();
        subject.Parameters.Should().BeEmpty();
        subject.Commands.Should().HaveCount(1).And.Contain(i => i.Name == "sub-command");
    }

    [Fact]
    public void Command_AddCommand_PassesDownOnlyInheritableOptions() {
        var parent = new Command("command");
        parent.AddOption<string>(new("option1", isInheritable: true));
        parent.AddOption<string>(new("option2", isInheritable: false));
        var subject = new Command("sub-command");

        parent.AddCommand(subject);

        parent.Options.Should().HaveCount(2).And.Contain(i => i.Name == "option1").And.Contain(i => i.Name == "option2");
        parent.Parameters.Should().BeEmpty();
        parent.Commands.Should().HaveCount(1).And.Contain(i => i.Name == "sub-command");
        subject.Options.Should().HaveCount(1).And.Contain(i => i.Name == "option1");
        subject.Parameters.Should().BeEmpty();
        subject.Commands.Should().BeEmpty();
    }
}
