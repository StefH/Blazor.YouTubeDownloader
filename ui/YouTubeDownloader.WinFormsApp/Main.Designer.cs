namespace YouTubeDownloader.WinFormsApp
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
            SuspendLayout();
            // 
            // btnDownloadManifest
            // 
            btnDownloadManifest.Font = new Font("Segoe UI", 15F);
            btnDownloadManifest.Location = new Point(986, 65);
            btnDownloadManifest.Name = "btnDownloadManifest";
            btnDownloadManifest.Size = new Size(180, 47);
            btnDownloadManifest.TabIndex = 0;
            btnDownloadManifest.Text = "Process";
            btnDownloadManifest.UseVisualStyleBackColor = true;
            btnDownloadManifest.Click += btnDownloadManifest_Click;
            // 
            // txtYouTubeUrl
            // 
            txtYouTubeUrl.Font = new Font("Segoe UI", 15F);
            txtYouTubeUrl.Location = new Point(45, 65);
            txtYouTubeUrl.Name = "txtYouTubeUrl";
            txtYouTubeUrl.Size = new Size(894, 47);
            txtYouTubeUrl.TabIndex = 1;
            txtYouTubeUrl.Text = "https://www.youtube.com/watch?v=spVJOzF0EJ0";
            // 
            // btnDownload
            // 
            btnDownload.Enabled = false;
            btnDownload.Font = new Font("Segoe UI", 15F);
            btnDownload.Location = new Point(986, 516);
            btnDownload.Name = "btnDownload";
            btnDownload.Size = new Size(180, 47);
            btnDownload.TabIndex = 2;
            btnDownload.Text = "Download";
            btnDownload.UseVisualStyleBackColor = true;
            btnDownload.Click += btnDownload_Click;
            // 
            // panelAudioStreams
            // 
            panelAudioStreams.BorderStyle = BorderStyle.FixedSingle;
            panelAudioStreams.Font = new Font("Segoe UI", 15F);
            panelAudioStreams.Location = new Point(45, 156);
            panelAudioStreams.Name = "panelAudioStreams";
            panelAudioStreams.Size = new Size(894, 407);
            panelAudioStreams.TabIndex = 3;
            // 
            // lblInfo
            // 
            lblInfo.AutoSize = true;
            lblInfo.Font = new Font("Segoe UI", 15F);
            lblInfo.Location = new Point(45, 493);
            lblInfo.Name = "lblInfo";
            lblInfo.Size = new Size(0, 41);
            lblInfo.TabIndex = 4;
            // 
            // Main
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1235, 670);
            Controls.Add(lblInfo);
            Controls.Add(panelAudioStreams);
            Controls.Add(btnDownload);
            Controls.Add(txtYouTubeUrl);
            Controls.Add(btnDownloadManifest);
            Name = "Main";
            Text = "YouTubeDownloader";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnDownloadManifest;
        private TextBox txtYouTubeUrl;
        private Button btnDownload;
        private Panel panelAudioStreams;
        private Label lblInfo;
    }
}
