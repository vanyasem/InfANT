namespace InfANT
{
    partial class LoadingScreen
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoadingScreen));
            this.lab_Wait = new System.Windows.Forms.Label();
            this.ProgressLoading = new prgbar.BlueProgressBar();
            this.SuspendLayout();
            // 
            // lab_Wait
            // 
            resources.ApplyResources(this.lab_Wait, "lab_Wait");
            this.lab_Wait.Name = "lab_Wait";
            // 
            // ProgressLoading
            // 
            this.ProgressLoading.BackColor = System.Drawing.SystemColors.ControlLight;
            resources.ApplyResources(this.ProgressLoading, "ProgressLoading");
            this.ProgressLoading.Maximum = 100;
            this.ProgressLoading.Minimum = 0;
            this.ProgressLoading.Name = "ProgressLoading";
            this.ProgressLoading.ProgressBarColor = System.Drawing.Color.DodgerBlue;
            this.ProgressLoading.Value = 0;
            // 
            // LoadingScreen
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ProgressLoading);
            this.Controls.Add(this.lab_Wait);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "LoadingScreen";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.loadingscreen_FormClosing);
            this.Load += new System.EventHandler(this.loadingscreen_Load);
            this.Shown += new System.EventHandler(this.loadingscreen_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lab_Wait;
        private prgbar.BlueProgressBar ProgressLoading;
    }
}

