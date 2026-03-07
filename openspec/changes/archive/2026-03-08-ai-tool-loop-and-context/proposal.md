## Why

The Fulora AI module has tool schema generation (`AiToolAttribute` + `IAiToolRegistry`) and streaming completion, but cannot **execute** tool calls or maintain **multi-turn conversation context**. These two primitives are required for any real AI-powered hybrid app — without them, AI interactions are limited to single-shot Q&A. This change closes the loop so JS-side apps can run agentic tool-calling workflows and multi-turn conversations through the bridge. Aligns with **G1 (Type-Safe Bridge)** and the AI-Native Hybrid Runtime direction.

## What Changes

- Integrate `FunctionInvokingChatClient` (from `Microsoft.Extensions.AI` 10.3.0) as a middleware decorator — automatic tool-calling loop using `IAiToolRegistry` tools
- Add `IAiConversationManager` — per-session conversation history with token-aware sliding window
- Add `RunWithTools` / `StreamWithTools` bridge methods — initiate tool-calling conversations from JS
- Add conversation bridge methods — `CreateConversation`, `SendMessage`, `GetHistory` for multi-turn sessions
- Update `FuloraAiBuilder` with `AddToolCalling()` and `AddConversation()` builder methods
- Update `@agibuild/bridge-ai` TypeScript types

## Non-goals

- Custom tool-calling loop implementation (leverage `FunctionInvokingChatClient`)
- Persistent conversation storage (in-memory only; persistence is application-layer concern)
- Agent orchestration patterns (ReAct, plan-and-execute — built on top by app developers)
- Conversation summarization for context compression (future enhancement)

## Capabilities

### New Capabilities
- `ai-tool-calling`: Tool-calling loop integration via `FunctionInvokingChatClient` + `IAiToolRegistry`, exposed through bridge
- `ai-conversation-context`: Multi-turn conversation management with token-aware sliding window and session isolation

### Modified Capabilities
- `ai-provider-integration`: Add tool-calling and conversation methods to bridge service
- `ai-tool-schema-generation`: Connect registry to `FunctionInvokingChatClient` via `ChatOptions.Tools`

## Impact

- **Modified**: `IAiBridgeService`, `AiBridgeService`, `FuloraAiBuilder`, `FuloraAiServiceCollectionExtensions`, `packages/bridge-ai/src/index.ts`
- **New files**: `IAiConversationManager.cs`, `InMemoryAiConversationManager.cs`, `AiConversationOptions.cs`
- **Dependencies**: `Microsoft.Extensions.AI` 10.3.0 (already available — contains `FunctionInvokingChatClient`; currently only `Abstractions` is referenced)
- **Tests**: Unit tests for conversation management, tool-calling loop integration, bridge method tests
