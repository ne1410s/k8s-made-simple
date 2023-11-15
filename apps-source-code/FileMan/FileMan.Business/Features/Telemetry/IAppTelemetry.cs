using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace FileMan.Business.Features.Telemetry;

public interface IAppTelemetry
{
    public ActivitySource Tracing { get; }

    public Meter Metrics { get; }
}
