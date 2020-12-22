using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
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
                CheckedAudioOnlyStreamInfo = (AudioOnlyStreamInfo?)AudioOnlyStreamInfos.WithHighestBitrate();
            }
            finally
            {
                DownloadButtonEnabled = true;
                ProcessYouTubeUrlButtonEnabled = true;
            }
        }

        void OnCheckedValueChanged(AudioOnlyStreamInfo value)
        {
            StateHasChanged();
        }

        async Task DownloadFileAsync()
        {
            if (CheckedAudioOnlyStreamInfo == null)
            {
                return;
            }

            DownloadButtonEnabled = false;

            try
            {
                var stream = await YouTubeDownloadApi.GetStreamAsync(CheckedAudioOnlyStreamInfo);

                string name = $"{HttpUtility.ParseQueryString(new Uri(YouTubeUrl).Query)["v"]}.{CheckedAudioOnlyStreamInfo.Container.Name}";
                await BlazorDownloadFileService.DownloadFile(name, stream);
            }
            finally
            {
                DownloadButtonEnabled = true;
            }
        }
    }
}