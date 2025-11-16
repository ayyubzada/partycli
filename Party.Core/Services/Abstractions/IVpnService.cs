using Party.Core.Entities;
using Party.Core.Enums;

namespace Party.Core.Services.Abstractions;

public interface IVpnService
{
    Task<IEnumerable<Server>> GetAllServersAsync(CancellationToken cancellationToken = default);

    Task<IEnumerable<Server>> GetServersByCountryAsync(CountryId countryId, CancellationToken cancellationToken = default);

    Task<IEnumerable<Server>> GetServersByProtocolAsync(VpnProtocol protocolId, CancellationToken cancellationToken = default);
}