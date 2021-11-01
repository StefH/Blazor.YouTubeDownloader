using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
// using VideoLibrary;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;
using YouTubeUrlDecoder;

namespace YoutubeExplode.DemoConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //var youTube = YouTube.Default; // starting point for YouTube actions
            //var videos = await youTube.GetAllVideosAsync("https://www.youtube.com/watch?v=spVJOzF0EJ0"); // gets a Video object with info about the video

            //var v = videos.ToList();

            //var audio = v.Where(vi => vi.AdaptiveKind == AdaptiveKind.Audio).OrderByDescending(vi => vi.AudioBitrate).First();

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

                var fixedUrl = new UrlDecoder().Decode(streamInfo.Url);

                Console.WriteLine(fixedUrl);

                //var client = new HttpClient()
                //{
                //    DefaultRequestVersion = HttpVersion.Version30,
                //    DefaultVersionPolicy = HttpVersionPolicy.RequestVersionExact
                //};

                //Console.WriteLine("GetAsync start " + DateTime.Now);
                //var result = await client.GetByteArrayAsync(streamInfo.Url);
                //File.WriteAllBytes("c:\\temp\\x.web", result);

                //Console.WriteLine("GetAsync end " + DateTime.Now);


                var client = new HttpClient();

                Console.WriteLine("GetAsync start " + DateTime.Now);
                var result = await client.GetByteArrayAsync(fixedUrl);
                File.WriteAllBytes("c:\\temp\\x.webm", result);
                Console.WriteLine("GetAsync end " + DateTime.Now);


                //var destinationStream = new MemoryStream();



                //Console.WriteLine("CopyToAsync start " + DateTime.Now);
                //await youtubeClient.Videos.Streams.CopyToAsync(streamInfo, destinationStream);
                //Console.WriteLine("CopyToAsync end " + DateTime.Now);
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