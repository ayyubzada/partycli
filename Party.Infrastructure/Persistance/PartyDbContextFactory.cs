using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Party.Infrastructure.Persistance;

/// Factory for creating PartyDbContext instances at design time
/// It is used by EF Core tools for migrations and scaffolding
public class PartyDbContextFactory : IDesignTimeDbContextFactory<PartyDbContext>
{
    public PartyDbContext CreateDbContext(string[] args)
    {
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Party.Cli");

        var config = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var connectionString = config.GetSection("Database").GetValue<string>("ConnectionString");

        var optionsBuilder = new DbContextOptionsBuilder<PartyDbContext>();
        optionsBuilder.UseSqlite(connectionString);

        return new PartyDbContext(optionsBuilder.Options);
    }
}