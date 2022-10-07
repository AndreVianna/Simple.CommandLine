namespace Simple.CommandLine.SampleApp;

internal static class MainCommand {
    public static void Execute(string[] arguments) {
        var command = new RootCommand();
        command.AddSubCommand(new RestCommand(command));
        command.Execute(arguments);
    }
}