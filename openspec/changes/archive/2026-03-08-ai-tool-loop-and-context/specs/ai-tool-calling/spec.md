## ADDED Requirements

### Requirement: Tool-calling loop via FunctionInvokingChatClient

The system SHALL integrate `FunctionInvokingChatClient` as a middleware decorator that automatically executes tool calls using tools registered in `IAiToolRegistry`.

#### Scenario: LLM returns a tool call and it is executed automatically
- **GIVEN** a tool `GetWeather(city)` is registered in `IAiToolRegistry`
- **WHEN** `RunWithTools` is called with a prompt like "What's the weather in Tokyo?"
- **AND** the LLM responds with a `FunctionCallContent` for `GetWeather`
- **THEN** the system SHALL invoke `GetWeather("Tokyo")` via `AIFunction.InvokeAsync`
- **AND** feed the result back to the LLM
- **AND** return the final assistant response to the caller

#### Scenario: Multiple sequential tool calls
- **GIVEN** tools `Search(query)` and `Summarize(text)` are registered
- **WHEN** the LLM first calls `Search` then calls `Summarize` on the result
- **THEN** the system SHALL execute both tool calls in sequence
- **AND** return the final response after all tool calls complete

#### Scenario: Maximum iteration limit prevents infinite loops
- **GIVEN** `MaxIterations` is set to 5
- **WHEN** the LLM continues requesting tool calls beyond 5 iterations
- **THEN** the system SHALL stop the loop and return the last response

#### Scenario: No tools registered falls back to normal completion
- **WHEN** `RunWithTools` is called but no tools are registered in `IAiToolRegistry`
- **THEN** the system SHALL behave identically to `Complete` (no tool calling)

### Requirement: Streaming tool-calling completion

The system SHALL support streaming responses during tool-calling conversations.

#### Scenario: Final response streams tokens after tool execution
- **GIVEN** tools are registered and the LLM calls a tool
- **WHEN** `StreamWithTools` is called
- **THEN** the system SHALL execute tool calls
- **AND** stream the final assistant response token-by-token as `IAsyncEnumerable<string>`

### Requirement: Tool-calling builder integration

The system SHALL provide `FuloraAiBuilder.AddToolCalling()` for opt-in configuration.

#### Scenario: Enable tool calling with default options
- **WHEN** developer calls `ai.AddToolCalling()`
- **THEN** `FunctionInvokingChatClient` SHALL be registered in the middleware pipeline
- **AND** `MaximumIterationsPerRequest` defaults to 10

#### Scenario: Custom tool-calling options
- **WHEN** developer calls `ai.AddToolCalling(opts => opts.MaxIterations = 5)`
- **THEN** `FunctionInvokingChatClient` SHALL use the configured iteration limit

### Requirement: Bridge exposure of tool-calling methods

`IAiBridgeService` SHALL expose `RunWithTools` and `StreamWithTools` to JavaScript.

#### Scenario: JS calls RunWithTools
- **WHEN** JS calls `aiBridgeService.runWithTools(request)` via bridge
- **THEN** the system SHALL execute the tool-calling loop and return the final result

#### Scenario: JS calls StreamWithTools
- **WHEN** JS calls `aiBridgeService.streamWithTools(request)` via bridge
- **THEN** the system SHALL stream the final response as `AsyncIterable<string>`
