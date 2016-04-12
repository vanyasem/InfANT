namespace databaseworker
{
    partial class databaseeditor
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
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(345, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "This app gives you an ability to add some entries to your local database.";
            // 
            // btnSelectFile
            // 
            this.btnSelectFile.Enabled = false;
            this.btnSelectFile.Location = new System.Drawing.Point(283, 62);
            this.btnSelectFile.Name = "btnSelectFile";
            this.btnSelectFile.Size = new System.Drawing.Size(86, 24);
            this.btnSelectFile.TabIndex = 5;
            this.btnSelectFile.Text = "Select";
            this.btnSelectFile.UseVisualStyleBackColor = true;
            this.btnSelectFile.Click += new System.EventHandler(this.btnSelectFile_Click);
            // 
            // text_VirusPath
            // 
            this.text_VirusPath.Location = new System.Drawing.Point(16, 64);
            this.text_VirusPath.Name = "text_VirusPath";
            this.text_VirusPath.ReadOnly = true;
            this.text_VirusPath.Size = new System.Drawing.Size(261, 20);
            this.text_VirusPath.TabIndex = 4;
            this.text_VirusPath.Text = "Select the file                                     ---------------------->";
            // 
            // btnAddIt
            // 
            this.btnAddIt.Enabled = false;
            this.btnAddIt.Location = new System.Drawing.Point(15, 116);
            this.btnAddIt.Name = "btnAddIt";
            this.btnAddIt.Size = new System.Drawing.Size(149, 22);
            this.btnAddIt.TabIndex = 8;
            this.btnAddIt.Text = "Add it!";
            this.btnAddIt.UseVisualStyleBackColor = true;
            this.btnAddIt.Click += new System.EventHandler(this.btnAddIt_Click);
            // 
            // radioSusp
            // 
            this.radioSusp.AutoSize = true;
            this.radioSusp.Location = new System.Drawing.Point(3, 25);
            this.radioSusp.Name = "radioSusp";
            this.radioSusp.Size = new System.Drawing.Size(76, 17);
            this.radioSusp.TabIndex = 0;
            this.radioSusp.Text = "Suspicious";
            this.radioSusp.UseVisualStyleBackColor = true;
            // 
            // textSHA1
            // 
            this.textSHA1.Location = new System.Drawing.Point(16, 90);
            this.textSHA1.Name = "textSHA1";
            this.textSHA1.ReadOnly = true;
            this.textSHA1.Size = new System.Drawing.Size(261, 20);
            this.textSHA1.TabIndex = 6;
            // 
            // radioVirus
            // 
            this.radioVirus.AutoSize = true;
            this.radioVirus.Checked = true;
            this.radioVirus.Location = new System.Drawing.Point(3, 3);
            this.radioVirus.Name = "radioVirus";
            this.radioVirus.Size = new System.Drawing.Size(48, 17);
            this.radioVirus.TabIndex = 1;
            this.radioVirus.TabStop = true;
            this.radioVirus.Text = "Virus";
            this.radioVirus.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.radioVirus);
            this.panel1.Controls.Add(this.radioSusp);
            this.panel1.Location = new System.Drawing.Point(284, 90);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(84, 48);
            this.panel1.TabIndex = 7;
            // 
            // btnSelectAntivirus
            // 
            this.btnSelectAntivirus.Location = new System.Drawing.Point(283, 27);
            this.btnSelectAntivirus.Name = "btnSelectAntivirus";
            this.btnSelectAntivirus.Size = new System.Drawing.Size(85, 24);
            this.btnSelectAntivirus.TabIndex = 2;
            this.btnSelectAntivirus.Text = "Select";
            this.btnSelectAntivirus.UseVisualStyleBackColor = true;
            this.btnSelectAntivirus.Click += new System.EventHandler(this.button1_Click);
            // 
            // textPathToAntivirus
            // 
            this.textPathToAntivirus.Location = new System.Drawing.Point(16, 29);
            this.textPathToAntivirus.Name = "textPathToAntivirus";
            this.textPathToAntivirus.ReadOnly = true;
            this.textPathToAntivirus.Size = new System.Drawing.Size(261, 20);
            this.textPathToAntivirus.TabIndex = 1;
            this.textPathToAntivirus.Text = "Select a path to an antivirus               --------------------->";
            // 
            // Seperator
            // 
            this.Seperator.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Seperator.ForeColor = System.Drawing.Color.Gray;
            this.Seperator.Location = new System.Drawing.Point(-12, 56);
            this.Seperator.Name = "Seperator";
            this.Seperator.Size = new System.Drawing.Size(394, 1);
            this.Seperator.TabIndex = 3;
            // 
            // databaseeditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(380, 145);
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
            this.Name = "databaseeditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "InfANT local database editor";
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

