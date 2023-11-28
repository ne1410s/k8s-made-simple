using System.Diagnostics.Metrics;
using Common.Observability;
using FileMan.Business.Features.Av;
using FluentErrors.Extensions;
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

    public async Task AssertIsClean(Stream content)
    {
        var scanResponse = await _client.SendAndScanFileAsync(content);
        var scanResult = scanResponse.Result;       
        var k8sTags = TraceTools.GetTags();

        _scanSizeMetric.Record(content.Length, k8sTags.Append(new("result", $"{scanResult}")).ToArray());
        _logger.LogInformation("File scanned, length: {length}, result: {result}", content.Length, scanResult);

        scanResult.MustBe(ClamScanResults.Clean, $"Unhappy scan of {content.Length} bytes. Result: {scanResult}");
    }
}