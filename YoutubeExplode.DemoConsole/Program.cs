using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;
using YouTubeUrlDecoder;

namespace YoutubeExplode.DemoConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var httpClientForYoutubeClient = new HttpClient(new YouTubeCookieConsentHandler());
            var youtubeClient = new YoutubeClient(httpClientForYoutubeClient);

            var videoId = VideoId.Parse("https://www.youtube.com/watch?v=spVJOzF0EJ0");

            // Get media streams & choose the best muxed stream
            var streams = await youtubeClient.Videos.Streams.GetManifestAsync(videoId);
            var streamInfo = streams.GetAudioOnlyStreams().TryGetWithHighestBitrate();
            if (streamInfo is null)
            {
                Console.WriteLine("This video has no AudioOnlyStreams");
            }
            else
            {
                Console.WriteLine(streamInfo.Bitrate);
                Console.WriteLine(streamInfo.Url);

                var body = File.ReadAllText(@"C:\temp\base.js");

                var fixedUrl = new UrlDescrambler().Decode(body, streamInfo.Url);
                Console.WriteLine(fixedUrl);

                IStreamInfo fixedStreamInfo = new AudioOnlyStreamInfo(fixedUrl, streamInfo.Container, streamInfo.Size, streamInfo.Bitrate, fixedUrl);


                Console.WriteLine("DownloadAsync start " + DateTime.Now);
                //await youtubeClient.Videos.Streams.CopyToAsync(audio, destinationStream);
                await youtubeClient.Videos.Streams.DownloadAsync(fixedStreamInfo, "c:\\temp\\x.webm");
                Console.WriteLine("DownloadAsync end " + DateTime.Now);
            }
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
}