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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Mainupdater));
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
            resources.ApplyResources(this.labLastText, "labLastText");
            this.labLastText.Name = "labLastText";
            // 
            // labLastActual
            // 
            resources.ApplyResources(this.labLastActual, "labLastActual");
            this.labLastActual.Name = "labLastActual";
            // 
            // btnLaunch
            // 
            resources.ApplyResources(this.btnLaunch, "btnLaunch");
            this.btnLaunch.Name = "btnLaunch";
            this.btnLaunch.UseVisualStyleBackColor = true;
            this.btnLaunch.Click += new System.EventHandler(this.btnLaunch_Click);
            // 
            // btnDatabaseEditor
            // 
            resources.ApplyResources(this.btnDatabaseEditor, "btnDatabaseEditor");
            this.btnDatabaseEditor.Name = "btnDatabaseEditor";
            this.btnDatabaseEditor.UseVisualStyleBackColor = true;
            this.btnDatabaseEditor.Click += new System.EventHandler(this.btnDatabaseEditor_Click);
            // 
            // btnFix
            // 
            resources.ApplyResources(this.btnFix, "btnFix");
            this.btnFix.Name = "btnFix";
            this.btnFix.UseVisualStyleBackColor = true;
            this.btnFix.Click += new System.EventHandler(this.btnFix_Click);
            // 
            // btnUninstall
            // 
            resources.ApplyResources(this.btnUninstall, "btnUninstall");
            this.btnUninstall.Name = "btnUninstall";
            this.btnUninstall.UseVisualStyleBackColor = true;
            this.btnUninstall.Click += new System.EventHandler(this.btnUninstall_Click);
            // 
            // btnUpdate
            // 
            resources.ApplyResources(this.btnUpdate, "btnUpdate");
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnChangelog
            // 
            resources.ApplyResources(this.btnChangelog, "btnChangelog");
            this.btnChangelog.Name = "btnChangelog";
            this.btnChangelog.UseVisualStyleBackColor = true;
            this.btnChangelog.Click += new System.EventHandler(this.btnChangelog_Click);
            // 
            // labInstalledText
            // 
            resources.ApplyResources(this.labInstalledText, "labInstalledText");
            this.labInstalledText.Name = "labInstalledText";
            // 
            // labInstalledActual
            // 
            resources.ApplyResources(this.labInstalledActual, "labInstalledActual");
            this.labInstalledActual.Name = "labInstalledActual";
            // 
            // labDownloadSizeText
            // 
            resources.ApplyResources(this.labDownloadSizeText, "labDownloadSizeText");
            this.labDownloadSizeText.Name = "labDownloadSizeText";
            // 
            // labIsOutdatedText
            // 
            resources.ApplyResources(this.labIsOutdatedText, "labIsOutdatedText");
            this.labIsOutdatedText.Name = "labIsOutdatedText";
            // 
            // labIsOutdatedTextActual
            // 
            resources.ApplyResources(this.labIsOutdatedTextActual, "labIsOutdatedTextActual");
            this.labIsOutdatedTextActual.Name = "labIsOutdatedTextActual";
            // 
            // labDownloadSizeActual
            // 
            resources.ApplyResources(this.labDownloadSizeActual, "labDownloadSizeActual");
            this.labDownloadSizeActual.Name = "labDownloadSizeActual";
            // 
            // progressBar
            // 
            resources.ApplyResources(this.progressBar, "progressBar");
            this.progressBar.Name = "progressBar";
            // 
            // Mainupdater
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
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

