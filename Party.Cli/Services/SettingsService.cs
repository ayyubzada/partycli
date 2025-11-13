namespace Party.Cli.Services;

internal class SettingsService
{
    internal const string ServerListSettingKey = "serverlist";
    internal const string ServerListLogKey = "log";

    private Dictionary<string, string> _settingsCache = new();

    public SettingsService()
    {
        LoadSettingsToCache();
    }

    internal void StoreValue(string name, string value, bool writeToConsole = true)
    {
        try
        {
            _settingsCache[name] = value;
            Save();
            if (writeToConsole)
                Console.WriteLine("Changed " + name + " to " + value);
        }
        catch
        {
            Console.WriteLine("Error: Couldn't save " + name + ". Check if command was input correctly.");
        }

    }

    internal string? GetValue(string name)
    {
        if (_settingsCache.ContainsKey(name))
            return _settingsCache[name];
        return null;
    }

    private void LoadSettingsToCache()
    {
        try
        {
            // TODO: load settings from persistent storage
        }
        catch (Exception)
        {
            _settingsCache = new Dictionary<string, string>();
        }
    }

    private void Save()
    {
        //TODO: save settings to persistent storage
    }

}