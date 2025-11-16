using Microsoft.Extensions.Logging;
using Party.Application.DTOs;
using Party.Application.Mappers;
using Party.Application.Services.Abstractions;
using Party.Core.Enums;
using Party.Core.Repositories.Abstractions;
using Party.Core.Services.Abstractions;

namespace Party.Application.Services;

public class ServerListService(
    ILogger<ServerListService> logger,
    IVpnService vpnService,
    IServerDisplayService serverDisplayService,
    IServerRepository serverRepository) : IServerListService
{
    private readonly ILogger<ServerListService> _logger = logger;
    private readonly IVpnService _vpnService = vpnService;
    private readonly IServerDisplayService _serverDisplayService = serverDisplayService;
    private readonly IServerRepository _serverRepository = serverRepository;

    public async Task GetAndDisplayServersAsync(ServerListQueryDto query, CancellationToken cancellationToken = default)
    {
        IEnumerable<ServerDto> servers = [];

        if (query.UseLocalData)
            servers = await GetLocalServersAsync(cancellationToken);
        else if (query.CountryId.HasValue)
            servers = await GetServersByCountryAsync(query.CountryId.Value, cancellationToken);
        else if (query.VpnProtocol.HasValue)
            servers = await GetServersByProtocolAsync(query.VpnProtocol.Value, cancellationToken);
        else
            servers = await GetAllServersAsync(cancellationToken);

        await _serverDisplayService.DisplayServersAsync(servers, cancellationToken);
    }

    public async Task<IEnumerable<ServerDto>> GetAllServersAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Fetching all servers from VPN service");
            var servers = await _vpnService.GetAllServersAsync(cancellationToken);

            if (servers?.Any() == true)
            {
                await _serverRepository.UpsertServersAsync(servers, cancellationToken);
                _logger.LogInformation("Retrieved/saved {Count} servers from VPN service", servers.Count());
                return servers.ToDtoModels();
            }

            _logger.LogWarning("There is no any found server from VPN service");
            return [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching all servers from VPN service");
            return [];
        }
    }

    public async Task<IEnumerable<ServerDto>> GetServersByCountryAsync(CountryId countryId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Fetching servers by country {CountryId} from VPN service", countryId);
            var servers = await _vpnService.GetServersByCountryAsync(countryId, cancellationToken);

            if (servers?.Any() == true)
            {
                await _serverRepository.UpsertServersAsync(servers, cancellationToken);
                _logger.LogInformation("Retrieved {Count} servers from VPN service for country {CountryId}", servers.Count(), countryId);
                return servers.ToDtoModels();
            }

            _logger.LogWarning("There is no any found server from VPN service for country {CountryId}", countryId);
            return [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching servers by country {CountryId} from VPN service", countryId);
            return [];
        }
    }

    public async Task<IEnumerable<ServerDto>> GetServersByProtocolAsync(VpnProtocol protocolId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Fetching servers by protocol {ProtocolId} from VPN service", protocolId);
            var servers = await _vpnService.GetServersByProtocolAsync(protocolId, cancellationToken);

            if (servers?.Any() == true)
            {
                await _serverRepository.UpsertServersAsync(servers, cancellationToken);
                _logger.LogInformation("Retrieved {Count} servers from VPN service for protocol {ProtocolId}", servers.Count(), protocolId);
                return servers.ToDtoModels();
            }

            _logger.LogWarning("There is no any found server from VPN service for protocol {ProtocolId}", protocolId);
            return [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching servers by protocol {ProtocolId} from VPN service", protocolId);
            return [];
        }
    }

    public async Task<IEnumerable<ServerDto>> GetLocalServersAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Fetching servers from local databese");
            var servers = await _serverRepository.GetAllServersAsync(cancellationToken: cancellationToken);

            if (servers?.Any() == true)
            {
                _logger.LogInformation("Retrieved {Count} servers from local db", servers.Count());
                return servers.ToDtoModels();
            }

            _logger.LogWarning("There is no any found server in local database");
            return [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching servers from local database");
            return [];
        }
    }
}