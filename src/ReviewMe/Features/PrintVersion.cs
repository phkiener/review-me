namespace ReviewMe.Features;

public sealed class PrintVersion : IFeature
{
    public bool Accepts(string[] args) => args is ["-v"] or ["--version"];

    public Task<int> RunAsync(string[] args)
    {
        Console.WriteLine("Ultra secret preview version v0.00");
        return Task.FromResult(ExitCodes.Success);
    }
}
