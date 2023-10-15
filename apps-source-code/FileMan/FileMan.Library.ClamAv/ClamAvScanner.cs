using System.Text.Json;
using FileMan.Business.Features.Av;
using Microsoft.Extensions.Logging;
using nClam;

namespace FileMan.Av.ClamAv;

public class ClamAvScanner : IAntiVirusScanner
{
    private readonly IClamClient _client;
    private readonly ILogger<ClamAvScanner> _logger;

    public ClamAvScanner(IClamClient client, ILogger<ClamAvScanner> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<bool> IsContentSafe(Stream content)
    {
        var scanResult = await _client.SendAndScanFileAsync(content);
        var resultJson = JsonSerializer.Serialize(scanResult);
        _logger.LogInformation("File scanned, length: {length}, result: {json}", content.Length, resultJson);

        var retVal = scanResult.Result switch
        {
            ClamScanResults.Clean => true,
            ClamScanResults.VirusDetected => false,
            _ => (bool?)null
        };

        return retVal ?? throw new InvalidOperationException("ClamAv scan failed");
    }
}