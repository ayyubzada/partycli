//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
//using Newtonsoft.Json;
//using Party.Core.Entities;
//using Party.Core.Enums;
//using Party.Core.Services.Abstractions;
//using Party.Infrastructure.Configurations;

//namespace Party.Infrastructure.Services;

//public class NordVpnService(
//    ILogger<NordVpnService> logger,
//    IOptions<VpnApiOptions> vpnApiOptions,
//    HttpClient httpClient) : IVpnService
//{
//    private readonly ILogger<NordVpnService> _logger = logger;
//    private readonly IOptions<VpnApiOptions> _vpnApiOptions = vpnApiOptions;
//    private readonly HttpClient _httpClient = httpClient;

//    public async Task<IEnumerable<Server>> GetAllServersAsync(CancellationToken cancellationToken = default)
//    {
//        try
//        {
//            _logger.LogInformation("Fetching all serveses from NordVpn API...");
//            var url = "/servers";
//            return await FetchServersAsync(url, cancellationToken);
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "Error occured during fetching servers from NordVPN");
//            return [];
//        }
//    }

//    public async Task<IEnumerable<Server>> GetServersByCountryAsync(CountryId countryId, CancellationToken cancellationToken = default)
//    {
//        try
//        {
//            _logger.LogInformation("Fetching servers for country: {CountryName}({CountryId})", countryId.ToString(), (int)countryId);
//            var url = $"/servers?filters[servers_technologies][id]=35&filters[country_id]={(int)countryId}";
//            return await FetchServersAsync(url, cancellationToken);
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "Error occured during fetching servers(with country) from NordVPN");
//            return [];
//        }
//    }

//    public async Task<IEnumerable<Server>> GetServersByProtocolAsync(VpnProtocol protocolId, CancellationToken cancellationToken = default)
//    {
//        try
//        {
//            _logger.LogInformation("Fetching servers for protocol: {ProtocolName}({ProtocolId})", protocolId.ToString(), (int)protocolId);
//            var url = $"/servers?filters[servers_technologies][id]={(int)protocolId}";
//            return await FetchServersAsync(url, cancellationToken);
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "Error occured during fetching servers(with protocol) from NordVPN");
//            return [];
//        }
//    }

//    private async Task<IEnumerable<Server>> FetchServersAsync(string url, CancellationToken cancellationToken = default)
//    {
//        var requestingUrl = $"{_vpnApiOptions.Value.BaseUrl}{url}";
//        _logger.LogInformation("Sending request to NordVpn API: {url}", requestingUrl);

//        var response = await _httpClient.GetAsync(requestingUrl, cancellationToken);
//        response.EnsureSuccessStatusCode();

//        var contetn = await response.Content.ReadAsStringAsync();
//        var servers = JsonConvert.DeserializeObject<IEnumerable<Server>>(contetn);

//        if (servers is null || !servers.Any())
//        {
//            _logger.LogWarning("No servers returned from API of Url: {url}", url);
//            return [];
//        }

//        _logger.LogInformation("Successfully fetcthed {Count} servers from API", servers.Count());
//        return servers;
//    }
//}