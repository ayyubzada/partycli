using Party.Cli.Models;
using Party.Cli.Persistance.Repositories;

namespace Party.Cli.Services;

public class PartyService(
    NordVpnService nordVpnService,
    Repository repository)
{
    private readonly NordVpnService _nordVpnService = nordVpnService;
    private readonly Repository _repository = repository;

    public async Task HandleRequest(string[] parameters)
    {
        var currentState = States.none;
        string name = null;
        int argIndex = 1;

        foreach (string arg in parameters)
        {
            if (currentState == States.none)
            {
                if (arg == "server_list")
                {
                    currentState = States.server_list;
                    if (argIndex >= parameters.Length)
                    {
                        var serverList = await _nordVpnService.GetAllServersListAsync();
                        await _repository.SaveServersAsync(serverList);
                        LogAsync("Saved new server list: " + serverList);
                        DisplayList(serverList.ToList());
                    }
                }
                if (arg == "config")
                {
                    currentState = States.config;
                }
            }
            else if (currentState == States.config)
            {
                if (name == null)
                {
                    name = arg;
                }
                else
                {
                    var config = new ConfigurationModel()
                    {
                        Key = proccessName(name),
                        Value = arg
                    };
                    await _repository.UpsertConfigurationAsync(config);
                    LogAsync("Changed " + proccessName(name) + " to " + arg);
                    name = null;
                }
            }
            else if (currentState == States.server_list)
            {
                if (arg == "--local")
                {
                    var servers = await _repository.GetServersAsync();
                    if (servers.Any())
                        DisplayList(servers.ToList());
                    else
                        Console.WriteLine("Error: There are no server data in local storage");
                }
                else if (arg == "--france")
                {
                    //france == 74
                    //albania == 2
                    //Argentina == 10
                    var query = new VpnServerQuery(null, 74, null, null, null, null);
                    var serverList = await _nordVpnService.GetAllServerByCountryListAsync(query.CountryId.Value); //France id == 74
                    await _repository.SaveServersAsync(serverList);
                    LogAsync("Saved new server list: " + serverList);
                    DisplayList(serverList.ToList());
                }
                else if (arg == "--TCP")
                {
                    //UDP = 3
                    //Tcp = 5
                    //Nordlynx = 35
                    var query = new VpnServerQuery(5, null, null, null, null, null);
                    var serverList = await _nordVpnService.GetAllServerByProtocolListAsync((int)query.Protocol.Value);
                    await _repository.SaveServersAsync(serverList);
                    LogAsync("Saved new server list: " + serverList);
                    DisplayList(serverList.ToList());
                }
            }
            argIndex = argIndex + 1;
        }

        if (currentState == States.none)
        {
            Console.WriteLine("To get and save all servers, use command: partycli.exe server_list");
            Console.WriteLine("To get and save France servers, use command: partycli.exe server_list --france");
            Console.WriteLine("To get and save servers that support TCP protocol, use command: partycli.exe server_list --TCP");
            Console.WriteLine("To see saved list of servers, use command: partycli.exe server_list --local ");
        }
        Console.Read();
    }

    static string proccessName(string name)
    {
        name = name.Replace("-", string.Empty);
        return name;
    }

    void DisplayList(List<ServerModel> serverlist)
    {
        Console.WriteLine("Server list: ");

        for (var index = 0; index < serverlist.Count(); index++)
            Console.WriteLine("Name: " + serverlist[index].Name);

        Console.WriteLine("Total servers: " + serverlist.Count);
    }

    async Task LogAsync(string action)
    {
        var newLog = new LogModel
        {
            Action = action,
            Time = DateTime.UtcNow
        };

        await _repository.AddLogAsync(newLog);
    }
}
