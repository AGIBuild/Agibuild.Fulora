using System.Runtime.CompilerServices;
using AvaloniAiChat.Bridge.Services;
using Microsoft.Extensions.AI;

namespace AvaloniAiChat.Desktop;

/// <summary>
/// AI chat service that wraps <see cref="IChatClient"/> to stream LLM responses
/// token-by-token via <see cref="IAsyncEnumerable{T}"/>.
/// </summary>
public sealed class AiChatService(IChatClient chatClient, string backendName) : IAiChatService
{
    private string? _lastDroppedFilePath;

    public async IAsyncEnumerable<string> StreamCompletion(
        string prompt,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ChatMessage[] messages = [new(ChatRole.User, prompt)];

        await foreach (var update in chatClient.GetStreamingResponseAsync(messages, cancellationToken: cancellationToken))
        {
            if (update.Text is { Length: > 0 } text)
            {
                yield return text;
            }
        }
    }

    public Task<string> GetBackendInfo() => Task.FromResult(backendName);

    /// <summary>
    /// Called by MainWindow when a native file drop occurs.
    /// </summary>
    internal void SetDroppedFile(string path) => _lastDroppedFilePath = path;

    public Task<DroppedFileResult?> ReadDroppedFile()
    {
        var path = Interlocked.Exchange(ref _lastDroppedFilePath, null);
        if (path is null || !File.Exists(path))
            return Task.FromResult<DroppedFileResult?>(null);

        var content = File.ReadAllText(path);
        var fileName = Path.GetFileName(path);
        return Task.FromResult<DroppedFileResult?>(new DroppedFileResult
        {
            FileName = fileName,
            Content = content
        });
    }
}
