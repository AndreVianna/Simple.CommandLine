namespace Simple.CommandLine.Utilities;

public static class WriterUtilities {
    internal static bool Colorize { get; set; } = true;
    internal static bool IsVerbose { get; set; }

    public static void WriteHelp(this IOutputWriter writer, Command command) {
        if (command is RootCommand) {
            var assembly = Assembly.GetEntryAssembly()!;
            var name = assembly.GetName().Name;
            var description = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description;
            var version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion;
            writer.WriteLine();
            writer.WriteLine($"{description ?? name} {version}");
        }

        writer.WriteLine();
        var arguments = command.GetTokenDescriptions(nameof(Argument));
        var options = command.GetTokenDescriptions(nameof(Parameter));
        var commands = command.GetTokenDescriptions(nameof(Command));
        writer.WriteLine($"Usage: {command.Path}{(arguments.Length == 0 ? "" : " [arguments]")}{(options.Length == 0 ? "" : " [options]")}{(commands.Length == 0 ? "" : " [command]")}");
        writer.WriteSection("Arguments:", arguments);
        writer.WriteSection("Options:", options);
        writer.WriteSection("Commands:", commands);
        if (commands.Length > 0) {
            writer.WriteLine();
            writer.WriteLine($"Use \"{command.Path} [command] --help\" for more information about a command.");
        }

        writer.WriteLine();
    }

    public static void WriteError(this IOutputWriter writer, string message, Exception ex) {
        writer.WriteError(message);
        if (IsVerbose) writer.WriteLine(ex.ToString());
        writer.WriteLine();
    }

    public static void WriteError(this IOutputWriter writer, string message) {
        if (!Colorize) {
            writer.WriteLine(message);
            return;
        }

        var oldColor = writer.ForegroundColor;
        try {
            writer.ForegroundColor = ConsoleColor.Red;
            writer.WriteLine(message);
        }
        finally {
            writer.ForegroundColor = oldColor;
        }
    }

    private static void WriteSection(this IOutputWriter writer, string header, IReadOnlyCollection<string> lines) {
        if (lines.Count == 0)
            return;
        writer.WriteLine();
        writer.WriteLine(header);
        foreach (var line in lines) {
            writer.WriteLine(line);
        }
    }
}