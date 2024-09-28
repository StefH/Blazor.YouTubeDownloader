namespace YouTubeAudioDownloader.App
{
    partial class Main
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnDownloadManifest = new Button();
            txtYouTubeUrl = new TextBox();
            btnDownload = new Button();
            panelAudioStreams = new Panel();
            lblInfo = new Label();
            progressBar = new ProgressBar();
            SuspendLayout();
            // 
            // btnDownloadManifest
            // 
            btnDownloadManifest.Font = new Font("Segoe UI", 15F);
            btnDownloadManifest.Location = new Point(935, 107);
            btnDownloadManifest.Margin = new Padding(5);
            btnDownloadManifest.Name = "btnDownloadManifest";
            btnDownloadManifest.Size = new Size(200, 47);
            btnDownloadManifest.TabIndex = 0;
            btnDownloadManifest.Text = "Process";
            btnDownloadManifest.UseVisualStyleBackColor = true;
            btnDownloadManifest.Click += btnDownloadManifest_Click;
            // 
            // txtYouTubeUrl
            // 
            txtYouTubeUrl.Font = new Font("Segoe UI", 15F);
            txtYouTubeUrl.Location = new Point(76, 107);
            txtYouTubeUrl.Margin = new Padding(5);
            txtYouTubeUrl.Name = "txtYouTubeUrl";
            txtYouTubeUrl.Size = new Size(800, 47);
            txtYouTubeUrl.TabIndex = 1;
            txtYouTubeUrl.Text = "https://www.youtube.com/watch?v=spVJOzF0EJ0";
            // 
            // btnDownload
            // 
            btnDownload.Enabled = false;
            btnDownload.Font = new Font("Segoe UI", 15F);
            btnDownload.Location = new Point(935, 858);
            btnDownload.Margin = new Padding(5);
            btnDownload.Name = "btnDownload";
            btnDownload.Size = new Size(200, 47);
            btnDownload.TabIndex = 2;
            btnDownload.Text = "Download";
            btnDownload.UseVisualStyleBackColor = true;
            btnDownload.Click += btnDownload_Click;
            // 
            // panelAudioStreams
            // 
            panelAudioStreams.BorderStyle = BorderStyle.FixedSingle;
            panelAudioStreams.Font = new Font("Segoe UI", 15F);
            panelAudioStreams.Location = new Point(76, 196);
            panelAudioStreams.Margin = new Padding(5);
            panelAudioStreams.Name = "panelAudioStreams";
            panelAudioStreams.Size = new Size(800, 600);
            panelAudioStreams.TabIndex = 3;
            // 
            // lblInfo
            // 
            lblInfo.AutoSize = true;
            lblInfo.Font = new Font("Segoe UI", 15F);
            lblInfo.Location = new Point(76, 920);
            lblInfo.Margin = new Padding(5, 0, 5, 0);
            lblInfo.Name = "lblInfo";
            lblInfo.Size = new Size(0, 41);
            lblInfo.TabIndex = 4;
            // 
            // progressBar
            // 
            progressBar.Location = new Point(76, 858);
            progressBar.Margin = new Padding(5);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(800, 42);
            progressBar.TabIndex = 5;
            progressBar.Click += progressBar_Click;
            // 
            // Main
            // 
            AutoScaleDimensions = new SizeF(17F, 41F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1209, 1018);
            Controls.Add(progressBar);
            Controls.Add(lblInfo);
            Controls.Add(panelAudioStreams);
            Controls.Add(btnDownload);
            Controls.Add(txtYouTubeUrl);
            Controls.Add(btnDownloadManifest);
            Font = new Font("Segoe UI", 15F);
            Margin = new Padding(5);
            Name = "Main";
            Text = "YouTube Audio Downloader";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnDownloadManifest;
        private TextBox txtYouTubeUrl;
        private Button btnDownload;
        private Panel panelAudioStreams;
        private Label lblInfo;
        private ProgressBar progressBar;
    }
}
