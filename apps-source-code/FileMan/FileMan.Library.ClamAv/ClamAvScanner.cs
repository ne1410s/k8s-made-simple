using System.Diagnostics.Metrics;
using Common.Observability;
using FileMan.Business.Features.Av;
using Microsoft.Extensions.Logging;
using nClam;

namespace FileMan.Av.ClamAv;

public class ClamAvScanner : IAntiVirusScanner
{
    private readonly IClamClient _client;
    private readonly ILogger<ClamAvScanner> _logger;
    private readonly Histogram<long> _scanSizeMetric;

    public ClamAvScanner(
        IClamClient client,
        Meter appMeter,
        ILogger<ClamAvScanner> logger)
    {
        _client = client;
        _logger = logger;
        _scanSizeMetric = appMeter.CreateHistogram<long>("av_file_size", "bytes");
    }

    public async Task<bool> IsContentSafe(Stream content)
    {
        var scanResponse = await _client.SendAndScanFileAsync(content);
        var resultText = scanResponse.Result.ToString();
        var k8sTags = TraceTools.GetTags();
        _scanSizeMetric.Record(content.Length, k8sTags.Append(new("result", resultText)).ToArray());
        _logger.LogInformation("File scanned, length: {length}, result: {result}", content.Length, resultText);

        var retVal = scanResponse.Result switch
        {
            ClamScanResults.Clean => true,
            ClamScanResults.VirusDetected => false,
            _ => (bool?)null
        };

        return retVal ?? throw new InvalidOperationException("ClamAv scan failed");
    }
}