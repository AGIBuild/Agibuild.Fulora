using Sentry;
using Sentry.Protocol.Envelopes;

namespace Agibuild.Fulora.Telemetry.Sentry.Tests;

/// <summary>
/// Minimal recording <see cref="IHub"/> for unit tests.
/// Captures breadcrumbs and exceptions added via ConfigureScope / CaptureEvent.
/// </summary>
internal sealed class RecordingHub : IHub
{
    public List<(Exception Exception, Scope? Scope)> CapturedExceptions { get; } = [];
    public int FlushCount { get; private set; }

    private readonly Scope _scope = new(new SentryOptions());

    public bool IsEnabled => true;
    public SentryId LastEventId => SentryId.Empty;

    public void ConfigureScope(Action<Scope> configureScope) => configureScope(_scope);
    public Task ConfigureScopeAsync(Func<Scope, Task> configureScope) => configureScope(_scope);

    public SentryId CaptureEvent(SentryEvent evt, Scope? scope, SentryHint? hint)
    {
        if (evt.Exception != null)
            CapturedExceptions.Add((evt.Exception, scope ?? _scope));
        return SentryId.Create();
    }

    public SentryId CaptureEvent(SentryEvent evt, Action<Scope> configureScope)
    {
        configureScope(_scope);
        return CaptureEvent(evt, _scope, null);
    }

    public SentryId CaptureEvent(SentryEvent evt, SentryHint? hint, Action<Scope> configureScope)
    {
        configureScope(_scope);
        return CaptureEvent(evt, _scope, hint);
    }

    public void CaptureTransaction(SentryTransaction transaction) { }
    public void CaptureTransaction(SentryTransaction transaction, Scope? scope, SentryHint? hint) { }
    public void CaptureSession(SessionUpdate sessionUpdate) { }

    public Task FlushAsync(TimeSpan timeout)
    {
        FlushCount++;
        return Task.CompletedTask;
    }

    public IDisposable PushScope() => new NoOpDisposable();
    public IDisposable PushScope<TState>(TState state) => new NoOpDisposable();
    public void WithScope(Action<Scope> scopeCallback) => scopeCallback(_scope);
    public void BindClient(ISentryClient client) { }

    public ITransactionTracer StartTransaction(ITransactionContext context, IReadOnlyDictionary<string, object?> customSamplingContext)
        => throw new NotImplementedException();

    public void BindException(Exception exception, ISpan span) { }
    public ISpan? GetSpan() => null;
    public SentryTraceHeader? GetTraceHeader() => null;
    public BaggageHeader? GetBaggage() => null;

    public TransactionContext ContinueTrace(SentryTraceHeader? traceHeader, BaggageHeader? baggageHeader, string? name = null, string? operation = null)
        => new(name ?? "test", operation ?? "test");

    public TransactionContext ContinueTrace(string? traceHeader, string? baggageHeader, string? name = null, string? operation = null)
        => new(name ?? "test", operation ?? "test");

    public void StartSession() { }
    public void PauseSession() { }
    public void ResumeSession() { }
    public void EndSession(SessionEndStatus status = SessionEndStatus.Exited) { }

#pragma warning disable CS0618
    public void CaptureUserFeedback(UserFeedback userFeedback) { }
#pragma warning restore CS0618
    public void CaptureFeedback(SentryFeedback feedback, Scope? scope = null, SentryHint? hint = null) { }
    public bool CaptureEnvelope(Envelope envelope) => true;

    public SentryId CaptureCheckIn(string monitorSlug, CheckInStatus status, SentryId? sentryId = null,
        TimeSpan? duration = null, Scope? scope = null, Action<SentryMonitorOptions>? configureMonitorOptions = null)
        => SentryId.Create();

    public IReadOnlyList<Breadcrumb> GetScopeBreadcrumbs() => _scope.Breadcrumbs.ToList();
    public Scope GetScope() => _scope;

    private sealed class NoOpDisposable : IDisposable
    {
        public void Dispose() { }
    }
}
