using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using RestEase;
using YoutubeExplode.Videos.Streams;

namespace Blazor.YouTubeDownloader.Services
{
    [BasePath("api")]
    public interface IYouTubeDownloadApi
    {
        [Get("GetAudioOnlyStreams")]
        public Task<IEnumerable<AudioOnlyStreamInfo>> GetAudioOnlyStreamsAsync([Query] string youTubeUrl);

        [Get("GetStream")]
        public Task<Stream> GetStreamAsync([Body] IStreamInfo streamInfo);
    }
}