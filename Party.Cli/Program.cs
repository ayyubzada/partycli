using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Party.Cli.Models;
using Party.Cli.Persistance;
using Party.Cli.Persistance.Repositories;
using Party.Cli.Services;
using Serilog;

var configBuilder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

var configuration = configBuilder.Build();

var services = new ServiceCollection();

services.Configure<DatabaseOptions>(configuration.GetSection("Database"));

var dbOptions = configuration.GetSection("Database").Get<DatabaseOptions>()!;

services.AddDbContext<PartyDbContext>(options =>
{
    options.UseSqlite(dbOptions.ConnectionString);
});

services.AddScoped<Repository>();
services.AddScoped<NordVpnService>();
services.AddScoped<PartyService>();
services.AddScoped<DisplayService>();

Log.Logger = new LoggerConfiguration()
       .ReadFrom.Configuration(configuration)
       .CreateLogger();

services.AddLogging(builder =>
{
    builder.ClearProviders();
    builder.AddSerilog();
});

var provider = services.BuildServiceProvider();

using (var scope = provider.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PartyDbContext>();
    db.Database.Migrate();
}

var partyService = provider.GetRequiredService<PartyService>();

await partyService.HandleRequest(args);
