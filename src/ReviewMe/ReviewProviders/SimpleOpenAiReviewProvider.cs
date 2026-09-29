using System.ClientModel;
using System.ClientModel.Primitives;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Chat;
using ChatFinishReason = Microsoft.Extensions.AI.ChatFinishReason;
using ChatMessage = Microsoft.Extensions.AI.ChatMessage;
using ChatResponseFormat = Microsoft.Extensions.AI.ChatResponseFormat;

namespace ReviewMe.ReviewProviders;

/// <summary>
/// An <see cref="IReviewProvider"/> based on an OpenAI compatible endpoint.
/// </summary>
public sealed class SimpleOpenAiReviewProvider : IReviewProvider
{
    private static readonly JsonSerializerOptions serializerOptions = new(JsonSerializerDefaults.Web) { Converters = { new JsonStringEnumConverter<Category>() }};
    private readonly IChatClient chatClient;

    /// <summary>
    /// Create a new instance of the <see cref="SimpleOpenAiReviewProvider"/>.
    /// </summary>
    public SimpleOpenAiReviewProvider()
    {
        // TODO: Establish "connection string"-y thing
        var openAiClient = new ChatClient(
            model: "unsloth/Qwen3.6-35B-A3B-GGUF:Q4_K_M",
            credential: new ApiKeyCredential("not-needed"),
            options: new OpenAIClientOptions
            {
                Endpoint = new Uri("http://127.0.0.1:8080/v1/"),
                RetryPolicy = new ClientRetryPolicy(maxRetries: 0)
            });

        chatClient = openAiClient.AsIChatClient();
    }

    /// <inheritdoc />
    public event EventHandler<ProgressUpdatedEventArgs>? ProgressUpdated;

    /// <inheritdoc />
    public async IAsyncEnumerable<ReviewSuggestion> GenerateReviewAsync(string content, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        ProgressUpdated?.Invoke(this, new ProgressUpdatedEventArgs("Preparing review..."));
        var (chatHistory, chatOptions) = PrepareReview(content);

        ProgressUpdated?.Invoke(this, new ProgressUpdatedEventArgs("Running review..."));
        var response = await chatClient.GetResponseAsync(chatHistory, chatOptions, cancellationToken);

        ProgressUpdated?.Invoke(this, new ProgressUpdatedEventArgs("Collecting suggestions..."));
        var suggestions = ReadSuggestions(response);

        // TODO: Would be awesome to stream this. Later...
        foreach (var suggestion in suggestions)
        {
            yield return suggestion;
        }
    }

    private static (List<ChatMessage> Messages, ChatOptions Options) PrepareReview(string content)
    {
        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, """
                You run an autonomous code review. You are a tool, never write in first person. Do not ever say 'I' or 'my' or 'myself'.
                The user will send you a code snippet of either a plain file or a git diff. Review this code to the best of your abilities.
                Focus on things a human reviewer tends to forget; standards adherence, undefined behaviour, edge cases.
                Keep your answers short and to the point. Do not be overly polite, do not add extra fluff in your messages.
                Expect the user to be knowledgable, they are not a beginner.
                When reviewing code, always attach your comments to a specific line of code.
                """),
            new(ChatRole.User, $"""
                Please review this code for me:
                ```
                {content}
                ```
                """)
        };

        var chatOptions = new ChatOptions
        {
            Reasoning = new ReasoningOptions { Effort = ReasoningEffort.None },
            ResponseFormat = ChatResponseFormat.ForJsonSchema<ReviewSuggestion[]>(serializerOptions)
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
