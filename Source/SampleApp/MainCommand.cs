namespace SampleApp;

internal static class MainCommand {
    public static async Task Execute(string[] arguments) {
        RootCommand command = new();
        command.Add(new RestCommand());
        await command.Execute(arguments);
    }
}
