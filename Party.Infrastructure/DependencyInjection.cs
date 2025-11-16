using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Party.Core.Repositories.Abstractions;
using Party.Core.Services.Abstractions;
using Party.Infrastructure.Configurations;
using Party.Infrastructure.Persistance;
using Party.Infrastructure.Persistance.Repositories;
using Party.Infrastructure.Services;

namespace Party.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.SectionName));

        var databaseOptions = configuration
            .GetSection(DatabaseOptions.SectionName)
            .Get<DatabaseOptions>();

        if (databaseOptions is not null)
            services.AddDbContext<PartyDbContext>(options =>
                options.UseSqlite(databaseOptions.ConnectionString));

        services.AddScoped<IServerRepository, ServerRepository>();
        services.AddScoped<IConfigurationRepository, ConfigurationRepository>();


        services.AddSingleton<IDisplayService, ConsoleDisplayService>();

        return services;
    }
}
