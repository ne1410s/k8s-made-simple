namespace FileMan.Api.Features.Common;

using FluentErrors.Api;
using FluentErrors.Errors;
using FluentErrors.Extensions;
using FluentErrors.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

/// <summary>
/// Extensions for configuring FluentErrors at startup.
/// </summary>
public static class FluentErrorsStartupExtensions
{
    /// <summary>
    /// Adds FluentErrors.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <returns>The service collection.</returns>
    /// <exception cref="StaticValidationException">Model error.</exception>
    public static IServiceCollection AddFluentErrorsFeature(
        this IServiceCollection services)
    {
        return services.Configure<ApiBehaviorOptions>(opts =>
        {
            opts.InvalidModelStateResponseFactory = ctx =>
            {
                var invalidItems = ctx.ModelState.ToItems();
                throw new StaticValidationException(invalidItems);
            };
        });
    }

    /// <summary>
    /// Uses the FluentErrors feature.
    /// </summary>
    /// <param name="app">The app builder.</param>
    /// <returns>The same app builder.</returns>
    public static IApplicationBuilder UseFluentErrorsFeature(
        this IApplicationBuilder app)
    {
        return app.UseMiddleware<FluentErrorHandlingMiddleware>();
    }

    public static InvalidItem[] ToItems(this ModelStateDictionary state)
        => state.Select(e => new InvalidItem(
            e.Key,
            string.Join(", ", e.Value!.Errors.Select(m => m.ErrorMessage)),
            e.Value.RawValue)).ToArray();

}

/// <summary>
/// Middleware for handling of FluentErrors.
/// </summary>
public class FluentErrorHandlingMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger<FluentErrorHandlingMiddleware> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="FluentErrorHandlingMiddleware"/> class.
    /// </summary>
    /// <param name="next">The request delegate.</param>
    public FluentErrorHandlingMiddleware(
        RequestDelegate next,
        ILogger<FluentErrorHandlingMiddleware> logger)
    {
        this.next = next;
        this.logger = logger;
    }

    /// <summary>
    /// Invokes the middleware.
    /// </summary>
    /// <param name="context">The http context.</param>
    /// <returns>Asynchronous task.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        context.MustExist();

        try
        {
            await this.next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");

            var httpOutcome = ex.ToOutcome();
            context.Response.StatusCode = httpOutcome.ErrorCode;
            await context.Response.WriteAsJsonAsync(httpOutcome.ErrorBody);
        }
    }
}
