using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.InteropServices;
using InfANT.Properties;

namespace antivirus
{
    public partial class main : Form
    {
        //------------------------------------
        //VARS
        public bool ?_infected = false; //determines whether the PC is infected or not, this one is a var.
        public bool ?infected { //this one is a function that triggers every time infected is changed, uses _infected as a var.
            get { return _infected; } 
            set { _infected = value; loadings.changeIco(); if (value == true) { 
                                                                  labelSafeUnsafeWelcome.Invoke(new MethodInvoker(
                                                                    delegate { labelSafeUnsafeWelcome.Text = "Your computer is INFECTED"; 
                                                                               labelSafeUnsafeWelcome.ForeColor = Color.Red; }));
                                                                  loadings.notifyIcon1.Text = "Your computer is INFECTED";
                                                                              }  

                                                           else {
                                                               if (value == null) //null = susp
                                                               {
                                                                   labelSafeUnsafeWelcome.Invoke(new MethodInvoker(
                                                                        delegate
                                                                        {
                                                                            labelSafeUnsafeWelcome.Text = "Your computer is partially safe";
                                                                            labelSafeUnsafeWelcome.ForeColor = Color.DarkOrange;
                                                                        }));
                                                                   loadings.notifyIcon1.Text = "Your computer is partially safe";
                                                               }
                                                               else
                                                               { 

                                                                      labelSafeUnsafeWelcome.Invoke(new MethodInvoker(
                                                                        delegate { labelSafeUnsafeWelcome.Text = "Your computer is safe";
                                                                                   labelSafeUnsafeWelcome.ForeColor = Color.FromArgb(82, 180, 60); }));
                                                                      loadings.notifyIcon1.Text = "Your computer is safe";
                                                               } 
                                                                } //end first else
                } 
                             }

        int scanned     { get { return _scanned; } //determines how many files were scanned, function, triggers on change
                          set { _scanned = value; 
                                labScannedNum.Invoke((new MethodInvoker(delegate { labScannedNum.Text = value.ToString() + "/" + overall; }))); 
                              } 
                        }
        int _scanned = 0; //determines how many files were scanned using advanced folder scan, var

        int scannedFast { get { return _scannedFast; } //determines how many files were scanned by the fast scanner, function, triggers on change
                          set { _scannedFast = value;
                               labScannedFastNum.Invoke((new MethodInvoker(delegate { labScannedFastNum.Text = value.ToString() + "/" + overallfast; }))); 
                              } 
                        }
        int _scannedFast = 0; //determines how many files were scanned by the fast scanner, var

        int scannedFull {
                         get { return _scannedFull; } //determines how many files were scanned by the fast scanner, function, triggers on change
                         set { _scannedFull = value;
                             labScannedFullNum.Invoke((new MethodInvoker(delegate { labScannedFullNum.Text = value.ToString() + "/" + overallfull; })));
                             }
                        }
        int _scannedFull = 0; //determines how many files were scanned by the fast scanner, var

        int overall = 0; //determines how many files were SELECTED (i.e. how many files have to be scanned) using advanced folder scan, var
        int overallfast = 0;
        int overallfull = 0;

        public bool logonlyimportant     = true; //used for CHANGELOG, switches important/all modes
        public bool _isinternetconnected = true; //determines whether the PC is conneted to the internet or not, this one is a var.
        public bool isinternetconnected { get { return _isinternetconnected; } set { _isinternetconnected = value; InternetConnectionActions(value); } }
                             // ^ this is a function that triggers every time isinternetconnected is changed, uses _isinternetconnected as a var.

        public List<string> hashes  = new List<string>(); //a list of SHA1 hashes for scanning
        public List<string> susphashes = new List<string>(); //a list of suspisious hashes

        public string ver   = "2.0.0.1";//this string indicates the current version of the app
        public string build = "RELEASE";
        
        Thread FoldScanning; //This thread is used for scanning files in a folder
        Thread FileCounting; //This thread is used for counting files in a folder
        //------------------------------------
        //END VARS



        //MESS
        //------------------------------------
        private loadingscreen loadings; //used to access loadingscreen
        public main(loadingscreen loadingscr) //recieves an instance of a loading screen
        {
            InitializeComponent();
            loadings = loadingscr; //makes it avalible to use whithin the whole form
        }
        private void main_Load(object sender, EventArgs e) //scrolls the text to end at form start 
        {                                                  //(have to do this because if the form is not shown "TextChanged" event won't fire
            textChangelog.SelectionStart = textChangelog.TextLength;
            textChangelog.ScrollToCaret();

            if (loadings.Suspiciouslogs.Count > 0)
                infected = null;
            if (loadings.Viruseslogs.Count > 0)
            {
                infected = true;
                loadings.changeIco();
            }  
        }
        private void textChangelog_TextChanged(object sender, EventArgs e) //scrolls the text to end at text change
        {
            textChangelog.SelectionStart = textChangelog.TextLength;
            textChangelog.ScrollToCaret(); 
        }
        private void tabScans_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnDeleteThisVirus.Visible = false;
            if (!isscanning)
            {
                if (tabScans.SelectedIndex == 3 || tabScans.SelectedIndex == 4)
                {
                    loadings.readLogs(0);
                    if (treeHistoryViruses.Nodes.Count == 0)
                    {
                        btnClearVirusesLog.Enabled = false;
                        richTextVirusesHistory.Text = "No viruses found! Hooray!";
                    }
                    else
                    {
                        btnClearVirusesLog.Enabled = true;
                        richTextVirusesHistory.Text = "Select the virus in the tree to see the detailed information about it";
                    }

                    if (treeHistoryScans.Nodes.Count == 0)
                    {
                        btnClearScansLog.Enabled = false;
                        richTextScansHistory.Text = "No scans were performed!";
                    }
                    else
                    {
                        btnClearScansLog.Enabled = true;
                        richTextScansHistory.Text = "Select the scan in the tree to see the detailed information about it";
                    }
                }
                
                infected = false;
                if (loadings.Suspiciouslogs.Count > 0)
                    infected = null;
                if (loadings.Viruseslogs.Count > 0)
                {
                    infected = true;
                }
                loadings.changeIco();

                if(tabScans.SelectedIndex == 1)
                {
                    comboDriveSelect.Items.Clear();
                    string[] drives = System.IO.Directory.GetLogicalDrives();
                    foreach(string str in drives)
                    {
                        comboDriveSelect.Items.Add(str);
                    }
                    btnFullScan.Enabled = false;
                    labSelectTheDrive.Visible = true;
                }
            }

        }

