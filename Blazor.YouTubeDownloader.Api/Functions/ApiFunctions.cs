using System.Web;
using Blazor.YouTubeDownloader.Api.Extensions;
using Matroska.Muxer;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace Blazor.YouTubeDownloader.Api.Functions;

internal class ApiFunctions
{
    private readonly ILogger<ApiFunctions> _logger;
    private readonly YoutubeClient _client;

    public ApiFunctions(ILogger<ApiFunctions> logger, YoutubeClient client)
    {
        _logger = logger;
        _client = client;
    }

    [Function("GetVideoMetaData")]
    public async Task<HttpResponseData> GetVideoMetaDataAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequestData req)
    {
        _logger.LogInformation("HttpTrigger - GetVideoMetaDataAsync");

        var url = GetYouTubeUrlFromQuery(req.Url.Query);

        var videoMetaData = await _client.Videos.GetAsync(url);

        return await req.CreateJsonResponseAsync(videoMetaData);
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

        return await req.CreateJsonResponseAsync(audioStreams);
    }

    [Function("GetAudioStream")]
    public async Task<HttpResponseData> GetAudioStreamAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
    {
        _logger.LogInformation("HttpTrigger - GetAudioStreamAsync");

        var body = await req.ReadAsStringAsync();

        var streamInfo = JsonConvert.DeserializeObject<AudioOnlyStreamInfo>(body!);

        return req.CreateStreamResponse(await _client.Videos.Streams.GetAsync(streamInfo!));
    }

    [Function("GetOggOpusAudioStream")]
    public async Task<HttpResponseData> GetOggOpusAudioStreamAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
    {
        _logger.LogInformation("HttpTrigger - GetOggOpusAudioStreamAsync");

        var body = await req.ReadAsStringAsync();

        var streamInfo = JsonConvert.DeserializeObject<AudioOnlyStreamInfo>(body!);

        using var destinationStream = new MemoryStream();
        await _client.Videos.Streams.CopyToAsync(streamInfo!, destinationStream);
        destinationStream.Position = 0;

        var oggOpusStream = new MemoryStream();
        MatroskaDemuxer.ExtractOggOpusAudio(destinationStream, oggOpusStream);

        return req.CreateStreamResponse(oggOpusStream);
    }

    //[Function("GetAudioBytes")]
    //public async Task<HttpResponseData> GetAudioBytesAsync([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
    //{
    //    _logger.LogInformation("HttpTrigger - GetAudioBytesAsync");

    //    var body = await req.ReadAsStringAsync();

    //    var streamInfo = JsonConvert.DeserializeObject<AudioOnlyStreamInfo>(body!);

    //    await using var destinationStream = new MemoryStream();

    //    await _client.Videos.Streams.CopyToAsync(streamInfo!, destinationStream);

    //    return destinationStream.ToArray();
    //}

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