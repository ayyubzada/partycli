using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Party.Application;
using Party.Cli.Commands;
using Party.Infrastructure;
using Party.Infrastructure.Persistance;
using Serilog;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

Log.Logger = new LoggerConfiguration()
       .ReadFrom.Configuration(configuration)
       .CreateLogger();

try
{
    Log.Information("Starting Party CLI application");

    var host = Host.CreateDefaultBuilder(args)
        .ConfigureServices((context, services) =>
        {
            services.AddInfrastructureServices(configuration);
            services.AddApplicationServices();

            services.AddScoped<CommandHandler>();
        })
        .ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddSerilog();
        })
        .Build();

    using (var scope = host.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<PartyDbContext>();
        await db.Database.MigrateAsync();
        Log.Information("Database migration completed successfully");
    }

    using (var scope = host.Services.CreateScope())
    {
        var commandHandler = scope.ServiceProvider.GetRequiredService<CommandHandler>();
        await commandHandler.HandleAsync(args);
    }
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application stopped unexpectedly");
    throw;
}
finally
{
    await Log.CloseAndFlushAsync();
}