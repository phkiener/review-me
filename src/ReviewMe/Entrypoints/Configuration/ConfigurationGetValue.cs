namespace ReviewMe.Entrypoints.Configuration;

/// <summary>
/// Get the current value of a configuration setting.
/// </summary>
public sealed class ConfigurationGetValue(Config.Configuration configuration) : IEntrypoint
{
    /// <inheritdoc />
    public bool Accepts(string[] args) => args is ["config", var key] && !key.StartsWith('-');

    /// <inheritdoc />
    public Task<int> RunAsync(string[] args)
    {
        var key = args[1];
        var value = key switch
        {
            nameof(Config.Configuration.ConnectionString) => configuration.ConnectionString,
            _ => throw new InvalidOperationException($"Unknown configuration key {key}")
        };

        Console.WriteLine(value);

        return Task.FromResult(ExitCodes.Success);
    }
}
