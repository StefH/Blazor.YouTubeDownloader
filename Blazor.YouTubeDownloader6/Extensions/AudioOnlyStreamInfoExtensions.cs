using YoutubeExplode.Videos.Streams;

namespace Blazor.YouTubeDownloader.Extensions
{
    internal static class AudioOnlyStreamInfoExtensions
    {
        private const string AudioCodecOpus = "opus";

        public static string GetTitle(this AudioOnlyStreamInfo info)
        {
            return $"{info.Container.Name} - {info.AudioCodec} - {info.Bitrate} - ({info.Size})";
        }

        public static bool IsOpus(this AudioOnlyStreamInfo info)
        {
            return info.AudioCodec == AudioCodecOpus;
        }
    }
}