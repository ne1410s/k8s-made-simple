using Microsoft.Extensions.Diagnostics.HealthChecks;
using nClam;

namespace FileMan.Api.Features.Common;

public static class HealthCheckStartupExtensions
{
    public static IServiceCollection AddHealthzFeature(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck<ClamAvHealthCheck>("ClamAv");
        return services;
    }

    public static void UseHealthzFeature(
        this IEndpointRouteBuilder app)
    {
        // Start: include all dependencies
        app.MapHealthChecks("healthz");

        // Ready: exclude non-vital dependencies
        app.MapHealthChecks("healthz/ready", new() { Predicate = c => !c.Tags.Contains("non-vital") });
        
        // Live: exclude all dependencies
        app.MapHealthChecks("healthz/live", new() { Predicate = _ => false });
    }

    private class ClamAvHealthCheck : IHealthCheck
    {
        private readonly IClamClient _clamClient;

        public ClamAvHealthCheck(IClamClient clamClient)
        {
            _clamClient = clamClient;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            var ping = await _clamClient.PingAsync(cancellationToken);
            return new(ping ? HealthStatus.Healthy : context.Registration.FailureStatus);
        }
    }
}