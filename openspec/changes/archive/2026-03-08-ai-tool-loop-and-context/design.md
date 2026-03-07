## Context

`Agibuild.Fulora.AI` currently has:
- `IAiToolRegistry` — discovers `[AiTool]`-marked methods, creates `AIFunction` wrappers
- `IAiBridgeService` — `Complete`, `CompleteTyped`, `StreamCompletion` (all stateless, single-turn)
- Middleware pipeline — ContentGate → Resilience → Metering → Provider

Missing:
1. No tool-call execution loop — `IAiToolRegistry` produces schemas but nothing invokes them
2. No conversation state — each bridge call is independent, no message history

## Goals / Non-Goals

**Goals:**
- Zero-config tool calling: register `[AiTool]` methods, call `RunWithTools` from JS → LLM calls tools automatically
- Conversation sessions with token-aware history trimming
- Both tool calling and conversation work through existing middleware pipeline (resilience, metering, content gate)
- All new features testable via mock `IChatClient` without real LLM

**Non-Goals:**
- Custom tool-calling loop implementation (use `FunctionInvokingChatClient`)
- Persistent conversation storage (in-memory only)
- Conversation summarization / compression
- Parallel tool execution (sequential is the default in `FunctionInvokingChatClient`)

## Decisions

### D1: Tool calling via `FunctionInvokingChatClient` decorator

`Microsoft.Extensions.AI` 10.3.0 provides `FunctionInvokingChatClient` — a delegating `IChatClient` that intercepts `FunctionCallContent` responses, invokes the matching `AIFunction`, and feeds results back. It handles the full loop with configurable `MaximumIterationsPerRequest` (default 40).

We wrap the provider's `IChatClient` with this decorator, passing `IAiToolRegistry.Tools` as `ChatOptions.Tools`. The middleware chain becomes:

```
ContentGate → Resilience → Metering → FunctionInvoking → Provider
```

**Why**: Proven, maintained by Microsoft, handles edge cases (unknown functions, iteration limits, concurrent calls). No reason to reimplement.

**Alternative**: Custom loop — rejected (unnecessary complexity, maintenance burden).

### D2: `IAiConversationManager` — in-memory session store

```csharp
public interface IAiConversationManager
{
    string CreateConversation(AiConversationOptions? options = null);
    void AddMessage(string conversationId, ChatMessage message);
    IReadOnlyList<ChatMessage> GetMessages(string conversationId, int? maxTokens = null);
    void ClearConversation(string conversationId);
    void RemoveConversation(string conversationId);
    IReadOnlyList<string> ListConversations();
}
```

Key design choices:
- **Session ID based** — each conversation has a unique ID, JS creates/references by ID
- **Token-aware windowing** — `GetMessages(id, maxTokens)` returns messages from newest to oldest that fit within the token budget, always keeping the system prompt
- **In-memory only** — `ConcurrentDictionary<string, ConversationState>` with optional TTL expiry
- **Thread-safe** — concurrent access from multiple bridge calls

**Why in-memory**: Persistence is application-specific (SQLite? cloud?). We provide the session primitive; apps add persistence if needed.

### D3: Token counting strategy

Use `Microsoft.Extensions.AI`'s `ChatMessage.Text.Length / 4` as a rough token estimate (industry standard approximation for English text). This avoids requiring a tokenizer dependency.

**Why**: Exact tokenization requires model-specific tokenizers (tiktoken, sentencepiece). A 4:1 char-to-token ratio is sufficient for context window management. Apps needing exact counts can override.

### D4: Bridge API additions

New methods on `IAiBridgeService`:

```csharp
// Tool-calling (stateless — single request with tools)
Task<AiChatResult> RunWithTools(AiChatRequest request);
IAsyncEnumerable<string> StreamWithTools(AiChatRequest request, CancellationToken ct);

// Conversation (stateful — multi-turn)
Task<string> CreateConversation(AiConversationCreateRequest request);
Task<AiChatResult> SendMessage(AiConversationMessageRequest request);
IAsyncEnumerable<string> StreamMessage(AiConversationMessageRequest request, CancellationToken ct);
Task<AiConversationHistory> GetHistory(string conversationId);
Task DeleteConversation(string conversationId);
```

**Why separate from `Complete`/`StreamCompletion`**: Existing methods remain simple and stateless. Tool-calling and conversation are opt-in capabilities with different semantics.

### D5: `FuloraAiBuilder` integration

```csharp
services.AddFuloraAi(ai =>
{
    ai.AddOllama("default");
    ai.AddToolCalling(opts => opts.MaxIterations = 10);
    ai.AddConversation(opts => opts.DefaultMaxTokens = 4096);
    ai.AddResilience();
});
```

`AddToolCalling()` registers `FunctionInvokingChatClient` in the pipeline. `AddConversation()` registers `IAiConversationManager`.

## Risks / Trade-offs

| Risk | Mitigation |
|------|------------|
| `FunctionInvokingChatClient` requires `Microsoft.Extensions.AI` (not just Abstractions) — larger dependency | Acceptable — it's the official Microsoft package, already used in samples |
| Token estimate (chars/4) is imprecise for non-English or code content | Document as approximation; allow custom token counter injection in future |
| In-memory conversations lost on app restart | Explicit design choice — document that persistence is app responsibility |
| Tool calling with streaming may not include usage metadata | `FunctionInvokingChatClient` handles this; metering tracks final accumulated usage |

## Testing Strategy

| Layer | Approach |
|-------|----------|
| `IAiConversationManager` | Unit tests: create, add, window, clear, TTL expiry |
| Tool calling integration | Unit test with mock `IChatClient` that returns `FunctionCallContent` → verify tool invocation and result round-trip |
| Bridge methods | Unit tests: `RunWithTools`, `SendMessage`, `StreamMessage` via DI with mock provider |
| Token windowing | Unit test: verify message trimming respects token budget while keeping system prompt |
