using System;
using System.Threading.Tasks;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace YoutubeExplode.DemoConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var youtube = new YoutubeClient();

            var videoId = new VideoId("https://www.youtube.com/watch?v=TPrnSACiTJ4");

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
}