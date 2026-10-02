namespace ReviewMe.Entrypoints.Configuration;

/// <summary>
/// List all supported configuration keys.
/// </summary>
public sealed class ConfigurationListKeys : IEntrypoint
{
    /// <inheritdoc />
    public bool Accepts(string[] args) => args is ["config"] or ["config", "--list-keys"];

    /// <inheritdoc />
    public Task<int> RunAsync(string[] args)
    {
        Console.WriteLine(nameof(Config.Configuration.ConnectionString));

        return Task.FromResult(ExitCodes.Success);
    }
}
