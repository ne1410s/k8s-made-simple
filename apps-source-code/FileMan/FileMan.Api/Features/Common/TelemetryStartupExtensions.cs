using System.Reflection;
using FileMan.Business.Features.Telemetry;
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
        ILoggingBuilder loggingBuilder,
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
            opts.Endpoint = new Uri(configuration["OpenTel:Grpc"]);
        }

        services.AddSingleton<IAppTelemetry>(sp => new AppTelemetry(appName, appVersion));

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
                //.AddPrometheusExporter()
                .AddOtlpExporter(openTelemetryOptsBuilder));

        return services;
    }

    public static IApplicationBuilder UseTelemetryFeature(this IApplicationBuilder app)
    {
        // return app.UseOpenTelemetryPrometheusScrapingEndpoint();
        return app;
    }
}
