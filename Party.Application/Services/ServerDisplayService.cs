using Party.Application.DTOs;
using Party.Application.Resources;
using Party.Application.Services.Abstractions;
using Party.Core.Services.Abstractions;

namespace Party.Application.Services;

public class ServerDisplayService(
    IDisplayService displayService) : IServerDisplayService
{
    private readonly IDisplayService _displayService = displayService;

    public async Task DisplayServersAsync(
        IEnumerable<ServerDto> servers,
        CancellationToken cancellationToken = default)
    {
        if (servers is null || !servers.Any())
        {
            await _displayService.DisplayWarningAsync(AppLevelResource.NoServersProvided, cancellationToken);
            return;
        }

        await _displayService.DisplayMessageAsync(AppLevelResource.ServerListing, cancellationToken: cancellationToken);
        await _displayService.DisplaSeperatorAsync(cancellationToken);

        foreach (var server in servers)
        {
            var serverInfo = string.Format(AppLevelResource.ServerInfoModel, server.Name, server.Load, server.Status);
            await _displayService.DisplayMessageAsync(serverInfo, cancellationToken: cancellationToken);
        }

        await _displayService.DisplaSeperatorAsync(cancellationToken);
        await _displayService.DisplaySuccessAsync(
            string.Format(AppLevelResource.TotalServers, servers.Count()),
            cancellationToken);
    }

    public async Task DisplayHelpAsync(CancellationToken cancellationToken = default)
    {
        await _displayService.DisplayMessageAsync(AppLevelResource.ConsoleAppTitle, cancellationToken: cancellationToken);
        await _displayService.DisplaSeperatorAsync(cancellationToken);
        //TODO: Add detailed info
    }
}