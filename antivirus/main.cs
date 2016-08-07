using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;
using System.Windows.Forms;

namespace InfANT
{
    public partial class Main : Form
    {
        //------------------------------------
        //VARS
        private bool ?_infected = false; //determines whether the PC is infected or not, this one is a var.
        public bool ?Infected { //this one is a function that triggers every time infected is changed, uses Infected as a var.
            get { return _infected; }
            private set { _infected = value; _loadings.ChangeIco(); switch (value)
            {
                case true:
                    labelSafeUnsafeWelcome.Invoke(new MethodInvoker(
                        delegate { labelSafeUnsafeWelcome.Text = LanguageResources.Computer_is_INFECTED; 
                                     labelSafeUnsafeWelcome.ForeColor = Color.Red; }));
                    _loadings.NotifyIcon1.Text = LanguageResources.Computer_is_INFECTED;
                    break;
                case null:
                    labelSafeUnsafeWelcome.Invoke(new MethodInvoker(
                        delegate
                        {
                            labelSafeUnsafeWelcome.Text = LanguageResources.Computer_is_partially_safe;
                            labelSafeUnsafeWelcome.ForeColor = Color.DarkOrange;
                        }));
                    _loadings.NotifyIcon1.Text = LanguageResources.Computer_is_partially_safe;
                    break;
                default:

                    labelSafeUnsafeWelcome.Invoke(new MethodInvoker(
                        delegate { labelSafeUnsafeWelcome.Text = LanguageResources.Computer_is_safe;
                                     labelSafeUnsafeWelcome.ForeColor = Color.FromArgb(82, 180, 60); }));
                    _loadings.NotifyIcon1.Text = LanguageResources.Computer_is_safe;
                    break;
            }
                } 
                             }

        private int Scanned     { get { return _scanned; } //determines how many files were scanned, function, triggers on change
                          set { _scanned = value; 
                                labScannedNum.Invoke((new MethodInvoker(delegate { labScannedNum.Text = value + @"/" + _overall; }))); 
                              } 
                        }

        private int _scanned; //determines how many files were scanned using advanced folder scan, var

        private int ScannedFast { get { return _scannedFast; } //determines how many files were scanned by the fast scanner, function, triggers on change
                          set { _scannedFast = value;
                               labScannedFastNum.Invoke((new MethodInvoker(delegate { labScannedFastNum.Text = value + @"/" + _overallfast; }))); 
                              } 
                        }

        private int _scannedFast; //determines how many files were scanned by the fast scanner, var

        private int ScannedFull {
                         get { return _scannedFull; } //determines how many files were scanned by the fast scanner, function, triggers on change
                         set { _scannedFull = value;
                             labScannedFullNum.Invoke((new MethodInvoker(delegate { labScannedFullNum.Text = value + @"/" + _overallfull; })));
                             }
                        }

        private int _scannedFull; //determines how many files were scanned by the fast scanner, var

        private int _overall; //determines how many files were SELECTED (i.e. how many files have to be scanned) using advanced folder scan, var
        private int _overallfast;
        private int _overallfull;

        public bool LogOnlyImportant     = true; //used for CHANGELOG, switches important/all modes
        private bool _isInternetConnected = true; //determines whether the PC is connected to the internet or not, this one is a var.
        public bool IsInternetConnected { get { return _isInternetConnected; } set { _isInternetConnected = value; InternetConnectionActions(value); } }
                             // ^ this is a function that triggers every time IsInternetConnected is changed, uses _isInternetConnected as a var.

        public List<string> Hashes  = new List<string>(); //a list of SHA1 hashes for scanning
        public List<string> SuspHashes = new List<string>(); //a list of suspicious hashes

        public string Ver; //this string indicates the current version of the app
        public const string Build = "UNSTABLE";

        private Thread _foldScanning; //This thread is used for scanning files in a folder
        private Thread _fileCounting; //This thread is used for counting files in a folder
        //------------------------------------
        //END VARS



        //MESS
        //------------------------------------
        private readonly LoadingScreen _loadings; //used to access loadingscreen
        private readonly CultureInfo _currentCulture;
        public Main(LoadingScreen loadingscr) //resieves an instance of a loading screen
        {
            string temp = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"\lang.ini");
            _currentCulture = new CultureInfo(temp);
            Thread.CurrentThread.CurrentCulture = _currentCulture;
            Thread.CurrentThread.CurrentUICulture = _currentCulture;
            InitializeComponent();
            _loadings = loadingscr; //makes it avaliable to use within the whole form
        }
        private void main_Load(object sender, EventArgs e) //scrolls the text to end at form start 
        {                                                  //(have to do this because if the form is not shown "TextChanged" event won't fire
            textChangelog.SelectionStart = textChangelog.TextLength;
            textChangelog.ScrollToCaret();

            if (_loadings.Suspiciouslogs.Count > 0)
                Infected = null;
            if (_loadings.Viruseslogs.Count <= 0) return;
            Infected = true;
            _loadings.ChangeIco();
        }
        private void textChangelog_TextChanged(object sender, EventArgs e) //scrolls the text to end at text change
        {
            textChangelog.SelectionStart = textChangelog.TextLength;
            textChangelog.ScrollToCaret(); 
        }
        private void tabScans_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnDeleteThisVirus.Visible = false;
            if (_isScanning) return;
            if (tabScans.SelectedIndex == 3 || tabScans.SelectedIndex == 4)
            {
                _loadings.ReadLogs(0);
                if (treeHistoryViruses.Nodes.Count == 0 & treeHistorySusp.Nodes.Count == 0)
                {
                    btnClearVirusesLog.Enabled = false;
                    richTextVirusesHistory.Text = LanguageResources.no_viruses_found;
                }
                else
                {
                    btnClearVirusesLog.Enabled = true;
                    richTextVirusesHistory.Text = LanguageResources.select_virus_to_see_detailed_info;
                }

                if (treeHistoryScans.Nodes.Count == 0)
                {
                    btnClearScansLog.Enabled = false;
                    richTextScansHistory.Text = LanguageResources.no_scans_performed;
                }
                else
                {
                    btnClearScansLog.Enabled = true;
                    richTextScansHistory.Text = LanguageResources.select_scan_to_see_detailed_info;
                }
            }
                
