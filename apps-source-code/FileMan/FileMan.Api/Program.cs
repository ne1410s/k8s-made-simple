using FileMan.Api.Features.Av;
using FileMan.Api.Features.Common;
using Microsoft.Extensions.Options;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddDiscoveryFeature();
builder.Services.AddCorsFeature(config);
builder.Services.AddAntiVirusFeature();

var app = builder.Build();

app.UseHttpMetrics(opts =>
{
    opts.ReduceStatusCodeCardinality();
    opts.AddCustomLabel("host", context => context.Request.Host.Host);
});
app.UseDiscoveryFeature(app.Environment);
app.UseAuthorization();
app.MapControllers();
app.UseCorsFeature();

app.Run();
