using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace FileMan.Business.Features.Telemetry;

public class AppTelemetry : IAppTelemetry
{
    public AppTelemetry(string appName, string? appVersion)
    {
        Tracing = new(appName, appVersion);
        Metrics = new(appName, appVersion);
    }

    public ActivitySource Tracing { get; }

    public Meter Metrics { get; }
}
