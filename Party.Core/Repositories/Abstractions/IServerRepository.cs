using Party.Core.Entities;

namespace Party.Core.Repositories.Abstractions;

public interface IServerRepository
{
    Task<bool> UpsertServersAsync(IEnumerable<Server> servers, CancellationToken cancellationToken = default);

    Task<IEnumerable<Server>> GetAllServersAsync(
        string? name = null,
        int? load = null,
        string? status = null,
        int take = 100,
        CancellationToken cancellationToken = default);

    Task<bool> ReplaceServersAsync(IEnumerable<Server> servers, CancellationToken cancellationToken = default);
}