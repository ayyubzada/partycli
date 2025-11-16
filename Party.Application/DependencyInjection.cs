using Microsoft.Extensions.DependencyInjection;
using Party.Application.Services;
using Party.Application.Services.Abstractions;

namespace Party.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IServerListService, ServerListService>();
        services.AddScoped<IServerDisplayService, ServerDisplayService>();

        return services;
    }
}
