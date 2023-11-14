using System.Reflection;
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

        services.AddOpenTelemetry()
            .ConfigureResource(appResourceBuilder)
            .WithTracing(builder => builder
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddSource("APITracing")
                //.AddConsoleExporter()
                .AddOtlpExporter(options => options.Endpoint = new Uri(configuration["Otlp:Endpoint"])))
            .WithMetrics(builder => builder
                .AddRuntimeInstrumentation()
                .AddAspNetCoreInstrumentation()
                .AddOtlpExporter(options => options.Endpoint = new Uri(configuration["Otlp:Endpoint"])));

        return services;
    }
}
