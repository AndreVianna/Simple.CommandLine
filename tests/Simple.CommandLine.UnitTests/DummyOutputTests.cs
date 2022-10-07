namespace Simple.CommandLine;

public class DummyOutputTests {
    private sealed class TestOutputWriter : DummyOutputWriter { }

    [Fact]
    public void DummyOutput_AllProperties_Throw() {
        var subject = new TestOutputWriter();

        ((Action)(() => _ = subject.ForegroundColor)).Should().Throw<NotImplementedException>();
        ((Action)(() => subject.ForegroundColor = ConsoleColor.Black)).Should().Throw<NotImplementedException>();
        ((Action)(() => _ = subject.BackgroundColor)).Should().Throw<NotImplementedException>();
        ((Action)(() => subject.BackgroundColor = ConsoleColor.White)).Should().Throw<NotImplementedException>();
        ((Action)(() => subject.ResetColor())).Should().Throw<NotImplementedException>();
        ((Action)(() => subject.Write(default(ulong)))).Should().Throw<NotImplementedException>();
        ((Action)(() => subject.Write(default(uint)))).Should().Throw<NotImplementedException>();
        ((Action)(() => subject.Write(default(StringBuilder?)))).Should().Throw<NotImplementedException>();
        ((Action)(() => subject.Write(default!, Array.Empty<object?>()))).Should().Throw<NotImplementedException>();
        ((Action)(() => subject.Write(default!, default, default, default))).Should().Throw<NotImplementedException>();
        ((Action)(() => subject.Write(default(string)!, default, default))).Should().Throw<NotImplementedException>();
        ((Action)(() => subject.Write(default!, default(object)))).Should().Throw<NotImplementedException>();
        ((Action)(() => subject.Write(default(string?)))).Should().Throw<NotImplementedException>();
        ((Action)(() => subject.Write(default(object?)))).Should().Throw<NotImplementedException>();
        ((Action)(() => subject.Write(default(long)))).Should().Throw<NotImplementedException>();
        ((Action)(() => subject.Write(default(int)))).Should().Throw<NotImplementedException>();
        ((Action)(() => subject.Write(default(double)))).Should().Throw<NotImplementedException>();
        ((Action)(() => subject.Write(default(decimal)))).Should().Throw<NotImplementedException>();
        ((Action)(() => subject.Write(default!, default, default))).Should().Throw<NotImplementedException>();
        ((Action)(() => subject.Write(default(char[])))).Should().Throw<NotImplementedException>();
        ((Action)(() => subject.Write(default(char)))).Should().Throw<NotImplementedException>();
        ((Action)(() => subject.Write(default(bool)))).Should().Throw<NotImplementedException>();
        ((Action)(() => subject.Write(default(float)))).Should().Throw<NotImplementedException>();
        ((Action)(() => subject.WriteLine(default(ulong)))).Should().Throw<NotImplementedException>();
        ((Action)(() => subject.WriteLine(default(uint)))).Should().Throw<NotImplementedException>();
        ((Action)(() => subject.WriteLine(default(StringBuilder?)))).Should().Throw<NotImplementedException>();
        ((Action)(() => subject.WriteLine(default!, Array.Empty<object?>()))).Should().Throw<NotImplementedException>();
        ((Action)(() => subject.WriteLine(default!, default(object)))).Should().Throw<NotImplementedException>();
        ((Action)(() => subject.WriteLine(default(string)!, default, default))).Should().Throw<NotImplementedException>();
        ((Action)(() => subject.WriteLine(default!, default, default, default))).Should().Throw<NotImplementedException>();
        ((Action)(() => subject.WriteLine(default(string?)))).Should().Throw<NotImplementedException>();
        ((Action)(() => subject.WriteLine(default(float)))).Should().Throw<NotImplementedException>();
        ((Action)(() => subject.WriteLine(default(object?)))).Should().Throw<NotImplementedException>();
        ((Action)(() => subject.WriteLine(default(bool)))).Should().Throw<NotImplementedException>();
        ((Action)(() => subject.WriteLine(default(char)))).Should().Throw<NotImplementedException>();
        ((Action)(() => subject.WriteLine(default(char[])))).Should().Throw<NotImplementedException>();
        ((Action)(() => subject.WriteLine())).Should().Throw<NotImplementedException>();
        ((Action)(() => subject.WriteLine(default(decimal)))).Should().Throw<NotImplementedException>();
        ((Action)(() => subject.WriteLine(default(double)))).Should().Throw<NotImplementedException>();
        ((Action)(() => subject.WriteLine(default(int)))).Should().Throw<NotImplementedException>();
        ((Action)(() => subject.WriteLine(default(long)))).Should().Throw<NotImplementedException>();
        ((Action)(() => subject.WriteLine(default!, default, default))).Should().Throw<NotImplementedException>();
    }
}