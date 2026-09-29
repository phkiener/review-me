using ReviewMe;
using ReviewMe.Features;

var features = new List<IFeature>
{
    new PrintUsage(),
    new PrintVersion(),
    new Fallback()
};

var matchingFeature = features.First(f => f.Accepts(args));
return await matchingFeature.RunAsync(args);
