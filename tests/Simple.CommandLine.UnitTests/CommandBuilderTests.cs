namespace Simple.CommandLine;

public class CommandBuilderTests {
    [Fact]
    public void CommandBuilder_FromRoot_CreatesEmptyRootCommand() {
        var writer = new InMemoryOutputWriter();
        var subject = CommandBuilder.FromRoot(c => c.Writer.WriteLine("Executing command..."), writer).Build();

        subject.Should().BeOfType<RootCommand>();
        subject.Writer.Should().Be(writer);
        subject.Name.Should().Be("testhost");
        subject.Path.Should().Be("testhost");
        subject.ToString().Should().Be("testhost");
        subject.GetPartDescriptions(nameof(Parameter)).Should().BeEmpty();
        subject.GetPartDescriptions(nameof(Option)).Should().BeEmpty();
        subject.GetPartDescriptions(nameof(Command)).Should().BeEmpty();
        var description = subject.Describe();
        description.Should().Be("  testhost");
    }

    [Fact]
    public void CommandBuilder_AddTerminalOption_CreatesCommandWithOption() {
        var subject = CommandBuilder.FromRoot().AddTerminalOption("option").Build();

        subject.GetPartDescriptions(nameof(Option)).Should().BeEquivalentTo("  --option");
        subject.GetPartDescriptions(nameof(Parameter)).Should().BeEmpty();
        subject.GetPartDescriptions(nameof(Command)).Should().BeEmpty();
    }

    [Fact]
    public void CommandBuilder_AddTerminalOption_WithDescription_CreatesCommandWithOption() {
        var subject = CommandBuilder.FromRoot().AddTerminalOption("option", "Some option.", true).Build();

        subject.GetPartDescriptions(nameof(Option)).Should().BeEquivalentTo("  --option               Some option.");
        subject.GetPartDescriptions(nameof(Parameter)).Should().BeEmpty();
        subject.GetPartDescriptions(nameof(Command)).Should().BeEmpty();
    }

    [Fact]
    public void CommandBuilder_AddTerminalOption_WithDescription_WithAlias_CreatesCommandWithOption() {
        var subject = CommandBuilder.FromRoot().AddTerminalOption("option", 'o', "Some option.").Build();

        subject.GetPartDescriptions(nameof(Option)).Should().BeEquivalentTo("  --option|-o            Some option.");
        subject.GetPartDescriptions(nameof(Parameter)).Should().BeEmpty();
        subject.GetPartDescriptions(nameof(Command)).Should().BeEmpty();
    }

    [Fact]
    public void CommandBuilder_AddTerminalOption_WithOption_CreatesCommandWithOption() {
        var subject = CommandBuilder.FromRoot().AddTerminalOption(new("option")).Build();

        subject.GetPartDescriptions(nameof(Option)).Should().BeEquivalentTo("  --option");
        subject.GetPartDescriptions(nameof(Parameter)).Should().BeEmpty();
        subject.GetPartDescriptions(nameof(Command)).Should().BeEmpty();
    }

    [Fact]
    public void CommandBuilder_AddOption_CreatesCommandWithOption() {
        var subject = CommandBuilder.FromRoot().AddOption<int>("option").Build();

        subject.GetPartDescriptions(nameof(Option)).Should().BeEquivalentTo("  --option <option>");
        subject.GetPartDescriptions(nameof(Parameter)).Should().BeEmpty();
        subject.GetPartDescriptions(nameof(Command)).Should().BeEmpty();
    }

    [Fact]
    public void CommandBuilder_AddOption_WithDescription_CreatesCommandWithOption() {
        var subject = CommandBuilder.FromRoot().AddOption<int>("option", "Some option.", true).Build();

        subject.GetPartDescriptions(nameof(Option)).Should().BeEquivalentTo("  --option <option>      Some option.");
        subject.GetPartDescriptions(nameof(Parameter)).Should().BeEmpty();
        subject.GetPartDescriptions(nameof(Command)).Should().BeEmpty();
    }

