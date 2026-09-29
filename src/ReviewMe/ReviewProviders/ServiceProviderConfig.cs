using Microsoft.Extensions.DependencyInjection;
using ReviewMe.Config;

namespace ReviewMe.ReviewProviders;

/// <summary>
/// DI registration for a <see cref="IReviewProvider"/>s.
/// </summary>
public static class ServiceProviderConfig
{
    /// <summary>
    /// Add the <see cref="IReviewProvider"/> to the given <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The given <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddReviewProvider(this IServiceCollection services)
    {
        services.AddTransient(CreateReviewProvider);

        return services;
    }

    private static IReviewProvider CreateReviewProvider(IServiceProvider serviceProvider)
    {
        var configuration = serviceProvider.GetRequiredService<Configuration>();
        return configuration.ConnectionString?.Provider switch
        {
            null => throw new InvalidOperationException("No connection string specified."),
            "OpenAI" => new SimpleOpenAiReviewProvider(configuration.ConnectionString),
            _ => throw new InvalidOperationException($"Unsupported provider {configuration.ConnectionString.Provider}"),
        };
    }
}
