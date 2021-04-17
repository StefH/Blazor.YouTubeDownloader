using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
// using VideoLibrary;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

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
            var youtube = new YoutubeClient(httpClientForYoutubeClient);

            var videoId = new VideoId("https://www.youtube.com/watch?v=spVJOzF0EJ0");

            // Get media streams & choose the best muxed stream
            var streams = await youtube.Videos.Streams.GetManifestAsync(videoId);
            var streamInfo = streams.GetMuxed().WithHighestBitrate();
            if (streamInfo is null)
            {
                Console.WriteLine("This videos has no streams");
            }
            else
            {
                Console.WriteLine(streamInfo.Bitrate);
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