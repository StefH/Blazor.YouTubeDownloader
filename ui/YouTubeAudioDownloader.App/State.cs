namespace YouTubeAudioDownloader.App;

public enum State
{
    Init,
    BeforeDownloadManifest,
    AfterDownloadManifest,
    BeforeDownload,
    AfterDownload
}