namespace Simple.CommandLine;

public class CommandGetDataTests {
    private readonly InMemoryOutputWriter _writer = new();
    private readonly Command _testCommand;

    public CommandGetDataTests() {
        _testCommand = new("test") {
            Writer = _writer
        };
    }

    [Fact]
    public void Command_GetValueOrDefault_BeforeExecute_ReturnsDefault() {
        _testCommand.AddOption<string>(new("option"));

        var value = _testCommand.GetValueOrDefault<string>("option");

        value.Should().BeNull();
    }

    [Fact]
    public void Command_GetValueOrDefault_WithEmptyName_Throws() {
        _testCommand.Execute();

        var action = () => _testCommand.GetValueOrDefault<string>("");

        action.Should().Throw<ArgumentException>().WithMessage("Name cannot be null or whitespace. (Parameter 'name')");
    }

    [Fact]
    public void Command_GetValueOrDefault_ReturnsValue() {
        _testCommand.AddOption<string>(new("option"));
        _testCommand.Execute(new[] { "--option", "some value" });

        var value = _testCommand.GetValueOrDefault<string>("option");

        value.Should().Be("some value");
    }

    [Fact]
    public void Command_GetValueOrDefault_ForParameter_WhenNotSet_ReturnsDefault() {
        _testCommand.AddParameter<string>(new("option"));
        _testCommand.Execute();

        var value = _testCommand.GetValueOrDefault<string>("option");

        value.Should().BeNull();
    }

    [Fact]
    public void Command_GetValueOrDefault_WithWrongType_Throws() {
        _testCommand.AddOption<string>(new("option"));
        _testCommand.Execute(new[] { "--option", "some value" });

        var action = () => _testCommand.GetValueOrDefault<int>("option");

        action.Should().Throw<InvalidCastException>().WithMessage("Cannot cast value 'option' to 'Int32'.");
    }

    [Fact]
    public void Command_GetValueOrDefault_ForParameter_WithWrongType_Throws() {
        _testCommand.AddParameter<string>(new("option"));
        _testCommand.Execute(new[] { "--option", "some value" });

        var action = () => _testCommand.GetValueOrDefault<int>("option");

        action.Should().Throw<InvalidCastException>().WithMessage("Cannot cast value 'option' to 'Int32'.");
    }

    [Fact]
    public void Command_GetValueOrDefault_WhenSetByAlias_ReturnsValue() {
        _testCommand.AddOption<string>(new("option", 'o'));
        _testCommand.Execute(new[] { "-o", "some value" });

        var value = _testCommand.GetValueOrDefault<string>("option");

        value.Should().Be("some value");
    }

    [Fact]
    public void Command_GetValueOrDefault_WhenNotSet_ReturnsDefault() {
        _testCommand.AddOption<string>(new("option"));
        _testCommand.Execute();

        var value = _testCommand.GetValueOrDefault<string>("option");

        value.Should().BeNull();
    }

    [Fact]
    public void Command_GetValueOrDefault_WhenNotAdded_ReturnsDefault() {
        _testCommand.Execute();

        var value = _testCommand.GetValueOrDefault<string>("option");

        value.Should().BeNull();
    }

    [Fact]
    public void Command_GetRequiredValue_BeforeExecute_Throws() {
        _testCommand.AddOption<string>(new("option"));

        var action = () => _testCommand.GetRequiredValue<string>("option");

        action.Should().Throw<InvalidOperationException>().WithMessage("Missing required value 'option'.");
    }

    [Fact]
    public void Command_GetRequiredValue_WithEmptyName_Throws() {
        _testCommand.Execute();

        var action = () => _testCommand.GetRequiredValue<string>("");

        action.Should().Throw<ArgumentException>().WithMessage("Name cannot be null or whitespace. (Parameter 'name')");
    }

    [Fact]
    public void Command_GetRequiredValue_ReturnsValue() {
        _testCommand.AddOption<string>(new("option"));
        _testCommand.Execute(new[] { "--option", "some value" });

        var value = _testCommand.GetRequiredValue<string>("option");

        value.Should().Be("some value");
    }

