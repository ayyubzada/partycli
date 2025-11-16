namespace Party.Infrastructure.Configurations;

public class DatabaseOptions
{
    public const string SectionName = "Database";

    public string ConnectionString { get; set; }
}