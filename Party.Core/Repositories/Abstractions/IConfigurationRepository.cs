using Party.Core.Entities;

namespace Party.Core.Repositories.Abstractions;

public interface IConfigurationRepository
{
    Task<bool> UpsertAsync(Configuration configuration, CancellationToken cancellationToken = default);

    Task<Configuration?> GetByKeyAsync(string key, CancellationToken cancellationToken = default);

    Task<IEnumerable<Configuration>> GetAllAsync(CancellationToken cancellationToken = default);
}