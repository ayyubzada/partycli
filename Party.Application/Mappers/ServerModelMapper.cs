using Party.Application.DTOs;
using Party.Core.Entities;

namespace Party.Application.Mappers;

public static class ServerModelMapper
{
    public static ServerDto ToDtoModel(this Server server) =>
         new ServerDto(server.Name, server.Load, server.Status, []);

    public static Server ToEntityModel(this ServerDto serverDto) =>
        new Server
        {
            Name = serverDto.Name,
            Load = serverDto.Load,
            Status = serverDto.Status,
            CountryId = serverDto.Locations?.FirstOrDefault()?.Country?.Id ?? 0
        };

    public static IEnumerable<ServerDto> ToDtoModels(this IEnumerable<Server> servers) =>
        servers.Select(s => s.ToDtoModel());

    public static IEnumerable<Server> ToEntityModels(this IEnumerable<ServerDto> serverDtos) =>
        serverDtos.Select(s => s.ToEntityModel());
}
