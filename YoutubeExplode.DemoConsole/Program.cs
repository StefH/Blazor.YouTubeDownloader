using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using YoutubeExplode.Extensions;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace YoutubeExplode.DemoConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var httpClientForYoutubeClient = new HttpClient();
            var youtubeClient = new YoutubeClient(httpClientForYoutubeClient);

            var videoId = VideoId.Parse("https://www.youtube.com/watch?v=spVJOzF0EJ0");

            var streamClient = youtubeClient.Videos.Streams;
            //var streamClientExposed = Exposed.From(streamClient);

            //var youtubeControllerExposed = Exposed.From(streamClientExposed._controller);
            //var watchPageExposed = Exposed.From(Exposed.From(youtubeControllerExposed.GetVideoWatchPageAsync(videoId, CancellationToken.None)).Result);
            //string playerSourceUrl = watchPageExposed.TryGetPlayerSourceUrl();
            //string playerSource = await httpClientForYoutubeClient.GetStringAsync(playerSourceUrl);

            var streams = await streamClient.GetManifestAndFixStreamUrlAsync(videoId);
            var streamInfo = streams.GetAudioOnlyStreams().TryGetWithHighestBitrate();
            if (streamInfo is null)
            {
                Console.WriteLine("This video has no AudioOnlyStreams");
            }
            else
            {
                Console.WriteLine(streamInfo.Bitrate);
                Console.WriteLine(streamInfo.Url);

                //  YoutubeController _controller;


                //var code = File.ReadAllText(@"C:\temp\base.js");

              //  var fixedUrl = UrlDescrambler.Decode(playerSource, streamInfo.Url); //new UrlDescrambler2().Decode(playerSource, streamInfo.Url);
                //Console.WriteLine(fixedUrl);

               // IStreamInfo fixedStreamInfo = new AudioOnlyStreamInfo(fixedUrl, streamInfo.Container, streamInfo.Size, streamInfo.Bitrate, fixedUrl);


                Console.WriteLine("DownloadAsync start " + DateTime.Now);
                //await youtubeClient.Videos.Streams.CopyToAsync(audio, destinationStream);
                await youtubeClient.Videos.Streams.DownloadAsync(streamInfo, "c:\\temp\\x.webm");
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