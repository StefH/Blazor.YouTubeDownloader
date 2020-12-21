using YoutubeExplode.Videos.Streams;

namespace Blazor.YouTubeDownloader.Extensions
{
    internal static class AudioOnlyStreamInfoExtensions
    {
        public static string GetTitle(this AudioOnlyStreamInfo info)
        {
            return $"{info.Container.Name} - {info.AudioCodec} - {info.Bitrate} - ({info.Size})";
        }
    }
}