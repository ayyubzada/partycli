using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Party.Cli.Models;
using Party.Cli.Persistance;
using Party.Cli.Persistance.Repositories;
using Party.Cli.Services;

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

var provider = services.BuildServiceProvider();

using (var scope = provider.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PartyDbContext>();
    db.Database.Migrate();
}

var partyService = provider.GetRequiredService<PartyService>();

await partyService.HandleRequest(args);
