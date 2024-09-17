using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Matroska.Muxer;
using YoutubeExplode.DemoConsole.Extensions;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace YoutubeExplode.DemoConsole;

class Program
{
    static async Task Main(string[] args)
    {
        var httpClientForYoutubeClient = new HttpClient();
        var youtubeClient = new YoutubeClient(httpClientForYoutubeClient);

        var videoId = VideoId.Parse("https://www.youtube.com/watch?v=spVJOzF0EJ0");

        // var videoMetaData = await youtubeClient.Videos.GetAsync(videoId);
        var videoStreams = await youtubeClient.Videos.Streams.GetManifestAsync(videoId);
        var highestAudioStreamInfo = videoStreams.GetAudioOnlyStreams().TryGetWithHighestBitrate();
        if (highestAudioStreamInfo is null)
        {
            Console.WriteLine("This video has no AudioOnlyStreams");
        }
        else
        {
            Console.WriteLine(highestAudioStreamInfo.Bitrate);
            Console.WriteLine(highestAudioStreamInfo.Url);

            Console.WriteLine("DownloadAsync start " + DateTime.Now);
            await youtubeClient.Videos.Streams.DownloadAsync(highestAudioStreamInfo, "c:\\temp\\x.webm");
            Console.WriteLine("DownloadAsync end " + DateTime.Now);

            using var destinationStream = new MemoryStream();
            await youtubeClient.Videos.Streams.CopyToAsync(highestAudioStreamInfo, destinationStream);
            destinationStream.Position = 0;

            await using var oggOpusFileStream = new FileStream($"c:\\temp\\{GetSafeFileName("x")}.opus", FileMode.OpenOrCreate);
            MatroskaDemuxer.ExtractOggOpusAudio(destinationStream, oggOpusFileStream);
        }
    }

    private static string GetSafeFileName(string fileName)
    {
        return Regex.Replace(fileName, "[" + Regex.Escape(new string(Path.GetInvalidPathChars())) + "]", string.Empty, RegexOptions.IgnoreCase);
    }
}

public class YouTubeCookieConsentHandler : HttpClientHandler
{
    public YouTubeCookieConsentHandler()
    {
        UseCookies = true;
        CookieContainer = new CookieContainer();
        CookieContainer.Add(new Cookie("CONSENT", "YES+cb", "/", "youtube.com"));
    }
}