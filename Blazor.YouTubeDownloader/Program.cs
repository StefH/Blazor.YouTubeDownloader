using System;
using System.Net.Http;
using System.Threading.Tasks;
using Blazor.YouTubeDownloader.Services;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RestEase;

namespace Blazor.YouTubeDownloader
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

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
                // Blazorise
                .AddBlazorise(options =>
                {
                    options.ChangeTextOnKeyPress = true;
                })
                .AddBootstrapProviders()
                .AddFontAwesomeIcons()

                // BlazorDownloadFile
                .AddBlazorDownloadFile()

                // Own services
                .AddScoped(sp =>
                {
                    var httpClient = new HttpClient
                    {
                        BaseAddress = new Uri(httpClientBaseAddress)
                    };
                    return new RestClient(httpClient).For<IYouTubeDownloadApi>();
                });

            var host = builder.Build();

            host.Services
                .UseBootstrapProviders()
                .UseFontAwesomeIcons();

            await host.RunAsync();
        }
    }
}