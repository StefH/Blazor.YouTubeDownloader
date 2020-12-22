using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Blazor.YouTubeDownloader.Api.Models;
using Blazor.YouTubeDownloader.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace Blazor.YouTubeDownloader.Api.Functions
{
    internal class ApiFunctions
    {
        private readonly ILogger<ApiFunctions> _logger;
        private readonly YoutubeClient _client;
        private readonly ISerializer _serializer;

        public ApiFunctions(ILogger<ApiFunctions> logger, YoutubeClient client, ISerializer serializer)
        {
            _logger = logger;
            _client = client;
            _serializer = serializer;
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

        [FunctionName("GetAudioStream")]
        public async Task<Stream> GetAudioStreamAsync([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            _logger.LogInformation("HttpTrigger - GetAudioStreamAsync");

            var streamInfo = await _serializer.DeserializeAsync<AudioOnlyStreamInfo>(req.Body);

            return await _client.Videos.Streams.GetAsync(streamInfo);
        }
    }
}