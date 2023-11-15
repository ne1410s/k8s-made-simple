using FileMan.Api.Features.Av;
using FileMan.Api.Features.Common;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers();
builder.Services.AddDiscoveryFeature();
builder.Services.AddCorsFeature(config);
builder.Services.AddAntiVirusFeature();
builder.Services.AddTelemetryFeature(config);

var app = builder.Build();

app.UseDiscoveryFeature(app.Environment);
app.UseAuthorization();
app.MapControllers();
app.UseCorsFeature();
app.UseTelemetryFeature();

app.Run();
