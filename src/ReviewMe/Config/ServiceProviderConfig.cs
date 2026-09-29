using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ReviewMe.Config;

/// <summary>
/// DI registration for <see cref="IConfiguration"/>s.
/// </summary>
public static class ServiceProviderConfig
{
    /// <summary>
    /// Add the <see cref="IConfiguration"/> to the given <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The given <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddConfiguration(this IServiceCollection services)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile(Configuration.Location, optional: true)
            .AddEnvironmentVariables(prefix: "REVIEWME_")
            .Build();

        services.AddSingleton<IConfiguration>(configuration);
        services.AddSingleton<Configuration>();

        return services;
    }
}
