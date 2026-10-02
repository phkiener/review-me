using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax.Inlines;
using ReviewMe.Review;
using Spectre.Console;

namespace ReviewMe.Output;

/// <summary>
/// An <see cref="IOutputWriter"/> that renders the suggestions to console.
/// </summary>
public sealed class ConsoleOutputWriter : IOutputWriter
{
    /// <inheritdoc />
    public async Task WriteOutputAsync(IAsyncEnumerable<ReviewSuggestion> suggestions, CancellationToken cancellationToken)
    {
        await foreach (var suggestion in suggestions.WithCancellation(cancellationToken))
        {
            var fileContent = await File.ReadAllLinesAsync(suggestion.FilePath, cancellationToken);
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