        private void main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(!isexited)
            {
                e.Cancel = true;
                this.Hide();
            }         
        }
        //------------------------------------
        //END MESS



        //TABS
        //------------------------------------    
        private void tabMainMenu_DrawItem(object sender, DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;
            Brush _textBrush;

            // Get the item from the collection.
            TabPage _tabPage = tabMainMenu.TabPages[e.Index];

            // Get the real bounds for the tab rectangle.
            Rectangle _tabBounds = tabMainMenu.GetTabRect(e.Index);
            var Fillbrush = new SolidBrush(Color.FromArgb(255, (byte)59, (byte)105, (byte)177));
            //https://stackoverflow.com/questions/16240581/how-to-get-a-brush-from-a-rgb-code

            if (e.State == DrawItemState.Selected)
            {
                // Draw a different background color, and don't paint a focus rectangle.
                _textBrush = new SolidBrush(Color.White);
                g.FillRectangle(Fillbrush, e.Bounds);
            }
            else
            {
                _textBrush = new System.Drawing.SolidBrush(e.ForeColor);
                e.DrawBackground();
            }

            // Use our own font.
            Font _tabFont = new Font("Arial", (float)10.0, FontStyle.Bold, GraphicsUnit.Pixel);

            // Draw string. Center the text.
            StringFormat _stringFlags = new StringFormat();
            _stringFlags.Alignment = StringAlignment.Center;
            _stringFlags.LineAlignment = StringAlignment.Center;
            g.DrawString(_tabPage.Text, _tabFont, _textBrush, _tabBounds, new StringFormat(_stringFlags));
        }

        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;
            Brush _textBrush;

            // Get the item from the collection.
            TabPage _tabPage = tabScans.TabPages[e.Index];

            // Get the real bounds for the tab rectangle.
            Rectangle _tabBounds = tabScans.GetTabRect(e.Index);
            var Fillbrush = new SolidBrush(Color.FromArgb(255, (byte)59, (byte)105, (byte)177));
            //https://stackoverflow.com/questions/16240581/how-to-get-a-brush-from-a-rgb-code

            if (e.State == DrawItemState.Selected)
            {
                // Draw a different background color, and don't paint a focus rectangle.
                _textBrush = new SolidBrush(Color.White);
                g.FillRectangle(Fillbrush, e.Bounds);
            }
            else
            {
                _textBrush = new System.Drawing.SolidBrush(e.ForeColor);
                e.DrawBackground();
            }

            // Use our own font.
            Font _tabFont = new Font("Arial", (float)10.0, FontStyle.Bold, GraphicsUnit.Pixel);

            // Draw string. Center the text.
            StringFormat _stringFlags = new StringFormat();
            _stringFlags.Alignment = StringAlignment.Center;
            _stringFlags.LineAlignment = StringAlignment.Center;
            g.DrawString(_tabPage.Text, _tabFont, _textBrush, _tabBounds, new StringFormat(_stringFlags));
        }
        //------------------------------------
        //END TABS




        //INTERNET
        //-----------------------------------
        private void InternetConnectionActions(bool isworking) //Enables/disables retry controls on trigger.
        {
            if (isworking == false)
            {
                labNotConnected.Visible  = true;
                btnRetryInternet.Visible = true;
            }
            else
            {
                labNotConnected.Visible  = false;
                btnRetryInternet.Visible = false;
            }
        }
        private void btnRetryInternet_Click(object sender, EventArgs e) 
        {
            RetryInt(); 
        }
        public void RetryInt() //Tries to update internet-based things
        {
            isinternetconnected = true; //sets the var to true, no matter what happpens
            loadings.updatedatabase();  //these functions by itself will determine whether
        }
        //-----------------------------------
        //END INTERNET




        //SCANNING
        //---------------------------------
        private void treeHistoryScans_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Name != "date")
            {                                                                                                         //ADV FILE     //ADV FOLDER
                string tmp  = loadings.getSquareBrackets(loadings.actionscontainer[Convert.ToInt16(e.Node.Name)], 1); //action name  //action name
                string tmp2 = loadings.getSquareBrackets(loadings.actionscontainer[Convert.ToInt16(e.Node.Name)], 3); //path         //path
                
                if (tmp.StartsWith("S"))
                {
                    string tmp3 = loadings.getSquareBrackets(loadings.actionscontainer[Convert.ToInt16(e.Node.Name)], 5); //how ended
                    string tmp4 = loadings.getSquareBrackets(loadings.actionscontainer[Convert.ToInt16(e.Node.Name)], 7); //how many files were scanned     
                    richTextScansHistory.Text = tmp.Remove(0, 1) + "\r\n" + "\r\n" + "Selected path: " + tmp2 + "\r\n" + tmp3 + "\r\n" + "\r\n" + "Amount of files scanned (n of N): " + tmp4;
                }

                if (tmp.StartsWith("F"))
                {
                    string tmp3 = loadings.getSquareBrackets(loadings.actionscontainer[Convert.ToInt16(e.Node.Name)], 5); //file status
                    richTextScansHistory.Text = tmp.Remove(0, 1) + "\r\n" + "\r\n" + "Selected path: " + tmp2 + "\r\n" + "\r\n" + "This file is: " + tmp3;
                }
            }
            else
                richTextScansHistory.Text = "Select the scan in the tree to see the detailed information about it";
        }

        int selectedNode;
        private void treeHistoryViruses_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Name != "date")
            {
                btnDeleteThisVirus.Visible = true;
                selectedNode = Convert.ToInt32(e.Node.Name);
                string tmp = loadings.getSquareBrackets(loadings.virusescontainer[selectedNode], 1);
                string tmp2 = loadings.getSquareBrackets(loadings.virusescontainer[selectedNode], 3);

                if (tmp2.StartsWith("S"))
                {
                    richTextVirusesHistory.Text = "This file looks suspicious" + "\r\n" + "\r\n" + "Path: " + tmp;
                }

                if (tmp2.StartsWith("V"))
                {
                    richTextVirusesHistory.Text = "This file is INFECTED" + "\r\n" + "\r\n" + "Path: " + tmp;
                }
            }
            else
            {
                richTextVirusesHistory.Text = "Select the virus in the tree to see the detailed information about it";
                btnDeleteThisVirus.Visible = false;
            }
                
        }

        private string GetSHA1(string filename) //gets SHA1 hash from a file.
        {
            using (var sha1 = SHA1.Create())
            {
                using (var stream = File.OpenRead(filename)) //do I need to try/catch here? What if the file is inaccessible?
                {
                    return BitConverter.ToString(sha1.ComputeHash(stream)).Replace("-", string.Empty); //Converts bits to string, removes all the dashes and returns it
                }
            }
        }


        private void btnClearVirusesLog_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This will delete your VIRUSES log PERMANENTLY. This cannot be undone. Are you sure you want to proceed?", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                loadings.Viruseslogs.Clear();
                loadings.Suspiciouslogs.Clear();
                richTextVirusesHistory.Text = "No viruses found! Hooray!";
                tabScans_SelectedIndexChanged(sender, e);
            }
        }

        private void btnClearScansLog_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This will delete your SCANS log PERMANENTLY. This cannot be undone. Are you sure you want to proceed?", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                loadings.OKlogs.Clear();
                loadings.OKlogs.Add("[I][G][NORE]");
                richTextScansHistory.Text = "No scans were performed!";
                tabScans_SelectedIndexChanged(sender, e);
                
            }
        }

        private void btnDeleteThisVirus_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This action will delete the file PERMANENTLY. This cannot be undone. Are you sure you want to proceed?", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                string tmp = loadings.getSquareBrackets(loadings.virusescontainer[selectedNode], 1);
                File.Delete(tmp);
                tabScans.SelectedIndex = 4;
                tabScans.SelectedIndex = 3;
            }
        }


        //SCAN FULL
        int filescountfull; //temp filescount, used only in CountFiles. "overall" is used in other places. I don't remember why, but, I guess, there's a reason for this
        private void CountFilesFull(string dir2)
        {
            try
            {
                foreach (string file in Directory.GetFiles(dir2)) //gets all files from the folder
                {
                    filescountfull++; //increase the temp filescount with every file
                    overallfull = filescountfull; //as we just changed the temp overall files count, we have to set it to the global files count
                    labScannedFullNum.Invoke(new MethodInvoker(delegate { labScannedFullNum.Text = scannedFull + "/" + overallfull; })); //change the count label
                    progressFull.Invoke(new MethodInvoker(delegate { progressFull.Maximum = overallfull; })); //set the maximum progressbar value to max files
                    progressFull.Invoke(new MethodInvoker(delegate { progressFull.Value = scannedFull; })); //if the scan is running at the same this will prevent blinking of a prbar
                    progressFull.Invalidate();
                }
            }
            catch
            { /*return;*/ } //we want to do nothing here, so nothing here. Do I need to log this? Don't think so

            try
            {
                foreach (string dir in Directory.GetDirectories(dir2)) //gets all folders from the folder and does the same for all of them
                {
                    CountFilesFull(dir);
                }
            }
            catch
            { /*return;*/ } //we want to do nothing here, so nothing here. Do I need to log this? Don't think so
        }
        private Thread StartTheScanFull(string param1, int wheretolongpass) //Starts the ADVANCED FOLDER scan
        {
            var t = new Thread(() => TreeScanFull(param1, wheretolongpass)); //this one is needed to start thread with params
            t.Start();
            t.IsBackground = true; //we want the thread to close when the app is closed, so this does it
            return t; //http://stackoverflow.com/questions/1195896/threadstart-with-parameters
        }

        private void TreeScanFull(string folder, int wheretopass) //wheretopass determines where should LogIt(whichlog,text,whichscan) pass it. 
        {                                                     //Wheretopass determines which scan is used. See more at the LogIt definition.
            try
            {
                foreach (string file in Directory.GetFiles(folder)) //gets all files' filenames from the folder
                {
                    string temphash = GetSHA1(file);
                    if (hashes.Contains(temphash)) //checks if this hash exists, should be probably replaced, too slow
                    {
                        LogIt(1, file, "INFECTED", wheretopass);
                        infected = true;
                        scannedFull++; //increases the OVERALL advanced folder scanned count
                        progressFull.Invoke(new MethodInvoker(delegate { progressFull.PerformStep(); }));
                    }
                    else
                    {
                        if (susphashes.Contains(temphash)) //checks if this hash exists, should be probably replaced, too slow
                        {
                            LogIt(2, file, "suspicious", wheretopass);
                            if (infected != true)
                                infected = null;
                            scannedFull++; //increases the OVERALL advanced folder scanned count
                            progressFull.Invoke(new MethodInvoker(delegate { progressFull.PerformStep(); }));
                        }
                        else
                        {
                            LogIt(4, file, " is clear", wheretopass);
                            scannedFull++; //increases the OVERALL advanced folder scanned count
                            progressFull.Invoke(new MethodInvoker(delegate { progressFull.PerformStep(); }));
                        }
                    }
                }
            }
            catch (ThreadAbortException) //we don't want an "thread terminated" exception to log (coz we do it by ourselves) so we check for that
            {
                return;
            }
            catch (Exception E)
            {
                LogIt(3, E.Message, wheretopass);
            }

            try
            {
                foreach (string dir in Directory.GetDirectories(folder)) //gets all folders from the folder and does the same for all of them
                {
                    TreeScanFull(dir, wheretopass);
                }
            }
            catch (ThreadAbortException) //we don't want an "thread terminated" exception to log (coz we do it by ourselves) so we check for that
            {
                return;
            }
            catch (Exception E)
            {
                LogIt(3, E.Message, wheretopass);
                //return;
            }
        }

        private Thread StartTheFilesCountFull(string param1)
        {
            var thr = new Thread(() => CountFilesFull(param1)); //this one is needed to start thread with params
            thr.Start();
            thr.IsBackground = true; //we want the thread to close when the app is closed, so this does it
            return thr; //http://stackoverflow.com/questions/1195896/threadstart-with-parameters
        }
        Thread FilescounttingFull;
        Thread Fullscanthread;
        System.Timers.Timer timerScanFullChecker;
        private void timerScanFullChecker_Tick(object sender, EventArgs e) //checks whether the fast scan is finished
        {
            if (Fullscanthread.ThreadState == ThreadState.Stopped)
            {
                timerScanFullChecker.Enabled = false;
                timerScanFullChecker.Stop();
                EnableEverything();
                btnFullScan.Invoke(new MethodInvoker(delegate { btnFullScan.Text = "Scan"; }));
                loadings.createLogEntry(4, String.Format("(EThe drive scan finished!)|{0}-{1}|", scannedFull, overallfull));
                LogIt(0, "The scan finished!", 1);
                isscanning = false;
                loadings.notifyIcon1.ShowBalloonTip(500, "The scan finished", String.Format("The drive scan was finished. Scanned {0} of {1} files.", scannedFull, overallfull), ToolTipIcon.Info);
            }
        }
        private string FullDrive;
        private void btnFullScan_Click(object sender, EventArgs e)
        {
            if (btnFullScan.Text == "Scan")
            {
                textFullLog.Clear();
                btnFullScan.Text = "Cancel";
                FullDrive = comboDriveSelect.Text;
                loadings.createLogEntry(4, String.Format("(SDrive scan was performed)|{0}|",FullDrive)); 
                isscanning = true;

                scannedFull = 0;  //sets the amount of scanned files by the fast scanner to zero
                filescountfull = 0;
                overallfull = 0;
                progressFull.Maximum = overallfull; //overall amount of files
                progressFull.Value = 0;
                textFastLog.Clear(); //clears the log
                LogIt(0, "Drive scan started!", 1);

                DisableEverything();
                btnFullScan.Enabled = true;

                FilescounttingFull = StartTheFilesCountFull(FullDrive);

                Fullscanthread = StartTheScanFull(FullDrive, 1);

                timerScanFullChecker = new System.Timers.Timer(500);
                timerScanFullChecker.Enabled = true;
                timerScanFullChecker.Elapsed += new System.Timers.ElapsedEventHandler(timerScanFullChecker_Tick);
                timerScanFullChecker.Start(); //starts the checker   
            }
            else
            {
                timerScanFullChecker.Enabled = false;
                timerScanFullChecker.Stop();
                loadings.createLogEntry(4, String.Format("(EThe drive scan was ABORTED)|{0}-{1}|", scannedFull, overallfull));
                LogIt(0, "The drive scan was ABORTED", 1);
                isscanning = false;
                btnFullScan.Text = "Scan";
                Fullscanthread.Abort();
                FilescounttingFull.Abort();
                EnableEverything();
            }
        }
        private void EnableEverything()
        {
            btnFastScan.Invoke(new MethodInvoker(delegate { btnFastScan.Enabled = true; }));
            btnFullScan.Invoke(new MethodInvoker(delegate { btnFullScan.Enabled = true; }));
            if (WasScanFileEnabled)
                btnScanFile.Invoke(new MethodInvoker(delegate { btnScanFile.Enabled = true; }));
            if (WasAdvaScanEnabled)
                btnScanFolder.Invoke(new MethodInvoker(delegate { btnScanFolder.Enabled = true; }));
            btnClearScansLog.Invoke(new MethodInvoker(delegate { btnClearScansLog.Enabled = true; }));
            btnClearVirusesLog.Invoke(new MethodInvoker(delegate { btnClearVirusesLog.Enabled = true; }));
            btnSelectFolder.Invoke(new MethodInvoker(delegate { btnSelectFolder.Enabled = true; }));
            btnSelectFile.Invoke(new MethodInvoker(delegate { btnSelectFile.Enabled = true; }));
            comboDriveSelect.Invoke(new MethodInvoker(delegate { comboDriveSelect.Enabled = true; }));
        }
        private void DisableEverything()
        {
            
            if (btnScanFile.Enabled == false)
                WasScanFileEnabled = false;
            else
                WasScanFileEnabled = true;

            if (btnScanFolder.Enabled == false)
                WasAdvaScanEnabled = false;
            else
                WasAdvaScanEnabled = true;

            btnFastScan.Invoke(new MethodInvoker(delegate { btnFastScan.Enabled = false; }));
            btnFullScan.Invoke(new MethodInvoker(delegate { btnFullScan.Enabled = false; }));
            btnScanFile.Invoke(new MethodInvoker(delegate { btnScanFile.Enabled = false; }));
            btnFullScan.Invoke(new MethodInvoker(delegate { btnFullScan.Enabled = false; }));
            btnScanFolder.Invoke(new MethodInvoker(delegate { btnScanFolder.Enabled = false; }));

            btnClearScansLog.Invoke(new MethodInvoker(delegate { btnClearScansLog.Enabled = false; }));
            btnClearVirusesLog.Invoke(new MethodInvoker(delegate { btnClearVirusesLog.Enabled = false; }));
            btnSelectFolder.Invoke(new MethodInvoker(delegate { btnSelectFolder.Enabled = false; }));
            btnSelectFile.Invoke(new MethodInvoker(delegate { btnSelectFile.Enabled = false; }));
            comboDriveSelect.Invoke(new MethodInvoker(delegate { comboDriveSelect.Enabled = false; }));
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) //DRIVE SELECTOR
        {
            btnFullScan.Enabled = true;
            labSelectTheDrive.Visible = false;
        }

        public bool logOKFull; //0
        public bool logSuspiciousFull = true; //1
        public bool logVirusesFull = true; //2
        public bool logErrorsFull = true; //3
        private void checkBox2_CheckedChanged(object sender, EventArgs e) //CHECKS
        {
            if (checkShowOKFull.Checked == true)
                logOKFull = true;
            else
                logOKFull = false;

            if (checkShowSuspiciousFull.Checked == true)
                logSuspiciousFull = true;
            else
                logSuspiciousFull = false;

            if (checkShowWarningsFull.Checked == true)
                logErrorsFull = true;
            else
                logErrorsFull = false;
        }
        //END SCAN FULL


        //SCAN FAST
        private Thread StartTheScanFast(string param1, int wheretolongpass) //Starts the ADVANCED FOLDER scan
        {
            var t = new Thread(() => TreeScanFast(param1, wheretolongpass)); //this one is needed to start thread with params
            t.Start();
            t.IsBackground = true; //we want the thread to close when the app is closed, so this does it
            return t; //http://stackoverflow.com/questions/1195896/threadstart-with-parameters
        }

        private void TreeScanFast(string folder, int wheretopass) //wheretopass determines where should LogIt(whichlog,text,whichscan) pass it. 
        {                                                     //Wheretopass determines which scan is used. See more at the LogIt definition.
            try
            {
                foreach (string file in Directory.GetFiles(folder)) //gets all files' filenames from the folder
                {
                    string temphash = GetSHA1(file);
                    if (hashes.Contains(temphash)) //checks if this hash exists, should be probably replaced, too slow
                    {
                        LogIt(1, file, "INFECTED", wheretopass);
                        infected = true;
                        scannedFast++; //increases the OVERALL advanced folder scanned count
                        progressScanFolder.Invoke(new MethodInvoker(delegate { progressFast.PerformStep(); }));
                    }
                    else
                    {
                        if (susphashes.Contains(temphash)) //checks if this hash exists, should be probably replaced, too slow
                        {
                            LogIt(2, file, "suspicious", wheretopass);
                            if (infected != true)
                                infected = null;
                            scannedFast++; //increases the OVERALL advanced folder scanned count
                            progressFast.Invoke(new MethodInvoker(delegate { progressFast.PerformStep(); }));
                        }
                        else
                        {
                            LogIt(4, file, " is clear", wheretopass);
                            scannedFast++; //increases the OVERALL advanced folder scanned count
                            progressFast.Invoke(new MethodInvoker(delegate { progressFast.PerformStep(); }));
                        }
                    }
                }
            }
            catch (ThreadAbortException) //we don't want an "thread terminated" exception to log (coz we do it by ourselves) so we check for that
            {
                return;
            }
            catch (Exception E)
            {
                LogIt(3, E.Message, wheretopass);
            }

            try
            {
                foreach (string dir in Directory.GetDirectories(folder)) //gets all folders from the folder and does the same for all of them
                {
                    TreeScanFast(dir, wheretopass);
                }
            }
            catch (ThreadAbortException) //we don't want an "thread terminated" exception to log (coz we do it by ourselves) so we check for that
            {
                return;
            }
            catch (Exception E)
            {
                LogIt(3, E.Message, wheretopass);
                //return;
            }
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textFullLog.SelectionStart = textFullLog.TextLength;
            textFullLog.ScrollToCaret();
        }
        Thread FilescounttingFast;
        Thread Fastscanthread;
        System.Timers.Timer timerScanFastChecker; //timer that is used to determine whether the scan is finished or not
        
        int ScanFastTurn = 0; //determines which folder 1,2,3 or 4 (0,1,2,3) is needed to scan.
        private void btnFastScan_Click(object sender, EventArgs e)
        {
            if(btnFastScan.Text == "Scan")
            {
                textFastLog.Clear(); //clears the log
                btnFastScan.Text = "Cancel";
                loadings.createLogEntry(4, "(SFast scan was performed)|Desktop, Appdata, Documents, Internet Cache|");
                isscanning = true;

                scannedFast = 0;  //sets the amount of scanned files by the fast scanner to zero
                filescountfast = 0;
                overallfast = 0;
                progressFast.Maximum = overallfast; //overall amount of files
                progressFast.Value = 0;
                LogIt(0, "Fast scan started!", 0);

                DisableEverything();
                btnFastScan.Enabled = true;

                FilescounttingFast = StartTheFilesCountFast(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));

                Fastscanthread = StartTheScanFast(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), 0);

                timerScanFastChecker = new System.Timers.Timer(500);
                timerScanFastChecker.Enabled = true;
                timerScanFastChecker.Elapsed += new System.Timers.ElapsedEventHandler(timerScanFastChecker_Tick);
                timerScanFastChecker.Start(); //starts the checker   
            }
            else
            {
                loadings.createLogEntry(4, String.Format("(EThe fast scan was ABORTED)|{0}-{1}|", scannedFast, overallfast));
                LogIt(0, "Fast scan was ABORTED", 0);
                isscanning = false;
                timerScanFastChecker.Enabled = false;
                timerScanFastChecker.Stop();
                btnFastScan.Text = "Scan";
                Fastscanthread.Abort();
                FilescounttingFast.Abort();

                EnableEverything();
            }
        }
        private Thread StartTheFilesCountFast(string param1)
        {
            var thr = new Thread(() => CountFilesFast(param1)); //this one is needed to start thread with params
            thr.Start();
            thr.IsBackground = true; //we want the thread to close when the app is closed, so this does it
            return thr; //http://stackoverflow.com/questions/1195896/threadstart-with-parameters
        }
        private void textFastLog_TextChanged(object sender, EventArgs e)
        {
            textFastLog.SelectionStart = textFastLog.TextLength;
            textFastLog.ScrollToCaret();
        }
        private void timerScanFastChecker_Tick(object sender, EventArgs e) //checks whether the fast scan is finished
        {
            if (Fastscanthread.ThreadState == ThreadState.Stopped)
            {
                switch(ScanFastTurn)
                {
                    case 0:
                        Fastscanthread = StartTheScanFast(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 0);
                        FilescounttingFast = StartTheFilesCountFast(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                        LogIt(0, "Desktop folder scanned!", 0);
                        ScanFastTurn++;
                        return;
                        break;
                    case 1:
                        Fastscanthread = StartTheScanFast(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), 0);
                        FilescounttingFast = StartTheFilesCountFast(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
                        LogIt(0, "AppData folder scanned!", 0);
                        ScanFastTurn++;
                        return;
                        break;
                    case 2:
                        Fastscanthread = StartTheScanFast(Environment.GetFolderPath(Environment.SpecialFolder.InternetCache), 0);
                        FilescounttingFast = StartTheFilesCountFast(Environment.GetFolderPath(Environment.SpecialFolder.InternetCache));
                        LogIt(0, "MyDocuments folder scanned!", 0);
                        ScanFastTurn++;
                        return;
                        break;
                    case 3:
                        LogIt(0, "InternetCache folder scanned!", 0);
                        ScanFastTurn = 0;
                        break;
                }
                isscanning = false;
                timerScanFastChecker.Enabled = false;
                timerScanFastChecker.Stop();
                EnableEverything();
                btnFastScan.Invoke(new MethodInvoker(delegate { btnFastScan.Text = "Scan"; }));
                loadings.createLogEntry(4, String.Format("(EThe fast scan was finished!)|{0}-{1}|",scannedFast,overallfast));
                LogIt(0, "The scan finished!", 0);
                loadings.notifyIcon1.ShowBalloonTip(500, "The scan finished", String.Format("The fast scan was finished. Scanned {0} of {1} files.", scannedFast, overallfast), ToolTipIcon.Info);
                //Fastscanthread = StartTheScanFast(text_FolderPath.Text, 0);
            }
        }

        int filescountfast; //temp filescount, used only in CountFiles. "overall" is used in other places. I don't remember why, but, I guess, there's a reason for this
        private void CountFilesFast(string dir2)
        {
            try
            {
                foreach (string file in Directory.GetFiles(dir2)) //gets all files from the folder
                {
                    filescountfast++; //increase the temp filescount with every file
                    overallfast = filescountfast; //as we just changed the temp overall files count, we have to set it to the global files count
                    labScannedFastNum.Invoke(new MethodInvoker(delegate { labScannedFastNum.Text = scannedFast + "/" + overallfast; })); //change the count label
                    progressFast.Invoke(new MethodInvoker(delegate { progressFast.Maximum = overallfast; })); //set the maximum progressbar value to max files
                    progressFast.Invoke(new MethodInvoker(delegate { progressFast.Value = scannedFast; })); //if the scan is running at the same this will prevent blinking of a prbar
                    progressFast.Invalidate();
                }
            }
            catch
            { /*return;*/ } //we want to do nothing here, so nothing here. Do I need to log this? Don't think so

            try
            {
                foreach (string dir in Directory.GetDirectories(dir2)) //gets all folders from the folder and does the same for all of them
                {
                    CountFilesFast(dir);
                }
            }
            catch
            { /*return;*/ } //we want to do nothing here, so nothing here. Do I need to log this? Don't think so
        }

        private void CheckShowLogChecksFast(object sender, EventArgs e) //Checks what it should print in the ADVANCED FOLDER scan log (not the log of the app)
        {
            if (checkShowOKFast.Checked == true)
                logOKFast = true;
            else
                logOKFast = false;

            if (checkShowSuspiciousFast.Checked == true)
                logSuspiciousFast = true;
            else
                logSuspiciousFast = false;

            if (checkShowWarningsFast.Checked == true)
                logErrorsFast = true;
            else
                logErrorsFast = false;
        }

        public bool logOKFast; //0
        public bool logSuspiciousFast = true; //1
        public bool logVirusesFast = true; //2
        public bool logErrorsFast = true; //3
        //END SCAN FAST
  


        //SCAN ADVANCED FILE
        private void btnSelectFile_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.ShowDialog();
            
            if (open.FileName != "") //checks if the file was opened
            {
                progressScanFile.Value = 0;
                textFilePath.Text = open.FileName;
                labThisFileStatus.Text      = "unscanned";
                labThisFileStatus.ForeColor = SystemColors.ControlText;
                btnScanFile.Enabled = true;
            }
            else
            {
                if(textFilePath.Text == "C:\\some.file") //sets everything to default if none were selected
                {
                    btnScanFile.Enabled = false;
                    labThisFileStatus.Text = "unselected";
                } 
            }   
        }  

        private void btn_ScanFile_Click(object sender, EventArgs e)
        {
            textFilePath.Enabled = false;
            string file = textFilePath.Text;
            progressScanFile.Value = 0;
            string temphash;;
            try
            {
                temphash = GetSHA1(textFilePath.Text); //gets the hash 
            }
            catch //it the file is inaccessible
            {
                labThisFileStatus.Text      = "error";
                labThisFileStatus.ForeColor = Color.Orange; //changes the color of a lablel
                MessageBox.Show("Can't scan the file. It may be corrupded, missing or protected.", "Oops!", MessageBoxButtons.OK, MessageBoxIcon.Error); //shows a msgbox
                return;
            }

            if (hashes.Contains(temphash))
            {
                loadings.createLogEntry(1, file);
                infected = true;
                labThisFileStatus.Text      = "INFECTED";
                labThisFileStatus.ForeColor = Color.Red; //changes the color of a lablel
                progressScanFile.PerformStep();
            }
            else
            {
                if (susphashes.Contains(temphash))
                {
                    loadings.createLogEntry(2, file);
                    labThisFileStatus.Text = "suspicious";
                    if (infected != true)
                        infected = null;
                    labThisFileStatus.ForeColor = Color.DarkOrange; //changes the color of a lablel
                    progressScanFile.PerformStep();
                }
                else
                {
                    labThisFileStatus.Text = "clear";
                    labThisFileStatus.ForeColor = Color.Green; //changes the color of a lablel
                    progressScanFile.PerformStep();
                }
            }
            loadings.createLogEntry(4, String.Format("(FAdvanced file scan was performed)|{0}|({1})", textFilePath.Text, labThisFileStatus.Text));
            loadings.readLogs(0);
            btnScanFile.Enabled = false;
            textFilePath.Enabled = true;
        }
        //END SCAN ADVANCED FILE



        //SCAN ADVANCED FOLDER
        private System.Timers.Timer timerScanChecker; //timer checks if the scan finished
        private bool WasScanFileEnabled;
        private bool WasAdvaScanEnabled;
        private bool isscanning = false;
        private void btn_Scan_Click(object sender, EventArgs e)
        {
            if (btnScanFolder.Text == "Scan") //if it's not scanning do this
            {
                isscanning = true;
                textLog.Clear();
                DisableEverything();
                btnScanFolder.Enabled = true;

                timerScanChecker = new System.Timers.Timer(500);
                timerScanChecker.Enabled = true;
                timerScanChecker.Elapsed += new System.Timers.ElapsedEventHandler(timerCheckForScanEnded_Tick);
                timerScanChecker.Start();
                FoldScanning = StartTheScanFolder(textFolderPath.Text, 2);

                loadings.createLogEntry(4, String.Format("(SAdvanced folder scan was performed)|{0}|", textFolderPath.Text));
                LogIt(0, "The scan started!", 2);
                btnScanFolder.Text = "Cancel"; //sets the label of the button to "cancel"
            }
            else
            {
                isscanning = false;
                FoldScanning.Abort();
                FileCounting.Abort(); //aborts all threads
                EnableEverything();

                loadings.createLogEntry(4, String.Format("(EThe advanced folder scan was ABORTED)|{0}-{1}|", scanned, overall));
                LogIt(0, "The scan was ABORTED",2);
                scanned = 0; //we want to reset everything
                progressScanFolder.Value = 0;
                progressScanFolder.Invalidate();
                btnScanFolder.Text = "Scan";  //sets the label of the button to "scan"
                loadings.readLogs(0);
                timerScanChecker.Enabled = false; //disables the timer
                timerScanChecker.Stop();
            }
        }

        private void timerCheckForScanEnded_Tick(object sender, EventArgs e) //checks if the advanced folder scan ended
        {
            if (FoldScanning.ThreadState == ThreadState.Stopped)
            {
                timerScanChecker.Enabled = false; //disables itself 
                timerScanChecker.Stop();
                isscanning = false;
                EnableEverything();
                btnScanFolder.Invoke(new MethodInvoker(delegate { btnScanFolder.Text = "Scan"; })); //sets the label of the button to "scan"
                btnScanFolder.Invoke(new MethodInvoker(delegate { btnScanFolder.Enabled = false; }));
                loadings.createLogEntry(4, String.Format("(EThe advanced folder scan was finished)|{1}-{2}|", textFolderPath.Text, scanned, overall));
                
                LogIt(0, "The scan finished!", 2);
                loadings.notifyIcon1.ShowBalloonTip(500, "The scan finished", String.Format("The advanced folder scan was finished. Scanned {0} of {1} files.", scanned, overall), ToolTipIcon.Info);    
            }
        }

        private void textLog_TextChanged(object sender, EventArgs e) //scrolls to end of the log on change
        {
            textLog.SelectionStart = textLog.TextLength;
            textLog.ScrollToCaret();
        }
        private string lastpath = "";
        private void btnSelectFolder_Click(object sender, EventArgs e)
        {
            try
            { FileCounting.Abort(); } //stops the scan if running, need catch if it's a first time and thread doesn't exist
            catch
            { /*NOTHING*/ }

            FolderBrowserDialog open = new FolderBrowserDialog();
            if (lastpath != String.Empty)
                open.SelectedPath = lastpath;

            open.ShowDialog();
            if (open.SelectedPath != String.Empty)
            {
                overall    = 0; //we want to reset everything
                filescount = 0;
                scanned = 0;
                progressScanFolder.Value = 0;
                textLog.Clear();
                lastpath = open.SelectedPath;
                textFolderPath.Text = open.SelectedPath; //and set the path to selected

                FileCounting = StartTheFilesCount(open.SelectedPath); //now we start the filecount and save the thread it returned to our public one

                btnScanFolder.Enabled = true; //enables the scan button
            }
            else
            {
                if(textFilePath.Text == "C:\\") //if none were selected sets everything to default
                {
                    btnScanFolder.Enabled = false;
                    labScannedNum.Text     = "0/0";
                }
            } 
        }


        //ALSO, IT'S USED IN THE FAST AND FULL SCANS   
        //LOGS:
        //0 - Actions (Eg. Scans, Changes)
        //1 - Viruses
        //2 - Suspicious
        //3 - Errors
        //4 - OK files
        private void LogIt(int whichlog, string text, int whichscan)
        {
            LogIt(whichlog,String.Empty,text,whichscan);
        }
        private void LogIt(int whichlog, string file, string text, int whichscan) // SCANS: 0 - fast, 1 - full, 2 - advfolder
        {
            switch(whichlog)
            {
                case 0:
                    switch (whichscan)
                    {
                        case 0:
                            textFastLog.Invoke((new MethodInvoker(delegate { textFastLog.Text += file + text + "\r\n"; })));
                            break;

                        case 1:
                            textFullLog.Invoke((new MethodInvoker(delegate { textFullLog.Text += file + text + "\r\n"; })));
                            break;

                        case 2:
                            textLog.Invoke((new MethodInvoker(delegate     { textLog.Text     += file + text + "\r\n"; })));
                            break;
                    }
                    break; 
                //END case 0

                case 1:
                    switch (whichscan)
                    {
                        case 0:
                            textFastLog.Invoke((new MethodInvoker(delegate { textFastLog.Text += file + " is " + text + "\r\n"; })));
                            loadings.createLogEntry(1, file);
                            break;

                        case 1:
                            textFullLog.Invoke((new MethodInvoker(delegate { textFullLog.Text += file + " is " + text + "\r\n"; })));
                            loadings.createLogEntry(1, file);
                            break;

                        case 2:
                            textLog.Invoke((new MethodInvoker(delegate {     textLog.Text     += file + " is " + text + "\r\n"; })));
                            loadings.createLogEntry(1, file);
                            break;
                    }
                    break; 
                //END case 1

                case 2:
                    switch (whichscan)
                    {
                        case 0:
                            if (logSuspiciousFast == true)
                                textFastLog.Invoke((new MethodInvoker(delegate { textFastLog.Text += file + " looks " + text + "\r\n"; })));
                            loadings.createLogEntry(2, file);
                            break;

                        case 1:
                            if (logSuspiciousFull == true)
                                textFullLog.Invoke((new MethodInvoker(delegate { textFullLog.Text += file + " looks " + text + "\r\n"; })));
                            loadings.createLogEntry(2, file);
                            break;

                        case 2:
                            if (logSuspicious == true)
                                textLog.Invoke((new MethodInvoker(delegate {     textLog.Text     += file + " looks " + text + "\r\n"; })));
                            loadings.createLogEntry(2, file);
                            break;
                    }
                    break; 
                //END case 2

                case 3:
                    switch (whichscan)
                    {
                        case 0:
                            if (logErrorsFast == true)
                                textFastLog.Invoke((new MethodInvoker(delegate { textFastLog.Text += text + "\r\n"; })));
                            loadings.createLogEntry(3, text);
                            break;

                        case 1:
                            if (logErrorsFull == true)
                                textFullLog.Invoke((new MethodInvoker(delegate { textFullLog.Text += text + "\r\n"; })));
                            loadings.createLogEntry(3, text);
                            break;

                        case 2:
                            if (logErrors == true)
                                textLog.Invoke((new MethodInvoker(delegate {     textLog.Text     += text + "\r\n"; })));
                            loadings.createLogEntry(3, text);
                            break;
                    }
                    break;
                //END case 3

                case 4:
                    switch (whichscan)
                    {
                        case 0:
                            if (logOKFast == true)
                                textFastLog.Invoke((new MethodInvoker(delegate { textFastLog.Text += file + text + "\r\n"; })));
                            break;

                        case 1:
                            if (logOKFull == true)
                                textFullLog.Invoke((new MethodInvoker(delegate { textFullLog.Text += file + text + "\r\n"; })));
                            break;

                        case 2:
                            if (logOK == true)
                                textLog.Invoke((new MethodInvoker(delegate {     textLog.Text     += file + text + "\r\n"; })));
                            break;
                    }
                    break;
                //END case 4
            }
        }

        public bool logOK; //0
        public bool logSuspicious = true; //1
        public bool logViruses = true; //2
        public bool logErrors = true; //3
        private void CheckShowLogChecksFolder(object sender, EventArgs e) //Checks what it should print in the ADVANCED FOLDER scan log (not the log of the app)
        {
            if (checkShowOK.Checked == true)
                logOK = true;
            else
                logOK = false;

            if (checkShowSuspicious.Checked == true)
                logSuspicious = true;
            else
                logSuspicious = false;

            if (checkShowWarnings.Checked == true)
                logErrors = true;
            else
                logErrors = false;
        }

        private Thread StartTheScanFolder(string param1,int wheretolongpass) //Starts the ADVANCED FOLDER scan
        {
            var t = new Thread(() => TreeScan(param1, wheretolongpass)); //this one is needed to start thread with params
            t.Start();
            t.IsBackground = true; //we want the thread to close when the app is closed, so this does it
            return t; //http://stackoverflow.com/questions/1195896/threadstart-with-parameters
        }
        private Thread StartTheFilesCount(string param1)
        {
            var thr = new Thread(() => CountFiles(param1)); //this one is needed to start thread with params
            thr.Start();
            thr.IsBackground = true; //we want the thread to close when the app is closed, so this does it
            return thr; //http://stackoverflow.com/questions/1195896/threadstart-with-parameters
        }

        private void TreeScan(string folder, int wheretopass) //wheretopass determines where should LogIt(whichlog,text,whichscan) pass it. 
        {                                                     //Wheretopass determines which scan is used. See more at the LogIt definition.
            try
            {
                foreach (string file in Directory.GetFiles(folder)) //gets all files' filenames from the folder
                {
                    string temphash = GetSHA1(file);
                    if (hashes.Contains(temphash)) //checks if this hash exists, should be probably replaced, too slow
                    {
                        LogIt(1, file, "INFECTED", wheretopass);
                        infected = true;
                        scanned++; //increases the OVERALL advanced folder scanned count
                        progressScanFolder.Invoke(new MethodInvoker(delegate { progressScanFolder.PerformStep(); }));
                    }
                    else
                    {
                        if (susphashes.Contains(temphash)) //checks if this hash exists, should be probably replaced, too slow
                        {
                            LogIt(2, file, "suspicious", wheretopass);
                            if(infected != true)
                                infected = null;
                            scanned++; //increases the OVERALL advanced folder scanned count
                            progressScanFolder.Invoke(new MethodInvoker(delegate { progressScanFolder.PerformStep(); }));
                        }
                        else
                        {
                            LogIt(4, file, " is clear", wheretopass);
                            scanned++; //increases the OVERALL advanced folder scanned count
                            progressScanFolder.Invoke(new MethodInvoker(delegate { progressScanFolder.PerformStep(); }));
                        }                 
                    }
                }
            }
            catch (ThreadAbortException) //we don't want an "thread terminated" exception to log (coz we do it by ourselves) so we check for that
            {
                return; 
            }
            catch (Exception E)
            {
                LogIt(3, E.Message, wheretopass);
            }

            try
            {
                foreach (string dir in Directory.GetDirectories(folder)) //gets all folders from the folder and does the same for all of them
                {
                    TreeScan(dir, wheretopass);
                }
            }
            catch (ThreadAbortException) //we don't want an "thread terminated" exception to log (coz we do it by ourselves) so we check for that
            {
                return;
            }
            catch (Exception E)
            {
                LogIt(3, E.Message, wheretopass);
            }
        }

        int filescount; //temp filescount, used only in CountFiles. "overall" is used in other places
        private void CountFiles(string dir2)
        {
            try
            {
                foreach (string file in Directory.GetFiles(dir2)) //gets all files from the folder
                {
                    filescount++; //increase the temp filescount with every file
                    overall = filescount; //as we just changed the temp overall files count, we have to set it to the global files count
                    labScannedNum.Invoke     (new MethodInvoker(delegate { labScannedNum.Text         = scanned + "/" + overall; })); //change the count label
                    progressScanFolder.Invoke(new MethodInvoker(delegate { progressScanFolder.Maximum = overall; })); //set the maximum progressbar value to max files
                    progressScanFolder.Invoke(new MethodInvoker(delegate { progressScanFolder.Value   = scanned; })); //if the scan is running at the same this will prevent blinking of a prbar
                    progressScanFolder.Invalidate();
                }
            }
            catch
            { /*return;*/ } //we want to do nothing here, so nothing here. Do I need to log this? Don't think so

            try
            {
                foreach (string dir in Directory.GetDirectories(dir2)) //gets all folders from the folder and does the same for all of them
                {
                    CountFiles(dir);
                }
            }
            catch
            { /*return;*/ } //we want to do nothing here, so nothing here. Do I need to log this? Don't think so
        }
        //END SCAN ADVANCED FOLDER
        //------------------------------
        //END SCANNING



        //APP'S MENU
        //-----------------------------
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SendMessage(IntPtr hWnd, //no idea how this works
                         int Msg, int wParam, int lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool ReleaseCapture();

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2; //no clues how this works
        private void PanelLogo_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0); //magic, don't touch
            }
        }

        private void Panel_Close_Click(object sender, EventArgs e)
        {
            this.Hide(); //we want the program to work in a background, so just hide it instead of closing
        }

        private void Panel_Minimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        //-----------------------------
        //END APP'S MENU


        private void panel1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("InfANT* Antivirus Scanner\r\n"+
                            "Version: " + ver + " " + build + 
                            "\r\nIt was developed by a group of Russian students"+
                            "\r\nInfANT was finished in early 2016"+
                            "\r\n\r\n*Inf = INFinity; ANT = A hard working (ant-like) ANTivirus", "About InfANT:");
        }

        private void checkLogOnlyImportant_CheckedChanged(object sender, EventArgs e) //Does the actions if the "Important" checkbox near the changelog is triggered
        {
            if (checkLogOnlyImportant.Checked == true)
                logonlyimportant = true;
            else
                logonlyimportant = false;
            loadings.formatchangelog();
        }

        //--------------------------------------------
        //MenuHandlers //Taskbar menu actions:

        bool isexited = false;
        public void MenuExit(object sender, System.EventArgs e)
        {
            isexited = true;
            Application.Exit();
        }
        public void MenuOpen(object sender, System.EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }
        public void MenuFast(object sender, System.EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            tabMainMenu.SelectTab(1);
            tabScans.SelectTab(0);
        }

        public void MenuSets(object sender, System.EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            tabMainMenu.SelectTab(3);
        }
        //END MenuHandlers
        //--------------------------------------------



        //WELCOME MENU
        //----------------------------------------------
        private void btnQuickFast_Click(object sender, EventArgs e)
        {
            tabMainMenu.SelectTab(1);
            tabScans.SelectTab(0);
        }

        private void buttonQuickFull_Click(object sender, EventArgs e)
        {
            tabMainMenu.SelectTab(1);
            tabScans.SelectTab(1);
        }

        private void buttonQuickLast_Click(object sender, EventArgs e)
        {
            tabMainMenu.SelectTab(1);
            tabScans.SelectTab(4);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Resources.protect);
        }

        
        //---------------------------------------------
        //END WELCOME MENU
    }
}