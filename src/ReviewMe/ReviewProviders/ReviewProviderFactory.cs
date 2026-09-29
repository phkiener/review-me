namespace ReviewMe.ReviewProviders;

/// <summary>
/// A helper that can be used to dynamically create a <see cref="IReviewProvider"/>, based on the configuration.
/// </summary>
public static class ReviewProviderFactory
{
    /// <summary>
    /// Create a new <see cref="IReviewProvider"/>.
    /// </summary>
    /// <param name="serviceProvider">The <see cref="IServiceProvider"/> to use to retrieve any required dependencies.</param>
    /// <returns>A newly built <see cref="IReviewProvider"/>.</returns>
    public static IReviewProvider Create(IServiceProvider serviceProvider)
    {
        return new SimpleOpenAiReviewProvider();
    }
}
