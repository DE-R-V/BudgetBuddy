using System.Text.Json;

public class ConfigService
{
    // Name of the configuration file used by the app.
    private const string FileName = "config.json";

    // Full path to the writable copy of the configuration file.
    private readonly string _configPath;

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    public ConfigService()
    {
        _configPath = Path.Combine(
            FileSystem.AppDataDirectory,
            FileName);
    }

    public async Task<AppConfig> LoadAsync()
    {
        // The config.json included in the application package
        // is read-only. On the first run, create a writable copy
        // inside AppDataDirectory.
        if (!File.Exists(_configPath))
        {
            await CopyDefaultConfigAsync();
        }

        // Read the writable configuration file.
        var json = await File.ReadAllTextAsync(_configPath);

        // Convert the JSON into our AppConfig model.
        // If deserialization somehow returns null,
        // return an empty configuration instead.
        return JsonSerializer.Deserialize<AppConfig>(
                   json,
                   _jsonOptions)
               ?? new AppConfig();
    }

    public async Task SaveAsync(AppConfig config)
    {
        // Convert the configuration model back to JSON.
        var json = JsonSerializer.Serialize(
            config,
            _jsonOptions);

        // Save it to the writable application directory.
        await File.WriteAllTextAsync(
            _configPath,
            json);
    }

    private async Task CopyDefaultConfigAsync()
    {
        // Open the default config.json bundled with the app.
        await using var source =
            await FileSystem.OpenAppPackageFileAsync(FileName);

        // Create the writable copy inside AppDataDirectory.
        await using var destination =
            File.Create(_configPath);

        // Copy the bundled configuration into the writable file.
        await source.CopyToAsync(destination);
    }
}