using System.Reflection;
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
        var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
        void appResourceBuilder(ResourceBuilder resource) => resource
                .AddTelemetrySdk()
                .AddService(assemblyName!);

        void otlpOptionsBuilder(OtlpExporterOptions opts)
        {
            opts.Protocol = OtlpExportProtocol.Grpc;
            opts.Endpoint = new Uri(configuration["Otlp:Endpoints:Grpc"]);
        }

        services.AddOpenTelemetry()
            .ConfigureResource(appResourceBuilder)
            .WithTracing(builder => builder
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddSource("APITracing")
                //.AddConsoleExporter()
                .AddOtlpExporter(otlpOptionsBuilder))
            .WithMetrics(builder => builder
                .AddRuntimeInstrumentation()
                .AddAspNetCoreInstrumentation()
                .AddOtlpExporter(otlpOptionsBuilder));

        return services;
    }
}
