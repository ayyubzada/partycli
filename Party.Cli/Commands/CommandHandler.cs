using Microsoft.Extensions.Logging;
using Party.Application.DTOs;
using Party.Application.Services.Abstractions;
using Party.Core.Enums;

namespace Party.Cli.Commands;

internal class CommandHandler(
    ILogger<CommandHandler> logger,
    IServerListService serverListService,
    IServerDisplayService serverDisplayService)
{
    private readonly ILogger<CommandHandler> _logger = logger;
    private readonly IServerListService _serverListService = serverListService;
    private readonly IServerDisplayService _serverDisplayService = serverDisplayService;

    public async Task HandleAsync(string[] args, CancellationToken cancellationToken = default)
    {
        try
        {
            if (args is null || args.Length == 0)
            {
                await _serverDisplayService.DisplayHelpAsync(cancellationToken);
                return;
            }

            var command = args[0].ToLowerInvariant();

            switch (command)
            {
                case "server_list":
                    await HandleServerListCommandAsync(args, cancellationToken);
                    break;
                case "config":
                    await _serverDisplayService.DisplayHelpAsync(cancellationToken);
                    break;
                default:
                    await _serverDisplayService.DisplayHelpAsync(cancellationToken);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while handling ListServers command");
        }
    }

    private async Task HandleServerListCommandAsync(string[] args, CancellationToken cancellationToken)
    {
        var query = new ServerListQueryDto();

        for (int i = 1; i < args.Length; i++)
        {
            var arg = args[i].ToLowerInvariant();
            var trimmedArg = arg.Trim('-');

            switch (arg)
            {
                case "--help":
                case "-h":
                    await _serverDisplayService.DisplayHelpAsync(cancellationToken);
                    return;
            }

            switch (GetParameterType(arg, trimmedArg))
            {
                case ParameterType.LocalData:
                    query.UseLocalData = true;
                    break;
                case ParameterType.Country:
                    query.CountryId = Enum.Parse<CountryId>(trimmedArg, true);
                    break;
                case ParameterType.Protocol:
                    query.VpnProtocol = Enum.Parse<VpnProtocol>(trimmedArg, true);
                    break;
                case ParameterType.Help:
                case ParameterType.Unknown:
                default:
                    await _serverDisplayService.DisplayHelpAsync(cancellationToken);
                    return;
            }
        }

        await _serverListService.GetAndDisplayServersAsync(query, cancellationToken);
    }

    private ParameterType GetParameterType(string normalizedParam, string trimmedParam)
    {
        if (string.IsNullOrWhiteSpace(normalizedParam))
            return ParameterType.Unknown;

        return normalizedParam switch
        {
            "-h" or "--help" => ParameterType.Help,
            "--local" => ParameterType.LocalData,
            _ => Enum.TryParse<CountryId>(trimmedParam, true, out _) ? ParameterType.Country :
                Enum.TryParse<VpnProtocol>(trimmedParam, true, out _) ? ParameterType.Protocol :
                ParameterType.Unknown
        };
    }
}


public enum ParameterType
{
    Unknown,
    LocalData,
    Country,
    Protocol,
    Help
}
