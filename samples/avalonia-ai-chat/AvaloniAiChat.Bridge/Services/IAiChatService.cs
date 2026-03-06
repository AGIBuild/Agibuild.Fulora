using Agibuild.Fulora;

namespace AvaloniAiChat.Bridge.Services;

/// <summary>
/// Result of reading a dropped file.
/// </summary>
public sealed class DroppedFileResult
{
    public string FileName { get; init; } = "";
    public string Content { get; init; } = "";
}

/// <summary>
/// AI chat service exposed to JavaScript. Streaming completion returns tokens one-by-one
/// via <see cref="IAsyncEnumerable{T}"/> which maps to JS <c>AsyncIterable&lt;string&gt;</c>.
/// </summary>
[JsExport]
public interface IAiChatService
{
    IAsyncEnumerable<string> StreamCompletion(string prompt, CancellationToken cancellationToken = default);

    Task<string> GetBackendInfo();

    Task<DroppedFileResult?> ReadDroppedFile();
}
