## ADDED Requirements

### Requirement: Conversation session management

The system SHALL provide `IAiConversationManager` for creating and managing multi-turn conversation sessions with in-memory storage.

#### Scenario: Create a new conversation
- **WHEN** `CreateConversation()` is called
- **THEN** the system SHALL return a unique conversation ID
- **AND** the conversation SHALL have an empty message history

#### Scenario: Create conversation with system prompt
- **WHEN** `CreateConversation(options)` is called with a system prompt
- **THEN** the conversation SHALL have the system prompt as the first message
- **AND** the system prompt SHALL always be retained during token windowing

#### Scenario: Add and retrieve messages
- **GIVEN** a conversation exists with ID "conv-1"
- **WHEN** messages are added via `AddMessage("conv-1", message)`
- **THEN** `GetMessages("conv-1")` SHALL return all messages in chronological order

#### Scenario: Clear conversation history
- **GIVEN** a conversation with messages
- **WHEN** `ClearConversation(id)` is called
- **THEN** all messages SHALL be removed
- **AND** the system prompt (if set) SHALL be retained

#### Scenario: Remove conversation entirely
- **WHEN** `RemoveConversation(id)` is called
- **THEN** the conversation and all its state SHALL be deleted
- **AND** subsequent operations on that ID SHALL throw `KeyNotFoundException`

### Requirement: Token-aware message windowing

The system SHALL trim conversation history to fit within a configurable token budget while preserving the system prompt and most recent messages.

#### Scenario: Messages within token budget
- **GIVEN** a conversation with 5 messages totaling 200 tokens
- **WHEN** `GetMessages(id, maxTokens: 1000)` is called
- **THEN** all 5 messages SHALL be returned

#### Scenario: Messages exceed token budget
- **GIVEN** a conversation with system prompt + 10 user/assistant messages totaling 5000 tokens
- **WHEN** `GetMessages(id, maxTokens: 2000)` is called
- **THEN** the system prompt SHALL always be included
- **AND** the most recent messages that fit within the remaining budget SHALL be included
- **AND** older messages SHALL be trimmed

#### Scenario: System prompt alone exceeds budget
- **GIVEN** a conversation with a very long system prompt (3000 tokens)
- **WHEN** `GetMessages(id, maxTokens: 2000)` is called
- **THEN** only the system prompt SHALL be returned (no user messages)

### Requirement: Conversation builder integration

The system SHALL provide `FuloraAiBuilder.AddConversation()` for opt-in configuration.

#### Scenario: Enable conversation management
- **WHEN** developer calls `ai.AddConversation()`
- **THEN** `IAiConversationManager` SHALL be registered as a singleton

#### Scenario: Custom conversation options
- **WHEN** developer calls `ai.AddConversation(opts => opts.DefaultMaxTokens = 4096)`
- **THEN** conversations SHALL use 4096 as the default token window

### Requirement: Bridge exposure of conversation methods

`IAiBridgeService` SHALL expose conversation management to JavaScript.

#### Scenario: JS creates and uses a conversation
- **WHEN** JS calls `createConversation({ systemPrompt: "You are helpful" })`
- **THEN** a conversation ID SHALL be returned
- **AND** subsequent `sendMessage({ conversationId, message })` calls SHALL maintain context

#### Scenario: JS streams a conversation message
- **WHEN** JS calls `streamMessage({ conversationId, message })`
- **THEN** the system SHALL use the conversation history as context
- **AND** stream the response token-by-token
- **AND** both the user message and assistant response SHALL be added to the conversation history

#### Scenario: JS retrieves conversation history
- **WHEN** JS calls `getHistory(conversationId)`
- **THEN** the system SHALL return all messages in the conversation
