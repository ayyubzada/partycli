using Party.Application.DTOs;
using Party.Core.Enums;

namespace Party.Application.Services.Abstractions;

public interface IServerListService
{
    Task GetAndDisplayServersAsync(ServerListQueryDto query, CancellationToken cancellationToken = default);

    Task<IEnumerable<ServerDto>> GetAllServersAsync(CancellationToken cancellationToken = default);

    Task<IEnumerable<ServerDto>> GetServersByCountryAsync(CountryId countryId, CancellationToken cancellationToken = default);

    Task<IEnumerable<ServerDto>> GetServersByProtocolAsync(VpnProtocol protocolId, CancellationToken cancellationToken = default);

    Task<IEnumerable<ServerDto>> GetLocalServersAsync(CancellationToken cancellationToken = default);
}