namespace databaseworker
{
    partial class Databaseeditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Databaseeditor));
            this.label1 = new System.Windows.Forms.Label();
            this.btnSelectFile = new System.Windows.Forms.Button();
            this.text_VirusPath = new System.Windows.Forms.TextBox();
            this.btnAddIt = new System.Windows.Forms.Button();
            this.radioSusp = new System.Windows.Forms.RadioButton();
            this.textSHA1 = new System.Windows.Forms.TextBox();
            this.radioVirus = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnSelectAntivirus = new System.Windows.Forms.Button();
            this.textPathToAntivirus = new System.Windows.Forms.TextBox();
            this.Seperator = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // btnSelectFile
            // 
            resources.ApplyResources(this.btnSelectFile, "btnSelectFile");
            this.btnSelectFile.Name = "btnSelectFile";
            this.btnSelectFile.UseVisualStyleBackColor = true;
            this.btnSelectFile.Click += new System.EventHandler(this.btnSelectFile_Click);
            // 
            // text_VirusPath
            // 
            resources.ApplyResources(this.text_VirusPath, "text_VirusPath");
            this.text_VirusPath.Name = "text_VirusPath";
            this.text_VirusPath.ReadOnly = true;
            // 
            // btnAddIt
            // 
            resources.ApplyResources(this.btnAddIt, "btnAddIt");
            this.btnAddIt.Name = "btnAddIt";
            this.btnAddIt.UseVisualStyleBackColor = true;
            this.btnAddIt.Click += new System.EventHandler(this.btnAddIt_Click);
            // 
            // radioSusp
            // 
            resources.ApplyResources(this.radioSusp, "radioSusp");
            this.radioSusp.Name = "radioSusp";
            this.radioSusp.UseVisualStyleBackColor = true;
            // 
            // textSHA1
            // 
            resources.ApplyResources(this.textSHA1, "textSHA1");
            this.textSHA1.Name = "textSHA1";
            this.textSHA1.ReadOnly = true;
            // 
            // radioVirus
            // 
            resources.ApplyResources(this.radioVirus, "radioVirus");
            this.radioVirus.Checked = true;
            this.radioVirus.Name = "radioVirus";
            this.radioVirus.TabStop = true;
            this.radioVirus.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.radioVirus);
            this.panel1.Controls.Add(this.radioSusp);
            this.panel1.Name = "panel1";
            // 
            // btnSelectAntivirus
            // 
            resources.ApplyResources(this.btnSelectAntivirus, "btnSelectAntivirus");
            this.btnSelectAntivirus.Name = "btnSelectAntivirus";
            this.btnSelectAntivirus.UseVisualStyleBackColor = true;
            this.btnSelectAntivirus.Click += new System.EventHandler(this.button1_Click);
            // 
            // textPathToAntivirus
            // 
            resources.ApplyResources(this.textPathToAntivirus, "textPathToAntivirus");
            this.textPathToAntivirus.Name = "textPathToAntivirus";
            this.textPathToAntivirus.ReadOnly = true;
            // 
            // Seperator
            // 
            resources.ApplyResources(this.Seperator, "Seperator");
            this.Seperator.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Seperator.ForeColor = System.Drawing.Color.Gray;
            this.Seperator.Name = "Seperator";
            // 
            // Databaseeditor
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Seperator);
            this.Controls.Add(this.textPathToAntivirus);
            this.Controls.Add(this.btnSelectAntivirus);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.textSHA1);
            this.Controls.Add(this.btnAddIt);
            this.Controls.Add(this.text_VirusPath);
            this.Controls.Add(this.btnSelectFile);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Databaseeditor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.databaseeditor_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSelectFile;
        private System.Windows.Forms.TextBox text_VirusPath;
        private System.Windows.Forms.Button btnAddIt;
        private System.Windows.Forms.RadioButton radioSusp;
        private System.Windows.Forms.TextBox textSHA1;
        private System.Windows.Forms.RadioButton radioVirus;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnSelectAntivirus;
        private System.Windows.Forms.TextBox textPathToAntivirus;
        private System.Windows.Forms.Panel Seperator;
    }
}

