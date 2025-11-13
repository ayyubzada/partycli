namespace Party.Cli.Models;

public class LogModel
{
    public string Action { get; set; } = null!;
    public DateTime Time { get; set; } = DateTime.UtcNow;
}