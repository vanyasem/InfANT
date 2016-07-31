namespace launcher
{
    partial class Mainupdater
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.labLastText = new System.Windows.Forms.Label();
            this.labLastActual = new System.Windows.Forms.Label();
            this.btnLaunch = new System.Windows.Forms.Button();
            this.btnDatabaseEditor = new System.Windows.Forms.Button();
            this.btnFix = new System.Windows.Forms.Button();
            this.btnUninstall = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnChangelog = new System.Windows.Forms.Button();
            this.labInstalledText = new System.Windows.Forms.Label();
            this.labInstalledActual = new System.Windows.Forms.Label();
            this.labDownloadSizeText = new System.Windows.Forms.Label();
            this.labIsOutdatedText = new System.Windows.Forms.Label();
            this.labIsOutdatedTextActual = new System.Windows.Forms.Label();
            this.labDownloadSizeActual = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // labLastText
            // 
            this.labLastText.AutoSize = true;
            this.labLastText.Location = new System.Drawing.Point(16, 19);
            this.labLastText.Name = "labLastText";
            this.labLastText.Size = new System.Drawing.Size(67, 13);
            this.labLastText.TabIndex = 0;
            this.labLastText.Text = "Last version:";
            // 
            // labLastActual
            // 
            this.labLastActual.AutoSize = true;
            this.labLastActual.Location = new System.Drawing.Point(80, 19);
            this.labLastActual.Name = "labLastActual";
            this.labLastActual.Size = new System.Drawing.Size(51, 13);
            this.labLastActual.TabIndex = 1;
            this.labLastActual.Text = "unknown";
            // 
            // btnLaunch
            // 
            this.btnLaunch.Enabled = false;
            this.btnLaunch.Location = new System.Drawing.Point(77, 67);
            this.btnLaunch.Name = "btnLaunch";
            this.btnLaunch.Size = new System.Drawing.Size(197, 26);
            this.btnLaunch.TabIndex = 8;
            this.btnLaunch.Text = "Launch";
            this.btnLaunch.UseVisualStyleBackColor = true;
            this.btnLaunch.Click += new System.EventHandler(this.btnLaunch_Click);
            // 
            // btnDatabaseEditor
            // 
            this.btnDatabaseEditor.Enabled = false;
            this.btnDatabaseEditor.Location = new System.Drawing.Point(280, 97);
            this.btnDatabaseEditor.Name = "btnDatabaseEditor";
            this.btnDatabaseEditor.Size = new System.Drawing.Size(120, 26);
            this.btnDatabaseEditor.TabIndex = 13;
            this.btnDatabaseEditor.Text = "Database Editor";
            this.btnDatabaseEditor.UseVisualStyleBackColor = true;
            this.btnDatabaseEditor.Click += new System.EventHandler(this.btnDatabaseEditor_Click);
            // 
            // btnFix
            // 
            this.btnFix.Enabled = false;
            this.btnFix.Location = new System.Drawing.Point(280, 67);
            this.btnFix.Name = "btnFix";
            this.btnFix.Size = new System.Drawing.Size(120, 26);
            this.btnFix.TabIndex = 12;
            this.btnFix.Text = "Fix Application";
            this.btnFix.UseVisualStyleBackColor = true;
            this.btnFix.Click += new System.EventHandler(this.btnFix_Click);
            // 
            // btnUninstall
            // 
            this.btnUninstall.Enabled = false;
            this.btnUninstall.Location = new System.Drawing.Point(280, 37);
            this.btnUninstall.Name = "btnUninstall";
            this.btnUninstall.Size = new System.Drawing.Size(120, 26);
            this.btnUninstall.TabIndex = 11;
            this.btnUninstall.Text = "Uninstall Application";
            this.btnUninstall.UseVisualStyleBackColor = true;
            this.btnUninstall.Click += new System.EventHandler(this.btnUninstall_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.Enabled = false;
            this.btnUpdate.Location = new System.Drawing.Point(7, 67);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(64, 26);
            this.btnUpdate.TabIndex = 9;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnChangelog
            // 
            this.btnChangelog.Enabled = false;
            this.btnChangelog.Location = new System.Drawing.Point(280, 7);
            this.btnChangelog.Name = "btnChangelog";
            this.btnChangelog.Size = new System.Drawing.Size(120, 26);
            this.btnChangelog.TabIndex = 10;
            this.btnChangelog.Text = "Changelog";
            this.btnChangelog.UseVisualStyleBackColor = true;
            this.btnChangelog.Click += new System.EventHandler(this.btnChangelog_Click);
            // 
            // labInstalledText
            // 
            this.labInstalledText.AutoSize = true;
            this.labInstalledText.Location = new System.Drawing.Point(129, 19);
            this.labInstalledText.Name = "labInstalledText";
            this.labInstalledText.Size = new System.Drawing.Size(86, 13);
            this.labInstalledText.TabIndex = 2;
            this.labInstalledText.Text = "Installed version:";
            // 
            // labInstalledActual
            // 
            this.labInstalledActual.AutoSize = true;
            this.labInstalledActual.Location = new System.Drawing.Point(212, 19);
            this.labInstalledActual.Name = "labInstalledActual";
            this.labInstalledActual.Size = new System.Drawing.Size(51, 13);
            this.labInstalledActual.TabIndex = 3;
            this.labInstalledActual.Text = "unknown";
            // 
            // labDownloadSizeText
            // 
            this.labDownloadSizeText.AutoSize = true;
            this.labDownloadSizeText.Location = new System.Drawing.Point(4, 36);
            this.labDownloadSizeText.Name = "labDownloadSizeText";
            this.labDownloadSizeText.Size = new System.Drawing.Size(79, 13);
            this.labDownloadSizeText.TabIndex = 4;
            this.labDownloadSizeText.Text = "Download size:";
            // 
            // labIsOutdatedText
            // 
            this.labIsOutdatedText.AutoSize = true;
            this.labIsOutdatedText.Location = new System.Drawing.Point(146, 36);
            this.labIsOutdatedText.Name = "labIsOutdatedText";
            this.labIsOutdatedText.Size = new System.Drawing.Size(69, 13);
            this.labIsOutdatedText.TabIndex = 6;
            this.labIsOutdatedText.Text = "Is outdated?:";
            // 
            // labIsOutdatedTextActual
            // 
            this.labIsOutdatedTextActual.AutoSize = true;
            this.labIsOutdatedTextActual.Location = new System.Drawing.Point(212, 36);
            this.labIsOutdatedTextActual.Name = "labIsOutdatedTextActual";
            this.labIsOutdatedTextActual.Size = new System.Drawing.Size(51, 13);
            this.labIsOutdatedTextActual.TabIndex = 7;
            this.labIsOutdatedTextActual.Text = "unknown";
            // 
            // labDownloadSizeActual
            // 
            this.labDownloadSizeActual.AutoSize = true;
            this.labDownloadSizeActual.Location = new System.Drawing.Point(80, 36);
            this.labDownloadSizeActual.Name = "labDownloadSizeActual";
            this.labDownloadSizeActual.Size = new System.Drawing.Size(30, 13);
            this.labDownloadSizeActual.TabIndex = 5;
            this.labDownloadSizeActual.Text = "0 KB";
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(8, 98);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(265, 24);
            this.progressBar.TabIndex = 14;
            // 
            // mainupdater
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(407, 130);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.labDownloadSizeActual);
            this.Controls.Add(this.labIsOutdatedTextActual);
            this.Controls.Add(this.labIsOutdatedText);
            this.Controls.Add(this.labDownloadSizeText);
            this.Controls.Add(this.labInstalledActual);
            this.Controls.Add(this.labInstalledText);
            this.Controls.Add(this.btnChangelog);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.btnUninstall);
            this.Controls.Add(this.btnFix);
            this.Controls.Add(this.btnDatabaseEditor);
            this.Controls.Add(this.btnLaunch);
            this.Controls.Add(this.labLastActual);
            this.Controls.Add(this.labLastText);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Mainupdater";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "InfANT Launcher";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.mainupdater_FormClosing);
            this.Load += new System.EventHandler(this.mainupdater_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labLastText;
        private System.Windows.Forms.Label labLastActual;
        private System.Windows.Forms.Button btnLaunch;
        private System.Windows.Forms.Button btnDatabaseEditor;
        private System.Windows.Forms.Button btnFix;
        private System.Windows.Forms.Button btnUninstall;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnChangelog;
        private System.Windows.Forms.Label labInstalledText;
        private System.Windows.Forms.Label labInstalledActual;
        private System.Windows.Forms.Label labDownloadSizeText;
        private System.Windows.Forms.Label labIsOutdatedText;
        private System.Windows.Forms.Label labIsOutdatedTextActual;
        private System.Windows.Forms.Label labDownloadSizeActual;
        private System.Windows.Forms.ProgressBar progressBar;
    }
}

