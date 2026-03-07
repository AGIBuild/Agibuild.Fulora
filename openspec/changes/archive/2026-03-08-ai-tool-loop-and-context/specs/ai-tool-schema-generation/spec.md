## MODIFIED Requirements

### Requirement: Tool registry connects to FunctionInvokingChatClient

`IAiToolRegistry.Tools` SHALL be passed as `ChatOptions.Tools` to `FunctionInvokingChatClient` so that registered `[AiTool]` methods are available for automatic invocation during tool-calling requests.

#### Scenario: Registered tools are available to the tool-calling pipeline
- **GIVEN** `[AiTool]` methods are registered via `IAiToolRegistry.Register(instance)`
- **WHEN** a tool-calling request is processed by `FunctionInvokingChatClient`
- **THEN** all registered `AIFunction` objects from the registry SHALL be included in `ChatOptions.Tools`
- **AND** the LLM SHALL be able to call any registered tool by name
