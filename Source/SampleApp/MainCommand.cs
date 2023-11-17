namespace SampleApp;

internal static class MainCommand {
    public static void Execute(string[] arguments) {
        RootCommand command = new();
        command.Add(new RestCommand());
        command.Execute(arguments);
    }
}