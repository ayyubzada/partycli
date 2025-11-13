using Microsoft.EntityFrameworkCore;
using Party.Cli.Models;

namespace Party.Cli.Persistance;

public class PartyDbContext : DbContext
{
    public DbSet<ServerModel> Servers => Set<ServerModel>();
    public DbSet<LogModel> Logs => Set<LogModel>();
    public DbSet<ConfigurationModel> Configurations => Set<ConfigurationModel>();

    public PartyDbContext(DbContextOptions<PartyDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ServerModel>()
            .HasKey(x => x.Name);

        modelBuilder.Entity<LogModel>()
            .HasKey(x => new { x.Action, x.Time });

        modelBuilder.Entity<ConfigurationModel>()
            .HasKey(x => x.Key);
    }
}