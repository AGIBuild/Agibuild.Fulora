using Microsoft.Extensions.AI;

namespace Agibuild.Fulora.AI;

/// <summary>
/// Manages multi-turn conversation sessions with in-memory storage
/// and token-aware message history windowing.
/// </summary>
public interface IAiConversationManager
{
    /// <summary>Creates a new conversation and returns its unique ID.</summary>
    string CreateConversation(string? systemPrompt = null);

    /// <summary>Adds a message to the conversation.</summary>
    void AddMessage(string conversationId, ChatMessage message);

    /// <summary>
    /// Returns messages that fit within the token budget.
    /// System prompt is always retained; oldest non-system messages are trimmed first.
    /// If <paramref name="maxTokens"/> is null, the default from options is used.
    /// </summary>
    IReadOnlyList<ChatMessage> GetMessages(string conversationId, int? maxTokens = null);

    /// <summary>Returns all messages without windowing.</summary>
    IReadOnlyList<ChatMessage> GetAllMessages(string conversationId);

    /// <summary>Clears messages but retains the system prompt (if any).</summary>
    void ClearConversation(string conversationId);

    /// <summary>Removes the conversation entirely.</summary>
    void RemoveConversation(string conversationId);

    /// <summary>Lists all active conversation IDs.</summary>
    IReadOnlyList<string> ListConversations();
}
