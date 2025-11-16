using Microsoft.EntityFrameworkCore;
using Party.Core.Entities;

namespace Party.Infrastructure.Persistance;

public class PartyDbContext : DbContext
{
    public DbSet<Server> Servers => Set<Server>();
    public DbSet<Configuration> Configurations => Set<Configuration>();

    public PartyDbContext(DbContextOptions<PartyDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Server>(entity =>
        {
            entity.HasKey(e => e.Name);
            entity.Property(e => e.Name).HasMaxLength(150).IsRequired();
            entity.Property(e => e.Load).IsRequired();
            entity.Property(e => e.Status).HasMaxLength(50).IsRequired();
        });

        modelBuilder.Entity<Configuration>(entity =>
        {
            entity.HasKey(e => e.Key);
            entity.Property(e => e.Key).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Value).IsRequired();
        });
    }
}