            Infected = false;
            if (_loadings.Suspiciouslogs.Count > 0)
                Infected = null;
            if (_loadings.Viruseslogs.Count > 0)
            {
                Infected = true;
            }
            _loadings.ChangeIco();

            if (tabScans.SelectedIndex != 1) return;

            comboDriveSelect.Items.Clear();
            string[] drives = Directory.GetLogicalDrives();
            foreach(string str in drives)
            {
                comboDriveSelect.Items.Add(str);
            }
            btnFullScan.Enabled = false;
            labSelectTheDrive.Visible = true;
        }

        private void main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_isExited) return;
            e.Cancel = true;
            Hide();
        }
        //------------------------------------
        //END MESS



        //TABS
        //------------------------------------    
        private void tabMainMenu_DrawItem(object sender, DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;
            Brush textBrush;

            // Get the item from the collection.
            TabPage tabPage = tabMainMenu.TabPages[e.Index];

            // Get the real bounds for the tab rectangle.
            Rectangle tabBounds = tabMainMenu.GetTabRect(e.Index);
            var fillbrush = new SolidBrush(Color.FromArgb(255, 59, 105, 177));
            //https://stackoverflow.com/questions/16240581/how-to-get-a-brush-from-a-rgb-code

            if (e.State == DrawItemState.Selected)
            {
                // Draw a different background color, and don't paint a focus rectangle.
                textBrush = new SolidBrush(Color.White);
                g.FillRectangle(fillbrush, e.Bounds);
            }
            else
            {
                textBrush = new SolidBrush(e.ForeColor);
                e.DrawBackground();
            }

            // Use our own font.
            Font tabFont = new Font("Arial", (float)10.0, FontStyle.Bold, GraphicsUnit.Pixel);

            // Draw string. Center the text.
            StringFormat stringFlags = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            g.DrawString(tabPage.Text, tabFont, textBrush, tabBounds, new StringFormat(stringFlags));
        }

        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;
            Brush textBrush;

            // Get the item from the collection.
            TabPage tabPage = tabScans.TabPages[e.Index];

            // Get the real bounds for the tab rectangle.
            Rectangle tabBounds = tabScans.GetTabRect(e.Index);
            var fillbrush = new SolidBrush(Color.FromArgb(255, 59, 105, 177));
            //https://stackoverflow.com/questions/16240581/how-to-get-a-brush-from-a-rgb-code

            if (e.State == DrawItemState.Selected)
            {
                // Draw a different background color, and don't paint a focus rectangle.
                textBrush = new SolidBrush(Color.White);
                g.FillRectangle(fillbrush, e.Bounds);
            }
            else
            {
                textBrush = new SolidBrush(e.ForeColor);
                e.DrawBackground();
            }

            // Use our own font.
            Font tabFont = new Font("Arial", (float)10.0, FontStyle.Bold, GraphicsUnit.Pixel);

            // Draw string. Center the text.
            StringFormat stringFlags = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            g.DrawString(tabPage.Text, tabFont, textBrush, tabBounds, new StringFormat(stringFlags));
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
            IsInternetConnected = true; //sets the var to true, no matter what happens
            _loadings.UpdateDatabase();  //these functions by itself will determine whether
        }
        //-----------------------------------
        //END INTERNET




        //SCANNING
        //---------------------------------
        private bool? _isVirusList;
        [SuppressMessage("ReSharper", "LocalizableElement")]
        private void treeHistoryScans_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Name != "date")
            {                                                                                                         //ADV FILE     //ADV FOLDER
                string tmp  = _loadings.GetSquareBrackets(_loadings.ActionsContainer[Convert.ToInt16(e.Node.Name)], 1); //action name  //action name
                string tmp2 = _loadings.GetSquareBrackets(_loadings.ActionsContainer[Convert.ToInt16(e.Node.Name)], 3); //path         //path
                
                if (tmp.StartsWith("S"))
                {
                    string tmp3 = _loadings.GetSquareBrackets(_loadings.ActionsContainer[Convert.ToInt16(e.Node.Name)], 5); //how ended
                    string tmp4 = _loadings.GetSquareBrackets(_loadings.ActionsContainer[Convert.ToInt16(e.Node.Name)], 7); //how many files were scanned     
                    richTextScansHistory.Text = tmp.Remove(0, 1) + "\r\n" + "\r\n" + LanguageResources.selected_path + tmp2 + "\r\n" + tmp3 + "\r\n" + "\r\n" + LanguageResources.amount_of_files_scanned + tmp4;
                }

                if (tmp.StartsWith("F"))
                {
                    string tmp3 = _loadings.GetSquareBrackets(_loadings.ActionsContainer[Convert.ToInt16(e.Node.Name)], 5); //file status
                    richTextScansHistory.Text = tmp.Remove(0, 1) + "\r\n" + "\r\n" + LanguageResources.selected_path + tmp2 + "\r\n" + "\r\n" + LanguageResources.this_file_is + tmp3;
                }
            }
            else
            {
                richTextScansHistory.Text = LanguageResources.select_scan_to_see_detailed_info;
            }
        }


        private int _selectedNode;
        [SuppressMessage("ReSharper", "LocalizableElement")]
        private void treeHistorySusp_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Name != "date")
            {
                btnDeleteThisVirus.Visible = true;
                _isVirusList = false;
                _selectedNode = Convert.ToInt32(e.Node.Name);
                string tmp = _loadings.GetSquareBrackets(_loadings.SuspContainer[_selectedNode], 1);
                string tmp2 = _loadings.GetSquareBrackets(_loadings.SuspContainer[_selectedNode], 3);

                if (tmp2.StartsWith("S"))
                {
                    richTextVirusesHistory.Text = LanguageResources.this_file_looks_susp + "\r\n" + "\r\n" + LanguageResources.path + tmp;
                }

                if (tmp2.StartsWith("V"))
                {
                    richTextVirusesHistory.Text = LanguageResources.this_file_is_infected + "\r\n" + "\r\n" + LanguageResources.path + tmp;
                }
            }
            else
            {
                _isVirusList = null;
                richTextVirusesHistory.Text = LanguageResources.select_virus_to_see_detailed_info;
                btnDeleteThisVirus.Visible = false;
            }
        }
        [SuppressMessage("ReSharper", "LocalizableElement")]
        private void treeHistoryViruses_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Name != "date")
            {
                _isVirusList = true;
                btnDeleteThisVirus.Visible = true;
                _selectedNode = Convert.ToInt32(e.Node.Name);
                string tmp = _loadings.GetSquareBrackets(_loadings.VirusesContainer[_selectedNode], 1);
                string tmp2 = _loadings.GetSquareBrackets(_loadings.VirusesContainer[_selectedNode], 3);

                if (tmp2.StartsWith("S"))
                {
                    richTextVirusesHistory.Text = LanguageResources.this_file_looks_susp + "\r\n" + "\r\n" + LanguageResources.path + tmp;
                }

                if (tmp2.StartsWith("V"))
                {
                    richTextVirusesHistory.Text = LanguageResources.this_file_is_infected + "\r\n" + "\r\n" + LanguageResources.path + tmp;
                }
            }
            else
            {
                _isVirusList = null;
                richTextVirusesHistory.Text = LanguageResources.select_virus_to_see_detailed_info;
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
            if (MessageBox.Show(LanguageResources.will_delete_your_VIRUSES_log_cannot_be_undone_Are_you_sure, LanguageResources.r_u_sure, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                _loadings.Viruseslogs.Clear();
                _loadings.Suspiciouslogs.Clear();
                richTextVirusesHistory.Text = LanguageResources.no_viruses_found;
                tabScans_SelectedIndexChanged(sender, e);
            }
        }

        private void btnClearScansLog_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(LanguageResources.will_delete_your_SCANS_log_cannot_be_undone_Are_you_sure, LanguageResources.r_u_sure, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                _loadings.OkLogs.Clear();
                _loadings.OkLogs.Add("[I][G][NORE]");
                richTextScansHistory.Text = LanguageResources.no_scans_performed;
                tabScans_SelectedIndexChanged(sender, e);
                
            }
        }

        private void btnDeleteThisVirus_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(LanguageResources.will_delete_the_file_PERMANENTLY_This_cannot_be_undone_Are_you_sure, LanguageResources.r_u_sure, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                try
                {
                    string tmp = _loadings.GetSquareBrackets(_loadings.VirusesContainer[_selectedNode], 1);
                    File.Delete(tmp);

                    if(_isVirusList == true)
                        _loadings.Viruseslogs.RemoveAt(_selectedNode);
                    if(_isVirusList == false)
                        _loadings.Suspiciouslogs.RemoveAt(_selectedNode);
                    
                    tabScans.SelectedIndex = 4;
                    tabScans.SelectedIndex = 3;
                }
                catch
                {
                    if (MessageBox.Show(LanguageResources.cant_delete_no_rights_or_deleted_want_remove_log_entry, LanguageResources.oops, MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                    {
                        if (_isVirusList == true)
                            _loadings.Viruseslogs.RemoveAt(_selectedNode);
                        if (_isVirusList == false)
                            _loadings.Suspiciouslogs.RemoveAt(_selectedNode);
                        tabScans.SelectedIndex = 4;
                        tabScans.SelectedIndex = 3;
                    }
                }
               
            }
        }


        //SCAN FULL
        private int _filesCountFull; //temp filescount, used only in CountFiles. "overall" is used in other places. I don't remember why, but, I guess, there's a reason for this
        private void CountFilesFull(string dir2)
        {
            try
            {
                foreach (string file in Directory.GetFiles(dir2)) //gets all files from the folder
                {
                    _filesCountFull++; //increase the temp filescount with every file
                    _overallfull = _filesCountFull; //as we just changed the temp overall files count, we have to set it to the global files count
                    labScannedFullNum.Invoke(new MethodInvoker(delegate { labScannedFullNum.Text = ScannedFull + "/" + _overallfull; })); //change the count label
                    progressFull.Invoke(new MethodInvoker(delegate { progressFull.Maximum = _overallfull; })); //set the maximum progressbar value to max files
                    progressFull.Invoke(new MethodInvoker(delegate { progressFull.Value = ScannedFull; })); //if the scan is running at the same this will prevent blinking of a prbar
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
        private Thread StartTheScanFull(string param1, int whereToLongPass) //Starts the ADVANCED FOLDER scan
        {
            var t = new Thread(() => TreeScanFull(param1, whereToLongPass)) //this one is needed to start thread with params
            {
                CurrentCulture = _currentCulture,
                CurrentUICulture = _currentCulture
            };
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
                    if (Hashes.Contains(temphash)) //checks if this hash exists, should be probably replaced, too slow
                    {
                        LogIt(1, file, LanguageResources.infected, wheretopass);
                        Infected = true;
                        ScannedFull++; //increases the OVERALL advanced folder scanned count
                        progressFull.Invoke(new MethodInvoker(delegate { progressFull.PerformStep(); }));
                    }
                    else
                    {
                        if (SuspHashes.Contains(temphash)) //checks if this hash exists, should be probably replaced, too slow
                        {
                            LogIt(2, file, LanguageResources.susp, wheretopass);
                            if (Infected != true)
                                Infected = null;
                            ScannedFull++; //increases the OVERALL advanced folder scanned count
                            progressFull.Invoke(new MethodInvoker(delegate { progressFull.PerformStep(); }));
                        }
                        else
                        {
                            LogIt(4, file, " is clear", wheretopass);
                            ScannedFull++; //increases the OVERALL advanced folder scanned count
                            progressFull.Invoke(new MethodInvoker(delegate { progressFull.PerformStep(); }));
                        }
                    }
                }
            }
            catch (ThreadAbortException) //we don't want an "thread terminated" exception to log (coz we do it by ourselves) so we check for that
            { return; }
            catch (Exception e)
            {
                LogIt(3, e.Message, wheretopass);
            }

            try
            {
                foreach (string dir in Directory.GetDirectories(folder)) //gets all folders from the folder and does the same for all of them
                {
                    TreeScanFull(dir, wheretopass);
                }
            }
            catch (ThreadAbortException) { /* we don't want an "thread terminated" exception to log (coz we do it by ourselves) so we check for that */ }
            catch (Exception e)
            {
                LogIt(3, e.Message, wheretopass);
            }
        }

        private Thread StartTheFilesCountFull(string param1)
        {
            var thr = new Thread(() => CountFilesFull(param1)) //this one is needed to start thread with params
            {
                CurrentCulture = _currentCulture,
                CurrentUICulture = _currentCulture
            };
            thr.Start();
            thr.IsBackground = true; //we want the thread to close when the app is closed, so this does it
            return thr; //http://stackoverflow.com/questions/1195896/threadstart-with-parameters
        }

        private Thread _filesCountingFull;
        private Thread _fullScanThread;
        private System.Timers.Timer _timerScanFullChecker;
        private void timerScanFullChecker_Tick(object sender, EventArgs e) //checks whether the fast scan is finished
        {
            if (_fullScanThread.ThreadState == ThreadState.Stopped)
            {
                _timerScanFullChecker.Enabled = false;
                _timerScanFullChecker.Stop();
                EnableEverything();
                btnFullScan.Invoke(new MethodInvoker(delegate { btnFullScan.Text = LanguageResources.IFBTN_SCAN; }));
                _loadings.CreateLogEntry(4, $"(EThe drive scan finished!)|{ScannedFull}-{_overallfull}|");
                LogIt(0, "The scan finished!", 1);
                _isScanning = false;
                _loadings.NotifyIcon1.ShowBalloonTip(500, "The scan finished", $"The drive scan was finished. Scanned {ScannedFull} of {_overallfull} files.", ToolTipIcon.Info);
            }
        }
        private string _fullDrive;
        private void btnFullScan_Click(object sender, EventArgs e)
        {
            if (btnFullScan.Text == LanguageResources.IFBTN_SCAN)
            {
                textFullLog.Clear();
                btnFullScan.Text = LanguageResources.cancel;
                _fullDrive = comboDriveSelect.Text;
                _loadings.CreateLogEntry(4, $"(SDrive scan was performed)|{_fullDrive}|"); 
                _isScanning = true;

                ScannedFull = 0;  //sets the amount of scanned files by the fast scanner to zero
                _filesCountFull = 0;
                _overallfull = 0;
                progressFull.Maximum = _overallfull; //overall amount of files
                progressFull.Value = 0;
                textFastLog.Clear(); //clears the log
                LogIt(0, "Drive scan started!", 1);

                DisableEverything();
                btnFullScan.Enabled = true;

                _filesCountingFull = StartTheFilesCountFull(_fullDrive);

                _fullScanThread = StartTheScanFull(_fullDrive, 1);

                _timerScanFullChecker = new System.Timers.Timer(500) {Enabled = true};
                _timerScanFullChecker.Elapsed += timerScanFullChecker_Tick;
                _timerScanFullChecker.Start(); //starts the checker   
            }
            else
            {
                _timerScanFullChecker.Enabled = false;
                _timerScanFullChecker.Stop();
                _loadings.CreateLogEntry(4, $"(EThe drive scan was ABORTED)|{ScannedFull}-{_overallfull}|");
                LogIt(0, "The drive scan was ABORTED", 1);
                _isScanning = false;
                btnFullScan.Text = LanguageResources.IFBTN_SCAN;
                _fullScanThread.Abort();
                _filesCountingFull.Abort();
                EnableEverything();
            }
        }
        private void EnableEverything()
        {
            btnFastScan.Invoke(new MethodInvoker(delegate { btnFastScan.Enabled = true; }));
            btnFullScan.Invoke(new MethodInvoker(delegate { btnFullScan.Enabled = true; }));
            if (_wasScanFileEnabled)
                btnScanFile.Invoke(new MethodInvoker(delegate { btnScanFile.Enabled = true; }));
            if (_wasAdvaScanEnabled)
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
                _wasScanFileEnabled = false;
            else
                _wasScanFileEnabled = true;

            if (btnScanFolder.Enabled == false)
                _wasAdvaScanEnabled = false;
            else
                _wasAdvaScanEnabled = true;

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

        private bool _logOkFull; //0
        private bool _logSuspiciousFull = true; //1
        private bool _logErrorsFull = true; //3
        private void checkBox2_CheckedChanged(object sender, EventArgs e) //CHECKS
        {
            if (checkShowOKFull.Checked)
                _logOkFull = true;
            else
                _logOkFull = false;

            if (checkShowSuspiciousFull.Checked)
                _logSuspiciousFull = true;
            else
                _logSuspiciousFull = false;

            if (checkShowWarningsFull.Checked)
                _logErrorsFull = true;
            else
                _logErrorsFull = false;
        }
        //END SCAN FULL


        //SCAN FAST
        private Thread StartTheScanFast(string param1, int whereToLongPass) //Starts the ADVANCED FOLDER scan
        {
            var t = new Thread(() => TreeScanFast(param1, whereToLongPass)) //this one is needed to start thread with params
            {
                CurrentCulture = _currentCulture,
                CurrentUICulture = _currentCulture
            };
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
                    if (Hashes.Contains(temphash)) //checks if this hash exists, should be probably replaced, too slow
                    {
                        LogIt(1, file, LanguageResources.infected, wheretopass);
                        Infected = true;
                        ScannedFast++; //increases the OVERALL advanced folder scanned count
                        progressScanFolder.Invoke(new MethodInvoker(delegate { progressFast.PerformStep(); }));
                    }
                    else
                    {
                        if (SuspHashes.Contains(temphash)) //checks if this hash exists, should be probably replaced, too slow
                        {
                            LogIt(2, file, LanguageResources.susp, wheretopass);
                            if (Infected != true)
                                Infected = null;
                            ScannedFast++; //increases the OVERALL advanced folder scanned count
                            progressFast.Invoke(new MethodInvoker(delegate { progressFast.PerformStep(); }));
                        }
                        else
                        {
                            LogIt(4, file, " is clear", wheretopass);
                            ScannedFast++; //increases the OVERALL advanced folder scanned count
                            progressFast.Invoke(new MethodInvoker(delegate { progressFast.PerformStep(); }));
                        }
                    }
                }
            }
            catch (ThreadAbortException) //we don't want an "thread terminated" exception to log (coz we do it by ourselves) so we check for that
            { return; }
            catch (Exception e)
            {
                LogIt(3, e.Message, wheretopass);
            }

            try
            {
                foreach (string dir in Directory.GetDirectories(folder)) //gets all folders from the folder and does the same for all of them
                {
                    TreeScanFast(dir, wheretopass);
                }
            }
            catch (ThreadAbortException) { /* we don't want an "thread terminated" exception to log (coz we do it by ourselves) so we check for that */ }
            catch (Exception e)
            {
                LogIt(3, e.Message, wheretopass);
            }
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textFullLog.SelectionStart = textFullLog.TextLength;
            textFullLog.ScrollToCaret();
        }

        private Thread _filesCountingFast;
        private Thread _fastScanThread;
        private System.Timers.Timer _timerScanFastChecker; //timer that is used to determine whether the scan is finished or not

        private int _scanFastTurn; //determines which folder 1,2,3 or 4 (0,1,2,3) is needed to scan.
        private void btnFastScan_Click(object sender, EventArgs e)
        {
            if(btnFastScan.Text == LanguageResources.IFBTN_SCAN) //TODO you are supposed to scan autorun.
            {
                textFastLog.Clear(); //clears the log
                btnFastScan.Text = LanguageResources.cancel;
                _loadings.CreateLogEntry(4, "(SFast scan was performed)|Desktop, Appdata, Documents, Internet Cache|");
                _isScanning = true;

                _scanFastTurn = 0;
                ScannedFast = 0;  //sets the amount of scanned files by the fast scanner to zero
                _filesCountFast = 0;
                _overallfast = 0;
                progressFast.Maximum = _overallfast; //overall amount of files
                progressFast.Value = 0;
                LogIt(0, "Fast scan started!", 0);

                DisableEverything();
                btnFastScan.Enabled = true;

                _filesCountingFast = StartTheFilesCountFast(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));

                _fastScanThread = StartTheScanFast(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), 0);

                _timerScanFastChecker = new System.Timers.Timer(500) {Enabled = true};
                _timerScanFastChecker.Elapsed += timerScanFastChecker_Tick;
                _timerScanFastChecker.Start(); //starts the checker   
            }
            else
            {
                _loadings.CreateLogEntry(4, $"(EThe fast scan was ABORTED)|{ScannedFast}-{_overallfast}|");
                LogIt(0, "Fast scan was ABORTED", 0);
                _isScanning = false;
                _timerScanFastChecker.Enabled = false;
                _timerScanFastChecker.Stop();
                btnFastScan.Text = LanguageResources.IFBTN_SCAN;
                _fastScanThread.Abort();
                _filesCountingFast.Abort();

                EnableEverything();
            }
        }
        private Thread StartTheFilesCountFast(string param1)
        {
            var thr = new Thread(() => CountFilesFast(param1)) //this one is needed to start thread with params
            {
                CurrentCulture = _currentCulture,
                CurrentUICulture = _currentCulture
            };
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
            if (_fastScanThread.ThreadState != ThreadState.Stopped) return;
            switch(_scanFastTurn)
            {
                case 0:
                    _fastScanThread = StartTheScanFast(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 0);
                    _filesCountingFast = StartTheFilesCountFast(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                    LogIt(0, "Desktop folder scanned!", 0);
                    _scanFastTurn++;
                    return;
                case 1:
                    _fastScanThread = StartTheScanFast(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), 0);
                    _filesCountingFast = StartTheFilesCountFast(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
                    LogIt(0, "AppData folder scanned!", 0);
                    _scanFastTurn++;
                    return;
                case 2:
                    _fastScanThread = StartTheScanFast(Environment.GetFolderPath(Environment.SpecialFolder.InternetCache), 0);
                    _filesCountingFast = StartTheFilesCountFast(Environment.GetFolderPath(Environment.SpecialFolder.InternetCache));
                    LogIt(0, "MyDocuments folder scanned!", 0);
                    _scanFastTurn++;
                    return;
                case 3:
                    LogIt(0, "InternetCache folder scanned!", 0);
                    _scanFastTurn = 4;
                    return;
            }
            _isScanning = false;
            _timerScanFastChecker.Enabled = false;
            _timerScanFastChecker.Stop();
            EnableEverything();
            btnFastScan.Invoke(new MethodInvoker(delegate { btnFastScan.Text = LanguageResources.IFBTN_SCAN; }));
            _loadings.CreateLogEntry(4, $"(EThe fast scan was finished!)|{ScannedFast}-{_overallfast}|");
            LogIt(0, "The scan finished!", 0);
            _loadings.NotifyIcon1.ShowBalloonTip(500, "The scan finished",$"The fast scan was finished. Scanned {ScannedFast} of {_overallfast} files.", ToolTipIcon.Info);
            ////Fastscanthread = StartTheScanFast(text_FolderPath.Text, 0);
        }

        private int _filesCountFast; //temp filescount, used only in CountFiles. "overall" is used in other places. I don't remember why, but, I guess, there's a reason for this
        private void CountFilesFast(string dir2)
        {
            try
            {
                foreach (string file in Directory.GetFiles(dir2)) //gets all files from the folder
                {
                    _filesCountFast++; //increase the temp filescount with every file
                    _overallfast = _filesCountFast; //as we just changed the temp overall files count, we have to set it to the global files count
                    labScannedFastNum.Invoke(new MethodInvoker(delegate { labScannedFastNum.Text = ScannedFast + @"/" + _overallfast; })); //change the count label
                    progressFast.Invoke(new MethodInvoker(delegate { progressFast.Maximum = _overallfast; })); //set the maximum progressbar value to max files
                    progressFast.Invoke(new MethodInvoker(delegate { progressFast.Value = ScannedFast; })); //if the scan is running at the same this will prevent blinking of a prbar
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
            if (checkShowOKFast.Checked)
                _logOkFast = true;
            else
                _logOkFast = false;

            if (checkShowSuspiciousFast.Checked)
                _logSuspiciousFast = true;
            else
                _logSuspiciousFast = false;

            if (checkShowWarningsFast.Checked)
                _logErrorsFast = true;
            else
                _logErrorsFast = false;
        }

        private bool _logOkFast; //0
        private bool _logSuspiciousFast = true; //1
        private bool _logErrorsFast = true; //3
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
                labThisFileStatus.Text      = LanguageResources.unscanned;
                labThisFileStatus.ForeColor = SystemColors.ControlText;
                btnScanFile.Enabled = true;
            }
            else
            {
                if(textFilePath.Text == @"C:\some.file") //sets everything to default if none were selected
                {
                    btnScanFile.Enabled = false;
                    labThisFileStatus.Text = LanguageResources.unselected;
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
                labThisFileStatus.Text      = LanguageResources.error;
                labThisFileStatus.ForeColor = Color.Orange; //changes the color of a label
                MessageBox.Show(LanguageResources.cant_scan_no_access, LanguageResources.oops, MessageBoxButtons.OK, MessageBoxIcon.Error); //shows a msgbox
                return;
            }

            if (Hashes.Contains(temphash))
            {
                _loadings.CreateLogEntry(1, file);
                Infected = true;
                labThisFileStatus.Text      = LanguageResources.infected;
                labThisFileStatus.ForeColor = Color.Red; //changes the color of a label
                progressScanFile.PerformStep();
            }
            else
            {
                if (SuspHashes.Contains(temphash))
                {
                    _loadings.CreateLogEntry(2, file);
                    labThisFileStatus.Text = LanguageResources.susp;
                    if (Infected != true)
                        Infected = null;
                    labThisFileStatus.ForeColor = Color.DarkOrange; //changes the color of a label
                    progressScanFile.PerformStep();
                }
                else
                {
                    labThisFileStatus.Text = LanguageResources.clear;
                    labThisFileStatus.ForeColor = Color.Green; //changes the color of a label
                    progressScanFile.PerformStep();
                }
            }
            _loadings.CreateLogEntry(4,$"(FAdvanced file scan was performed)|{textFilePath.Text}|({labThisFileStatus.Text})");
            _loadings.ReadLogs(0);
            btnScanFile.Enabled = false;
            textFilePath.Enabled = true;
        }
        //END SCAN ADVANCED FILE



        //SCAN ADVANCED FOLDER
        private System.Timers.Timer _timerScanChecker; //timer checks if the scan finished
        private bool _wasScanFileEnabled;
        private bool _wasAdvaScanEnabled;
        private bool _isScanning;
        private void btn_Scan_Click(object sender, EventArgs e)
        {
            if (btnScanFolder.Text == LanguageResources.IFBTN_scansmall) //if it's not scanning do this
            {
                _isScanning = true;
                textLog.Clear();
                DisableEverything();
                btnScanFolder.Enabled = true;

                _timerScanChecker = new System.Timers.Timer(500) {Enabled = true};
                _timerScanChecker.Elapsed += timerCheckForScanEnded_Tick;
                _timerScanChecker.Start();
                _foldScanning = StartTheScanFolder(textFolderPath.Text, 2);

                _loadings.CreateLogEntry(4, $"(SAdvanced folder scan was performed)|{textFolderPath.Text}|");
                LogIt(0, "The scan started!", 2);
                btnScanFolder.Text = LanguageResources.cancel; //sets the label of the button to "cancel"
            }
            else
            {
                _isScanning = false;
                _foldScanning.Abort();
                _fileCounting.Abort(); //aborts all threads
                EnableEverything();

                _loadings.CreateLogEntry(4, $"(EThe advanced folder scan was ABORTED)|{Scanned}-{_overall}|");
                LogIt(0, "The scan was ABORTED",2);
                Scanned = 0; //we want to reset everything
                progressScanFolder.Value = 0;
                progressScanFolder.Invalidate();
                btnScanFolder.Text = LanguageResources.IFBTN_SCAN;  //sets the label of the button to "scan"
                _loadings.ReadLogs(0);
                _timerScanChecker.Enabled = false; //disables the timer
                _timerScanChecker.Stop();
            }
        }

        private void timerCheckForScanEnded_Tick(object sender, EventArgs e) //checks if the advanced folder scan ended
        {
            if (_foldScanning.ThreadState == ThreadState.Stopped)
            {
                _timerScanChecker.Enabled = false; //disables itself 
                _timerScanChecker.Stop();
                _isScanning = false;
                EnableEverything();
                btnScanFolder.Invoke(new MethodInvoker(delegate { btnScanFolder.Text = LanguageResources.IFBTN_SCAN; })); //sets the label of the button to "scan"
                btnScanFolder.Invoke(new MethodInvoker(delegate { btnScanFolder.Enabled = false; }));
                _loadings.CreateLogEntry(4, $"(EThe advanced folder scan was finished)|{Scanned}-{_overall}|");
                
                LogIt(0, "The scan finished!", 2);
                _loadings.NotifyIcon1.ShowBalloonTip(500, "The scan finished", $"The advanced folder scan was finished. Scanned {Scanned} of {_overall} files.", ToolTipIcon.Info);    
            }
        }

        private void textLog_TextChanged(object sender, EventArgs e) //scrolls to end of the log on change
        {
            textLog.SelectionStart = textLog.TextLength;
            textLog.ScrollToCaret();
        }
        private string _lastPath = "";
        private void btnSelectFolder_Click(object sender, EventArgs e)
        {
            try
            { _fileCounting.Abort(); } //stops the scan if running, need catch if it's a first time and thread doesn't exist
            catch
            { /*NOTHING*/ }

            FolderBrowserDialog open = new FolderBrowserDialog();
            if (_lastPath != string.Empty)
                open.SelectedPath = _lastPath;

            open.ShowDialog();
            if (open.SelectedPath != string.Empty)
            {
                _overall    = 0; //we want to reset everything
                _filescount = 0;
                Scanned = 0;
                progressScanFolder.Value = 0;
                textLog.Clear();
                _lastPath = open.SelectedPath;
                textFolderPath.Text = open.SelectedPath; //and set the path to selected

                _fileCounting = StartTheFilesCount(open.SelectedPath); //now we start the filecount and save the thread it returned to our public one

                btnScanFolder.Enabled = true; //enables the scan button
            }
            else
            {
                if(textFilePath.Text == @"C:\") //if none were selected sets everything to default
                {
                    btnScanFolder.Enabled = false;
                    labScannedNum.Text     = @"0/0";
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
            LogIt(whichlog,string.Empty,text,whichscan);
        }
        [SuppressMessage("ReSharper", "LocalizableElement")]
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
                            textFastLog.Invoke((new MethodInvoker(delegate { textFastLog.Text += file + LanguageResources.is_ + text + "\r\n"; })));
                            _loadings.CreateLogEntry(1, file);
                            break;

                        case 1:
                            textFullLog.Invoke((new MethodInvoker(delegate { textFullLog.Text += file + LanguageResources.is_ + text + "\r\n"; })));
                            _loadings.CreateLogEntry(1, file);
                            break;

                        case 2:
                            textLog.Invoke((new MethodInvoker(delegate {     textLog.Text     += file + LanguageResources.is_ + text + "\r\n"; })));
                            _loadings.CreateLogEntry(1, file);
                            break;
                    }
                    break; 
                //END case 1

                case 2:
                    switch (whichscan)
                    {
                        case 0:
                            if (_logSuspiciousFast)
                                textFastLog.Invoke((new MethodInvoker(delegate { textFastLog.Text += file + LanguageResources.looks + text + "\r\n"; })));
                            _loadings.CreateLogEntry(2, file);
                            break;

                        case 1:
                            if (_logSuspiciousFull)
                                textFullLog.Invoke((new MethodInvoker(delegate { textFullLog.Text += file + LanguageResources.looks + text + "\r\n"; })));
                            _loadings.CreateLogEntry(2, file);
                            break;

                        case 2:
                            if (_logSuspicious)
                                textLog.Invoke((new MethodInvoker(delegate {     textLog.Text     += file + LanguageResources.looks + text + "\r\n"; })));
                            _loadings.CreateLogEntry(2, file);
                            break;
                    }
                    break; 
                //END case 2

                case 3:
                    switch (whichscan)
                    {
                        case 0:
                            if (_logErrorsFast)
                                textFastLog.Invoke((new MethodInvoker(delegate { textFastLog.Text += text + "\r\n"; })));
                            _loadings.CreateLogEntry(3, text);
                            break;

                        case 1:
                            if (_logErrorsFull)
                                textFullLog.Invoke((new MethodInvoker(delegate { textFullLog.Text += text + "\r\n"; })));
                            _loadings.CreateLogEntry(3, text);
                            break;

                        case 2:
                            if (_logErrors)
                                textLog.Invoke((new MethodInvoker(delegate {     textLog.Text     += text + "\r\n"; })));
                            _loadings.CreateLogEntry(3, text);
                            break;
                    }
                    break;
                //END case 3

                case 4:
                    switch (whichscan)
                    {
                        case 0:
                            if (_logOkFast)
                                textFastLog.Invoke((new MethodInvoker(delegate { textFastLog.Text += file + text + "\r\n"; })));
                            break;

                        case 1:
                            if (_logOkFull)
                                textFullLog.Invoke((new MethodInvoker(delegate { textFullLog.Text += file + text + "\r\n"; })));
                            break;

                        case 2:
                            if (_logOk)
                                textLog.Invoke((new MethodInvoker(delegate {     textLog.Text     += file + text + "\r\n"; })));
                            break;
                    }
                    break;
                //END case 4
            }
        }

        private bool _logOk; //0
        private bool _logSuspicious = true; //1
        private bool _logErrors = true; //3
        private void CheckShowLogChecksFolder(object sender, EventArgs e) //Checks what it should print in the ADVANCED FOLDER scan log (not the log of the app)
        {
            if (checkShowOK.Checked)
                _logOk = true;
            else
                _logOk = false;

            if (checkShowSuspicious.Checked)
                _logSuspicious = true;
            else
                _logSuspicious = false;

            if (checkShowWarnings.Checked)
                _logErrors = true;
            else
                _logErrors = false;
        }

        private Thread StartTheScanFolder(string param1,int whereToLongPass) //Starts the ADVANCED FOLDER scan
        {
            var t = new Thread(() => TreeScan(param1, whereToLongPass))
            {
                CurrentCulture = _currentCulture,
                CurrentUICulture = _currentCulture
            }; //this one is needed to start thread with params
            t.Start();
            t.IsBackground = true; //we want the thread to close when the app is closed, so this does it
            return t; //http://stackoverflow.com/questions/1195896/threadstart-with-parameters
        }
        private Thread StartTheFilesCount(string param1)
        {
            var thr = new Thread(() => CountFiles(param1)) //this one is needed to start thread with params
            {
                CurrentCulture = _currentCulture,
                CurrentUICulture = _currentCulture
            };
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
                    if (Hashes.Contains(temphash)) //checks if this hash exists, should be probably replaced, too slow
                    {
                        LogIt(1, file, LanguageResources.infected, wheretopass);
                        Infected = true;
                        Scanned++; //increases the OVERALL advanced folder scanned count
                        progressScanFolder.Invoke(new MethodInvoker(delegate { progressScanFolder.PerformStep(); }));
                    }
                    else
                    {
                        if (SuspHashes.Contains(temphash)) //checks if this hash exists, should be probably replaced, too slow
                        {
                            LogIt(2, file, LanguageResources.susp, wheretopass);
                            if(Infected != true)
                                Infected = null;
                            Scanned++; //increases the OVERALL advanced folder scanned count
                            progressScanFolder.Invoke(new MethodInvoker(delegate { progressScanFolder.PerformStep(); }));
                        }
                        else
                        {
                            LogIt(4, file, " is clear", wheretopass);
                            Scanned++; //increases the OVERALL advanced folder scanned count
                            progressScanFolder.Invoke(new MethodInvoker(delegate { progressScanFolder.PerformStep(); }));
                        }                 
                    }
                }
            }
            catch (ThreadAbortException) //we don't want an "thread terminated" exception to log (coz we do it by ourselves) so we check for that
            { return; }
            catch (Exception e)
            {
                LogIt(3, e.Message, wheretopass);
            }

            try
            {
                foreach (string dir in Directory.GetDirectories(folder)) //gets all folders from the folder and does the same for all of them
                {
                    TreeScan(dir, wheretopass);
                }
            }
            catch (ThreadAbortException) { /* we don't want an "thread terminated" exception to log (coz we do it by ourselves) so we check for that */ } 
            catch (Exception e)
            {
                LogIt(3, e.Message, wheretopass);
            }
        }

        private int _filescount; //temp filescount, used only in CountFiles. "overall" is used in other places
        private void CountFiles(string dir2)
        {
            try
            {
                foreach (string file in Directory.GetFiles(dir2)) //gets all files from the folder
                {
                    _filescount++; //increase the temp filescount with every file
                    _overall = _filescount; //as we just changed the temp overall files count, we have to set it to the global files count
                    labScannedNum.Invoke     (new MethodInvoker(delegate { labScannedNum.Text         = Scanned + "/" + _overall; })); //change the count label
                    progressScanFolder.Invoke(new MethodInvoker(delegate { progressScanFolder.Maximum = _overall; })); //set the maximum progressbar value to max files
                    progressScanFolder.Invoke(new MethodInvoker(delegate { progressScanFolder.Value   = Scanned; })); //if the scan is running at the same this will prevent blinking of a prbar
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
            if (e.Button != MouseButtons.Left) return;
            ReleaseCapture();
            SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0); //magic, don't touch
        }

        private void Panel_Close_Click(object sender, EventArgs e)
        {
            Hide(); //we want the program to work in a background, so just hide it instead of closing
        }

        private void Panel_Minimize_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }
        //-----------------------------
        //END APP'S MENU


        [SuppressMessage("ReSharper", "LocalizableElement")]
        private void panel1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(LanguageResources.infant_antivirus_scanner+
                            "\r\n" + LanguageResources.version + Ver + @" " + Build +
                            "\r\n" + LanguageResources.it_was_developed_by_students+
                            "\r\n" + LanguageResources.infant_was_released+
                            "\r\n\r\n" + LanguageResources.intfant_explanation, LanguageResources.about_infant);
        }

        private void checkLogOnlyImportant_CheckedChanged(object sender, EventArgs e) //Does the actions if the "Important" checkbox near the changelog is triggered
        {
            if (checkLogOnlyImportant.Checked)
                LogOnlyImportant = true;
            else
                LogOnlyImportant = false;
            _loadings.FormatChangelog();
        }

        //--------------------------------------------
        //MenuHandlers //Taskbar menu actions:

        private bool _isExited;
        public void MenuExit(object sender, EventArgs e)
        {
            _isExited = true;
            Application.Exit();
        }
        public void MenuOpen(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }
        public void MenuFast(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
            tabMainMenu.SelectTab(1);
            tabScans.SelectTab(0);
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
            MessageBox.Show(LanguageResources.protect);
        }
        //---------------------------------------------
        //END WELCOME MENU
    }
}