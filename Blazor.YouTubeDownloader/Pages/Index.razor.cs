using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Blazor.DownloadFileFast.Interfaces;
using Blazor.YouTubeDownloader.Extensions;
using Blazor.YouTubeDownloader.Services;
using Microsoft.AspNetCore.Components;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace Blazor.YouTubeDownloader.Pages;

public partial class Index
{
    private static int NoSelection = -1;

    [Inject] 
    public IYouTubeDownloadApi YouTubeDownloadApi { get; set; } = null!;

    [Inject]
    public IBlazorDownloadFileService BlazorDownloadFileService { get; set; } = null!;

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

            await Task.WhenAll(taskVideoMetaData, taskAudioOnlyStreamInfos);

            VideoMetaData = await taskVideoMetaData;
            AudioOnlyStreamInfos = (await taskAudioOnlyStreamInfos).ToArray();

            var highest = AudioOnlyStreamInfos.TryGetWithHighestBitrate();
            CheckedAudioOnlyStreamInfoHashCode = highest?.GetHashCode() ?? NoSelection;
            OpusAudioStreamPresent = AudioOnlyStreamInfos.Any(a => a.IsOpus());
            ExtractOpus = OpusAudioStreamPresent;
        }
        finally
        {
            DownloadButtonEnabled = true;
            ProcessYouTubeUrlButtonEnabled = true;
        }
    }

    void OnAudioOnlyStreamCheckedValueChanged(int value)
    {
        CheckedAudioOnlyStreamInfoHashCode = value;

        var streamInfo = AudioOnlyStreamInfos.Single(x => x.GetHashCode() == value);
        OpusAudioStreamPresent = streamInfo.IsOpus();
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

            var stopwatch = new Stopwatch();

            stopwatch.Reset();
            stopwatch.Start();

            // bool oggOpusIsExtractedByServer = false;
            Stream stream;
            if (streamInfo.IsOpus() && ExtractOpus)
            {
                stream = await YouTubeDownloadApi.GetOggOpusAudioStreamAsync(streamInfo);
                // oggOpusIsExtractedByServer = true;
                stopwatch.Stop();
                Console.WriteLine("GetOggOpusAudioStreamAsync = " + stopwatch.Elapsed);
            }
            else
            {
                stream = await YouTubeDownloadApi.GetAudioStreamAsync(streamInfo);
                stopwatch.Stop();
                Console.WriteLine("GetAudioStreamAsync = " + stopwatch.Elapsed);
            }

            var filename = GetFileNameWithExtension(streamInfo);

            stopwatch.Reset();
            stopwatch.Start();
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
            stopwatch.Stop();
            Console.WriteLine("CopyToAsync = " + stopwatch.Elapsed);

            //byte[] array;
            //if (OpusAudioStreamPresent && ExtractOpus && !oggOpusIsExtractedByServer)
            //{
            //    using var opusStream = new MemoryStream();

            //    x.Start();
            //    memoryStream.Position = 0;
            //    MatroskaDemuxer.ExtractOggOpusAudio(memoryStream, opusStream);

            //    Console.WriteLine("ExtractOggOpusAudio and ToArray = " + x.Elapsed);

            //    array = opusStream.ToArray();
            //    x.Stop();
            //}
            //else
            //{
            //    x.Start();
            //    array = memoryStream.ToArray();
            //    x.Stop();
            //    Console.WriteLine("ToArray = " + x.Elapsed);
            //}

            stopwatch.Reset();
            stopwatch.Start();
            byte[] array = memoryStream.ToArray();
            stopwatch.Stop();
            Console.WriteLine("ToArray = " + stopwatch.Elapsed);

            stopwatch.Reset();
            stopwatch.Start();
            await BlazorDownloadFileService.DownloadFileAsync(filename, array);
            stopwatch.Stop();

            Console.WriteLine("Download = " + stopwatch.Elapsed);
        }
        finally
        {
            DownloadButtonEnabled = true;
        }
    }

    private string GetFileNameWithExtension(AudioOnlyStreamInfo streamInfo)
    {
        string extension = streamInfo.IsOpus() && ExtractOpus ? "opus" : streamInfo.Container.Name;

        string fileName = GetSafeFileName(VideoMetaData?.Title ?? HttpUtility.ParseQueryString(new Uri(YouTubeUrl).Query)["v"] ?? Path.GetRandomFileName());

        return $"{fileName}.{extension}";
    }

    private static string GetSafeFileName(string fileName)
    {
        return Regex.Replace(fileName, "[" + Regex.Escape(new string(Path.GetInvalidPathChars())) + "]", string.Empty, RegexOptions.IgnoreCase);
    }
}