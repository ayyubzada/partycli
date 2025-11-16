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