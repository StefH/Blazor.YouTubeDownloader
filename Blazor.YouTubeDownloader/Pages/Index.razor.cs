using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Blazor.YouTubeDownloader.Services;
using BlazorDownloadFile;
using Microsoft.AspNetCore.Components;
using MimeTypes;
using YoutubeExplode;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace Blazor.YouTubeDownloader.Pages
{
    public partial class Index
    {
        private static int NoSelection = -1;
        private static string AudioCodecOpus = "opus";

        [Inject]
        public IYouTubeDownloadApi YouTubeDownloadApi { get; set; }

        [Inject]
        public IBlazorDownloadFileService BlazorDownloadFileService { get; set; }

        public bool ProcessYouTubeUrlButtonEnabled = true;
                
        public bool DownloadButtonEnabled;

        public Video? VideoMetaData;

        public IEnumerable<AudioOnlyStreamInfo> AudioOnlyStreamInfos = Array.Empty<AudioOnlyStreamInfo>();

        public int CheckedAudioOnlyStreamInfoHashCode; // Workaround for https://github.com/stsrki/Blazorise/issues/1635

        public string YouTubeUrl { get; set; } = "https://www.youtube.com/watch?v=spVJOzF0EJ0";

        async Task ProcessYouTubeUrlAsync()
        {
            VideoMetaData = null;
            AudioOnlyStreamInfos = Array.Empty<AudioOnlyStreamInfo>();
            CheckedAudioOnlyStreamInfoHashCode = NoSelection;
            ProcessYouTubeUrlButtonEnabled = false;
            DownloadButtonEnabled = false;

            try
            {
                VideoMetaData = await YouTubeDownloadApi.GetVideoMetaDataAsync(YouTubeUrl);
                AudioOnlyStreamInfos = await YouTubeDownloadApi.GetAudioOnlyStreamsAsync(YouTubeUrl);

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

            DownloadButtonEnabled = false;

            try
            {
                var streamInfo = AudioOnlyStreamInfos.Single(x => x.GetHashCode() == CheckedAudioOnlyStreamInfoHashCode);

                var stream = await YouTubeDownloadApi.GetAudioStreamAsync(streamInfo);

                var (filename, contentType) = GetFilenameWithContentType(streamInfo);

                await BlazorDownloadFileService.DownloadFile(filename, stream, contentType);
            }
            finally
            {
                DownloadButtonEnabled = true;
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