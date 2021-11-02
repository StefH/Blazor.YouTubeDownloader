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
                //Console.WriteLine(streamInfo.Url);

                var memoryStream = new MemoryStream();

                var client = new HttpClient();
                //Stream stream = await client.GetStreamAsync(streamInfo.Url);

                ////long progress;
                //await stream.CopyToAsync(memoryStream, (e) =>
                //{
                //    //progress = 100 * e.TotalBytesRead / e.SourceLength;
                //    Console.WriteLine(e.TotalBytesRead);

                //    return Task.CompletedTask;
                //});

                var body = File.ReadAllText(@"C:\temp\base.js");

                //var testUrl = "https://r1---sn-32o-guh6.googlevideo.com/videoplayback?expire=1635823311&ei=b1qAYa_bA4OG7gPRqYzADg&ip=86.88.148.62&id=o-AGKyEPPjiInIru0u_Hwxgoigt0C2W4VNGViHkVZrVkX2&itag=248&aitags=133%2C134%2C135%2C136%2C137%2C160%2C242%2C243%2C244%2C247%2C248%2C278&source=youtube&requiressl=yes&mh=nj&mm=31%2C29&mn=sn-32o-guh6%2Csn-32o-5hnl&ms=au%2Crdu&mv=m&mvi=1&pl=26&gcr=nl&initcwndbps=1677500&vprv=1&mime=video%2Fwebm&ns=MjBiZLMhCi6j-Dsoq-6EsNIG&gir=yes&clen=11744985&dur=273.520&lmt=1633848781402471&mt=1635801414&fvip=7&keepalive=yes&fexp=24001373%2C24007246&c=WEB&txp=2316224&n=KHy-yHaPN1HvjA&sparams=expire%2Cei%2Cip%2Cid%2Caitags%2Csource%2Crequiressl%2Cgcr%2Cvprv%2Cmime%2Cns%2Cgir%2Cclen%2Cdur%2Clmt&lsparams=mh%2Cmm%2Cmn%2Cms%2Cmv%2Cmvi%2Cpl%2Cinitcwndbps&lsig=AG3C_xAwRgIhAL4TRGZp3V-51SXgIH-MPIhxEP90ojyp8-8jip8ZEfNzAiEA44Qc3kg3uplFAR8ZDoCZofQkp0lE_LixfFTwqy7qYAM%3D&alr=yes&sig=AOq0QJ8wRAIgSDU3nT-VrT_6r9RhbYLgkEk0Hhd6C2Fi6Wk9TLwa2FUCIB6p4xvBPLW7PdMoHI2aQ14-4aW28du1lebJbXIzV7hb&cpn=3IfT7jVqN-rankol&cver=2.20211101.01.00&range=1309434-3271894&rn=9&rbuf=27339";
                //var fixedUrl = new UrlDescrambler().Decode(body, testUrl);

                var fixedUrl = new UrlDescrambler().Decode(body, streamInfo.Url);

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


                IStreamInfo fixedStreamInfo = new AudioOnlyStreamInfo(fixedUrl, streamInfo.Container, streamInfo.Size, streamInfo.Bitrate, fixedUrl);
               // audio = streamInfo; // new AudioOnlyStreamInfo(fixedUrl, streamInfo.Container, streamInfo.Size, streamInfo.Bitrate, fixedUrl);

                //Console.WriteLine("GetAsync start " + DateTime.Now);
                //var result = await client.GetByteArrayAsync(fixedUrl);
                //File.WriteAllBytes("c:\\temp\\x.webm", result);
                //Console.WriteLine("GetAsync end " + DateTime.Now);


                var destinationStream = new MemoryStream();



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