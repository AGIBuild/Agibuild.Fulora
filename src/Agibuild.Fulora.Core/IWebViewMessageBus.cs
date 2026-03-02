namespace Agibuild.Fulora;

/// <summary>
/// Message bus for cross-WebView communication. Allows publishing and subscribing to topics
/// across multiple WebView instances, with the host runtime acting as mediator.
/// </summary>
public interface IWebViewMessageBus
{
    /// <summary>
    /// Publishes a message to all subscribers of the given topic.
    /// </summary>
    /// <param name="topic">The topic to publish to.</param>
    /// <param name="payloadJson">Optional JSON payload.</param>
    /// <param name="sourceWebViewId">Optional ID of the WebView that originated the message.</param>
    void Publish(string topic, string? payloadJson = null, string? sourceWebViewId = null);

    /// <summary>
    /// Subscribes to a topic. Receives all messages published to that topic.
    /// </summary>
    /// <param name="topic">The topic to subscribe to.</param>
    /// <param name="handler">Callback invoked for each matching message.</param>
    /// <returns>A disposable that removes the subscription when disposed.</returns>
    IDisposable Subscribe(string topic, Action<WebViewMessage> handler);

    /// <summary>
    /// Subscribes to a topic with a target WebView filter. Only receives messages whose
    /// <see cref="WebViewMessage.SourceWebViewId"/> matches <paramref name="targetWebViewId"/>.
    /// </summary>
    /// <param name="topic">The topic to subscribe to.</param>
    /// <param name="targetWebViewId">Only receive messages from this source WebView ID.</param>
    /// <param name="handler">Callback invoked for each matching message.</param>
    /// <returns>A disposable that removes the subscription when disposed.</returns>
    IDisposable Subscribe(string topic, string targetWebViewId, Action<WebViewMessage> handler);
}

/// <summary>
/// A message delivered through the WebView message bus.
/// </summary>
/// <param name="Topic">The topic the message was published to.</param>
/// <param name="PayloadJson">Optional JSON payload.</param>
/// <param name="SourceWebViewId">Optional ID of the WebView that originated the message.</param>
/// <param name="Timestamp">When the message was published.</param>
public sealed record WebViewMessage(string Topic, string? PayloadJson, string? SourceWebViewId, DateTimeOffset Timestamp);
