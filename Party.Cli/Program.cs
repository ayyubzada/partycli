using Newtonsoft.Json;
using Party.Cli.Models;
using Party.Cli.Services;

class Program
{
    private static readonly NordVpnService _nordVpnService = new NordVpnService();
    private static readonly SettingsService _settingsService = new SettingsService();

    static async Task Main(string[] args)
    {
        var currentState = States.none;
        string name = null;
        int argIndex = 1;

        foreach (string arg in args)
        {
            if (currentState == States.none)
            {
                if (arg == "server_list")
                {
                    currentState = States.server_list;
                    if (argIndex >= args.Count())
                    {
                        var serverList = await _nordVpnService.GetAllServersListAsync();
                        _settingsService.StoreValue("serverlist", serverList, false);
                        log("Saved new server list: " + serverList);
                        DisplayList(serverList);
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
                    _settingsService.StoreValue(proccessName(name), arg);
                    log("Changed " + proccessName(name) + " to " + arg);
                    name = null;
                }
            }
            else if (currentState == States.server_list)
            {
                if (arg == "--local")
                {
                    var data = _settingsService.GetValue(SettingsService.ServerListSettingKey);
                    if (!string.IsNullOrEmpty(data))
                        DisplayList(data);
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
                    _settingsService.StoreValue("serverlist", serverList, false);
                    log("Saved new server list: " + serverList);
                    DisplayList(serverList);
                }
                else if (arg == "--TCP")
                {
                    //UDP = 3
                    //Tcp = 5
                    //Nordlynx = 35
                    var query = new VpnServerQuery(5, null, null, null, null, null);
                    var serverList = await _nordVpnService.GetAllServerByProtocolListAsync((int)query.Protocol.Value);
                    _settingsService.StoreValue("serverlist", serverList, false);
                    log("Saved new server list: " + serverList);
                    DisplayList(serverList);
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

    static void DisplayList(string serverListString)
    {
        var serverlist = JsonConvert.DeserializeObject<List<ServerModel>>(serverListString);
        Console.WriteLine("Server list: ");
        for (var index = 0; index < serverlist.Count; index++)
        {
            Console.WriteLine("Name: " + serverlist[index].Name);
        }
        Console.WriteLine("Total servers: " + serverlist.Count);
    }

    static void log(string action)
    {
        var newLog = new LogModel
        {
            Action = action,
            Time = DateTime.Now
        };

        List<LogModel> currentLog;
        string logData = _settingsService.GetValue(SettingsService.ServerListLogKey);

        if (!string.IsNullOrEmpty(logData))
        {
            currentLog = JsonConvert.DeserializeObject<List<LogModel>>(logData);
            currentLog.Add(newLog);
        }
        else
            currentLog = new List<LogModel> { newLog };

        _settingsService.StoreValue("log", JsonConvert.SerializeObject(currentLog), false);
    }
}


