using Microsoft.EntityFrameworkCore;
using Party.Cli.Models;

namespace Party.Cli.Persistance.Repositories;

public class Repository(PartyDbContext context)
{
    private readonly PartyDbContext _context = context;

    public async Task<bool> SaveServersAsync(
        IEnumerable<ServerModel> servers,
        CancellationToken ct = default)
    {
        try
        {
            await _context.Servers.AddRangeAsync(servers, ct);
            var affectedRows = await _context.SaveChangesAsync(ct);

            return affectedRows > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return false;
        }
    }

    public async Task<IEnumerable<ServerModel>?> GetServersAsync(
        string? name = null,
        int? load = null,
        string? status = null,
        int take = 100,
        CancellationToken ct = default)
    {
        try
        {
            var query = _context.Servers.AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(s => s.Name == name);

            if (load.HasValue)
                query = query.Where(s => s.Load == load.Value);

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(s => s.Status == status);

            if (take <= 0 || take >= 1000)
                take = 100;

            query = query
                .OrderBy(s => s.Name)
                .Take(take);

            return await query.ToListAsync(ct);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return null;
        }
    }

    public async Task<bool> UpsertConfigurationAsync(
        ConfigurationModel config,
        CancellationToken ct = default)
    {
        try
        {
            var existing = await _context.Configurations
                .FirstOrDefaultAsync(c => c.Key == config.Key, ct);

            if (existing != null)
            {
                existing.Value = config.Value;
                _context.Configurations.Update(existing);
            }
            else
                await _context.Configurations.AddAsync(config, ct);

            var affectedRows = await _context.SaveChangesAsync(ct);

            return affectedRows > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return false;
        }
    }

    public async Task<ConfigurationModel?> GetConfigurationAsync(
        string key,
        CancellationToken ct = default)
    {
        try
        {
            return await _context.Configurations
                .FirstOrDefaultAsync(c => c.Key == key, ct);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return null;
        }
    }
}
