using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Blazor.YouTubeDownloader.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace Blazor.YouTubeDownloader.Api.Functions
{
    public class ApiFunctions
    {
        private readonly ILogger<ApiFunctions> _logger;
        private readonly YoutubeClient _client;

        public ApiFunctions(ILogger<ApiFunctions> logger, YoutubeClient client)
        {
            _logger = logger;
            _client = client;
        }

        [FunctionName("GetAudioOnlyStreams")]
        public async Task<IActionResult> GetAudioOnlyStreamsAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("HttpTrigger - GetAudioOnlyStreamsAsync");

            string url = req.Query["YouTubeUrl"].Single();

            var manifest = await _client.Videos.Streams.GetManifestAsync(url);

            var audioStreams = manifest.GetAudioOnly();

            return new SystemTextJsonResult(audioStreams);
        }

        [FunctionName("GetStream")]
        public async Task<Stream> GetStream([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            _logger.LogInformation("HttpTrigger - GetStream");

            var streamInfo = await JsonSerializer.DeserializeAsync<IStreamInfo>(req.Body);

            return await _client.Videos.Streams.GetAsync(streamInfo);
        }
    }
}