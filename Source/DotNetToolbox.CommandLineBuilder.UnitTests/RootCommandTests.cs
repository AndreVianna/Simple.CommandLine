namespace DotNetToolbox.CommandLineBuilder;

public class RootCommandTests {
    [Fact]
    public void RootCommand_Execute_WithWriter_ExecutesDelegate() {
        InMemoryOutputWriter writer = new();
        RootCommand subject = new(writer, c => {
            var who = c.GetValueOrDefault<string>("who");
            c.Writer.WriteLine($"Hello {who}!");
        });
        subject.Add(new Parameter<string>("who"));

        subject.Execute("world");

        _ = writer.Output.Should().Be("Hello world!\n");
    }

    [Fact]
    public void RootCommand_Execute_ExecutesDelegate() {
        RootCommand subject = new(null, c => {
            var who = c.GetValueOrDefault<string>("who");
            c.Writer.WriteLine($"Hello {who}!");
        });
        subject.Add(new Parameter<string>("who"));

        subject.Execute("world");
    }
}
