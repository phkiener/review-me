namespace ReviewMe.Features;

/// <summary>
/// Reviews a specific file.
/// </summary>
public sealed class ReviewFile(ConsoleHost host) : IEntrypoint
{
    /// <inheritdoc/>
    public bool Accepts(string[] args) => args is ["--file", _];

    /// <inheritdoc/>
    public async Task<int> RunAsync(string[] args)
    {
        var filePath = args[1];

        if (!File.Exists(filePath))
        {
            return ExitCodes.Error;
        }

        var content = await File.ReadAllTextAsync(args[1]);
        await host.ReviewAsync(content);

        return ExitCodes.Success;
    }
}
