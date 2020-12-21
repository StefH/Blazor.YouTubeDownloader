using System;
using System.IO;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace YoutubeExplodeConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("YoutubeClient");

            var youtube = new YoutubeClient();

            var streamManifest = await youtube.Videos.Streams.GetManifestAsync("spVJOzF0EJ0");

            // Highest bitrate audio-only stream
            var streamInfo = streamManifest.GetAudioOnly().WithHighestBitrate();

            if (streamInfo != null)
            {
                var progress1 = new Progress<FileCopyProgressInfo>();
                progress1.ProgressChanged += (sender, p) =>
                {
                    var x = (int) (p.TotalBytesCopied / (double) p.SourceLength * 100);
                    Console.WriteLine(x);
                };

                // Get the actual stream
                var stream = await youtube.Videos.Streams.GetAsync(streamInfo);

                await using var destinationStream = File.OpenWrite("T:\\Eh0N0Edlpng." + streamInfo.Container.Name);

                await stream.CopyToAsync(destinationStream, progress1);

                // Download the stream to file
                var progress2 = new Progress<double>();
                progress2.ProgressChanged += (sender, pct) =>
                {
                    Console.WriteLine(pct);
                };

                //await youtube.Videos.Streams.DownloadAsync(streamInfo, "T:\\spVJOzF0EJ0." + streamInfo.Container.Name, progress2);

                //var fileStream = File.OpenWrite("T:\\spVJOzF0EJ0." + streamInfo.Container.Name);
            }

            Console.WriteLine("YoutubeClient -- done");
        }
    }
}