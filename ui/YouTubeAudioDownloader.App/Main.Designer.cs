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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            btnDownloadManifest = new Button();
            txtYouTubeUrl = new TextBox();
            btnDownload = new Button();
            panelAudioStreams = new Panel();
            lblInfo = new Label();
            progressBar = new ProgressBar();
            lblYouTubeUrl = new Label();
            lblAudioStreams = new Label();
            lblTitle = new Label();
            SuspendLayout();
            // 
            // btnDownloadManifest
            // 
            btnDownloadManifest.Font = new Font("Segoe UI", 15F);
            btnDownloadManifest.Location = new Point(935, 70);
            btnDownloadManifest.Margin = new Padding(5);
            btnDownloadManifest.Name = "btnDownloadManifest";
            btnDownloadManifest.Size = new Size(300, 49);
            btnDownloadManifest.TabIndex = 0;
            btnDownloadManifest.Text = "Download Manifest";
            btnDownloadManifest.UseVisualStyleBackColor = true;
            btnDownloadManifest.Click += btnDownloadManifest_Click;
            // 
            // txtYouTubeUrl
            // 
            txtYouTubeUrl.Font = new Font("Segoe UI", 15F);
            txtYouTubeUrl.Location = new Point(70, 70);
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
            btnDownload.Location = new Point(935, 901);
            btnDownload.Margin = new Padding(5);
            btnDownload.Name = "btnDownload";
            btnDownload.Size = new Size(300, 49);
            btnDownload.TabIndex = 2;
            btnDownload.Text = "Download Audio";
            btnDownload.UseVisualStyleBackColor = true;
            btnDownload.Click += btnDownload_Click;
            // 
            // panelAudioStreams
            // 
            panelAudioStreams.BorderStyle = BorderStyle.FixedSingle;
            panelAudioStreams.Font = new Font("Segoe UI", 15F);
            panelAudioStreams.Location = new Point(70, 295);
            panelAudioStreams.Margin = new Padding(5);
            panelAudioStreams.Name = "panelAudioStreams";
            panelAudioStreams.Size = new Size(800, 555);
            panelAudioStreams.TabIndex = 3;
            // 
            // lblInfo
            // 
            lblInfo.AutoSize = true;
            lblInfo.Font = new Font("Segoe UI", 15F);
            lblInfo.Location = new Point(70, 961);
            lblInfo.Margin = new Padding(5, 0, 5, 0);
            lblInfo.Name = "lblInfo";
            lblInfo.Size = new Size(91, 41);
            lblInfo.TabIndex = 4;
            lblInfo.Text = "Info...";
            // 
            // progressBar
            // 
            progressBar.Location = new Point(70, 901);
            progressBar.Margin = new Padding(5);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(800, 42);
            progressBar.TabIndex = 5;
            progressBar.Click += progressBar_Click;
            // 
            // lblYouTubeUrl
            // 
            lblYouTubeUrl.AutoSize = true;
            lblYouTubeUrl.Location = new Point(70, 24);
            lblYouTubeUrl.Name = "lblYouTubeUrl";
            lblYouTubeUrl.Size = new Size(266, 41);
            lblYouTubeUrl.TabIndex = 6;
            lblYouTubeUrl.Text = "YouTube Video Url";
            // 
            // lblAudioStreams
            // 
            lblAudioStreams.AutoSize = true;
            lblAudioStreams.Location = new Point(70, 249);
            lblAudioStreams.Name = "lblAudioStreams";
            lblAudioStreams.Size = new Size(210, 41);
            lblAudioStreams.TabIndex = 7;
            lblAudioStreams.Text = "Audio Streams";
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(70, 159);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(95, 41);
            lblTitle.TabIndex = 8;
            lblTitle.Text = "Title...";
            // 
            // Main
            // 
            AutoScaleDimensions = new SizeF(17F, 41F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1405, 1019);
            Controls.Add(lblTitle);
            Controls.Add(lblAudioStreams);
            Controls.Add(lblYouTubeUrl);
            Controls.Add(progressBar);
            Controls.Add(lblInfo);
            Controls.Add(panelAudioStreams);
            Controls.Add(btnDownload);
            Controls.Add(txtYouTubeUrl);
            Controls.Add(btnDownloadManifest);
            Font = new Font("Segoe UI", 15F);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(5);
            Name = "Main";
            Text = "YouTube Audio Downloader";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label lblAudioStreams;
        public TextBox txtYouTubeUrl;
        public Panel panelAudioStreams;
        public Label lblInfo;
        public ProgressBar progressBar;
        public Label lblYouTubeUrl;
        public Label lblTitle;
        public Button btnDownloadManifest;
        public Button btnDownload;
    }
}
