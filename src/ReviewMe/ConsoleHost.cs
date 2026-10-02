using Microsoft.Extensions.DependencyInjection;
using ReviewMe.Review;
using Spectre.Console;

namespace ReviewMe;

public sealed class ConsoleHost(IServiceProvider serviceProvider, IOutputWriter outputWriter)
{
    public async Task ReviewAsync(IReadOnlyList<ReviewRequest> requests)
    {
        using var reviewProvider = serviceProvider.GetRequiredService<IReviewProvider>();

        await AnsiConsole.Status()
            .Spinner(Spinner.Known.BouncingBar)
            .StartAsync("Running review", async ctx =>
            {
                for (var index = 0; index < requests.Count; index++)
                {
                    var request = requests[index];
                    ctx.Status($"Reviewing {request.FilePath} ({index + 1}/{requests.Count})");

                    using var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(90));
                    var suggestions = reviewProvider.GenerateReviewAsync(request, cancellation.Token);

                    await outputWriter.WriteOutputAsync(suggestions, CancellationToken.None);
                }
            });
    }
}
