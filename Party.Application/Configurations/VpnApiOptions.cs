namespace Party.Application.Configurations;

public class VpnApiOptions
{
    public const string SectionName = "VpnApi";

    public string BaseUrl { get; set; } = null!;
    public int TimeoutSeconds { get; set; }
}