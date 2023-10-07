using FileMan.Av.ClamAv;
using FileMan.Business.Features.Av;
using nClam;

namespace FileMan.Api.Features.Av;

public static class AntiVirusStartupExtensions
{
    public static IServiceCollection AddAntiVirusFeature(
        this IServiceCollection services)
    {
        return services
            .AddScoped(GetClamClient)
            .AddScoped<IAntiVirusScanner, ClamAvScanner>();
    }

    private static IClamClient GetClamClient(IServiceProvider provider)
    {
        var config = provider.GetRequiredService<IConfiguration>();

        var clamAvServer = config.GetValue<string>("ClamAv:Hostname");
        var clamAvPort = config.GetValue<int>("ClamAv:Port");
        var clamAvMaxSize = config.GetValue<long>("ClamAv:MaxStreamSize");

        return new ClamClient(clamAvServer, clamAvPort)
        {
            MaxStreamSize = clamAvMaxSize
        };
    }
}
