using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Party.Core.Entities;
using Party.Core.Repositories.Abstractions;

namespace Party.Infrastructure.Persistance.Repositories;

public class ConfigurationRepository(
    ILogger<ConfigurationRepository> logger,
    PartyDbContext dbContext) : IConfigurationRepository
{
    private readonly ILogger<ConfigurationRepository> _logger = logger;
    private readonly PartyDbContext _dbContext = dbContext;

    public async Task<bool> UpsertAsync(
        Configuration configuration,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (configuration is null)
            {
                _logger.LogWarning("Attempted to upsert an invalid configuration");
                return false;
            }

            var existing = await GetByKeyAsync(configuration.Key, cancellationToken);

            if (existing is null)
            {
                await _dbContext.Configurations.AddAsync(configuration, cancellationToken);
                _logger.LogInformation("Added new configuration with key {Key}", configuration.Key);
            }
            else
            {
                existing.Value = configuration.Value;
                _dbContext.Configurations.Update(existing);
                _logger.LogInformation("Updated configuration with key {Key}", configuration.Key);
            }

            var affectedRows = await _dbContext.SaveChangesAsync(cancellationToken);
            return affectedRows > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while upserting configuration with key {Key}", configuration?.Key);
            return false;
        }
    }

    public async Task<Configuration?> GetByKeyAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbContext.Configurations
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Key == key, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving configuration with key {Key}", key);
            return null;
        }
    }

    public async Task<IEnumerable<Configuration>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbContext.Configurations
                .AsNoTracking()
                .OrderBy(c => c.Key)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving all configurations");
            return [];
        }
    }
}

public class ServerRepository(
    ILogger<ServerRepository> logger,
    PartyDbContext dbContext) : IServerRepository
{
    private readonly ILogger<ServerRepository> _logger = logger;
    private readonly PartyDbContext _dbContext = dbContext;

    public async Task<bool> UpsertServersAsync(IEnumerable<Server> servers, CancellationToken cancellationToken = default)
    {
        try
        {
            if (servers is null || !servers.Any())
            {
                _logger.LogWarning("Attempted to save an invalid or empty server list");
                return false;
            }

            var attemptedServerNames = servers.Select(s => s.Name);

            var existingServers = await _dbContext.Servers
                .Where(s => attemptedServerNames.Contains(s.Name))
                .ToListAsync(cancellationToken);

            if (existingServers is not null && existingServers.Any())
            {
                var requestedUpdateServers = servers.ExceptBy(existingServers.Select(es => es.Name), s => s.Name);
                var replaced = await ReplaceServersAsync(requestedUpdateServers, cancellationToken);

                servers = servers.IntersectBy(existingServers.Select(es => es.Name), s => s.Name);

                if (!servers.Any())
                {
                    _logger.LogInformation("All servers already exist and they were replaced.");
                    return replaced;
                }
            }

            await _dbContext.Servers.AddRangeAsync(servers, cancellationToken);
            var affectedRows = await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Saved {Count} new servers", servers.Count());
            return affectedRows > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while saving servers");
            throw;
        }
    }

    public async Task<bool> ReplaceServersAsync(IEnumerable<Server> servers, CancellationToken cancellationToken = default)
    {
        try
        {
            if (servers is null || !servers.Any())
            {
                _logger.LogWarning("Attempted to replace an invalid or empty server list");
                return false;
            }

            var serverNames = servers.Select(s => s.Name);
            var existingServers = await _dbContext.Servers
                .Where(s => serverNames.Contains(s.Name))
                .ToListAsync(cancellationToken);

            if (existingServers is null || !existingServers.Any())
            {
                _logger.LogInformation("No existing servers found to replace.");
                return false;
            }

            foreach (var existingServer in existingServers)
            {
                existingServer.Load = servers.First(s => s.Name == existingServer.Name).Load;
                existingServer.Status = servers.First(s => s.Name == existingServer.Name).Status;
            }

            _dbContext.Servers.UpdateRange(existingServers);
            var affectedRows = await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Replaced {Count} existing servers", affectedRows);
            return affectedRows > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while replacing servers");
            return false;
        }
    }

    public async Task<IEnumerable<Server>> GetAllServersAsync(
        string? name = null,
        int? load = null,
        string? status = null,
        int take = 100,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _dbContext.Servers.AsNoTracking().AsQueryable();

            if (!string.IsNullOrEmpty(name))
                query = query.Where(s => s.Name == name);

            if (load.HasValue)
                query = query.Where(s => s.Load == load.Value);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(s => s.Status == status);

            // Ensure take is within reasonable range
            if (take <= 0 && take > 1000)
                take = 100;

            return await query
                .OrderBy(s => s.Name)
                .Take(take)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving servers with filters Name: {Name}, Load: {Load}, Status: {Status}", name, load, status);
            throw;
        }
    }


}
