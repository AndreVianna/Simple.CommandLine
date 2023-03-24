namespace Simple.CommandLine;

public class CommandBuilderTests {
    [Fact]
    public void CommandBuilder_FromRoot_CreatesEmptyRootCommand() {
        var subject = CommandBuilder.FromRoot().Build();

        subject.Should().BeOfType<RootCommand>();
        subject.Name.Should().Be("testhost");
        subject.Path.Should().Be("testhost");
        subject.ToString().Should().Be("Command 'testhost'");
        subject.Tokens.Should().HaveCount(4);
    }

    [Fact]
    public void CommandBuilder_OnExecute_WithNull_Throws() {
        var action = () => CommandBuilder.FromRoot().OnExecute(null!);

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CommandBuilder_OnBeforeExecuteChild_WithNull_Throws() {
        var action = () => CommandBuilder.FromRoot().OnBeforeSubCommand(null!);

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CommandBuilder_OnAfterExecuteChild_WithNull_Throws()
    {
        var action = () => CommandBuilder.FromRoot().OnAfterSubCommand(null!);

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CommandBuilder_AddOption_CreatesCommandWithOption() {
        var subject = CommandBuilder.FromRoot().AddOption<int>("option").Build();

        subject.Tokens.Should().HaveCount(5);
        subject.Tokens[4].Should().BeOfType<Option<int>>().Subject.Name.Should().Be("option");
    }

    [Fact]
    public void CommandBuilder_AddOption_WithDescription_CreatesCommandWithOption() {
        var subject = CommandBuilder.FromRoot().AddOption<int>("option", "Some option.").Build();

        subject.Tokens.Should().HaveCount(5);
        subject.Tokens[4].Should().BeOfType<Option<int>>().Subject.Name.Should().Be("option");
    }

    [Fact]
    public void CommandBuilder_AddOption_WithDescription_WithAlias_CreatesCommandWithOption() {
        var subject = CommandBuilder.FromRoot().AddOption<int>("option", 'o').Build();

        subject.Tokens.Should().HaveCount(5);
        subject.Tokens[4].Should().BeOfType<Option<int>>().Subject.Name.Should().Be("option");
    }

    [Fact]
    public void CommandBuilder_Add_WithToken_CreatesCommandWithToken() {
        var option = new Option<int>("option");
        var subject = CommandBuilder.FromRoot().Add(option).Build();

        subject.Tokens.Should().HaveCount(5);
        subject.Tokens[4].Should().BeOfType<Option<int>>().Subject.Name.Should().Be("option");
    }

    [Fact]
    public void CommandBuilder_AddOptions_CreatesCommandWithOption() {
        var subject = CommandBuilder.FromRoot().AddOptions<int>("option").Build();

        subject.Tokens.Should().HaveCount(5);
        subject.Tokens[4].Should().BeOfType<Options<int>>().Subject.Name.Should().Be("option");
    }

    [Fact]
    public void CommandBuilder_AddOptions_WithDescription_CreatesCommandWithOption() {
        var subject = CommandBuilder.FromRoot().AddOptions<int>("option", "Some option.").Build();

        subject.Tokens.Should().HaveCount(5);
        subject.Tokens[4].Should().BeOfType<Options<int>>().Subject.Name.Should().Be("option");
    }

    [Fact]
    public void CommandBuilder_AddOptions_WithDescription_WithAlias_CreatesCommandWithOption() {
        var subject = CommandBuilder.FromRoot().AddOptions<int>("option", 'o').Build();

        subject.Tokens.Should().HaveCount(5);
        subject.Tokens[4].Should().BeOfType<Options<int>>().Subject.Name.Should().Be("option");
    }

    [Fact]
    public void CommandBuilder_AddFlag_CreatesCommandWithFlag() {
        var subject = CommandBuilder.FromRoot().AddFlag("flag").Build();

        subject.Tokens.Should().HaveCount(5);
        subject.Tokens[4].Should().BeOfType<Flag>().Subject.Name.Should().Be("flag");
    }

    [Fact]
    public void CommandBuilder_AddFlag_WithDescription_CreatesCommandWithFlag() {
        var subject = CommandBuilder.FromRoot().AddFlag("flag", "Some flag.", true).Build();

        subject.Tokens.Should().HaveCount(5);
        subject.Tokens[4].Should().BeOfType<Flag>().Subject.Name.Should().Be("flag");
    }

    [Fact]
    public void CommandBuilder_AddFlag_WithAlias_CreatesCommandWithFlag() {
        var subject = CommandBuilder.FromRoot().AddFlag("flag", 'f').Build();

        subject.Tokens.Should().HaveCount(5);
        var flag = subject.Tokens[4].Should().BeOfType<Flag>().Subject;
        flag.Name.Should().Be("flag");
        flag.ValueType.Should().Be(typeof(bool));
        flag.ToString().Should().Be("Flag '--flag' | '-f'");
    }

    [Fact]
    public void CommandBuilder_AddParameter_CreatesCommandWithArgument() {
        var subject = CommandBuilder.FromRoot().AddParameter<int>("parameter").Build();

        subject.Tokens.Should().HaveCount(5);
        subject.Tokens[4].Should().BeOfType<Parameter<int>>().Subject.Name.Should().Be("parameter");
    }

    [Fact]
    public void CommandBuilder_AddParameter_WithDescription_CreatesCommandWithArgument() {
        var subject = CommandBuilder.FromRoot().AddParameter<int>("parameter", "Some argument.").Build();

        subject.Tokens.Should().HaveCount(5);
        subject.Tokens[4].Should().BeOfType<Parameter<int>>().Subject.Name.Should().Be("parameter");
    }

    [Fact]
    public void CommandBuilder_AddCommand_CreatesCommandWithSubCommand() {
        var subject = CommandBuilder.FromRoot().AddSubCommand("sub").Build();

        subject.Tokens.Should().HaveCount(5);
        subject.Tokens[4].Should().BeOfType<SubCommand>().Subject.Name.Should().Be("sub");
    }

    [Fact]
    public void CommandBuilder_AddCommand_WithSetup_CreatesCommandWithSubCommand() {
        var subject = CommandBuilder.FromRoot().AddSubCommand("sub", setup: b => b.AddOption<int>("option")).Build();

        subject.Tokens.Should().HaveCount(5);
        subject.Tokens[4].Should().BeOfType<SubCommand>().Subject.Name.Should().Be("sub");
    }

    [Fact]
    public void CommandBuilder_AddCommand_WithDescription_CreatesCommandWithSubCommand() {
        var subject = CommandBuilder.FromRoot().AddSubCommand("sub", "Some sub-command.").Build();

        subject.Tokens.Should().HaveCount(5);
        subject.Tokens[4].Should().BeOfType<SubCommand>().Subject.Name.Should().Be("sub");
    }

    [Fact]
    public void CommandBuilder_AddCommand_WithDescriptionAndSetup_CreatesCommandWithSubCommand() {
        var subject = CommandBuilder.FromRoot().AddSubCommand("sub", "Some sub-command.", b => b.AddOption<int>("option")).Build();

        subject.Tokens.Should().HaveCount(5);
        subject.Tokens[4].Should().BeOfType<SubCommand>().Subject.Name.Should().Be("sub");
    }

    [Fact]
    public void CommandBuilder_Add_WithCommand_CreatesCommandWithSubCommand() {
        var command = new SubCommand("sub");
        var subject = CommandBuilder.FromRoot().Add(command).Build();

        subject.Tokens.Should().HaveCount(5);
        subject.Tokens[4].Should().BeOfType<SubCommand>().Subject.Name.Should().Be("sub");
    }
}
