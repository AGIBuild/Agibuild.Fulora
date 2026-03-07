using Agibuild.Fulora.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Agibuild.Fulora.Telemetry;

/// <summary>
/// Extension methods for registering Sentry telemetry with Fulora.
/// </summary>
public static class FuloraSentryExtensions
{
    /// <summary>
    /// Registers Sentry-based telemetry and bridge tracing for Fulora with default options.
    /// Requires Sentry SDK to be initialized by the application (via <c>SentrySdk.Init</c>).
    /// </summary>
    public static FuloraServiceBuilder AddSentry(this FuloraServiceBuilder builder)
        => builder.AddSentry(_ => { });

    /// <summary>
    /// Registers Sentry-based telemetry and bridge tracing for Fulora with custom options.
    /// Requires Sentry SDK to be initialized by the application (via <c>SentrySdk.Init</c>).
    /// </summary>
    public static FuloraServiceBuilder AddSentry(
        this FuloraServiceBuilder builder,
        Action<SentryFuloraOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configure);

        var options = new SentryFuloraOptions();
        configure(options);

        var telemetry = new SentryTelemetryProvider(options);
        var tracer = new SentryBridgeTracer(options);

        builder.AddTelemetry(telemetry);
        builder.Services.AddSingleton<IBridgeTracer>(tracer);

        return builder;
    }
}
