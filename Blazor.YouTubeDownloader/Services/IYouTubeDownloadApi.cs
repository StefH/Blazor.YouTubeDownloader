using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using RestEase;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace Blazor.YouTubeDownloader.Services
{
    [BasePath("api")]
    public interface IYouTubeDownloadApi
    {
        [Get("GetVideoMetaData")]
        public Task<Video> GetVideoMetaDataAsync([Query] string youTubeUrl);

        [Get("GetAudioOnlyStreams")]
        public Task<IEnumerable<AudioOnlyStreamInfo>> GetAudioOnlyStreamsAsync([Query] string youTubeUrl);

        [Post("GetAudioStream")]
        public Task<Stream> GetAudioStreamAsync([Body] AudioOnlyStreamInfo streamInfo);

        [Post("GetAudioBytes")]
        public Task<byte[]> GetAudioBytesAsync([Body] AudioOnlyStreamInfo streamInfo);
    }
}