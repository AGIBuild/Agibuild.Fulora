## MODIFIED Requirements

### Requirement: Bridge exposure of AI chat
The system SHALL expose AI chat capabilities to JavaScript via the typed bridge, supporting single-response, streaming, tool-calling, and conversation modes.

#### Scenario: JS calls chat completion
- **WHEN** JS calls `aiChat.complete(request)` via bridge
- **THEN** the system invokes `IChatClient.GetResponseAsync` on the resolved provider and returns the response through the bridge

#### Scenario: JS calls streaming chat completion
- **WHEN** JS calls `aiChat.streamCompletion(request)` via bridge
- **THEN** the system invokes `IChatClient.GetStreamingResponseAsync` and streams `IAsyncEnumerable<string>` chunks to JS as `AsyncIterable<string>`

#### Scenario: JS lists available providers
- **WHEN** JS calls `aiChat.listProviders()` via bridge
- **THEN** the system returns an array of registered provider names

#### Scenario: JS calls tool-calling completion
- **WHEN** JS calls `aiChat.runWithTools(request)` via bridge
- **THEN** the system executes the tool-calling loop with registered tools and returns the final result

#### Scenario: JS calls streaming tool-calling completion
- **WHEN** JS calls `aiChat.streamWithTools(request)` via bridge
- **THEN** the system streams the final response after tool execution as `AsyncIterable<string>`

#### Scenario: JS manages conversations
- **WHEN** JS calls conversation methods (create, send, stream, getHistory, delete) via bridge
- **THEN** the system manages stateful multi-turn conversations with token-aware history
