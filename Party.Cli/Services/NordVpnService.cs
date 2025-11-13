namespace Party.Cli.Services;

internal class NordVpnService
{
    private static readonly HttpClient client = new HttpClient();

    internal async Task<string> GetAllServersListAsync()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "https://api.nordvpn.com/v1/servers");
        var response = await client.SendAsync(request);
        var responseString = await response.Content.ReadAsStringAsync();
        return responseString;
    }

    internal async Task<string> GetAllServerByCountryListAsync(int countryId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "https://api.nordvpn.com/v1/servers?filters[servers_technologies][id]=35&filters[country_id]=" + countryId);
        var response = await client.SendAsync(request);
        var responseString = await response.Content.ReadAsStringAsync();
        return responseString;
    }

    internal async Task<string> GetAllServerByProtocolListAsync(int vpnProtocol)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "https://api.nordvpn.com/v1/servers?filters[servers_technologies][id]=" + vpnProtocol);
        var response = await client.SendAsync(request);
        var responseString = await response.Content.ReadAsStringAsync();
        return responseString;
    }

}
