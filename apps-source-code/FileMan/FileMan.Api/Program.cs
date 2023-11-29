using Common.Observability;
using FileMan.Api.Features.Av;
using FileMan.Api.Features.Common;
using FileMan.Api.Features.Pdf;

[assembly:TraceThis]

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers();
builder.Services.AddCorsFeature();
builder.Services.AddDiscoveryFeature();
builder.Services.AddFluentErrorsFeature();
builder.Services.AddAntiVirusFeature(config);
builder.Services.AddPdfConversionFeature(config);
builder.Services.AddHealthzFeature();
builder.Services.AddTelemetryFeature(config);

var app = builder.Build();

app.UseDiscoveryFeature(app.Environment);
app.UseAuthorization();
app.UseCorsFeature();
app.UseFluentErrorsFeature();
app.MapControllers();
app.UseHealthzFeature();
app.UseTelemetryFeature();

app.Run();
