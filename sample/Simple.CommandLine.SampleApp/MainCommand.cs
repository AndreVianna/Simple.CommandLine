namespace Simple.CommandLine.SampleApp;

internal static class MainCommand {
    public static void Execute(string[] arguments) {
        var command = new DefaultRootCommand();
        command.AddCommand(new RestCommand());
        command.Execute(arguments);
    }
}