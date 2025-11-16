using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Party.Core.Entities;
using Party.Core.Repositories.Abstractions;

namespace Party.Infrastructure.Persistance.Repositories;

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

            var serverNames = servers.Select(s => s.Name);
            var existingServers = await _dbContext.Servers
                .Where(s => serverNames.Contains(s.Name))
                .ToDictionaryAsync(s => s.Name, cancellationToken);

            var serversToInsert = new List<Server>();
            var updatedCount = 0;

            foreach (var server in servers)
            {
                if (existingServers.TryGetValue(server.Name, out var existingServer))
                {
                    existingServer.Load = server.Load;
                    existingServer.Status = server.Status;
                    existingServer.CountryId = server.CountryId;
                    updatedCount++;
                }
                else
                    serversToInsert.Add(server);
            }

            if (serversToInsert.Any())
                await _dbContext.Servers.AddRangeAsync(serversToInsert, cancellationToken);

            var affectedRows = await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Upserted servers. Inserted: {InsertedCount}, Updated: {UpdatedCount}", serversToInsert.Count, updatedCount);

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
                existingServer.CountryId = servers.First(s => s.Name == existingServer.Name).CountryId;
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
