using System.Text.Json.Extensions.Services;
using System.Web;
using Matroska.Muxer;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace Blazor.YouTubeDownloader.Api.Functions;

internal class ApiFunctions
{
    private const string ContentTypeApplicationJson = "application/json";

    private readonly ILogger<ApiFunctions> _logger;
    private readonly YoutubeClient _client;
    private readonly IHttpClientFactory _factory;
    private readonly ISerializer _serializer;

    public ApiFunctions(ILogger<ApiFunctions> logger, YoutubeClient client, IHttpClientFactory factory, ISerializer serializer)
    {
        _logger = logger;
        _client = client;
        _factory = factory;
        _serializer = serializer;
    }

    private async Task<HttpResponseData> CreateJsonResponseAsync<T>(HttpRequestData req, T[] values)
    {
        var response = req.CreateResponse();
        response.Headers.Add("Content-Type", ContentTypeApplicationJson);

        await response.WriteStringAsync(_serializer.Serialize(values));

        return response;
    }

    private async Task<HttpResponseData> CreateJsonResponseAsync<T>(HttpRequestData req, T value)
    {
        var response = req.CreateResponse();
        response.Headers.Add("Content-Type", ContentTypeApplicationJson);

        await response.WriteStringAsync(_serializer.Serialize(value));

        return response;
    }

    [Function("GetVideoMetaData")]
    public async Task<HttpResponseData> GetVideoMetaDataAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequestData req)
    {
        _logger.LogInformation("HttpTrigger - GetVideoMetaDataAsync");

        var url = GetYouTubeUrlFromQuery(req.Url.Query);

        var videoMetaData = await _client.Videos.GetAsync(url);

        return await CreateJsonResponseAsync(req, videoMetaData);
    }


    [Function("GetAudioOnlyStreams")]
    public async Task<HttpResponseData> GetAudioOnlyStreamsAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequestData req)
    {
        _logger.LogInformation("HttpTrigger - GetAudioOnlyStreamsAsync");

        var url = GetYouTubeUrlFromQuery(req.Url.Query);

        //var manifest = await _client.Videos.Streams.GetManifestAndFixStreamUrlsAsync(url);
        var manifest = await _client.Videos.Streams.GetManifestAsync(url);

        var audioStreams = manifest.GetAudioOnlyStreams().OrderBy(a => a.Bitrate).ToArray();

        return await CreateJsonResponseAsync(req, audioStreams);

        //var response = req.CreateResponse();

        //await response.WriteStringAsync(_serializer.Serialize(audioStreams));

        //return response;

        //return new SystemTextJsonResult(audioStreams);
    }

    [Function("GetAudioStream")]
    public async Task<Stream> GetAudioStreamAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
    {
        _logger.LogInformation("HttpTrigger - GetAudioStreamAsync");

        var streamInfo = await _serializer.DeserializeAsync<AudioOnlyStreamInfo>(req.Body);

        return await _client.Videos.Streams.GetAsync(streamInfo);
    }

    [Function("GetOggOpusAudioStream")]
    public async Task<Stream> GetOggOpusAudioStreamAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
    {
        _logger.LogInformation("HttpTrigger - GetOggOpusAudioStreamAsync");

        var streamInfo = await _serializer.DeserializeAsync<AudioOnlyStreamInfo>(req.Body);

        var destinationStream = new MemoryStream();

        await _client.Videos.Streams.CopyToAsync(streamInfo, destinationStream);

        destinationStream.Position = 0;

        var oggOpusStream = new MemoryStream();
        MatroskaDemuxer.ExtractOggOpusAudio(destinationStream, oggOpusStream);

        oggOpusStream.Position = 0;
        return oggOpusStream;
    }

    [Function("GetAudioBytes")]
    public async Task<byte[]> GetAudioBytesAsync([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
    {
        _logger.LogInformation("HttpTrigger - GetAudioBytesAsync");

        var streamInfo = await _serializer.DeserializeAsync<AudioOnlyStreamInfo>(req.Body);

        await using var destinationStream = new MemoryStream();

        await _client.Videos.Streams.CopyToAsync(streamInfo, destinationStream);

        return destinationStream.ToArray();
    }


    private static string GetYouTubeUrlFromQuery(string query)
    {
        var url = HttpUtility.ParseQueryString(query)["YouTubeUrl"];
        if (url is null)
        {
            throw new ArgumentException();
        }

        return url;
    }
}