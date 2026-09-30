using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ReviewMe;
using ReviewMe.Config;
using ReviewMe.Features;
using ReviewMe.Review;
using ReviewMe.Review.Providers;

await using var serviceProvider = new ServiceCollection()
    .AddLogging(static b => b.AddConsole()) // TODO: When proper output is built, this logger might interfere with the output itself...
    .AddConfiguration()
    .AddEntrypoints()
    .AddReviewProviders()
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
