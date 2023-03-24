namespace Simple.CommandLine.SampleApp;

internal static class MainCommand {
    public static void Execute(string[] arguments) {
        var command = new RootCommand();
        command.Add(new RestCommand());
        command.Execute(arguments);
    }
}