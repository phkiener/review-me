using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax.Inlines;
using Microsoft.Extensions.DependencyInjection;
using ReviewMe.Review;
using Spectre.Console;

namespace ReviewMe;

public sealed class ConsoleHost(IServiceProvider serviceProvider)
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

                    var fileContent = await File.ReadAllLinesAsync(request.FilePath);
                    using var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(90));
                    var suggestions = reviewProvider.GenerateReviewAsync(request, cancellation.Token);

                    await foreach (var suggestion in suggestions)
                    {
                        AnsiConsole.Write(new Rule(suggestion.FilePath) { Style = Style.Parse("Gray")});
                        for (var offset = -2; offset <= 2; offset++)
                        {
                            var line = fileContent.ElementAtOrDefault(suggestion.LineNumber + offset - 1);
                            if (line is not null)
                            {
                                AnsiConsole.MarkupLineInterpolated($"[Gray]{suggestion.LineNumber + offset:0000}|[/] [Blue]{line}[/]");
                            }
                        }
                        AnsiConsole.Write(new Rule(suggestion.FilePath) { Style = Style.Parse("Gray")});
                        AnsiConsole.WriteLine();

                        var writer = new StringWriter();
                        var renderer = new MarkdownRenderer(writer);
                        Markdown.Convert(suggestion.Content, renderer);

                        AnsiConsole.MarkupLine(writer.ToString());
                        AnsiConsole.WriteLine();

                        var colorForCategory = suggestion.Category switch
                        {
                            SuggestionCategory.Nitpick => "Cyan",
                            SuggestionCategory.Suggestion => "Green",
                            SuggestionCategory.Issue => "Orange1",
                            _ => "White"
                        };

                        AnsiConsole.MarkupLineInterpolated($"Severity: [{colorForCategory}]{suggestion.Category}[/]");
                    }
                }
            });
    }

    private sealed class MarkdownRenderer : HtmlRenderer
    {
        public MarkdownRenderer(TextWriter writer) : base(writer)
        {
            EnableHtmlEscape = false;
            EnableHtmlForBlock = false;
            EnableHtmlForInline = false;

            ObjectRenderers.RemoveAll(s => s is HtmlObjectRenderer<CodeInline>);
            ObjectRenderers.Add(new SpectreConsoleRenderer());
        }

        private sealed class SpectreConsoleRenderer : HtmlObjectRenderer<CodeInline>
        {
            protected override void Write(HtmlRenderer renderer, CodeInline obj)
            {
                renderer.Write($"[Blue]{AnsiMarkup.Escape(obj.Content)}[/]");
            }
        }
    }
}
