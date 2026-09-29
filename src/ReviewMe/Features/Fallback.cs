namespace ReviewMe.Features;

public sealed class Fallback : IFeature
{
    public bool Accepts(string[] args) => true;

    public async Task<int> RunAsync(string[] args)
    {
        Console.WriteLine("Unknown command.");
        Console.WriteLine();

        var innerFeature = new PrintUsage();
        await innerFeature.RunAsync(args);

        return ExitCodes.UnknownArguments;
    }
}
