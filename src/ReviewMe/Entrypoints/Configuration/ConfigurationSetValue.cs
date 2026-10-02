using System.Text.Json;
using System.Text.Json.Nodes;
using ReviewMe.Config;

namespace ReviewMe.Entrypoints.Configuration;

/// <summary>
/// Set the current value of a configuration setting.
/// </summary>
public sealed class ConfigurationSetValue : IEntrypoint
{
    private static readonly JsonSerializerOptions serializerOptions = new() { WriteIndented = true };

    /// <inheritdoc />
    public bool Accepts(string[] args) => args is ["config", var key, _] && !key.StartsWith('-');

    /// <inheritdoc />
    public async Task<int> RunAsync(string[] args)
    {
        var key = args[1];
        Action<JsonNode, string> update = key switch
        {
            nameof(Config.Configuration.ConnectionString) => UpdateConnectionString,
            _ => throw new InvalidOperationException($"Unknown configuration key {key}")
        };

        var document = await ReadConfigAsync();

        var value = args[2];
        update(document, value);

        await SaveConfigAsync(document);

        return ExitCodes.Success;
    }

    private static void UpdateConnectionString(JsonNode document, string value)
    {
        if (!ModelConnectionString.TryParse(value, out _))
        {
            throw new InvalidOperationException("Not a valid connection string");
        }

        document[nameof(Config.Configuration.ConnectionString)] = value;
    }

    private static async Task<JsonNode> ReadConfigAsync()
    {
        if (File.Exists(Config.Configuration.Location))
        {
            var content = await File.ReadAllTextAsync(Config.Configuration.Location);
            var document = JsonNode.Parse(content);

            return document ?? new JsonObject();
        }

        return new JsonObject();
    }

    private static async Task SaveConfigAsync(JsonNode document)
    {
        if (!File.Exists(Config.Configuration.Location))
        {
            var directory = Directory.GetParent(Config.Configuration.Location);
            Directory.CreateDirectory(directory!.FullName);
        }

        var configContent = document.ToJsonString(serializerOptions);
        await File.WriteAllTextAsync(Config.Configuration.Location, configContent);
    }
}
