namespace FileMan.Api.Features.Common;

public static class CorsStartupExtensions
{
    public static IServiceCollection AddCorsFeature(
        this IServiceCollection services)
    {
        return services.AddCors(o => o
            .AddDefaultPolicy(builder => builder
                .WithMethods("GET", "POST", "PUT", "PATCH", "DELETE")
                .WithHeaders("Authorization", "Content-Type")
                .AllowAnyOrigin()
                .SetIsOriginAllowedToAllowWildcardSubdomains()));
    }

    public static IApplicationBuilder UseCorsFeature(
        this IApplicationBuilder app)
    {
        return app.UseCors();
    }
}
