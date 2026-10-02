using System.ClientModel;
using System.ClientModel.Primitives;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Chat;
using ReviewMe.Config;
using ChatFinishReason = Microsoft.Extensions.AI.ChatFinishReason;
using ChatMessage = Microsoft.Extensions.AI.ChatMessage;
using ChatResponseFormat = Microsoft.Extensions.AI.ChatResponseFormat;

namespace ReviewMe.Review.Providers;

/// <summary>
/// An <see cref="IReviewProvider"/> based on an OpenAI compatible endpoint.
/// </summary>
public sealed class SimpleOpenAiReviewProvider : IReviewProvider
{
    private const string MainSystemPrompt = """
        You are an autonomous code reviewer. Phrase your answers neutrally, addressing only the code.
        Review the code to the best of your abilities. Focus on things a human reviewer tends to forget: standards adherence, undefined behaviour, edge cases.
        If an issue can be detected by a standard toolchain (e.g. package version mismatches, missing import statements), skip the issue entirely.
        If you need additional information for a specific issue, prefer to skip it entirely or assume the best-case scenario.
        Keep the suggestions short and to the point. Do not be overly polite, assume that the user is very proficient and knowledgeable.
        When reviewing code, always attach your comments to the relevant line of code.
        """;

    private static readonly JsonSerializerOptions serializerOptions = new(JsonSerializerDefaults.Web) { Converters = { new JsonStringEnumConverter<SuggestionCategory>() }};
    private static readonly ChatOptions chatOptions = new()
    {
        Reasoning = new ReasoningOptions { Effort = ReasoningEffort.Low },
        ResponseFormat = ChatResponseFormat.ForJsonSchema<GeneratedComment[]>(serializerOptions, schemaDescription: "Schema for a review comment for a specific line of code."),
        Temperature = 0.25f,
        TopP = 0.1f
    };

    private readonly IChatClient chatClient;

    /// <summary>
    /// Create a new instance of the <see cref="SimpleOpenAiReviewProvider"/>.
    /// </summary>
    /// <param name="connectionString">The connection string to use.</param>
    public SimpleOpenAiReviewProvider(ModelConnectionString connectionString)
    {
        var openAiClient = new ChatClient(
            model: connectionString.Model,
            credential: new ApiKeyCredential(connectionString.ApiKey ?? "not-used"),
            options: new OpenAIClientOptions
            {
                Endpoint = connectionString.Endpoint,
                RetryPolicy = new ClientRetryPolicy(maxRetries: 0)
            });

        chatClient = openAiClient.AsIChatClient();
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<ReviewSuggestion> GenerateReviewAsync(FileContent fileContent, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, MainSystemPrompt),
            new(ChatRole.System, "Review the full content of the file."),
            new(ChatRole.User, $"Review the file for `{fileContent.FilePath}` for me."),
            new(ChatRole.User, $"The full content is:\n```\n{fileContent.Content}\n```"),
        };

        var response = await chatClient.GetResponseAsync(messages, chatOptions, cancellationToken);
        var suggestions = ReadSuggestions(response);

        foreach (var suggestion in suggestions)
        {
            yield return new ReviewSuggestion(fileContent.FilePath, suggestion.LineNumber, suggestion.Category, suggestion.Comment);
        }
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<ReviewSuggestion> GenerateReviewAsync(FileDiff diff, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, MainSystemPrompt),
            new(ChatRole.System, "Mention issues only for lines changed in the diff. The full content is provided only for context, skip all issues found in lines that are not modified."),
            new(ChatRole.User, $"Review the diff for `{diff.FilePath}` for me."),
            new(ChatRole.User, $"The full content is:\n```\n{diff.UpdatedContent}\n```"),
            new(ChatRole.User, $"The changes are in the following diff:\n```\n{diff.Diff}\n```"),
        };

        var response = await chatClient.GetResponseAsync(messages, chatOptions, cancellationToken);
        var suggestions = ReadSuggestions(response);

        foreach (var suggestion in suggestions)
        {
            yield return new ReviewSuggestion(diff.FilePath, suggestion.LineNumber, suggestion.Category, suggestion.Comment);
        }
    }

    private static IEnumerable<GeneratedComment> ReadSuggestions(ChatResponse response)
    {
        if (response.FinishReason != ChatFinishReason.Stop)
        {
            throw new InvalidOperationException($"Model finished with unexpected finish reason {response.FinishReason}");
        }

        return JsonSerializer.Deserialize<GeneratedComment[]>(response.Text, serializerOptions) ?? [];
    }

    public void Dispose()
    {
        chatClient.Dispose();
    }

    private sealed record GeneratedComment(int LineNumber, string Comment, SuggestionCategory Category);
}
