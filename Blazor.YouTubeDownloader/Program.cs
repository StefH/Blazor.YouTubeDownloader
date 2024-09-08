using Blazor.YouTubeDownloader;
using Blazor.YouTubeDownloader.Services;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using RestEase;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// HttpClient
var baseAddress = builder.HostEnvironment.BaseAddress;
Console.WriteLine("HostEnvironment.BaseAddress = " + baseAddress);

bool isLocalHost = baseAddress.Contains("localhost");
Console.WriteLine("isLocalHost = " + isLocalHost);

bool isAzure = baseAddress.Contains("azurestaticapps.net") || baseAddress.Contains("youtube-downloader.heyenrath.nl");
Console.WriteLine("isAzure = " + isAzure);

string httpClientBaseAddress = isLocalHost ? "http://localhost:7071/" : baseAddress;
Console.WriteLine("httpClientBaseAddress = " + httpClientBaseAddress);

builder.Services
  .AddBlazorise(options =>
    {
        options.Immediate = true;
    })
    .AddBootstrapProviders()
    .AddFontAwesomeIcons()

    // BlazorDownloadFile
    .AddBlazorDownloadFile()

    // Own services
    .AddScoped(_ =>
    {
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri(httpClientBaseAddress)
        };
        return new RestClient(httpClient).For<IYouTubeDownloadApi>();
    });

//await builder.Build().RunAsync();

var host = builder.Build();

//host.Services
    //.UseBootstrapProviders()
    //.UseFontAwesomeIcons();

await host.RunAsync();
