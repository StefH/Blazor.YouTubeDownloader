using System;
using System.Threading.Tasks;
using Blazor.YouTubeDownloader.Services;
using BlazorDownloadFile;
using Microsoft.AspNetCore.Components;

namespace Blazor.YouTubeDownloader.Pages
{
    public partial class Index
    {
        [Inject]
        public IYouTubeDownloadApi YouTubeDownloadApi { get; set; }

        [Inject]
        public IBlazorDownloadFileService BlazorDownloadFileService { get; set; }
        
        public bool ProcessYouTubeUrlButtonEnabled = true;

        public string YouTubeUrl { get; set; } = "https://www.youtube.com/watch?v=spVJOzF0EJ0";

        async Task ProcessYouTubeUrlAsync()
        {
            ProcessYouTubeUrlButtonEnabled = false;

            try
            {
                var x = await YouTubeDownloadApi.GetAudioOnlyStreamsAsync(YouTubeUrl);
                int y = 0;
            }
            catch (Exception ex)
            {
                
            }
            finally
            {
                ProcessYouTubeUrlButtonEnabled = true;
            }
        }

        async Task DownloadFileAsync()
        {
            // string name = $"{Path.GetFileNameWithoutExtension(settings.FileName)}.zip";
            // await BlazorDownloadFileService.DownloadFile(name, bytes);
        }
    }
}