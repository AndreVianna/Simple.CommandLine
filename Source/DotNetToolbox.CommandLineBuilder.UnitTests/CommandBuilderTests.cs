namespace DotNetToolbox.CommandLineBuilder;

public class CommandBuilderTests {
    [Fact]
    public void CommandBuilder_FromRoot_CreatesEmptyRootCommand() {
        var subject = CommandBuilder.FromRoot().Build();

        _ = subject.Should().BeOfType<RootCommand>();
        _ = subject.Name.Should().Be("testhost");
        _ = subject.Path.Should().Be("testhost");
        _ = subject.ToString().Should().Be("Command 'testhost'");
        _ = subject.Tokens.Should().HaveCount(4);
    }

    [Fact]
    public void CommandBuilder_OnExecute_WithNull_Throws() {
        var action = () => CommandBuilder.FromRoot().OnExecute(null!);

        _ = action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CommandBuilder_OnBeforeExecuteChild_WithNull_Throws() {
        var action = () => CommandBuilder.FromRoot().OnBeforeSubCommand(null!);

        _ = action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CommandBuilder_OnAfterExecuteChild_WithNull_Throws() {
        var action = () => CommandBuilder.FromRoot().OnAfterSubCommand(null!);

        _ = action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CommandBuilder_AddOption_CreatesCommandWithOption() {
        var subject = CommandBuilder.FromRoot().AddOption<int>("option").Build();

        _ = subject.Tokens.Should().HaveCount(5);
        _ = subject.Tokens[4].Should().BeOfType<Option<int>>().Subject.Name.Should().Be("option");
    }

    [Fact]
    public void CommandBuilder_AddOption_WithDescription_CreatesCommandWithOption() {
        var subject = CommandBuilder.FromRoot().AddOption<int>("option", "Some option.").Build();

        _ = subject.Tokens.Should().HaveCount(5);
        _ = subject.Tokens[4].Should().BeOfType<Option<int>>().Subject.Name.Should().Be("option");
    }

    [Fact]
    public void CommandBuilder_AddOption_WithDescription_WithAlias_CreatesCommandWithOption() {
        var subject = CommandBuilder.FromRoot().AddOption<int>("option", 'o').Build();

        _ = subject.Tokens.Should().HaveCount(5);
        _ = subject.Tokens[4].Should().BeOfType<Option<int>>().Subject.Name.Should().Be("option");
    }

    [Fact]
    public void CommandBuilder_Add_WithToken_CreatesCommandWithToken() {
        var option = new Option<int>("option");
        var subject = CommandBuilder.FromRoot().Add(option).Build();

        _ = subject.Tokens.Should().HaveCount(5);
        _ = subject.Tokens[4].Should().BeOfType<Option<int>>().Subject.Name.Should().Be("option");
    }

    [Fact]
    public void CommandBuilder_AddOptions_CreatesCommandWithOption() {
        var subject = CommandBuilder.FromRoot().AddOptions<int>("option").Build();

        _ = subject.Tokens.Should().HaveCount(5);
        _ = subject.Tokens[4].Should().BeOfType<Options<int>>().Subject.Name.Should().Be("option");
    }

    [Fact]
    public void CommandBuilder_AddOptions_WithDescription_CreatesCommandWithOption() {
        var subject = CommandBuilder.FromRoot().AddOptions<int>("option", "Some option.").Build();

        _ = subject.Tokens.Should().HaveCount(5);
        _ = subject.Tokens[4].Should().BeOfType<Options<int>>().Subject.Name.Should().Be("option");
    }

    [Fact]
    public void CommandBuilder_AddOptions_WithDescription_WithAlias_CreatesCommandWithOption() {
        var subject = CommandBuilder.FromRoot().AddOptions<int>("option", 'o').Build();

        _ = subject.Tokens.Should().HaveCount(5);
        _ = subject.Tokens[4].Should().BeOfType<Options<int>>().Subject.Name.Should().Be("option");
    }

    [Fact]
    public void CommandBuilder_AddFlag_CreatesCommandWithFlag() {
        var subject = CommandBuilder.FromRoot().AddFlag("flag").Build();

        _ = subject.Tokens.Should().HaveCount(5);
        _ = subject.Tokens[4].Should().BeOfType<Flag>().Subject.Name.Should().Be("flag");
    }

    [Fact]
    public void CommandBuilder_AddFlag_WithDescription_CreatesCommandWithFlag() {
        var subject = CommandBuilder.FromRoot().AddFlag("flag", "Some flag.", true).Build();

        _ = subject.Tokens.Should().HaveCount(5);
        _ = subject.Tokens[4].Should().BeOfType<Flag>().Subject.Name.Should().Be("flag");
    }

    [Fact]
    public void CommandBuilder_AddFlag_WithAlias_CreatesCommandWithFlag() {
        var subject = CommandBuilder.FromRoot().AddFlag("flag", 'f').Build();

        _ = subject.Tokens.Should().HaveCount(5);
        var flag = subject.Tokens[4].Should().BeOfType<Flag>().Subject;
        _ = flag.Name.Should().Be("flag");
        _ = flag.ValueType.Should().Be(typeof(bool));
        _ = flag.ToString().Should().Be("Flag '--flag' | '-f'");
    }

    [Fact]
    public void CommandBuilder_AddParameter_CreatesCommandWithArgument() {
        var subject = CommandBuilder.FromRoot().AddParameter<int>("parameter").Build();

        _ = subject.Tokens.Should().HaveCount(5);
        _ = subject.Tokens[4].Should().BeOfType<Parameter<int>>().Subject.Name.Should().Be("parameter");
    }

    [Fact]
    public void CommandBuilder_AddParameter_WithDescription_CreatesCommandWithArgument() {
        var subject = CommandBuilder.FromRoot().AddParameter<int>("parameter", "Some argument.").Build();

        _ = subject.Tokens.Should().HaveCount(5);
        _ = subject.Tokens[4].Should().BeOfType<Parameter<int>>().Subject.Name.Should().Be("parameter");
    }

    [Fact]
    public void CommandBuilder_AddCommand_CreatesCommandWithSubCommand() {
        var subject = CommandBuilder.FromRoot().AddSubCommand("sub").Build();

        _ = subject.Tokens.Should().HaveCount(5);
        _ = subject.Tokens[4].Should().BeOfType<SubCommand>().Subject.Name.Should().Be("sub");
    }

    [Fact]
    public void CommandBuilder_AddCommand_WithSetup_CreatesCommandWithSubCommand() {
        var subject = CommandBuilder.FromRoot().AddSubCommand("sub", setup: b => b.AddOption<int>("option")).Build();

        _ = subject.Tokens.Should().HaveCount(5);
        _ = subject.Tokens[4].Should().BeOfType<SubCommand>().Subject.Name.Should().Be("sub");
    }

    [Fact]
    public void CommandBuilder_AddCommand_WithDescription_CreatesCommandWithSubCommand() {
        var subject = CommandBuilder.FromRoot().AddSubCommand("sub", "Some sub-command.").Build();

        _ = subject.Tokens.Should().HaveCount(5);
        _ = subject.Tokens[4].Should().BeOfType<SubCommand>().Subject.Name.Should().Be("sub");
    }

    [Fact]
    public void CommandBuilder_AddCommand_WithDescriptionAndSetup_CreatesCommandWithSubCommand() {
        var subject = CommandBuilder.FromRoot().AddSubCommand("sub", "Some sub-command.", b => b.AddOption<int>("option")).Build();

        _ = subject.Tokens.Should().HaveCount(5);
        _ = subject.Tokens[4].Should().BeOfType<SubCommand>().Subject.Name.Should().Be("sub");
    }

    [Fact]
    public void CommandBuilder_Add_WithCommand_CreatesCommandWithSubCommand() {
        var command = new SubCommand("sub");
        var subject = CommandBuilder.FromRoot().Add(command).Build();

        _ = subject.Tokens.Should().HaveCount(5);
        _ = subject.Tokens[4].Should().BeOfType<SubCommand>().Subject.Name.Should().Be("sub");
    }
}
