using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Text.Json;
using FileMan.Business.Features.Av;
using FileMan.Business.Features.Telemetry;
using Microsoft.Extensions.Logging;
using nClam;

namespace FileMan.Av.ClamAv;

public class ClamAvScanner : IAntiVirusScanner
{
    private readonly IClamClient _client;
    private readonly ILogger<ClamAvScanner> _logger;
    private readonly ActivitySource _tracing;
    private readonly Histogram<long> _scanSizeMetric;

    public ClamAvScanner(IClamClient client, ILogger<ClamAvScanner> logger, IAppTelemetry telemetry)
    {
        _client = client;
        _logger = logger;
        _tracing = telemetry.Tracing;
        _scanSizeMetric = telemetry.Metrics.CreateHistogram<long>("clam_av.scan.file_size", "bytes");
    }

    public async Task<bool> IsContentSafe(Stream content)
    {
        using var activity = _tracing.StartActivity("clam_av.scan");

        var scanResponse = await _client.SendAndScanFileAsync(content);
        var resultText = scanResponse.Result.ToString();
        _scanSizeMetric.Record(content.Length, new KeyValuePair<string, object?>("result", resultText));
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