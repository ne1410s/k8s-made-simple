using System.Diagnostics;
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
    private readonly ActivitySource _activitySource;

    public ClamAvScanner(
        IClamClient client,
        Meter appMeter,
        ActivitySource activitySource,
        ILogger<ClamAvScanner> logger)
    {
        _client = client;
        _logger = logger;
        _activitySource = activitySource;
        _scanSizeMetric = appMeter.CreateHistogram<long>("av_file_size", "bytes");
    }

    public async Task<bool> IsContentSafe(Stream content)
    {
        ClamScanResults scanResult;
        using (var activity = _activitySource.StartActivity("Send to clam av", ActivityKind.Client))
        {
            var scanResponse = await _client.SendAndScanFileAsync(content);
            scanResult = scanResponse.Result;
        }
        
        var k8sTags = TraceTools.GetTags();
        _scanSizeMetric.Record(content.Length, k8sTags.Append(new("result", $"{scanResult}")).ToArray());
        _logger.LogInformation("File scanned, length: {length}, result: {result}", content.Length, scanResult);

        var retVal = scanResult switch
        {
            ClamScanResults.Clean => true,
            ClamScanResults.VirusDetected => false,
            _ => (bool?)null
        };

        return retVal ?? throw new InvalidOperationException("ClamAv scan failed");
    }
}