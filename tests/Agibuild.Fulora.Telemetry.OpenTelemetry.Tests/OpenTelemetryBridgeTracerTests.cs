using System.Diagnostics;
using System.Linq;
using Agibuild.Fulora.DependencyInjection;
using Agibuild.Fulora.Telemetry;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Agibuild.Fulora.Telemetry.OpenTelemetry.Tests;

public class OpenTelemetryBridgeTracerTests
{
    [Fact]
    public void Constructor_creates_tracer_without_throwing()
    {
        var tracer = new OpenTelemetryBridgeTracer();
        Assert.NotNull(tracer);
    }

    [Fact]
    public void OnExportCallStart_creates_activity()
    {
        var tracer = new OpenTelemetryBridgeTracer();
        var activities = new List<Activity>();
        using var listener = new ActivityListener
        {
            ActivityStarted = a => activities.Add(a),
            ShouldListenTo = s => s.Name == OpenTelemetryBridgeTracer.ActivitySourceName,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData
        };
        ActivitySource.AddActivityListener(listener);

        tracer.OnExportCallStart("Calc", "Add", "{\"a\":1,\"b\":2}");
        tracer.OnExportCallEnd("Calc", "Add", 5, "int");

        var matching = activities.Where(a => a.OperationName == "Calc.Add").ToList();
        Assert.Single(matching);
        var activity = matching[0];
        Assert.Equal("Calc.Add", activity.OperationName);
        Assert.Equal("Calc", activity.GetTagItem(OpenTelemetryBridgeTracer.ServiceNameKey));
        Assert.Equal("Add", activity.GetTagItem(OpenTelemetryBridgeTracer.MethodNameKey));
        Assert.Equal("export", activity.GetTagItem(OpenTelemetryBridgeTracer.DirectionKey));
    }

    [Fact]
    public void OnImportCallStart_creates_activity()
    {
        var tracer = new OpenTelemetryBridgeTracer();
        var activities = new List<Activity>();
        using var listener = new ActivityListener
        {
            ActivityStarted = a => activities.Add(a),
            ShouldListenTo = s => s.Name == OpenTelemetryBridgeTracer.ActivitySourceName,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData
        };
        ActivitySource.AddActivityListener(listener);

        tracer.OnImportCallStart("JsService", "Notify", null);
        tracer.OnImportCallEnd("JsService", "Notify", 12);

        var matching = activities.Where(a => a.OperationName == "JsService.Notify").ToList();
        Assert.Single(matching);
        var activity = matching[0];
        Assert.Equal("JsService.Notify", activity.OperationName);
        Assert.Equal("import", activity.GetTagItem(OpenTelemetryBridgeTracer.DirectionKey));
    }

    [Fact]
    public void OnExportCallError_records_error_status()
    {
        var tracer = new OpenTelemetryBridgeTracer();
        Activity? capturedActivity = null;
        using var listener = new ActivityListener
        {
            ActivityStopped = a => { if (a.OperationName == "Svc.Fail") capturedActivity = a; },
            ShouldListenTo = s => s.Name == OpenTelemetryBridgeTracer.ActivitySourceName,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData
        };
        ActivitySource.AddActivityListener(listener);

        tracer.OnExportCallStart("Svc", "Fail", null);
        tracer.OnExportCallError("Svc", "Fail", 10, new InvalidOperationException("test error"));

        Assert.NotNull(capturedActivity);
        Assert.Equal(ActivityStatusCode.Error, capturedActivity.Status);
    }

    [Fact]
    public void OnServiceExposed_and_OnServiceRemoved_do_not_throw()
    {
        var tracer = new OpenTelemetryBridgeTracer();
        tracer.OnServiceExposed("svc", 3, true);
        tracer.OnServiceRemoved("svc");
    }

    [Fact]
    public void AddOpenTelemetry_registers_provider_and_tracer()
    {
        var services = new ServiceCollection();
        services.AddFulora().AddOpenTelemetry();

        var sp = services.BuildServiceProvider();

        var telemetry = sp.GetService<Agibuild.Fulora.ITelemetryProvider>();
        var tracer = sp.GetService<Agibuild.Fulora.IBridgeTracer>();

        Assert.NotNull(telemetry);
        Assert.IsType<OpenTelemetryTelemetryProvider>(telemetry);

        Assert.NotNull(tracer);
        Assert.IsType<OpenTelemetryBridgeTracer>(tracer);
    }
}
