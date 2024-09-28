using System.Text.RegularExpressions;
using System.Web;
using Matroska.Muxer;
using YouTubeDownloader.WinFormsApp.Extensions;
using YoutubeExplode;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace YouTubeDownloader.WinFormsApp;

public partial class Main : Form
{
    private const bool ExtractOpus = true;

    private readonly YoutubeClient _youtubeClient = new();

    private string _youTubeUrl = string.Empty;
    private StreamManifest? _videoStreams;
    private IEnumerable<AudioOnlyStreamInfo> _audioOnlyStreamInfos = Array.Empty<AudioOnlyStreamInfo>();
    private AudioOnlyStreamInfo? _highestAudioStreamInfo;
    private AudioOnlyStreamInfo? _selectedAudioStreamInfo;
    private Video? _videoMetaData;

    public Main()
    {
        InitializeComponent();
    }

    private async void btnDownloadManifest_Click(object sender, EventArgs e)
    {
        txtYouTubeUrl.Enabled = false;
        btnDownloadManifest.Enabled = false;
        btnDownload.Enabled = false;

        _youTubeUrl = txtYouTubeUrl.Text;

        try
        {
            var videoMetaDataTask = _youtubeClient.Videos.GetAsync(_youTubeUrl).AsTask();
            var videoStreamsTask = _youtubeClient.Videos.Streams.GetManifestAsync(_youTubeUrl).AsTask();

            await Task.WhenAll(videoMetaDataTask, videoStreamsTask);

            _videoMetaData = await videoMetaDataTask;
            _videoStreams = await videoStreamsTask;

            _audioOnlyStreamInfos = _videoStreams.GetAudioOnlyStreams().ToArray();
            _highestAudioStreamInfo = _audioOnlyStreamInfos.TryGetWithHighestBitrate() as AudioOnlyStreamInfo;
            _selectedAudioStreamInfo = _highestAudioStreamInfo;

            GenerateAudioStreamRadioButtons();
        }
        finally
        {
            txtYouTubeUrl.Enabled = true;
            btnDownloadManifest.Enabled = true;
            btnDownload.Enabled = true;
        }
    }

    private async void btnDownload_Click(object sender, EventArgs e)
    {
        lblInfo.Text = string.Empty;
        if (_selectedAudioStreamInfo == null)
        {
            lblInfo.Text = "Please select an audio stream to download.";
            return;
        }

        btnDownload.Enabled = false;

        var folder = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
        try
        {
            var filename = GetFileNameWithExtension(_selectedAudioStreamInfo);
            var path = Path.Combine(folder, filename);

            var fileStream = new FileStream(path, FileMode.OpenOrCreate);
            if (_selectedAudioStreamInfo.IsOpus() && ExtractOpus)
            {
                using var destinationStream = new MemoryStream();
                await _youtubeClient.Videos.Streams.CopyToAsync(_selectedAudioStreamInfo, destinationStream);
                destinationStream.Position = 0;

                MatroskaDemuxer.ExtractOggOpusAudio(destinationStream, fileStream);
            }
            else
            {
                await _youtubeClient.Videos.Streams.CopyToAsync(_selectedAudioStreamInfo, fileStream);
            }
        }
        finally
        {
            lblInfo.Text = $"File saved to '{folder}'.";
            btnDownload.Enabled = true;
        }
    }

    private void GenerateAudioStreamRadioButtons()
    {
        // Unsubscribe from CheckedChanged event for existing radio buttons
        foreach (Control control in panelAudioStreams.Controls)
        {
            if (control is RadioButton radioButton)
            {
                radioButton.CheckedChanged -= RadioButton_CheckedChanged;
            }
        }
        // Clear the panel controls
        panelAudioStreams.Controls.Clear();

        var yOffset = 10;

        // Generate new radio buttons based on _audioOnlyStreamInfos
        foreach (var audioStream in _audioOnlyStreamInfos.DistinctBy(a => a.GetTitle()))
        {
            var radioButton = new RadioButton
            {
                Text = audioStream.GetTitle(),
                Tag = audioStream,
                Location = new Point(0, yOffset),
                AutoSize = true,
                Checked = audioStream.Url == _highestAudioStreamInfo?.Url
            };

            radioButton.CheckedChanged += RadioButton_CheckedChanged;
            panelAudioStreams.Controls.Add(radioButton);
            yOffset += 45;
        }
    }

    private void RadioButton_CheckedChanged(object? sender, EventArgs e)
    {
        if (sender is RadioButton { Checked: true } radioButton)
        {
            _selectedAudioStreamInfo = radioButton.Tag as AudioOnlyStreamInfo;
        }
    }

    private string GetFileNameWithExtension(AudioOnlyStreamInfo streamInfo)
    {
        var extension = streamInfo.IsOpus() && ExtractOpus ? "opus" : streamInfo.Container.Name;

        var fileName = GetSafeFileName(_videoMetaData?.Title ?? HttpUtility.ParseQueryString(new Uri(_youTubeUrl).Query)["v"] ?? Path.GetRandomFileName());

        return $"{fileName}.{extension}";
    }

    private static string GetSafeFileName(string fileName)
    {
        return Regex.Replace(fileName, "[" + Regex.Escape(new string(Path.GetInvalidPathChars())) + "]", string.Empty, RegexOptions.IgnoreCase);
    }
}