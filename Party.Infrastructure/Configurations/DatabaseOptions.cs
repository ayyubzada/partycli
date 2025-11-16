namespace Party.Infrastructure.Configurations;

public record DatabaseOptions(string ConnectionString)
{
    public const string SectionName = "Database";
}