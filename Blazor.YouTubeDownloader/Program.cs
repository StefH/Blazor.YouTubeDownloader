using System;
using System.Net.Http;
using System.Threading.Tasks;
using Blazor.YouTubeDownloader.Services;
using BlazorDownloadFile;
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

            builder.Services
                .AddBlazorise(options =>
                {
                    options.ChangeTextOnKeyPress = true;
                })
                .AddBootstrapProviders()
                .AddFontAwesomeIcons()
                .AddBlazorDownloadFile();

            // HttpClient
            var baseAddress = builder.HostEnvironment.BaseAddress;
            Console.WriteLine("HostEnvironment.BaseAddress = " + baseAddress);

            bool isLocalHost = baseAddress.Contains("localhost");
            Console.WriteLine("isLocalHost = " + isLocalHost);

            bool isAzure = baseAddress.Contains("azurestaticapps.net") || baseAddress.Contains("youtubedownloader.heyenrath.nl");
            Console.WriteLine("isAzure = " + isAzure);

            string httpClientBaseAddress = isLocalHost ? "http://localhost:7071/" : baseAddress;
            Console.WriteLine("httpClientBaseAddress = " + httpClientBaseAddress);
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(httpClientBaseAddress) });

            builder.Services.AddScoped(sp =>
            {
                var httpClient = sp.GetService<HttpClient>();
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