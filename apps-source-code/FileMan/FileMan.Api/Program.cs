using FileMan.Api.Features.Av;
using FileMan.Api.Features.Common;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddDiscoveryFeature();
builder.Services.AddCorsFeature(config);
builder.Services.AddAntiVirusFeature();

var app = builder.Build();

app.UseCorsFeature();
app.UseDiscoveryFeature(app.Environment);
app.UseAuthorization();
app.MapControllers();

app.Run();
