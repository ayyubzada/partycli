using Party.Cli.Models;

namespace Party.Cli.Services;

public class DisplayService
{
    ConsoleColor _originalColor = Console.ForegroundColor;

    public void DisplayMessage(string message, MessageType messageType = MessageType.Info)
    {
        if (string.IsNullOrEmpty(message))
            return;

        var colorToUse = _originalColor;

        switch (messageType)
        {
            case MessageType.Warning:
                colorToUse = ConsoleColor.Yellow;
                break;
            case MessageType.Error:
                colorToUse = ConsoleColor.Red;
                break;
            case MessageType.Info:
            default:
                colorToUse = ConsoleColor.Gray;
                break;
        }

        DisplayMessage(message, colorToUse);
    }

    public void DisplayWarning(string message) => DisplayMessage(message, MessageType.Warning);

    public void DisplayError(string message) => DisplayMessage(message, MessageType.Error);

    public void DisplayServers(IEnumerable<ServerModel> servers)
    {
        if (!servers.Any())
            return;

        DisplayMessage("Server list: ", ConsoleColor.Blue);

        foreach (var server in servers)
            DisplayMessage($"Name: {server.Name}", ConsoleColor.DarkGray);

        DisplayMessage($"Total servers: {servers.Count()}", ConsoleColor.Blue);
    }

    private void DisplayMessage(
        string message,
        ConsoleColor colorToUse)
    {
        if (string.IsNullOrEmpty(message))
            return;

        Console.ForegroundColor = colorToUse;
        Console.WriteLine(message);

        Console.ForegroundColor = _originalColor;
    }
}
