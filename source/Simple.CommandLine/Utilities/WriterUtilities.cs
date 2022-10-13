namespace Simple.CommandLine.Utilities;

public static class WriterUtilities {
    public static void WriteVersion(this IOutputWriter writer, Command command) {
        var assembly = command.GetType().Assembly;
        var name = assembly.GetName().Name;
        var title = assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title;
        var version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion;
        writer.WriteLine(title ?? name);
        writer.WriteLine(version);
    }


    public static void WriteHelp(this IOutputWriter writer, Command command) {
        if (command is RootCommand) {
            var assembly = command.GetType().Assembly;
            var name = assembly.GetName().Name;
            var title = assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title;
            var version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion;
            writer.WriteLine();
            writer.WriteLine($"{title ?? name} {version}");

            var description = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description;
            if (description is not null) {
                writer.WriteLine();
                writer.WriteLine(description);
            }
        }

        writer.WriteLine();
        var arguments = command.GetPartDescriptions(nameof(Parameter));
        var options = command.GetPartDescriptions(nameof(Option));
        var commands = command.GetPartDescriptions(nameof(Command));
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
        if (writer.IsVerbose) writer.WriteLine(ex.ToString().Replace("\r", ""));
    }

    public static void WriteError(this IOutputWriter writer, string message) {
        if (!writer.UseColors) {
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