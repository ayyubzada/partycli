namespace Party.Cli.Models;

public class ServerModel
{
    public string Name { get; set; } = null!;
    public int Load { get; set; }
    public string Status { get; set; } = null!;
}