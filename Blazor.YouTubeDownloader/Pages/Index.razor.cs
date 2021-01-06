using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Blazor.DownloadFileFast.Interfaces;
using Blazor.YouTubeDownloader.Services;
using Matroska.Muxer;
using Microsoft.AspNetCore.Components;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace Blazor.YouTubeDownloader.Pages
{
    public partial class Index
    {
        private const string AudioCodecOpus = "opus";

        private static int NoSelection = -1;

        [Inject]
        public IYouTubeDownloadApi YouTubeDownloadApi { get; set; }

        [Inject]
        public IBlazorDownloadFileService BlazorDownloadFileService { get; set; }

        public string YouTubeUrl { get; set; } = "https://www.youtube.com/watch?v=spVJOzF0EJ0";

        public bool ProcessYouTubeUrlButtonEnabled = true;

        public bool DownloadButtonEnabled;

        public bool OpusAudioStreamPresent;

        public bool ExtractOpus;

        public Video? VideoMetaData;

        public IEnumerable<AudioOnlyStreamInfo> AudioOnlyStreamInfos = Array.Empty<AudioOnlyStreamInfo>();

        public int CheckedAudioOnlyStreamInfoHashCode; // Workaround for https://github.com/stsrki/Blazorise/issues/1635        

        public long Progress;

        async Task ProcessYouTubeUrlAsync()
        {
            VideoMetaData = null;
            AudioOnlyStreamInfos = Array.Empty<AudioOnlyStreamInfo>();
            CheckedAudioOnlyStreamInfoHashCode = NoSelection;
            ProcessYouTubeUrlButtonEnabled = false;
            DownloadButtonEnabled = false;
            Progress = 0;
            OpusAudioStreamPresent = false;
            ExtractOpus = false;

            try
            {
                var taskVideoMetaData = YouTubeDownloadApi.GetVideoMetaDataAsync(YouTubeUrl);
                var taskAudioOnlyStreamInfos = YouTubeDownloadApi.GetAudioOnlyStreamsAsync(YouTubeUrl);

                await Task.WhenAll(new Task[] { taskVideoMetaData, taskAudioOnlyStreamInfos });

                VideoMetaData = await taskVideoMetaData;
                AudioOnlyStreamInfos = await taskAudioOnlyStreamInfos;

                var highest = AudioOnlyStreamInfos.WithHighestBitrate();
                CheckedAudioOnlyStreamInfoHashCode = highest != null ? highest.GetHashCode() : NoSelection;
                OpusAudioStreamPresent = AudioOnlyStreamInfos.Any(a => a.AudioCodec == AudioCodecOpus);
                ExtractOpus = OpusAudioStreamPresent;
            }
            finally
            {
                DownloadButtonEnabled = true;
                ProcessYouTubeUrlButtonEnabled = true;
            }
        }

        async Task DownloadFileAsync()
        {
            if (CheckedAudioOnlyStreamInfoHashCode == NoSelection)
            {
                return;
            }

            Progress = 0;
            DownloadButtonEnabled = false;

            try
            {
                var streamInfo = AudioOnlyStreamInfos.Single(x => x.GetHashCode() == CheckedAudioOnlyStreamInfoHashCode);

                var x = new Stopwatch();

                x.Reset();
                x.Start();
                var stream = await YouTubeDownloadApi.GetAudioStreamAsync(streamInfo);
                x.Stop();

                Console.WriteLine("GetAudioStreamAsync = " + x.Elapsed);

                var filename = GetFileNameWithExtension(streamInfo);

                x.Reset();
                x.Start();
                using var memoryStream = new MemoryStream();

                long lastValue = 0;
                await stream.CopyToAsync(memoryStream, async (e) =>
                {
                    Progress = 100 * e.TotalBytesRead / e.SourceLength;

                    if (Progress != lastValue)
                    {
                        lastValue = Progress;
                        StateHasChanged();
                        await Task.Delay(5); // give the UI some time to catch up
                    }
                });
                x.Stop();
                Console.WriteLine("CopyToAsync = " + x.Elapsed);

                x.Reset();
                byte[] array;
                if (ExtractOpus)
                {
                    using var opusStream = new MemoryStream();

                    x.Start();
                    MatroskaDemuxer.ExtractOggOpusAudio(memoryStream, opusStream);
                    x.Stop();
                    Console.WriteLine("ExtractOggOpusAudio = " + x.Elapsed);

                    array = opusStream.ToArray();
                }
                else
                {
                    x.Start();
                    array = memoryStream.ToArray();
                    x.Stop();
                    Console.WriteLine("ToArray = " + x.Elapsed);
                }

                x.Reset();
                x.Start();
                await BlazorDownloadFileService.DownloadFileAsync(filename, array);
                x.Stop();

                Console.WriteLine("Download = " + x.Elapsed);
            }
            finally
            {
                DownloadButtonEnabled = true;
            }
        }

        private string GetFileNameWithExtension(AudioOnlyStreamInfo streamInfo)
        {
            string extension = streamInfo.AudioCodec == AudioCodecOpus && ExtractOpus ? AudioCodecOpus : streamInfo.Container.Name;

            string fileName = GetSafeFileName(VideoMetaData?.Title ?? HttpUtility.ParseQueryString(new Uri(YouTubeUrl).Query)["v"] ?? Path.GetRandomFileName());

            return $"{fileName}.{extension}";
        }

        private static string GetSafeFileName(string fileName)
        {
            return Regex.Replace(fileName, "[" + Regex.Escape(new string(Path.GetInvalidPathChars())) + "]", string.Empty, RegexOptions.IgnoreCase);
        }
    }
}