using Newtonsoft.Json;
using Party.Cli.Models;

namespace Party.Cli.Services;

public class NordVpnService
{
    private static readonly HttpClient client = new HttpClient();

    public async Task<IEnumerable<ServerModel>?> GetAllServersListAsync()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "https://api.nordvpn.com/v1/servers");
        var response = await client.SendAsync(request);
        var responseString = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<IEnumerable<ServerModel>>(responseString);
    }

    public async Task<IEnumerable<ServerModel>?> GetAllServerByCountryListAsync(int countryId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "https://api.nordvpn.com/v1/servers?filters[servers_technologies][id]=35&filters[country_id]=" + countryId);
        var response = await client.SendAsync(request);
        var responseString = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<IEnumerable<ServerModel>>(responseString);
    }

    public async Task<IEnumerable<ServerModel>?> GetAllServerByProtocolListAsync(int vpnProtocol)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "https://api.nordvpn.com/v1/servers?filters[servers_technologies][id]=" + vpnProtocol);
        var response = await client.SendAsync(request);
        var responseString = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<IEnumerable<ServerModel>>(responseString);
    }

}