    [Fact]
    public void Command_GetRequiredValue_ForParameter_ReturnsValue() {
        _testCommand.AddParameter<string>(new("option"));
        _testCommand.Execute(new[] { "some value" });

        var value = _testCommand.GetRequiredValue<string>("option");

        value.Should().Be("some value");
    }

    [Fact]
    public void Command_GetRequiredValue_WithWrongType_ReturnsValue() {
        _testCommand.AddOption<string>(new("option"));
        _testCommand.Execute(new[] { "--option", "some value" });

        var action = () => _testCommand.GetRequiredValue<int>("option");

        action.Should().Throw<InvalidCastException>().WithMessage("Cannot cast value 'option' to 'Int32'.");
    }

    [Fact]
    public void Command_GetRequiredValue_ForParameter_WithWrongType_ReturnsValue() {
        _testCommand.AddParameter<string>(new("option"));
        _testCommand.Execute(new[] { "some value" });

        var action = () => _testCommand.GetRequiredValue<int>("option");

        action.Should().Throw<InvalidCastException>().WithMessage("Cannot cast value 'option' to 'Int32'.");
    }

    [Fact]
    public void Command_GetRequiredValue_WhenSetByAlias_ReturnsValue() {
        _testCommand.AddOption<string>(new("option", 'o'));
        _testCommand.Execute(new[] { "-o", "some value" });

        var value = _testCommand.GetRequiredValue<string>("option");

        value.Should().Be("some value");
    }

    [Fact]
    public void Command_GetRequiredValue_WhenNotSet_Throws() {
        _testCommand.AddOption<string>(new("option"));
        _testCommand.Execute();

        var action = () => _testCommand.GetRequiredValue<string>("option");

        action.Should().Throw<InvalidOperationException>().WithMessage("Missing required value 'option'.");
    }

    [Fact]
    public void Command_GetRequiredValue_ForParameter_WhenNotSet_ReturnsValue() {
        _testCommand.AddParameter<string>(new("option"));
        _testCommand.Execute();

        var action = () => _testCommand.GetRequiredValue<string>("option");

        action.Should().Throw<InvalidOperationException>().WithMessage("Missing required value 'option'.");
    }

    [Fact]
    public void Command_GetRequiredValue_WhenNotAdded_Throws() {
        _testCommand.Execute();

        var action = () => _testCommand.GetRequiredValue<string>("option");

        action.Should().Throw<InvalidOperationException>().WithMessage("Required value 'option' not defined.");
    }

    [Fact]
    public void Command_GetValueOrDefault_ByIndex_BeforeExecute_ReturnsDefault() {
        _testCommand.AddParameter<string>(new("argument"));

        var value = _testCommand.GetValueOrDefault<string>(0);

        value.Should().BeNull();
    }

    [Fact]
    public void Command_GetValueOrDefault_ByIndex_ReturnsValue() {
        _testCommand.AddParameter<string>(new("argument"));
        _testCommand.Execute(new[] { "some value" });

        var value = _testCommand.GetValueOrDefault<string>(0);

        value.Should().Be("some value");
    }

    [Fact]
    public void Command_GetValueOrDefault_ByIndex_WithIndexOutOfRange_ReturnsDefault() {
        _testCommand.Execute();

        var value = _testCommand.GetValueOrDefault<string>(0);

        value.Should().BeNull();
    }

    [Fact]
    public void Command_GetValueOrDefault_ByIndex_WhenNotSet_ReturnsDefault() {
        _testCommand.AddParameter<string>(new("argument"));
        _testCommand.Execute();

        var value = _testCommand.GetValueOrDefault<string>(0);

        value.Should().BeNull();
    }

    [Fact]
    public void Command_GetValueOrDefault_ByIndex_WithWrongType_Throws() {
        _testCommand.AddParameter<string>(new("argument"));
        _testCommand.Execute(new[] { "some value" });

        var action = () => _testCommand.GetValueOrDefault<int>(0);

        action.Should().Throw<InvalidCastException>().WithMessage("Cannot cast parameter at index 0 to 'Int32'.");
    }

