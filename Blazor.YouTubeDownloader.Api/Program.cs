using Blazor.YouTubeDownloader.Api.HttpClientHandlers;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using YoutubeExplode;

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("local.settings.json", true, false)
    .AddEnvironmentVariables()
    .Build();

// Register Serilog provider
var logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.ApplicationInsights(TelemetryConfiguration.CreateDefault(), TelemetryConverter.Traces)
    .ReadFrom.Configuration(config)
    .CreateLogger();

var builder = new HostBuilder();

builder.ConfigureServices(serviceCollection =>
{
    serviceCollection.AddLogging(lb => lb.AddSerilog(logger, dispose: true));

    serviceCollection.AddScoped(_ =>
    {
        var httpClient = new HttpClient(new YouTubeCookieConsentHandler());
        return new YoutubeClient(httpClient);
    });

    serviceCollection.AddHttpClient();
});

var host = builder
    .ConfigureFunctionsWorkerDefaults()
    .Build();

host.Run();