using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Reflection;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace FileMan.Api.Features.Common;

public static class TelemetryStartupExtensions
{
    public static IServiceCollection AddTelemetryFeature(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var callingAssembly = Assembly.GetCallingAssembly().GetName();
        var appName = callingAssembly.Name!;
        var appVersion = callingAssembly.Version?.ToString();

        var appResourceBuilder = ResourceBuilder
            .CreateDefault()
            .AddTelemetrySdk()
            .AddService(appName)
            .AddEnvironmentVariableDetector();

        void openTelemetryOptsBuilder(OtlpExporterOptions opts)
        {
            opts.Protocol = OtlpExportProtocol.Grpc;
            opts.Endpoint = new Uri(configuration["OpenTel:Grpc"]!);
        }

        services.AddSingleton(new Meter(appName, appVersion));
        services.AddSingleton(new ActivitySource(appName, appVersion));

        services.AddOpenTelemetry()
            .WithTracing(builder => builder
                .SetResourceBuilder(appResourceBuilder)
                .AddSource(appName)
                .AddAspNetCoreInstrumentation(opts => opts.RecordException = true)
                .AddHttpClientInstrumentation()
                .AddEntityFrameworkCoreInstrumentation()
                .AddOtlpExporter(openTelemetryOptsBuilder))
            .WithMetrics(builder => builder
                .SetResourceBuilder(appResourceBuilder)
                .AddMeter(appName)
                .AddRuntimeInstrumentation()
                .AddAspNetCoreInstrumentation()
                .AddOtlpExporter(openTelemetryOptsBuilder));

        return services;
    }

    public static IApplicationBuilder UseTelemetryFeature(this IApplicationBuilder app)
    {
        return app;
    }
}
