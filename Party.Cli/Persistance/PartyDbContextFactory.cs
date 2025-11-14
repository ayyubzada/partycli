using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Party.Cli.Persistance;

public class PartyDbContextFactory : IDesignTimeDbContextFactory<PartyDbContext>
{
    public PartyDbContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = config.GetSection("Database").GetValue<string>("ConnectionString");

        var optionsBuilder = new DbContextOptionsBuilder<PartyDbContext>();
        optionsBuilder.UseSqlite(connectionString);

        return new PartyDbContext(optionsBuilder.Options);
    }
}