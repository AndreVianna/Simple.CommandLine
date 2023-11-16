namespace DotNetToolbox.CommandLineBuilder.Utilities;

public static class WriterUtilities {
    public static void WriteVersion(this IOutputWriter writer, Command command) {
        Assembly assembly = command.GetType().Assembly;
        writer.WriteLine(GetAssemblyTitle(assembly));
        writer.WriteLine(GetAssemblyVersion(assembly));
    }

    public static void WriteHelp(this IOutputWriter writer, Command command) {
        if (command is RootCommand) writer.WriteRootHeader(command);

        Token[] parameters = command.Tokens.OfType<Parameter>().Cast<Token>().ToArray();
        Token[] options = command.Tokens.OfType<Option>().Cast<Token>().ToArray();
        Token[] flags = command.Tokens.OfType<Flag>().Cast<Token>().ToArray();
        Token[] subCommands = command.Tokens.OfType<SubCommand>().Cast<Token>().ToArray();

        writer.WriteLine();
        if (parameters.Length != 0) writer.WriteLine($"Usage: {command.Path} [parameters]{(options.Length != 0 ? " [options]" : "")}");
        writer.WriteLine($"Usage: {command.Path}{(options.Length != 0 ? " [options]" : "")}{(subCommands.Length != 0 ? " [command]" : "")}");
        writer.WriteSection("Options:", options.Union(flags).OrderBy(i => i.Name).ToArray());
        writer.WriteSection("Parameters:", parameters);
        writer.WriteSection("Commands:", subCommands);
        if (subCommands.Length > 0) {
            writer.WriteLine();
            writer.WriteLine($"Use \"{command.Path} [command] --help\" for more information about a command.");
        }

        writer.WriteLine();
    }

    public static void WriteError(this IOutputWriter writer, string message, Exception ex) {
        if (writer.VerboseLevel == VerboseLevel.Silent) return;

        writer.WriteError(message);
        if (writer.VerboseLevel == VerboseLevel.Debug)
            writer.WriteLine(ex.ToString().Replace("\r", ""));
    }

    public static void WriteError(this IOutputWriter writer, string message) {
        if (writer.VerboseLevel == VerboseLevel.Silent) return;

        ConsoleColor oldColor = writer.ForegroundColor;
        try {
            if (writer.UseColors) writer.ForegroundColor = ConsoleColor.Red;
            writer.WriteLine(message);
        }
        finally {
            writer.ForegroundColor = oldColor;
        }
    }

    [ExcludeFromCodeCoverage]
    private static void WriteRootHeader(this IOutputWriter writer, Command command) {
        Assembly assembly = command.GetType().Assembly;
        writer.WriteLine();
        writer.WriteLine($"{GetAssemblyTitle(assembly)} {GetAssemblyVersion(assembly)}");

        string? description = GetAssemblyDescription(assembly);
        if (description is null) return;
        writer.WriteLine();
        writer.WriteLine(description);
    }

    private static void WriteSection(this IOutputWriter writer, string header, IReadOnlyCollection<Token> tokens) {
        if (tokens.Count == 0) return;
        writer.WriteLine();
        writer.WriteLine(header);
        foreach (Token token in tokens) writer.WriteLine(DescribeToken(token));
    }

    private static string DescribeToken(Token token) {
        StringBuilder builder = new();
        _ = builder.Append(' ', 2);

        switch (token) {
            case Parameter:
                _ = builder.Append('<').Append(token.Name).Append('>');
                break;
            case Option option:
                if (option.Alias != '\0') _ = builder.Append('-').Append(option.Alias).Append(", ");
                _ = builder.Append("--").Append(token.Name);
                if (option is IHasValue) _ = builder.Append(" <").Append(token.Name).Append('>');
                break;
            case Flag flag:
                if (flag.Alias != '\0') _ = builder.Append('-').Append(flag.Alias).Append(", ");
                _ = builder.Append("--").Append(token.Name);
                break;
            default:
                _ = builder.Append(token.Name);
                break;
        }

        if (token.Description.Length == 0) return builder.ToString();

        int padding = 25;
        int length = builder.Length;
        if (padding <= length) padding = length + 1;
        _ = builder.Append(' ', padding - length);

        _ = builder.Append(token.Description);
        return builder.ToString();
    }

    [ExcludeFromCodeCoverage]
    private static string GetAssemblyTitle(Assembly assembly) {
        AssemblyTitleAttribute? attribute = assembly.GetCustomAttribute<AssemblyTitleAttribute>();
        return attribute?.Title ?? assembly.GetName().Name!;
    }

    [ExcludeFromCodeCoverage]
    private static string? GetAssemblyDescription(Assembly assembly)
        => assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description;

    [ExcludeFromCodeCoverage]
    private static string GetAssemblyVersion(Assembly assembly)
        => assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion;
}