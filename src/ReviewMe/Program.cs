using ReviewMe;
using ReviewMe.Features;

var features = new List<IFeature>
{
    new PrintUsage(),
    new PrintVersion(),
    new ReviewUncommitted(),
    new ReviewDiff(),
    new ReviewFile(),
    new ReviewAutomatic(),
    new Fallback()
};

var matchingFeature = features.First(f => f.Accepts(args));

try
{
    return await matchingFeature.RunAsync(args);
}
catch (Exception e)
{
    Console.WriteLine(e.Message);

    return ExitCodes.Error;
}
