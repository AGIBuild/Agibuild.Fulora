## 1. Conversation Manager

- [x] 1.1 Create `AiConversationOptions.cs` — `DefaultMaxTokens`, `SessionTtl`, `EstimateTokens` delegate
- [x] 1.2 Create `IAiConversationManager.cs` — `CreateConversation`, `AddMessage`, `GetMessages`, `ClearConversation`, `RemoveConversation`, `ListConversations`
- [x] 1.3 Implement `InMemoryAiConversationManager.cs` — ConcurrentDictionary-based with token-aware windowing
- [x] 1.4 Unit test: create, add messages, retrieve in order
- [x] 1.5 Unit test: token windowing keeps system prompt + trims oldest messages
- [x] 1.6 Unit test: clear retains system prompt, remove deletes entirely
- [x] 1.7 Unit test: operations on nonexistent conversation throw KeyNotFoundException

## 2. Tool-Calling Integration

- [x] 2.1 Add `AiToolCallingOptions.cs` — `MaxIterations` (default 10)
- [x] 2.2 Add `FuloraAiBuilder.AddToolCalling()` — registers options
- [x] 2.3 Add `FuloraAiBuilder.AddConversation()` — registers `IAiConversationManager`
- [x] 2.4 Update `FuloraAiServiceCollectionExtensions` to register conversation manager and tool-calling options
- [x] 2.5 Add `Microsoft.Extensions.AI` package reference (not just Abstractions) to `Agibuild.Fulora.AI.csproj`
- [x] 2.6 Unit test: `AddToolCalling` registers options in DI
- [x] 2.7 Unit test: `AddConversation` registers `IAiConversationManager` singleton

## 3. Bridge Methods

- [x] 3.1 Add DTOs: `AiConversationCreateRequest`, `AiConversationMessageRequest`, `AiConversationHistory`, `AiHistoryMessage`
- [x] 3.2 Add `RunWithTools`, `StreamWithTools` to `IAiBridgeService` and implement in `AiBridgeService`
- [x] 3.3 Add `CreateConversation`, `SendMessage`, `StreamMessage`, `GetHistory`, `DeleteConversation` to `IAiBridgeService` and implement
- [x] 3.4 Unit test: `RunWithTools` with mock tool-calling client verifies tool execution
- [x] 3.5 Unit test: `SendMessage` accumulates conversation history
- [x] 3.6 Unit test: `StreamMessage` streams response and adds to history
- [x] 3.7 Unit test: `GetHistory` returns correct messages
- [x] 3.8 Unit test: `DeleteConversation` removes session

## 4. TypeScript Types

- [x] 4.1 Update `packages/bridge-ai/src/index.ts` — add DTOs and methods for tool-calling and conversation

## 5. Build & Verify

- [x] 5.1 `dotnet build` succeeds
- [x] 5.2 `dotnet test` passes all existing + new tests (1747 total, 0 failures)
