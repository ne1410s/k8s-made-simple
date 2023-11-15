using System.Reflection;
using FileMan.Business.Features.Telemetry;
using OpenTelemetry.Exporter;
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

        void appResourceBuilder(ResourceBuilder resource) => resource
                .AddTelemetrySdk()
                .AddService(appName);

        void openTelemetryOptsBuilder(OtlpExporterOptions opts)
        {
            opts.Protocol = OtlpExportProtocol.Grpc;
            opts.Endpoint = new Uri(configuration["OpenTel:Grpc"]);
        }

        services.AddSingleton<IAppTelemetry>(sp => new AppTelemetry(appName, appVersion));

        services.AddOpenTelemetry()
            .ConfigureResource(appResourceBuilder)
            .WithTracing(builder => builder
                .AddSource(appName)
                .AddAspNetCoreInstrumentation(opts => opts.RecordException = true)
                .AddHttpClientInstrumentation()
                .AddEntityFrameworkCoreInstrumentation()
                .AddOtlpExporter(openTelemetryOptsBuilder))
            .WithMetrics(builder => builder
                .AddMeter(appName)
                .AddRuntimeInstrumentation()
                .AddAspNetCoreInstrumentation()
                .AddPrometheusExporter()
                .AddOtlpExporter(openTelemetryOptsBuilder));

        return services;
    }

    public static IApplicationBuilder UseTelemetryFeature(this IApplicationBuilder app)
    {
        return app.UseOpenTelemetryPrometheusScrapingEndpoint();
    }
}
