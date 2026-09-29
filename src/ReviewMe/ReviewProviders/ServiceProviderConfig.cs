using Microsoft.Extensions.DependencyInjection;

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
        return new SimpleOpenAiReviewProvider();
    }
}