    [Fact]
    public void CommandBuilder_AddOption_WithDescription_WithAlias_CreatesCommandWithOption() {
        var subject = CommandBuilder.FromRoot().AddOption<int>("option", 'o', "Some option.").Build();

        subject.GetPartDescriptions(nameof(Option)).Should().BeEquivalentTo("  --option|-o <option>   Some option.");
        subject.GetPartDescriptions(nameof(Parameter)).Should().BeEmpty();
        subject.GetPartDescriptions(nameof(Command)).Should().BeEmpty();
    }

    [Fact]
    public void CommandBuilder_AddOption_WithOption_CreatesCommandWithOption() {
        var subject = CommandBuilder.FromRoot().AddOption(new Option<int>("option")).Build();

        subject.GetPartDescriptions(nameof(Option)).Should().BeEquivalentTo("  --option <option>");
        subject.GetPartDescriptions(nameof(Parameter)).Should().BeEmpty();
        subject.GetPartDescriptions(nameof(Command)).Should().BeEmpty();
    }

    [Fact]
    public void CommandBuilder_AddListOption_CreatesCommandWithOption() {
        var subject = CommandBuilder.FromRoot().AddListOption<int>("option").Build();

        subject.GetPartDescriptions(nameof(Option)).Should().BeEquivalentTo("  --option <option>");
        subject.GetPartDescriptions(nameof(Parameter)).Should().BeEmpty();
        subject.GetPartDescriptions(nameof(Command)).Should().BeEmpty();
    }

    [Fact]
    public void CommandBuilder_AddListOption_WithDescription_CreatesCommandWithOption() {
        var subject = CommandBuilder.FromRoot().AddListOption<int>("option", "Some option.", true).Build();

        subject.GetPartDescriptions(nameof(Option)).Should().BeEquivalentTo("  --option <option>      Some option.");
        subject.GetPartDescriptions(nameof(Parameter)).Should().BeEmpty();
        subject.GetPartDescriptions(nameof(Command)).Should().BeEmpty();
    }

    [Fact]
    public void CommandBuilder_AddListOption_WithDescription_WithAlias_CreatesCommandWithOption() {
        var subject = CommandBuilder.FromRoot().AddListOption<int>("option", 'o', "Some option.").Build();

        subject.GetPartDescriptions(nameof(Option)).Should().BeEquivalentTo("  --option|-o <option>   Some option.");
        subject.GetPartDescriptions(nameof(Parameter)).Should().BeEmpty();
        subject.GetPartDescriptions(nameof(Command)).Should().BeEmpty();
    }

    [Fact]
    public void CommandBuilder_AddListOption_WithOption_CreatesCommandWithOption() {
        var subject = CommandBuilder.FromRoot().AddListOption(new MultiOption<int>("option")).Build();

        subject.GetPartDescriptions(nameof(Option)).Should().BeEquivalentTo("  --option <option>");
        subject.GetPartDescriptions(nameof(Parameter)).Should().BeEmpty();
        subject.GetPartDescriptions(nameof(Command)).Should().BeEmpty();
    }

    [Fact]
    public void CommandBuilder_AddFlag_CreatesCommandWithFlag() {
        var subject = CommandBuilder.FromRoot().AddFlag("flag").Build();

        subject.GetPartDescriptions(nameof(Option)).Should().BeEquivalentTo("  --flag");
        subject.GetPartDescriptions(nameof(Parameter)).Should().BeEmpty();
        subject.GetPartDescriptions(nameof(Command)).Should().BeEmpty();
    }

    [Fact]
    public void CommandBuilder_AddFlag_WithDescription_CreatesCommandWithFlag() {
        var subject = CommandBuilder.FromRoot().AddFlag("flag", "Some flag.", true).Build();

        subject.GetPartDescriptions(nameof(Option)).Should().BeEquivalentTo("  --flag                 Some flag.");
        subject.GetPartDescriptions(nameof(Parameter)).Should().BeEmpty();
        subject.GetPartDescriptions(nameof(Command)).Should().BeEmpty();
    }

