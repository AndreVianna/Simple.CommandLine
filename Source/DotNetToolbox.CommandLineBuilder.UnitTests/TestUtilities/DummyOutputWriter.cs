namespace DotNetToolbox.CommandLineBuilder.TestUtilities;

public abstract class DummyOutputWriter : IOutputWriter {
    public ConsoleColor ForegroundColor {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public ConsoleColor BackgroundColor {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public bool UseColors {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public VerboseLevel VerboseLevel {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public void ResetColor() => throw new NotImplementedException();

    public virtual void Write(bool value) => throw new NotImplementedException();
    public virtual void Write(uint value) => throw new NotImplementedException();
    public virtual void Write(ulong value) => throw new NotImplementedException();
    public virtual void Write(int value) => throw new NotImplementedException();
    public virtual void Write(long value) => throw new NotImplementedException();
    public virtual void Write(float value) => throw new NotImplementedException();
    public virtual void Write(double value) => throw new NotImplementedException();
    public virtual void Write(decimal value) => throw new NotImplementedException();
    public virtual void Write(char value) => throw new NotImplementedException();
    public virtual void Write(string? value) => throw new NotImplementedException();
    public virtual void Write(object? value) => throw new NotImplementedException();

    public virtual void Write(StringBuilder? builder) => throw new NotImplementedException();

    public virtual void Write(string format, object? arg0) => throw new NotImplementedException();
    public virtual void Write(string format, object? arg0, object? arg1) => throw new NotImplementedException();
    public virtual void Write(string format, object? arg0, object? arg1, object? arg2) => throw new NotImplementedException();
    public virtual void Write(string format, params object?[] arg) => throw new NotImplementedException();

    public virtual void Write(char[]? buffer) => throw new NotImplementedException();
    public virtual void Write(char[] buffer, int index, int count) => throw new NotImplementedException();

    public virtual void WriteLine() => throw new NotImplementedException();

    public virtual void WriteLine(bool value) => throw new NotImplementedException();
    public virtual void WriteLine(uint value) => throw new NotImplementedException();
    public virtual void WriteLine(ulong value) => throw new NotImplementedException();
    public virtual void WriteLine(int value) => throw new NotImplementedException();
    public virtual void WriteLine(long value) => throw new NotImplementedException();
    public virtual void WriteLine(float value) => throw new NotImplementedException();
    public virtual void WriteLine(double value) => throw new NotImplementedException();
    public virtual void WriteLine(decimal value) => throw new NotImplementedException();
    public virtual void WriteLine(char value) => throw new NotImplementedException();
    public virtual void WriteLine(string? value) => throw new NotImplementedException();
    public virtual void WriteLine(object? value) => throw new NotImplementedException();

    public virtual void WriteLine(StringBuilder? builder) => throw new NotImplementedException();

    public virtual void WriteLine(string format, object? arg0) => throw new NotImplementedException();
    public virtual void WriteLine(string format, object? arg0, object? arg1) => throw new NotImplementedException();
    public virtual void WriteLine(string format, object? arg0, object? arg1, object? arg2) => throw new NotImplementedException();
    public virtual void WriteLine(string format, params object?[] arg) => throw new NotImplementedException();

    public virtual void WriteLine(char[]? buffer) => throw new NotImplementedException();
    public virtual void WriteLine(char[] buffer, int index, int count) => throw new NotImplementedException();
}
