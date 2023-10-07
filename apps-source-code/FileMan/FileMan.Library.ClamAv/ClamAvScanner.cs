using System.Text.Json;
using FileMan.Business.Features.Av;
using nClam;

namespace FileMan.Av.ClamAv;

public class ClamAvScanner : IAntiVirusScanner
{
    private readonly IClamClient _client;

    public ClamAvScanner(IClamClient client)
    {
        _client = client;
    }

    public async Task<bool> IsContentSafe(Stream content)
    {
        var scanResult = await _client.SendAndScanFileAsync(content);
        var resultJson = JsonSerializer.Serialize(scanResult);
        Console.WriteLine(resultJson);

        var retVal = scanResult.Result switch
        {
            ClamScanResults.Clean => true,
            ClamScanResults.VirusDetected => false,
            _ => (bool?)null
        };

        return retVal ?? throw new InvalidOperationException("ClamAv scan failed");
    }
}