    [Fact]
    public void CommandBuilder_AddFlag_WithDescription_WithAlias_CreatesCommandWithFlag() {
        var subject = CommandBuilder.FromRoot().AddFlag("flag", 'f', "Some flag.").Build();

        subject.GetPartDescriptions(nameof(Option)).Should().BeEquivalentTo("  --flag|-f              Some flag.");
        subject.GetPartDescriptions(nameof(Parameter)).Should().BeEmpty();
        subject.GetPartDescriptions(nameof(Command)).Should().BeEmpty();
    }

    [Fact]
    public void CommandBuilder_AddFlag_WithFlag_CreatesCommandWithOption() {
        var subject = CommandBuilder.FromRoot().AddFlag(new("flag")).Build();

        subject.GetPartDescriptions(nameof(Option)).Should().BeEquivalentTo("  --flag");
        subject.GetPartDescriptions(nameof(Parameter)).Should().BeEmpty();
        subject.GetPartDescriptions(nameof(Command)).Should().BeEmpty();
    }

    [Fact]
    public void CommandBuilder_AddArgument_CreatesCommandWithArgument() {
        var subject = CommandBuilder.FromRoot().AddArgument<int>("argument").Build();

        subject.GetPartDescriptions(nameof(Parameter)).Should().BeEquivalentTo("  <argument>");
        subject.GetPartDescriptions(nameof(Option)).Should().BeEmpty();
        subject.GetPartDescriptions(nameof(Command)).Should().BeEmpty();
    }

    [Fact]
    public void CommandBuilder_AddArgument_WithDescription_CreatesCommandWithArgument() {
        var subject = CommandBuilder.FromRoot().AddArgument<int>("argument", "Some argument.").Build();

        subject.GetPartDescriptions(nameof(Parameter)).Should().BeEquivalentTo("  <argument>             Some argument.");
        subject.GetPartDescriptions(nameof(Option)).Should().BeEmpty();
        subject.GetPartDescriptions(nameof(Command)).Should().BeEmpty();
    }

    [Fact]
    public void CommandBuilder_AddArgument_WithArgument_CreatesCommandWithOption() {
        var subject = CommandBuilder.FromRoot().AddArgument(new Parameter<int>("argument")).Build();

        subject.GetPartDescriptions(nameof(Parameter)).Should().BeEquivalentTo("  <argument>");
        subject.GetPartDescriptions(nameof(Option)).Should().BeEmpty();
        subject.GetPartDescriptions(nameof(Command)).Should().BeEmpty();
    }

    [Fact]
    public void CommandBuilder_AddSubCommand_CreatesCommandWithSubCommand() {
        var subject = CommandBuilder.FromRoot().AddSubCommand("sub", _ => { }).Build();

        subject.GetPartDescriptions(nameof(Command)).Should().BeEquivalentTo("  sub");
        subject.GetPartDescriptions(nameof(Parameter)).Should().BeEmpty();
        subject.GetPartDescriptions(nameof(Option)).Should().BeEmpty();
    }

    [Fact]
    public void CommandBuilder_AddSubCommand_WithDescription_CreatesCommandWithSubCommand() {
        var subject = CommandBuilder.FromRoot().AddSubCommand("sub", "Some sub-command.", _ => { }).Build();

        subject.GetPartDescriptions(nameof(Command)).Should().BeEquivalentTo("  sub                    Some sub-command.");
        subject.GetPartDescriptions(nameof(Parameter)).Should().BeEmpty();
        subject.GetPartDescriptions(nameof(Option)).Should().BeEmpty();
    }

    [Fact]
    public void CommandBuilder_AddSubCommand_WithCommand_CreatesCommandWithSubCommand() {
        var subject = CommandBuilder.FromRoot().AddSubCommand(new("sub")).Build();

        subject.GetPartDescriptions(nameof(Command)).Should().BeEquivalentTo("  sub");
        subject.GetPartDescriptions(nameof(Parameter)).Should().BeEmpty();
        subject.GetPartDescriptions(nameof(Option)).Should().BeEmpty();
    }
}
