using System.ClientModel;
using System.ClientModel.Primitives;
using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Chat;
using ChatFinishReason = Microsoft.Extensions.AI.ChatFinishReason;
using ChatMessage = Microsoft.Extensions.AI.ChatMessage;
using ChatResponseFormat = Microsoft.Extensions.AI.ChatResponseFormat;

namespace ReviewMe;

public sealed class ProgressUpdatedEventArgs(string message) : EventArgs
{
    public string Message => message;
}

public sealed class ReviewSession : IDisposable
{
    private readonly IChatClient chatClient;

    public ReviewSession()
    {
        var openAiClient = new ChatClient(
            model: "unsloth/Qwen3.6-35B-A3B-GGUF:Q4_K_M",
            credential: new ApiKeyCredential("dummy"),
            options: new OpenAIClientOptions
            {
                Endpoint = new Uri("http://127.0.0.1:8080/v1/"),
                RetryPolicy = new ClientRetryPolicy(maxRetries: 0)
            });

        chatClient = openAiClient.AsIChatClient();
    }

    public event EventHandler<ProgressUpdatedEventArgs>? ProgressUpdated;

    public async Task ReviewAsync(string content)
    {
        var chatHistory = new List<ChatMessage>
        {
            new(ChatRole.System, """
                You run an autonomous code review. You are a tool, never write in first person. Do not ever say 'I' or 'my' or 'myself'.
                The user will send you a code snippet of either a plain file or a git diff. Review this code to the best of your abilities.
                Do not comment on the code style itself. Focus on things a human reviewer tends to forget; standards adherence, undefined behaviour,
                edge cases. Keep your answers short and to the point. Do not be overly polite, do not add extra fluff in your messages. Expect the user
                to be knowledgable, they are not a beginner. When reviewing code, always attach your comments to a specific line of code. Report all findings,
                ordered by importance.
                """),
            new(ChatRole.User, $"""
                Please review this code for me:
                ```
                {content}
                ```
                """)
        };

        ProgressUpdated?.Invoke(this, new ProgressUpdatedEventArgs("Running review..."));

        var response = await chatClient.GetResponseAsync(
            chatHistory,
            new ChatOptions
            {
                Reasoning = new ReasoningOptions { Effort = ReasoningEffort.None },
                ResponseFormat = ChatResponseFormat.ForJsonSchema<ResponseFormat>()
            });

        if (response.FinishReason.HasValue && response.FinishReason == ChatFinishReason.Stop)
        {
            ProgressUpdated?.Invoke(this, new ProgressUpdatedEventArgs("Finishing..."));
        }
        else
        {
            throw new InvalidOperationException("Unexpected finish reason.");
        }
    }

    public void Dispose()
    {
        chatClient.Dispose();
    }

    private sealed record ResponseFormat(Comment[] Comments);

    private sealed record Comment(string FilePath, int LineStart, int LineEnd, string Content, string Importance);
}
