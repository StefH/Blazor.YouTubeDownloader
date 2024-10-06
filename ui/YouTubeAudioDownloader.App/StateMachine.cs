using YouTubeAudioDownloader.App;

public class StateMachine
{
    private readonly Main _main;
    private State _currentState;

    public StateMachine(Main main)
    {
        _main = main;
        TransitionToState(State.Init);
    }

    public void TransitionToState(State newState)
    {
        _currentState = newState;
        UpdateUI();
    }

    private void UpdateUI()
    {
        switch (_currentState)
        {
            case State.Init:
                _main.btnDownload.Enabled = false;
                _main.lblInfo.Text = "Enter YouTube URL and click 'Download Manifest'.";
                _main.ClearPanel();
                break;

            case State.BeforeDownloadManifest:
                _main.ClearPanel();
                _main.txtYouTubeUrl.Enabled = false;
                _main.btnDownloadManifest.Enabled = false;
                _main.btnDownload.Enabled = false;
                _main.lblInfo.Text = "Downloading manifest...";
                _main.lblTitle.Text = string.Empty;
                _main.progressBar.Value = 0;
                break;

            case State.AfterDownloadManifest:
                _main.txtYouTubeUrl.Enabled = true;
                _main.btnDownloadManifest.Enabled = true;
                _main.btnDownload.Enabled = true;
                _main.lblInfo.Text = "Manifest downloaded. Select an Audio Stream and click 'Download'.";
                break;

            case State.BeforeDownload:
                _main.txtYouTubeUrl.Enabled = false; ;
                _main.btnDownloadManifest.Enabled = false;
                _main.progressBar.Value = 0;
                _main.btnDownload.Enabled = false;
                _main.lblInfo.Text = "Downloading Audio Stream...";
                break;

            case State.AfterDownload:
                _main.txtYouTubeUrl.Enabled = true;
                _main.btnDownloadManifest.Enabled = true;
                _main.progressBar.Value = 100;
                _main.btnDownload.Enabled = true;
                break;
        }
    }
}