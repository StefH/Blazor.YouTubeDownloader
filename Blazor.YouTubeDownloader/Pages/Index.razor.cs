using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using Blazor.YouTubeDownloader.Services;
using BlazorDownloadFile;
using Microsoft.AspNetCore.Components;
using YoutubeExplode.Videos.Streams;

namespace Blazor.YouTubeDownloader.Pages
{
    public partial class Index
    {
        [Inject]
        public IYouTubeDownloadApi YouTubeDownloadApi { get; set; }

        [Inject]
        public IBlazorDownloadFileService BlazorDownloadFileService { get; set; }
        
        public bool ProcessYouTubeUrlButtonEnabled = true;

        public bool DownloadButtonEnabled;

        public IEnumerable<AudioOnlyStreamInfo> AudioOnlyStreamInfos = Array.Empty<AudioOnlyStreamInfo>();

        public AudioOnlyStreamInfo? CheckedAudioOnlyStreamInfo;

        public int DownloadProgress = 77;

        public string YouTubeUrl { get; set; } = "https://www.youtube.com/watch?v=spVJOzF0EJ0";

        async Task ProcessYouTubeUrlAsync()
        {
            AudioOnlyStreamInfos = Array.Empty<AudioOnlyStreamInfo>();
            CheckedAudioOnlyStreamInfo = null;
            ProcessYouTubeUrlButtonEnabled = false;
            DownloadButtonEnabled = false;

            try
            {
                AudioOnlyStreamInfos = await YouTubeDownloadApi.GetAudioOnlyStreamsAsync(YouTubeUrl);
            }
            finally
            {
                ProcessYouTubeUrlButtonEnabled = true;
            }
        }

        void OnCheckedValueChanged(AudioOnlyStreamInfo value)
        {
            DownloadButtonEnabled = true;
            StateHasChanged();
        }

        async Task DownloadFileAsync()
        {
            if (CheckedAudioOnlyStreamInfo == null)
            {
                return;
            }

            try
            {
                var stream = await YouTubeDownloadApi.GetStreamAsync(CheckedAudioOnlyStreamInfo);
                await BlazorDownloadFileService.DownloadFile(Path.GetFileNameWithoutExtension(YouTubeUrl), stream);
            }
            finally
            {
                ProcessYouTubeUrlButtonEnabled = true;
            }
        }
    }
}