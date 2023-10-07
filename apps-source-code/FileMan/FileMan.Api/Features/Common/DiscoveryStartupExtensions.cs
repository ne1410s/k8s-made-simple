namespace FileMan.Api.Features.Common;

public static class DiscoveryStartupExtensions
{
    public static IServiceCollection AddDiscoveryFeature(
        this IServiceCollection services)
    {
        return services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen();
    }

    public static IApplicationBuilder UseDiscoveryFeature(
        this IApplicationBuilder app,
        IWebHostEnvironment environment)
    {
        if (environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        return app;
    }
}
