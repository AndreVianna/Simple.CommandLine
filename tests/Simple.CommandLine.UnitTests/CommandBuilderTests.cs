namespace Simple.CommandLine;

public class CommandBuilderTests {
    [Fact]
    public void CommandBuilder_FromRoot_CreatesEmptyRootCommand() {
        var subject = CommandBuilder.FromRoot().Build();

        subject.Should().BeOfType<RootCommand>();
        subject.Name.Should().Be("testhost");
        subject.Path.Should().Be("testhost");
        subject.ToString().Should().Be("testhost");
        subject.Parameters.Should().BeEmpty();
        subject.Options.Should().BeEmpty();
        subject.Commands.Should().BeEmpty();
    }

    [Fact]
    public void CommandBuilder_FromDefaultRoot_CreatesDefaultRootCommand() {
        var subject = CommandBuilder.FromDefaultRoot().Build();

        subject.Should().BeOfType<DefaultRootCommand>();
        subject.Name.Should().Be("testhost");
        subject.Path.Should().Be("testhost");
        subject.ToString().Should().Be("testhost");
        subject.Parameters.Should().BeEmpty();
        subject.Options.Should().HaveCount(4);
        subject.Commands.Should().BeEmpty();
    }

    [Fact]
    public void CommandBuilder_WithWriter_WithNull_Throws() {
        var action = () => CommandBuilder.FromRoot().WithWriter(null!);

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CommandBuilder_OnExecute_WithNull_Throws() {
        var action = () => CommandBuilder.FromRoot().OnExecute(null!);

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CommandBuilder_OnBeforeExecuteChild_WithNull_Throws() {
        var action = () => CommandBuilder.FromRoot().OnBeforeExecuteChild(null!);

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CommandBuilder_AddTerminalOption_CreatesCommandWithOption() {
        var subject = CommandBuilder.FromRoot().AddTerminalOption("option").Build();

        subject.Options.Should().HaveCount(1).And.ContainEquivalentOf(new TerminalOption("option"));
        subject.Parameters.Should().BeEmpty();
        subject.Commands.Should().BeEmpty();
    }

    [Fact]
    public void CommandBuilder_AddTerminalOption_WithDescription_CreatesCommandWithOption() {
        var subject = CommandBuilder.FromRoot().AddTerminalOption("option", "Some option.", true, _ => {}).Build();

        subject.Options.Should().HaveCount(1).And.ContainEquivalentOf(new TerminalOption("option", "Some option.", true, _ => { }));
        subject.Parameters.Should().BeEmpty();
        subject.Commands.Should().BeEmpty();
    }

    [Fact]
    public void CommandBuilder_AddTerminalOption_WithAlias_CreatesCommandWithOption() {
        var subject = CommandBuilder.FromRoot().AddTerminalOption("option", 'o').Build();

        subject.Options.Should().HaveCount(1).And.ContainEquivalentOf(new TerminalOption("option"));
        subject.Parameters.Should().BeEmpty();
        subject.Commands.Should().BeEmpty();
    }

    [Fact]
    public void CommandBuilder_AddTerminalOption_WithOption_CreatesCommandWithOption() {
        var option = new TerminalOption("option");
        var subject = CommandBuilder.FromRoot().AddTerminalOption(option).Build();

        subject.Options.Should().HaveCount(1).And.Contain(option);
        subject.Parameters.Should().BeEmpty();
        subject.Commands.Should().BeEmpty();
    }

    [Fact]
    public void CommandBuilder_AddOption_CreatesCommandWithOption() {
        var subject = CommandBuilder.FromRoot().AddOption<int>("option").Build();

        subject.Options.Should().HaveCount(1).And.ContainEquivalentOf(new Option<int>("option"));
        subject.Parameters.Should().BeEmpty();
        subject.Commands.Should().BeEmpty();
    }

    [Fact]
    public void CommandBuilder_AddOption_WithDescription_CreatesCommandWithOption() {
        var subject = CommandBuilder.FromRoot().AddOption<int>("option", "Some option.", true, _ => { }).Build();

        subject.Options.Should().HaveCount(1).And.ContainEquivalentOf(new Option<int>("option", "Some option.", true, _ => { }));
        subject.Parameters.Should().BeEmpty();
        subject.Commands.Should().BeEmpty();
    }

    [Fact]
    public void CommandBuilder_AddOption_WithDescription_WithAlias_CreatesCommandWithOption() {
        var subject = CommandBuilder.FromRoot().AddOption<int>("option", 'o').Build();

        subject.Options.Should().HaveCount(1).And.ContainEquivalentOf(new Option<int>("option", 'o'));
        subject.Parameters.Should().BeEmpty();
        subject.Commands.Should().BeEmpty();
    }

    [Fact]
    public void CommandBuilder_AddOption_WithOption_CreatesCommandWithOption() {
        var option = new Option<int>("option");
        var subject = CommandBuilder.FromRoot().AddOption(option).Build();

        subject.Options.Should().HaveCount(1).And.Contain(option);
        subject.Parameters.Should().BeEmpty();
        subject.Commands.Should().BeEmpty();
    }

    [Fact]
    public void CommandBuilder_AddMultiOption_CreatesCommandWithOption() {
        var subject = CommandBuilder.FromRoot().AddMultiOption<int>("option").Build();

        subject.Options.Should().HaveCount(1).And.ContainEquivalentOf(new MultiOption<int>("option"));
        subject.Parameters.Should().BeEmpty();
        subject.Commands.Should().BeEmpty();
    }

    [Fact]
    public void CommandBuilder_AddMultiOption_WithDescription_CreatesCommandWithOption() {
        var subject = CommandBuilder.FromRoot().AddMultiOption<int>("option", "Some option.", true, _ => { }).Build();

        subject.Options.Should().HaveCount(1).And.ContainEquivalentOf(new MultiOption<int>("option", "Some option.", true, _ => { }));
        subject.Parameters.Should().BeEmpty();
        subject.Commands.Should().BeEmpty();
    }

    [Fact]
    public void CommandBuilder_AddMultiOption_WithDescription_WithAlias_CreatesCommandWithOption() {
        var subject = CommandBuilder.FromRoot().AddMultiOption<int>("option", 'o').Build();

        subject.Options.Should().HaveCount(1).And.ContainEquivalentOf(new MultiOption<int>("option", 'o'));
        subject.Parameters.Should().BeEmpty();
        subject.Commands.Should().BeEmpty();
    }

    [Fact]
    public void CommandBuilder_AddMultiOption_WithOption_CreatesCommandWithOption() {
        var option = new MultiOption<int>("option");
        var subject = CommandBuilder.FromRoot().AddMultiOption(option).Build();

        subject.Options.Should().HaveCount(1).And.Contain(option);
        subject.Parameters.Should().BeEmpty();
        subject.Commands.Should().BeEmpty();
    }

    [Fact]
    public void CommandBuilder_AddFlag_CreatesCommandWithFlag() {
        var subject = CommandBuilder.FromRoot().AddFlag("flag").Build();

        subject.Options.Should().HaveCount(1).And.ContainEquivalentOf(new Flag("flag"));
        subject.Parameters.Should().BeEmpty();
        subject.Commands.Should().BeEmpty();
    }

    [Fact]
    public void CommandBuilder_AddFlag_WithDescription_CreatesCommandWithFlag() {
        var subject = CommandBuilder.FromRoot().AddFlag("flag", "Some flag.", true, _=> {}).Build();

        subject.Options.Should().HaveCount(1).And.ContainEquivalentOf(new Flag("flag", "Some flag.", true, _ => { }));
        subject.Parameters.Should().BeEmpty();
        subject.Commands.Should().BeEmpty();
    }

    [Fact]
    public void CommandBuilder_AddFlag_WithDescription_WithAlias_CreatesCommandWithFlag() {
        var subject = CommandBuilder.FromRoot().AddFlag("flag", 'f').Build();

        subject.Options.Should().HaveCount(1).And.ContainEquivalentOf(new Flag("flag", 'f'));
        subject.Parameters.Should().BeEmpty();
        subject.Commands.Should().BeEmpty();
    }

    [Fact]
    public void CommandBuilder_AddFlag_WithFlag_CreatesCommandWithOption() {
        var flag = new Flag("flag");
        var subject = CommandBuilder.FromRoot().AddFlag(flag).Build();

        subject.Options.Should().HaveCount(1).And.Contain(flag);
        subject.Parameters.Should().BeEmpty();
        subject.Commands.Should().BeEmpty();
    }

    [Fact]
    public void CommandBuilder_AddArgument_CreatesCommandWithArgument() {
        var subject = CommandBuilder.FromRoot().AddParameter<int>("parameter").Build();

        subject.Options.Should().BeEmpty();
        subject.Parameters.Should().HaveCount(1).And.ContainEquivalentOf(new Parameter<int>("parameter"));
        subject.Commands.Should().BeEmpty();
    }

    [Fact]
    public void CommandBuilder_AddArgument_WithDescription_CreatesCommandWithArgument() {
        var subject = CommandBuilder.FromRoot().AddParameter<int>("parameter", "Some argument.", _ => { }).Build();

        subject.Options.Should().BeEmpty();
        subject.Parameters.Should().HaveCount(1).And.ContainEquivalentOf(new Parameter<int>("parameter", "Some argument.", _ => { }));
        subject.Commands.Should().BeEmpty();
    }

    [Fact]
    public void CommandBuilder_AddArgument_WithArgument_CreatesCommandWithOption() {
        var parameter = new Parameter<int>("parameter");
        var subject = CommandBuilder.FromRoot().AddParameter(parameter).Build();

        subject.Options.Should().BeEmpty();
        subject.Parameters.Should().HaveCount(1).And.Contain(parameter);
        subject.Commands.Should().BeEmpty();
    }

    [Fact]
    public void CommandBuilder_AddCommand_CreatesCommandWithSubCommand() {
        var subject = CommandBuilder.FromRoot().AddCommand("sub").Build();

        subject.Options.Should().BeEmpty();
        subject.Parameters.Should().BeEmpty();
        subject.Commands.Should().HaveCount(1).And.ContainEquivalentOf(new Command("sub"));
    }

    [Fact]
    public void CommandBuilder_AddCommand_WithSetup_CreatesCommandWithSubCommand() {
        var expectedCommand = new Command("sub");
        expectedCommand.AddOption<int>(new("option"));

        var subject = CommandBuilder.FromRoot().AddCommand("sub", setup: b => b.AddOption<int>("option")).Build();

        subject.Options.Should().BeEmpty();
        subject.Parameters.Should().BeEmpty();
        subject.Commands.Should().HaveCount(1).And.ContainEquivalentOf(expectedCommand);
    }

    [Fact]
    public void CommandBuilder_AddCommand_WithDescription_CreatesCommandWithSubCommand() {
        var expectedCommand = new Command("sub", "Some sub-command.");

        var subject = CommandBuilder.FromRoot().AddCommand("sub", "Some sub-command.").Build();

        subject.Options.Should().BeEmpty();
        subject.Parameters.Should().BeEmpty();
        subject.Commands.Should().HaveCount(1).And.ContainEquivalentOf(expectedCommand);
    }

    [Fact]
    public void CommandBuilder_AddCommand_WithDescriptionAndSetup_CreatesCommandWithSubCommand() {
        var expectedCommand = new Command("sub", "Some sub-command.");
        expectedCommand.AddOption<int>(new("option"));

        var subject = CommandBuilder.FromRoot().AddCommand("sub", "Some sub-command.", b => b.AddOption<int>("option")).Build();

        subject.Options.Should().BeEmpty();
        subject.Parameters.Should().BeEmpty();
        subject.Commands.Should().HaveCount(1).And.ContainEquivalentOf(expectedCommand);
    }

    [Fact]
    public void CommandBuilder_AddCommand_WithCommand_CreatesCommandWithSubCommand() {
        var command = new Command("sub");
        var subject = CommandBuilder.FromRoot().AddCommand(command).Build();

        subject.Options.Should().BeEmpty();
        subject.Parameters.Should().BeEmpty();
        subject.Commands.Should().HaveCount(1).And.Contain(command);
    }
}
