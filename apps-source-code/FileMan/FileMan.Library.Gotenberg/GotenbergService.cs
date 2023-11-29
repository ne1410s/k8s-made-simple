using System.Diagnostics.Metrics;
using Common.Observability;
using FileMan.Business.Features.Pdf;
using Gotenberg.Sharp.API.Client;
using Gotenberg.Sharp.API.Client.Domain.Builders;
using Gotenberg.Sharp.API.Client.Domain.Builders.Faceted;
using Microsoft.Extensions.Logging;

namespace FileMan.Pdf.Gotenberg;

public class GotenbergService : IPdfConverter
{
    private readonly GotenbergSharpClient _client;
    private readonly ILogger<GotenbergService> _logger;
    private readonly Histogram<long> _pdfSizeMetric;

    public GotenbergService(
        GotenbergSharpClient client,
        Meter appMeter,
        ILogger<GotenbergService> logger)
    {
        _client = client;
        _logger = logger;
        _pdfSizeMetric = appMeter.CreateHistogram<long>("pdf_file_size", "bytes");
    }

    public async Task<Stream> FromUrl(string url)
    {
        var builder = new UrlRequestBuilder()
            .SetUrl(url)
            .WithDimensions(dims => dims
                .SetPaperSize(PaperSizes.A4)
                .SetMargins(Margins.None)
                .SetScale(.90)
                .LandScape());

        var request = await builder.BuildAsync();
        var retVal = await _client.UrlToPdfAsync(request);
        var k8sTags = TraceTools.GetTags();

        _pdfSizeMetric.Record(retVal.Length, k8sTags.ToArray());
        _logger.LogInformation("Pdf of {bytes} bytes converted from url: {url}", retVal.Length, url);

        return retVal;
    }
}