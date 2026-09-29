namespace ReviewMe.Features;

public sealed class ReviewFile : IFeature
{
    public bool Accepts(string[] args) => args is ["--file", _];

    public async Task<int> RunAsync(string[] args)
    {
        var filePath = args[1];

        if (!File.Exists(filePath))
        {
            return ExitCodes.Error;
        }

        var content = await File.ReadAllTextAsync(args[1]);
        _ = content;

        return ExitCodes.Success;
    }
}
