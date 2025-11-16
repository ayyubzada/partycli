namespace Party.Core.Entities;

public class Server
{
    public string Name { get; set; } = null!;
    public int Load { get; set; }
    public string Status { get; set; } = null!;
}
