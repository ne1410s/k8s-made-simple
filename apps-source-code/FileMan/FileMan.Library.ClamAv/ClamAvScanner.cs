using System.Diagnostics;
using System.Diagnostics.Metrics;
using FileMan.Business.Features.Av;
using FileMan.Business.Features.Telemetry;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using nClam;

namespace FileMan.Av.ClamAv;

public class ClamAvScanner : IAntiVirusScanner
{
    private readonly IConfiguration _config;
    private readonly IClamClient _client;
    private readonly ILogger<ClamAvScanner> _logger;
    private readonly ActivitySource _tracing;
    private readonly Histogram<long> _scanSizeMetric;

    public ClamAvScanner(
        IConfiguration config,
        IClamClient client,
        ILogger<ClamAvScanner> logger,
        IAppTelemetry telemetry)
    {
        _config = config;
        _client = client;
        _logger = logger;
        _tracing = telemetry.Tracing;
        _scanSizeMetric = telemetry.Metrics.CreateHistogram<long>("clam_av.scan.file_size", "bytes");
    }

    public async Task<bool> IsContentSafe(Stream content)
    {
        var k8sTags = new Dictionary<string, object?>
        {
            ["namespace"] = _config["K8S_NAMESPACE"],
            ["app"] = _config["K8S_APP"],
            ["pod"] = _config["K8S_POD"],
        };

        using var activity = _tracing.StartActivity("clam_av.scan", ActivityKind.Internal, null!, k8sTags);

        var scanResponse = await _client.SendAndScanFileAsync(content);
        var resultText = scanResponse.Result.ToString();
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