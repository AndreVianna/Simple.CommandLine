namespace Simple.CommandLine.Utilities;

public interface IOutputWriter {
    bool UseColors { get; set; }
    VerboseLevel VerboseLevel { get; set; }

    ConsoleColor ForegroundColor { get; set; }
    ConsoleColor BackgroundColor { get; set; }
    void ResetColor();

    void Write(bool value);
    void Write(uint value);
    void Write(ulong value);
    void Write(int value);
    void Write(long value);
    void Write(float value);
    void Write(double value);
    void Write(decimal value);
    void Write(char value);
    void Write(string? value);
    void Write(object? value);

    void Write(StringBuilder? builder);

    void Write(string format, object? arg0);
    void Write(string format, object? arg0, object? arg1);
    void Write(string format, object? arg0, object? arg1, object? arg2);
    void Write(string format, params object?[] arg);

    void Write(char[]? buffer);
    void Write(char[] buffer, int index, int count);

    void WriteLine();

    void WriteLine(bool value);
    void WriteLine(uint value);
    void WriteLine(ulong value);
    void WriteLine(int value);
    void WriteLine(long value);
    void WriteLine(float value);
    void WriteLine(double value);
    void WriteLine(decimal value);
    void WriteLine(char value);
    void WriteLine(string? value);
    void WriteLine(object? value);

    void WriteLine(StringBuilder? builder);

    void WriteLine(string format, object? arg0);
    void WriteLine(string format, object? arg0, object? arg1);
    void WriteLine(string format, object? arg0, object? arg1, object? arg2);
    void WriteLine(string format, params object?[] arg);

    void WriteLine(char[]? buffer);
    void WriteLine(char[] buffer, int index, int count);
}
