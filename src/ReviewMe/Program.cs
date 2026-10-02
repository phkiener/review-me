using Microsoft.Extensions.DependencyInjection;
using ReviewMe;
using ReviewMe.Config;
using ReviewMe.Features;
using ReviewMe.Output;
using ReviewMe.Review;

await using var serviceProvider = new ServiceCollection()
    .AddConfiguration()
    .AddEntrypoints()
    .AddReviewProviders()
    .AddOutputWriters()
    .AddSingleton<ConsoleHost>()
    .BuildServiceProvider();

var features = serviceProvider.GetServices<IEntrypoint>();
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
