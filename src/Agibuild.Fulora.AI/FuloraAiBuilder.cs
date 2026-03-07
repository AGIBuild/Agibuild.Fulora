using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;

namespace Agibuild.Fulora.AI;

/// <summary>
/// Builder for configuring Fulora AI services.
/// </summary>
public sealed class FuloraAiBuilder
{
    private readonly AiProviderRegistry _registry = new();
    internal AiResilienceOptions ResilienceOptions { get; } = new();
    internal AiMeteringOptions MeteringOptions { get; } = new();
    internal AiToolCallingOptions ToolCallingOptions { get; } = new();
    internal AiConversationOptions ConversationOptions { get; } = new();
    internal bool ResilienceEnabled { get; private set; }
    internal bool MeteringEnabled { get; private set; }
    internal bool ToolCallingEnabled { get; private set; }
    internal bool ConversationEnabled { get; private set; }

    internal IServiceCollection Services { get; }

    internal FuloraAiBuilder(IServiceCollection services)
    {
        Services = services;
    }

    /// <summary>Registers a named <see cref="IChatClient"/>.</summary>
    public FuloraAiBuilder AddChatClient(string name, IChatClient client)
    {
        _registry.RegisterChatClient(name, client);
        return this;
    }

    /// <summary>Registers a named embedding generator.</summary>
    public FuloraAiBuilder AddEmbeddingGenerator(string name, IEmbeddingGenerator<string, Embedding<float>> generator)
    {
        _registry.RegisterEmbeddingGenerator(name, generator);
        return this;
    }

    /// <summary>Enables resilience middleware with optional configuration.</summary>
    public FuloraAiBuilder AddResilience(Action<AiResilienceOptions>? configure = null)
    {
        ResilienceEnabled = true;
        configure?.Invoke(ResilienceOptions);
        return this;
    }

    /// <summary>Enables token metering with optional configuration.</summary>
    public FuloraAiBuilder AddMetering(Action<AiMeteringOptions>? configure = null)
    {
        MeteringEnabled = true;
        configure?.Invoke(MeteringOptions);
        return this;
    }

    /// <summary>Enables tool-calling loop via FunctionInvokingChatClient.</summary>
    public FuloraAiBuilder AddToolCalling(Action<AiToolCallingOptions>? configure = null)
    {
        ToolCallingEnabled = true;
        configure?.Invoke(ToolCallingOptions);
        return this;
    }

    /// <summary>Enables conversation session management.</summary>
    public FuloraAiBuilder AddConversation(Action<AiConversationOptions>? configure = null)
    {
        ConversationEnabled = true;
        configure?.Invoke(ConversationOptions);
        return this;
    }

    internal AiProviderRegistry BuildRegistry() => _registry;
}
