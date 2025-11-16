using Party.Core.Enums;

namespace Party.Application.DTOs;

public class ServerListQueryDto
{
    public bool UseLocalData { get; set; }
    public CountryId? CountryId { get; set; }
    public VpnProtocol? VpnProtocol { get; set; }

    public ServerListQueryDto()
    {
    }

    public ServerListQueryDto(
        bool useLocalData,
        CountryId? countryId = null,
        VpnProtocol? vpnProtocol = null)
    {
        UseLocalData = useLocalData;
        CountryId = countryId;
        VpnProtocol = vpnProtocol;
    }
}
