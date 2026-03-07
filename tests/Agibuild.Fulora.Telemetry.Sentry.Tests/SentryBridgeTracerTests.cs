using Sentry;
using Xunit;

namespace Agibuild.Fulora.Telemetry.Sentry.Tests;

public class SentryBridgeTracerTests
{
    private readonly RecordingHub _hub = new();

    private SentryBridgeTracer CreateTracer(bool captureParams = false, int maxLen = 20) =>
        new(_hub, new SentryFuloraOptions
        {
            CaptureBridgeParams = captureParams,
            MaxBreadcrumbParamsLength = maxLen,
        });

    [Fact]
    public void ExportCallStart_adds_breadcrumb()
    {
        var tracer = CreateTracer();

        tracer.OnExportCallStart("AppService", "getUser", """{"id":1}""");

        var crumbs = _hub.GetScopeBreadcrumbs();
        Assert.Single(crumbs);
        Assert.Equal("bridge.export.start", crumbs[0].Message);
        Assert.Equal("fulora.bridge", crumbs[0].Category);
    }

    [Fact]
    public void ExportCallStart_omits_params_when_disabled()
    {
        var tracer = CreateTracer(captureParams: false);

        tracer.OnExportCallStart("Svc", "method", """{"secret":"data"}""");

        var crumbs = _hub.GetScopeBreadcrumbs();
        Assert.DoesNotContain("params", crumbs[0].Data?.Keys ?? []);
    }

    [Fact]
    public void ExportCallStart_includes_params_when_enabled()
    {
        var tracer = CreateTracer(captureParams: true);

        tracer.OnExportCallStart("Svc", "method", """{"id":1}""");

        var crumbs = _hub.GetScopeBreadcrumbs();
        Assert.Contains("params", crumbs[0].Data?.Keys ?? []);
    }

    [Fact]
    public void ExportCallStart_truncates_long_params()
    {
        var tracer = CreateTracer(captureParams: true, maxLen: 20);
        var longParams = new string('x', 100);

        tracer.OnExportCallStart("Svc", "method", longParams);

        var crumbs = _hub.GetScopeBreadcrumbs();
        var captured = crumbs[0].Data?["params"];
        Assert.NotNull(captured);
        Assert.True(captured!.Length <= 23); // 20 chars + "..."
        Assert.EndsWith("...", captured);
    }

    [Fact]
    public void ExportCallEnd_adds_breadcrumb_with_elapsed()
    {
        var tracer = CreateTracer();

        tracer.OnExportCallEnd("AppService", "getUser", 150, "UserProfile");

        var crumbs = _hub.GetScopeBreadcrumbs();
        Assert.Single(crumbs);
        Assert.Equal("bridge.export.end", crumbs[0].Message);
        Assert.Equal("150", crumbs[0].Data?["elapsed_ms"]);
    }

    [Fact]
    public void ExportCallError_captures_exception()
    {
        var tracer = CreateTracer();
        var ex = new InvalidOperationException("bridge fail");

        tracer.OnExportCallError("AppService", "getUser", 200, ex);

        Assert.Single(_hub.CapturedExceptions);
        Assert.Same(ex, _hub.CapturedExceptions[0].Exception);
    }

    [Fact]
    public void ImportCallStart_adds_breadcrumb()
    {
        var tracer = CreateTracer();

        tracer.OnImportCallStart("UiController", "showAlert", null);

        var crumbs = _hub.GetScopeBreadcrumbs();
        Assert.Single(crumbs);
        Assert.Equal("bridge.import.start", crumbs[0].Message);
    }

    [Fact]
    public void ImportCallEnd_adds_breadcrumb()
    {
        var tracer = CreateTracer();

        tracer.OnImportCallEnd("UiController", "showAlert", 50);

        var crumbs = _hub.GetScopeBreadcrumbs();
        Assert.Single(crumbs);
        Assert.Equal("bridge.import.end", crumbs[0].Message);
    }

    [Fact]
    public void ServiceExposed_adds_breadcrumb()
    {
        var tracer = CreateTracer();

        tracer.OnServiceExposed("AppService", 5, true);

        var crumbs = _hub.GetScopeBreadcrumbs();
        Assert.Single(crumbs);
        Assert.Equal("bridge.service.exposed", crumbs[0].Message);
    }

    [Fact]
    public void ServiceRemoved_adds_breadcrumb()
    {
        var tracer = CreateTracer();

        tracer.OnServiceRemoved("AppService");

        var crumbs = _hub.GetScopeBreadcrumbs();
        Assert.Single(crumbs);
        Assert.Equal("bridge.service.removed", crumbs[0].Message);
    }

    [Fact]
    public void Constructor_throws_on_null_hub()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new SentryBridgeTracer(null!, new SentryFuloraOptions()));
    }

    [Fact]
    public void Constructor_throws_on_null_options()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new SentryBridgeTracer(_hub, null!));
    }
}
