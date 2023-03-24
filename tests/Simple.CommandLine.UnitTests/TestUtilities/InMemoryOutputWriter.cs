namespace Simple.CommandLine.TestUtilities;

public class InMemoryOutputWriter : IOutputWriter
{
    public string Output = string.Empty;

    public ConsoleColor ForegroundColor { get; set; }
    public ConsoleColor BackgroundColor { get; set; }
    public bool UseColors { get; set; } = true;
    public VerboseLevel VerboseLevel { get; set; }

    public void ResetColor() { }

    public void Write(bool value) => Output += $"{value}";
    public void Write(uint value) => Output += $"{value}";
    public void Write(ulong value) => Output += $"{value}";
    public void Write(int value) => Output += $"{value}";
    public void Write(long value) => Output += $"{value}";
    public void Write(float value) => Output += $"{value}";
    public void Write(double value) => Output += $"{value}";
    public void Write(decimal value) => Output += $"{value}";
    public void Write(char value) => Output += $"{value}";
    public void Write(string? value) => Output += $"{value}";
    public void Write(object? value) => Output += $"{value}";
    public void Write(StringBuilder? builder) => Output += builder?.ToString() ?? string.Empty;
    public void Write(string format, object? arg0) => Output += string.Format(format, arg0);
    public void Write(string format, object? arg0, object? arg1) => Output += string.Format(format, arg0, arg1);
    public void Write(string format, object? arg0, object? arg1, object? arg2) => Output += string.Format(format, arg0, arg1, arg2);
    public void Write(string format, params object?[] args) => Output += string.Format(format, args);
    public void Write(char[]? buffer) => Output += new string(buffer);
    public void Write(char[] buffer, int index, int count) => Output += new string(buffer, index, count);
    public void WriteLine() => Output += "\n";
    public void WriteLine(bool value) => Output += $"{value}\n";
    public void WriteLine(uint value) => Output += $"{value}\n";
    public void WriteLine(ulong value) => Output += $"{value}\n";
    public void WriteLine(int value) => Output += $"{value}\n";
    public void WriteLine(long value) => Output += $"{value}\n";
    public void WriteLine(float value) => Output += $"{value}\n";
    public void WriteLine(double value) => Output += $"{value}\n";
    public void WriteLine(decimal value) => Output += $"{value}\n";
    public void WriteLine(char value) => Output += $"{value}\n";
    public void WriteLine(string? value) => Output += $"{value}\n";
    public void WriteLine(object? value) => Output += $"{value}\n";
    public void WriteLine(StringBuilder? builder) => Output += builder?.AppendLine().ToString() ?? "\n";
    public void WriteLine(string format, object? arg0) => Output += string.Format(format, arg0) + "\n";
    public void WriteLine(string format, object? arg0, object? arg1) => Output += string.Format(format, arg0, arg1) + "\n";
    public void WriteLine(string format, object? arg0, object? arg1, object? arg2) => Output += string.Format(format, arg0, arg1, arg2) + "\n";
    public void WriteLine(string format, params object?[] args) => Output += string.Format(format, args) + "\n";
    public void WriteLine(char[]? buffer) => Output += new string(buffer) + "\n";
    public void WriteLine(char[] buffer, int index, int count) => Output += new string(buffer, index, count) + "\n";
}
