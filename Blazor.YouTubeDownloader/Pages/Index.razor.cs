using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Blazor.YouTubeDownloader.Services;
using BlazorDownloadFile;
using BlazorFileSaver;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MimeTypes;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace Blazor.YouTubeDownloader.Pages
{
    public partial class Index
    {
        private static int NoSelection = -1;
        private static string AudioCodecOpus = "opus";

        [Inject]
        public IJSRuntime js { get; set; }

        [Inject]
        public IYouTubeDownloadApi YouTubeDownloadApi { get; set; }

        [Inject]
        public IBlazorDownloadFileService BlazorDownloadFileService { get; set; }

        [Inject]
        private IBlazorFileSaver BlazorFileSaver { get; set; }

        public bool ProcessYouTubeUrlButtonEnabled = true;

        public bool DownloadButtonEnabled;

        public Video? VideoMetaData;

        public IEnumerable<AudioOnlyStreamInfo> AudioOnlyStreamInfos = Array.Empty<AudioOnlyStreamInfo>();

        public int CheckedAudioOnlyStreamInfoHashCode; // Workaround for https://github.com/stsrki/Blazorise/issues/1635

        public string YouTubeUrl { get; set; } = "https://www.youtube.com/watch?v=spVJOzF0EJ0";

        public string Progress = "";

        async Task ProcessYouTubeUrlAsync()
        {
            VideoMetaData = null;
            AudioOnlyStreamInfos = Array.Empty<AudioOnlyStreamInfo>();
            CheckedAudioOnlyStreamInfoHashCode = NoSelection;
            ProcessYouTubeUrlButtonEnabled = false;
            DownloadButtonEnabled = false;

            try
            {
                var taskVideoMetaData = YouTubeDownloadApi.GetVideoMetaDataAsync(YouTubeUrl);
                var taskAudioOnlyStreamInfos = YouTubeDownloadApi.GetAudioOnlyStreamsAsync(YouTubeUrl);

                await Task.WhenAll(new Task[] { taskVideoMetaData, taskAudioOnlyStreamInfos });

                VideoMetaData = await taskVideoMetaData;
                AudioOnlyStreamInfos = await taskAudioOnlyStreamInfos;

                //VideoMetaData = await YouTubeDownloadApi.GetVideoMetaDataAsync(YouTubeUrl);
                //AudioOnlyStreamInfos = await YouTubeDownloadApi.GetAudioOnlyStreamsAsync(YouTubeUrl);

                var highest = AudioOnlyStreamInfos.WithHighestBitrate();
                CheckedAudioOnlyStreamInfoHashCode = highest != null ? highest.GetHashCode() : NoSelection;
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

            Progress = "";
            DownloadButtonEnabled = false;

            try
            {
                var streamInfo = AudioOnlyStreamInfos.Single(x => x.GetHashCode() == CheckedAudioOnlyStreamInfoHashCode);

                var x = new Stopwatch();
                x.Start();

                var data = await YouTubeDownloadApi.GetAudioStreamAsync(streamInfo);
                // var data = await YouTubeDownloadApi.GetAudioBytesAsync(streamInfo);

                var (filename, contentType) = GetFilenameWithContentType(streamInfo);

                var p = new Progress<FileCopyProgressInfo>();

                p.ProgressChanged += (object? sender, FileCopyProgressInfo e) =>
                {
                    InvokeAsync(() =>
                    {
                        Progress = $"{100 * e.TotalBytesCopied / e.SourceLength}%";

                        StateHasChanged();
                        Console.WriteLine(Progress);
                        Task.Delay(1);  // give the UI some time to catch up
                    });

                };

                //await BlazorDownloadFileService.DownloadFile(filename, data, 3276800, p, contentType);

                using var ms = new MemoryStream();

                //await data.CopyToAsync(ms, p);

                //await InvokeAsync(async () =>
                //{
                    await data.CopyToAsync(ms, async (e) =>
                    {
                        Progress = $"{100 * e.TotalBytesCopied / e.SourceLength}%";

                        StateHasChanged();
                        await Task.Delay(5); // give the UI some time to catch up

                        //await InvokeAsync(async () =>
                        //{
                        //    Progress = $"{100 * e.TotalBytesCopied / e.SourceLength}%";

                        //    //StateHasChanged();
                        //    InvokeAsync(() => StateHasChanged());
                        //    Console.WriteLine(Progress);
                        //});
                    });
                //});


                x.Stop();

                Console.WriteLine(x.Elapsed);

                x.Start();
                var array = ms.ToArray();
                // var b = Convert.ToBase64String(array);
                x.Stop();

                Console.WriteLine("ToArray = " + x.Elapsed);

                //    var b = await data.ToByteArrayAsync();

                x.Start();

                //await BlazorFileSaver.SaveAsBase64(filename, b, contentType);

                //await js.InvokeVoidAsync("saveAsFile", filename, contentType, b);

                // await BlazorDownloadFileService.DownloadFile(filename, b, contentType);

                await DownloadAsync(filename, contentType, array);

                x.Stop();

                Console.WriteLine("Download = " + x.Elapsed);

            }
            finally
            {
                DownloadButtonEnabled = true;
            }
        }

        public async Task DownloadAsync(string fileName, string contentType, byte[] file)
        {
            // Check if the IJSRuntime is the WebAssembly implementation of the JSRuntime
            if (js is IJSUnmarshalledRuntime webAssemblyJSRuntime)
            {
                Console.WriteLine("Using BlazorDownloadFileFast");
                webAssemblyJSRuntime.InvokeUnmarshalled<string, string, byte[], bool>("BlazorDownloadFileFast", fileName, contentType, file);
            }
            else
            {
                // Fall back to the slow method if not in WebAssembly
                Console.WriteLine("Using BlazorDownloadFile");
                await js.InvokeVoidAsync("BlazorDownloadFile", fileName, contentType, file);
            }
        }

        private (string Filename, string ContentType) GetFilenameWithContentType(AudioOnlyStreamInfo streamInfo)
        {
            string extension = streamInfo.AudioCodec == AudioCodecOpus ? AudioCodecOpus : streamInfo.Container.Name;

            string fileName = GetSafeFileName(VideoMetaData?.Title ?? HttpUtility.ParseQueryString(new Uri(YouTubeUrl).Query)["v"] ?? Path.GetRandomFileName());

            return ($"{fileName}.{extension}", MimeTypeMap.GetMimeType(extension));
        }

        private static string GetSafeFileName(string fileName)
        {
            return Regex.Replace(fileName, "[" + Regex.Escape(new string(Path.GetInvalidPathChars())) + "]", string.Empty, RegexOptions.IgnoreCase);
        }
    }
}