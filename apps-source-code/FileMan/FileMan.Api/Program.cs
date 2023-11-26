using Common.Observability;
using FileMan.Api.Features.Av;
using FileMan.Api.Features.Common;

[assembly:TraceThis]

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers();
builder.Services.AddCorsFeature();
builder.Services.AddDiscoveryFeature();
builder.Services.AddAntiVirusFeature(config);
builder.Services.AddHealthzFeature();
builder.Services.AddTelemetryFeature(config);

var app = builder.Build();

app.UseDiscoveryFeature(app.Environment);
app.UseAuthorization();
app.UseCorsFeature();
app.MapControllers();
app.UseHealthzFeature();
app.UseTelemetryFeature();

app.Run();
