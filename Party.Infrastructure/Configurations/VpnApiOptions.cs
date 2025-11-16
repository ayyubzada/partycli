namespace Party.Infrastructure.Configurations;

public record VpnApiOptions(string BaseUrl, int TimeoutSeconds)
{
    public const string SectionName = "VpnApi";
}