using FileMan.Business.Features.Pdf;
using FileMan.Pdf.Gotenberg;
using Gotenberg.Sharp.API.Client;
using Gotenberg.Sharp.API.Client.Domain.Settings;
using Gotenberg.Sharp.API.Client.Extensions;

namespace FileMan.Api.Features.Pdf;

public static class PdfConversionStartupExtensions
{
    public static IServiceCollection AddPdfConversionFeature(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var section = configuration.GetSection(nameof(GotenbergSharpClient));
        services.AddOptions<GotenbergSharpClientOptions>().Bind(section);
        services.AddGotenbergSharpClient();

        return services.AddScoped<IPdfConverter, GotenbergService>();
    }
}
