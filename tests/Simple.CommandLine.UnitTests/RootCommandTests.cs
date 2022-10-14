namespace Simple.CommandLine;

public class RootCommandTests {
    [Fact]
    public void RootCommand_Execute_ExecutesDelegate() {
        var writer = new InMemoryOutputWriter();
        var subject = new RootCommand(c => {
            var who = c.GetValueOrDefault<string>("who");
            c.Writer.WriteLine($"Hello {who}!");
        }) {
            Writer = writer
        };
        subject.AddParameter(new Parameter<string>("who"));

        subject.Execute("world");

        writer.Output.Should().Be("Hello world!\n");
    }
}
