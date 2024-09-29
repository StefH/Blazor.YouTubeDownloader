using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Web;
using Matroska.Muxer;
using YouTubeAudioDownloader.App.Extensions;
using YoutubeExplode;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace YouTubeAudioDownloader.App;

public partial class Main : Form
{
    private const bool ExtractOpus = true;

    private readonly StateMachine _stateMachine;
    private readonly YoutubeClient _youtubeClient = new();

    private string _youTubeUrl = string.Empty;
    private string? _title;
    private IEnumerable<AudioOnlyStreamInfo> _audioOnlyStreams = Array.Empty<AudioOnlyStreamInfo>();
    private AudioOnlyStreamInfo? _highestAudioStream;
    private AudioOnlyStreamInfo? _selectedAudioStream;
    private Video? _videoMetaData;


    public Main()
    {
        InitializeComponent();

        _stateMachine = new StateMachine(this);
    }

    public void ClearPanel()
    {
        // Unsubscribe from CheckedChanged event for existing radio buttons
        foreach (var radioButton in panelAudioStreams.Controls.OfType<RadioButton>())
        {
            radioButton.CheckedChanged -= RadioButton_CheckedChanged;
        }
        // Clear the panel controls
        panelAudioStreams.Controls.Clear();
    }

    private async void btnDownloadManifest_Click(object sender, EventArgs e)
    {
        _stateMachine.TransitionToState(State.BeforeDownloadManifest);
        _title = string.Empty;
        _youTubeUrl = txtYouTubeUrl.Text;

        try
        {
            var audioStreamsTask = Task.Run(async () =>
            {
                // Try multiple times to download the manifest to get all available audio streams
                var videoStreamsTasks = Enumerable.Range(0, 4)
                    .Select(_ => _youtubeClient.Videos.Streams.GetManifestAsync(_youTubeUrl).AsTask());

                var streamManifests = await Task.WhenAll(videoStreamsTasks);

                return streamManifests
                    .SelectMany(s => s.GetAudioOnlyStreams())
                    .DistinctBy(s => s.GetTitle())
                    .ToArray();
            });
            var videoMetaDataTask = _youtubeClient.Videos.GetAsync(_youTubeUrl).AsTask();

            await Task.WhenAll(audioStreamsTask, videoMetaDataTask);

            _videoMetaData = await videoMetaDataTask;
            _title = _videoMetaData.Title;

            _audioOnlyStreams = await audioStreamsTask;
            _highestAudioStream = _audioOnlyStreams.TryGetWithHighestBitrate() as AudioOnlyStreamInfo;
            _selectedAudioStream = _highestAudioStream;

            GenerateAudioStreamRadioButtons();
        }
        finally
        {
            lblTitle.Text = _title;
            _stateMachine.TransitionToState(State.AfterDownloadManifest);
        }
    }

    private async void btnDownload_Click(object sender, EventArgs e)
    {
        if (_selectedAudioStream == null)
        {
            lblInfo.Text = "Please select an audio stream to download.";
            return;
        }

        _stateMachine.TransitionToState(State.BeforeDownload);

        var folder = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
        try
        {
            var filename = GetFileNameWithExtension(_selectedAudioStream);
            var path = Path.Combine(folder, filename);

            await using var fileStream = new FileStream(path, FileMode.OpenOrCreate);
            if (_selectedAudioStream.IsOpus() && ExtractOpus)
            {
                Debug.WriteLine(DateTime.Now);
                using var destinationStream = new MemoryStream();
                await _youtubeClient.Videos.Streams.CopyToAsync(_selectedAudioStream, destinationStream, new DownloadProgress(this, 80));
                destinationStream.Position = 0;

                Debug.WriteLine(DateTime.Now);
                MatroskaDemuxer.ExtractOggOpusAudio(destinationStream, fileStream);
                Debug.WriteLine(DateTime.Now);
            }
            else
            {
                Debug.WriteLine(DateTime.Now);
                await _youtubeClient.Videos.Streams.CopyToAsync(_selectedAudioStream, fileStream, new DownloadProgress(this));
                Debug.WriteLine(DateTime.Now);
            }
        }
        finally
        {
            _stateMachine.TransitionToState(State.AfterDownload);
            lblInfo.Text = $"File saved to '{folder}'.";
        }
    }

    private void GenerateAudioStreamRadioButtons()
    {
        ClearPanel();

        var yOffset = 10;

        // Generate new radio buttons based on _audioOnlyStreamInfos
        foreach (var audioStream in _audioOnlyStreams)
        {
            var radioButton = new RadioButton
            {
                Text = audioStream.GetTitle(),
                Tag = audioStream,
                Location = new Point(5, yOffset),
                AutoSize = true,
                Checked = audioStream.Url == _highestAudioStream?.Url
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
            _selectedAudioStream = radioButton.Tag as AudioOnlyStreamInfo;
        }
    }

    private string GetFileNameWithExtension(AudioOnlyStreamInfo streamInfo)
    {
        var extension = streamInfo.IsOpus() && ExtractOpus ? "opus" : streamInfo.Container.Name;

        var fileName = GetSafeFileName(_title ?? HttpUtility.ParseQueryString(new Uri(_youTubeUrl).Query)["v"] ?? Path.GetRandomFileName());

        return $"{fileName} ({streamInfo.GetCodecAndBitrate()}).{extension}";
    }

    private static string GetSafeFileName(string fileName)
    {
        return Regex.Replace(fileName, "[" + Regex.Escape(new string(Path.GetInvalidPathChars())) + "]", string.Empty, RegexOptions.IgnoreCase);
    }

    private class DownloadProgress(Main main, double max = 100) : IProgress<double>
    {
        public void Report(double value)
        {
            main.progressBar.Value = Math.Clamp((int)(value * 100 * (max / 100)), 0, 100);
        }
    }

    private void progressBar_Click(object sender, EventArgs e)
    {

    }
}