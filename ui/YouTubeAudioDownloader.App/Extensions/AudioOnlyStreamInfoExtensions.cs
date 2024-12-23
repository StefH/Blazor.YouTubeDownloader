using YoutubeExplode.Videos.Streams;

namespace YouTubeAudioDownloader.App.Extensions;

internal static class AudioOnlyStreamInfoExtensions
{
    private const string AudioCodecOpus = "opus";

    public static string GetContainerAndAudioCodec(this AudioOnlyStreamInfo info)
    {
        return $"{info.Container.Name} - {info.AudioCodec}";
    }

    public static string GetTitle(this AudioOnlyStreamInfo info)
    {
        return $"{info.GetContainerAndAudioCodec()} - {Math.Round(info.Bitrate.KiloBitsPerSecond, 0)} kbps - ({Math.Round(info.Size.MegaBytes, 2)} MB)";
    }

    public static string GetCodecAndBitrate(this AudioOnlyStreamInfo info)
    {
        return $"{info.AudioCodec} - {Math.Round(info.Bitrate.KiloBitsPerSecond, 0)} kbps";
    }

    public static bool IsOpus(this AudioOnlyStreamInfo info)
    {
        return info.AudioCodec == AudioCodecOpus;
    }
}