    [Fact]
    public void Command_GetFlagOrDefault_BeforeExecute_ReturnsDefault() {
        _testCommand.AddParameter<string>(new("argument"));

        var value = _testCommand.GetFlagOrDefault("flag");

        value.Should().BeFalse();
    }

    [Fact]
    public void Command_GetRequiredValue_ByIndex_BeforeExecute_Throws() {
        _testCommand.AddParameter<string>(new("option"));

        var action = () => _testCommand.GetRequiredValue<string>(0);

        action.Should().Throw<InvalidOperationException>().WithMessage("Missing required parameter at index 0.");
    }

    [Fact]
    public void Command_GetRequiredValue_ByIndex_ReturnsValue() {
        _testCommand.AddParameter<string>(new("option"));
        _testCommand.Execute(new[] { "some value" });

        var value = _testCommand.GetRequiredValue<string>(0);

        value.Should().Be("some value");
    }

    [Fact]
    public void Command_GetRequiredValue_ByIndex_WithWrongType_Throws() {
        _testCommand.AddParameter<string>(new("option"));
        _testCommand.Execute(new[] { "some value" });

        var action = () => _testCommand.GetRequiredValue<int>(0);

        action.Should().Throw<InvalidCastException>().WithMessage("Cannot cast parameter at index 0 to 'Int32'.");
    }

    [Fact]
    public void Command_GetRequiredValue_ByIndex_WhenNotSet_Throws() {
        _testCommand.AddParameter<string>(new("option"));
        _testCommand.Execute();

        var action = () => _testCommand.GetRequiredValue<string>(0);

        action.Should().Throw<InvalidOperationException>().WithMessage("Missing required parameter at index 0.");
    }

    [Fact]
    public void Command_GetRequiredValue_ByIndex_WhenNotAdded_Throws() {
        _testCommand.Execute();

        var action = () => _testCommand.GetRequiredValue<string>(0);

        action.Should().Throw<ArgumentOutOfRangeException>().WithMessage("Parameter at index 0 not found. Parameter count is 0. (Parameter 'index')");
    }

    [Fact]
    public void Command_GetFlagOrDefault_WithEmptyName_Throws() {
        _testCommand.Execute();

        var action = () => _testCommand.GetFlagOrDefault("");

        action.Should().Throw<ArgumentException>().WithMessage("Name cannot be null or whitespace. (Parameter 'name')");
    }

    [Fact]
    public void Command_GetFlagOrDefault_ReturnsValue() {
        _testCommand.AddFlag(new("flag"));
        _testCommand.Execute(new[] { "--flag" });

        var value = _testCommand.GetFlagOrDefault("flag");

        value.Should().BeTrue();
    }

    [Fact]
    public void Command_GetFlagOrDefault_WhenSetByAlias_ReturnsValue() {
        _testCommand.AddFlag(new("flag", 'f'));
        _testCommand.Execute(new[] { "-f" });

        var value = _testCommand.GetFlagOrDefault("flag");

        value.Should().BeTrue();
    }

    [Fact]
    public void Command_GetFlagOrDefault_WhenNotSet_ReturnsDefault() {
        _testCommand.AddFlag(new("flag"));
        _testCommand.Execute();

        var value = _testCommand.GetFlagOrDefault("flag");

        value.Should().BeFalse();
    }

    [Fact]
    public void Command_GetRequiredFlagOrDefault_WithEmptyName_Throws() {
        _testCommand.Execute();

        var action = () => _testCommand.GetRequiredFlag("");

        action.Should().Throw<ArgumentException>().WithMessage("Name cannot be null or whitespace. (Parameter 'name')");
    }

    [Fact]
    public void Command_GetRequiredFlagOrDefault_ReturnsValue() {
        _testCommand.AddFlag(new("flag"));
        _testCommand.Execute(new[] { "--flag" });

        var value = _testCommand.GetRequiredFlag("flag");

        value.Should().BeTrue();
    }

    [Fact]
    public void Command_GetRequiredFlagOrDefault_WhenSetByAlias_ReturnsValue() {
        _testCommand.AddFlag(new("flag", 'f'));
        _testCommand.Execute(new[] { "-f" });

        var value = _testCommand.GetRequiredFlag("flag");

        value.Should().BeTrue();
    }

