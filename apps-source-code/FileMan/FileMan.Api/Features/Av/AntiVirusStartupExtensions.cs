using FileMan.Av.ClamAv;
using FileMan.Business.Features.Av;
using nClam;

namespace FileMan.Api.Features.Av;

public static class AntiVirusStartupExtensions
{
    public static IServiceCollection AddAntiVirusFeature(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services
            .AddScoped(provider => GetClamClient(configuration))
            .AddScoped<IAntiVirusScanner, ClamAvScanner>();
    }

    private static IClamClient GetClamClient(IConfiguration configuration)
    {
        var clamAvServer = configuration.GetValue<string>("ClamAv:Hostname");
        var clamAvPort = configuration.GetValue<int>("ClamAv:Port");
        var clamAvMaxSize = configuration.GetValue<long>("ClamAv:MaxStreamSize");

        return new ClamClient(clamAvServer, clamAvPort)
        {
            MaxStreamSize = clamAvMaxSize,
        };
    }
}
