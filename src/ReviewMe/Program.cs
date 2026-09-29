using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ReviewMe;
using ReviewMe.Features;
using ReviewMe.ReviewProviders;

var configuration = new ConfigurationBuilder()
    .AddJsonFile(Configuration.Location, optional: true)
    .AddEnvironmentVariables(prefix: "REVIEWME_")
    .Build();

await using var serviceProvider = new ServiceCollection()
    .AddLogging(static b => b.AddConsole())
    .AddSingleton<IConfiguration>(configuration)
    .AddSingleton<IEntrypoint, PrintUsage>()
    .AddSingleton<IEntrypoint, PrintVersion>()
    .AddSingleton<IEntrypoint, ReviewUncommitted>()
    .AddSingleton<IEntrypoint, ReviewDiff>()
    .AddSingleton<IEntrypoint, ReviewFile>()
    .AddSingleton<IEntrypoint, ReviewAutomatic>()
    .AddSingleton<IEntrypoint, Fallback>()
    .AddTransient(ReviewProviderFactory.Create)
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