    [Fact]
    public void Command_GetRequiredFlagOrDefault_WhenNotSet_Throws() {
        _testCommand.AddFlag(new("flag"));
        _testCommand.Execute();

        var action = () => _testCommand.GetRequiredFlag("flag");

        action.Should().Throw<InvalidOperationException>().WithMessage("Missing required flag 'flag'.");
    }

    [Fact]
    public void Command_GetRequiredFlagOrDefault_WhenNotAdd_Throws() {
        _testCommand.Execute();

        var action = () => _testCommand.GetRequiredFlag("flag");

        action.Should().Throw<InvalidOperationException>().WithMessage("Required flag 'flag' not defined.");
    }

    [Fact]
    public void Command_GetCollectionOrDefault_WithEmptyName_Throws() {
        _testCommand.Execute();

        var action = () => _testCommand.GetCollectionOrDefault<string>("");

        action.Should().Throw<ArgumentException>().WithMessage("Name cannot be null or whitespace. (Parameter 'name')");
    }

    [Fact]
    public void Command_GetCollectionOrDefault_ReturnsValue() {
        _testCommand.AddMultiOption<string>(new("option", 'o'));
        _testCommand.Execute(new[] { "--option", "value1", "-o", "value2" });

        var value = _testCommand.GetCollectionOrDefault<string>("option");

        value.Should().BeEquivalentTo("value1", "value2");
    }

    [Fact]
    public void Command_GetCollectionOrDefault_WhenNotSet_ReturnsEmpty() {
        _testCommand.AddMultiOption<string>(new("option"));
        _testCommand.Execute();

        var value = _testCommand.GetCollectionOrDefault<string>("option");

        value.Should().BeEmpty();
    }

    [Fact]
    public void Command_GetCollectionOrDefault_WithWrongType_Throws() {
        _testCommand.AddMultiOption<string>(new("option"));
        _testCommand.Execute(new[] { "--option", "value1", "-o", "value2" });

        var action = () => _testCommand.GetCollectionOrDefault<int>("option");

        action.Should().Throw<InvalidCastException>().WithMessage("Cannot cast value 'option' to a collection of 'Int32'.");
    }

    [Fact]
    public void Command_GetCollectionOrDefault_WhenNotAdded_ReturnsEmpty() {
        _testCommand.Execute();

        var value = _testCommand.GetCollectionOrDefault<string>("option");

        value.Should().BeEmpty();
    }

    [Fact]
    public void Command_GetRequiredCollection_WithEmptyName_Throws() {
        _testCommand.Execute();

        var action = () => _testCommand.GetRequiredCollection<string>("");

        action.Should().Throw<ArgumentException>().WithMessage("Name cannot be null or whitespace. (Parameter 'name')");
    }

    [Fact]
    public void Command_GetRequiredCollection_ReturnsValue() {
        _testCommand.AddMultiOption<string>(new("option", 'o'));
        _testCommand.Execute(new[] { "--option", "value1", "-o", "value2" });

        var value = _testCommand.GetRequiredCollection<string>("option");

        value.Should().BeEquivalentTo("value1", "value2");
    }

    [Fact]
    public void Command_GetRequiredCollection_WithWrongType_Throws() {
        _testCommand.AddMultiOption<string>(new("option"));
        _testCommand.Execute(new[] { "--option", "value1", "-o", "value2" });

        var action = () => _testCommand.GetRequiredCollection<int>("option");

        action.Should().Throw<InvalidCastException>().WithMessage("Cannot cast value 'option' to a collection of 'Int32'.");
    }

    [Fact]
    public void Command_GetRequiredCollection_WhenNotSet_ReturnsEmpty() {
        _testCommand.AddMultiOption<string>(new("option"));
        _testCommand.Execute();

        var value = _testCommand.GetRequiredCollection<string>("option");

        value.Should().BeEmpty();
    }

    [Fact]
    public void Command_GetRequiredCollection_WhenNotAdd_Throws() {
        _testCommand.Execute();

        var action = () => _testCommand.GetRequiredCollection<string>("option");

        action.Should().Throw<InvalidOperationException>().WithMessage("Required option 'option' not defined.");
    }
}
