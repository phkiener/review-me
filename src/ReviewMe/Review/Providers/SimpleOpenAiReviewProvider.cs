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
    private static readonly JsonSerializerOptions serializerOptions = new(JsonSerializerDefaults.Web) { Converters = { new JsonStringEnumConverter<SuggestionCategory>() }};
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
    public async IAsyncEnumerable<ReviewSuggestion> GenerateReviewAsync(ReviewRequest request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var (chatHistory, chatOptions) = PrepareReview(request.Content, request.IsDiff);
        var response = await chatClient.GetResponseAsync(chatHistory, chatOptions, cancellationToken);

        var suggestions = ReadSuggestions(response);

        // TODO: Would be awesome to stream this. Later...
        foreach (var suggestion in suggestions)
        {
            yield return suggestion with { FilePath = request.FilePath };
        }
    }

    private static (List<ChatMessage> Messages, ChatOptions Options) PrepareReview(string content, bool isDiff)
    {
        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, """
                You are an autonomous code reviewer. Phrase your answers neutrally, addressing only the code.
                The user will send you a code snippet, either a full file or a diff. Review this snippet to the best of your abilities.
                Focus on things a human reviewer tends to forget: standards adherence, undefined behaviour, edge cases.
                If the issue can be detected by a standard toolchain (e.g. package version mismatches, missing import statements), skip the issue
                entirely.
                Keep the suggestions short and to the point. Do not be overly polite, assume that the user is very proficient and knowledgeable.
                When reviewing code, always attach your comments to a specific line of code.
                """),
            new(ChatRole.User, $"""
                Review this {(isDiff ? "diff" : "file")}:
                ```
                {content}
                ```
                """)
        };

        var chatOptions = new ChatOptions
        {
            Reasoning = new ReasoningOptions { Effort = ReasoningEffort.None },
            ResponseFormat = ChatResponseFormat.ForJsonSchema<ReviewSuggestion[]>(serializerOptions),
            Temperature = 0.25f,
            TopP = 0.1f
        };

        return (messages, chatOptions);
    }

    private static IEnumerable<ReviewSuggestion> ReadSuggestions(ChatResponse response)
    {
        if (response.FinishReason != ChatFinishReason.Stop)
        {
            throw new InvalidOperationException($"Model finished with unexpected finish reason {response.FinishReason}");
        }

        return JsonSerializer.Deserialize<ReviewSuggestion[]>(response.Text, serializerOptions) ?? [];
    }

    public void Dispose()
    {
        chatClient.Dispose();
    }
}
