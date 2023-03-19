using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using RestEase;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace Blazor.YouTubeDownloader.Services;

[BasePath("api")]
public interface IYouTubeDownloadApi
{
    /// <summary>
    /// Returns the metadata for a YouTube video specified by its URL.
    /// </summary>
    [Get("GetVideoMetaData")]
    public Task<Video> GetVideoMetaDataAsync([Query] string youTubeUrl);

    /// <summary>
    /// Returns a collection of information about available audio-only streams for a YouTube video specified by its URL.
    /// </summary>
    [Get("GetAudioOnlyStreams")]
    public Task<IEnumerable<AudioOnlyStreamInfo>> GetAudioOnlyStreamsAsync([Query] string youTubeUrl);

    /// <summary>
    /// Returns a stream that contains the audio data for a specified audio-only stream.
    /// </summary>
    [Post("GetAudioStream")]
    public Task<Stream> GetAudioStreamAsync([Body] AudioOnlyStreamInfo streamInfo);

    /// <summary>
    /// Returns a stream that contains the audio data for a specified audio-only stream, encoded in the Ogg Opus format.
    /// </summary>
    [Post("GetOggOpusAudioStream")]
    public Task<Stream> GetOggOpusAudioStreamAsync([Body] AudioOnlyStreamInfo streamInfo);

    ///// <summary>
    ///// Returns the audio data for a specified audio-only stream as a byte array.
    ///// </summary>
    //[Post("GetAudioBytes")]
    //public Task<byte[]> GetAudioBytesAsync([Body] AudioOnlyStreamInfo streamInfo);
}