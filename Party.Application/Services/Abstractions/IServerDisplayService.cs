using Party.Application.DTOs;

namespace Party.Application.Services.Abstractions;

public interface IServerDisplayService
{
    Task DisplayServersAsync(IEnumerable<ServerDto> servers, CancellationToken cancellationToken = default);

    Task DisplayHelpAsync(CancellationToken cancellationToken = default);
}