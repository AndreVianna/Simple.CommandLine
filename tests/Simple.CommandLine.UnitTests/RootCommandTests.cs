namespace Simple.CommandLine;

public class RootCommandTests {
    [Fact]
    public void RootCommand_Execute_WithWriter_ExecutesDelegate() {
        var writer = new InMemoryOutputWriter();
        var subject = new RootCommand(writer, c => {
            var who = c.GetValueOrDefault<string>("who");
            c.Writer.WriteLine($"Hello {who}!");
        });
        subject.Add(new Parameter<string>("who"));

        subject.Execute("world");

        writer.Output.Should().Be("Hello world!\n");
    }

    [Fact]
    public void RootCommand_Execute_ExecutesDelegate()
    {
        var subject = new RootCommand(null, c => {
            var who = c.GetValueOrDefault<string>("who");
            c.Writer.WriteLine($"Hello {who}!");
        });
        subject.Add(new Parameter<string>("who"));

        subject.Execute("world");
    }
}
