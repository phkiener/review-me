namespace ReviewMe.Features;

/// <summary>
/// Print version information of the compiled binary.
/// </summary>
public sealed class PrintVersion : IEntrypoint
{
    /// <inheritdoc/>
    public bool Accepts(string[] args) => args is ["-v"] or ["--version"];

    /// <inheritdoc/>
    public Task<int> RunAsync(string[] args)
    {
        Console.WriteLine("Ultra secret preview version v0.00");
        return Task.FromResult(ExitCodes.Success);
    }
}
