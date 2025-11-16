using Party.Core.Enums;
using Party.Core.Services.Abstractions;

namespace Party.Infrastructure.Services;

public class ConsoleDisplayService : IDisplayService
{
    private readonly ConsoleColor _originalColor = Console.ForegroundColor;

    public Task DisplayMessageAsync(
        string message,
        MessageType messageType = MessageType.Info,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(message))
            return Task.CompletedTask;

        var color = messageType switch
        {
            MessageType.Warning => ConsoleColor.Yellow,
            MessageType.Error => ConsoleColor.Red,
            MessageType.Success => ConsoleColor.Green,
            MessageType.Info => ConsoleColor.Gray,
            _ => _originalColor
        };

        DisplayWithColor(message, color);

        return Task.CompletedTask;
    }

    public Task DisplayWarningAsync(string message, CancellationToken cancellationToken = default)
        => DisplayMessageAsync(message, MessageType.Warning, cancellationToken);

    public Task DisplayErrorAsync(string message, CancellationToken cancellationToken = default)
        => DisplayMessageAsync(message, MessageType.Error, cancellationToken);

    public Task DisplaySuccessAsync(string message, CancellationToken cancellationToken = default)
        => DisplayMessageAsync(message, MessageType.Success, cancellationToken);

    public Task DisplaSeperatorAsync(CancellationToken cancellationToken = default)
    {
        DisplayWithColor(new string('-', 60), ConsoleColor.DarkGray);
        return Task.CompletedTask;
    }

    private void DisplayWithColor(string message, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ForegroundColor = _originalColor;
    }
}