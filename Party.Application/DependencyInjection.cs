using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Party.Application.Configurations;
using Party.Application.Services;
using Party.Application.Services.Abstractions;
using Party.Core.Services.Abstractions;

namespace Party.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<VpnApiOptions>(configuration.GetSection(VpnApiOptions.SectionName));

        services.AddScoped<IServerListService, ServerListService>();
        services.AddScoped<IServerDisplayService, ServerDisplayService>();

        services.AddHttpClient<IVpnService, NordVpnService>();

        return services;
    }
}
