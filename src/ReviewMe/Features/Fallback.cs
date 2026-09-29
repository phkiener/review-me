namespace ReviewMe.Features;

/// <summary>
/// A fallback feature meant to handle the "no other feature matches"-case.
/// </summary>
public sealed class Fallback : IFeature
{
    /// <inheritdoc/>
    public bool Accepts(string[] args) => true;

    /// <inheritdoc/>
    public async Task<int> RunAsync(string[] args)
    {
        Console.WriteLine("Unknown command.");
        Console.WriteLine();

        var innerFeature = new PrintUsage();
        await innerFeature.RunAsync(args);

        return ExitCodes.UnknownArguments;
    }
}
