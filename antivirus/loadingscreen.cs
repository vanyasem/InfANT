using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using InfANT.Properties;

namespace InfANT
{
    public partial class LoadingScreen : Form
    {
        private Main _mainForm; //we use that to access Main form :3

        private readonly bool _usedLauncher;
        public LoadingScreen(bool usedLauncherBool)
        {
            string temp = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"\lang.ini");
            Thread.CurrentThread.CurrentCulture = new CultureInfo(temp);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(temp);
            InitializeComponent();
            _usedLauncher = usedLauncherBool;
        }

        private System.Windows.Forms.Timer _myTimer;
        private void loadingscreen_Shown(object sender, EventArgs e) //happens right after it's shown BUT NOT FULLY DRAWN! Load happened BEFORE, so it makes a blank gap.
        {
            _myTimer = new System.Windows.Forms.Timer {Interval = 200};
            _myTimer.Tick += Kostil;
            _myTimer.Start(); //as it's not fully drawn but we WANT it to we set a small timer. The timer works in an another thread so the UI thread will be able to finish drawing.
        }
        private void Kostil(object sender, EventArgs e)
        {
            _myTimer.Stop(); //we don't want to fire it more than I time, so immediatly stop it.

            Thread th = new Thread(Kostil2) {IsBackground = true};
            /* we need an another thread so the main thread will remain responsible.
            we want our thread to close with the program and not run apart, IsBackground will do it */
            th.Start();
            ProgressLoading.Invoke(new MethodInvoker(delegate { ProgressLoading.Value = 20; }));
        }
        private void Kostil2()
        {
            ProgressLoading.Invoke(new MethodInvoker(delegate { ProgressLoading.Value = 40; }));
            _mainForm = new Main(this); //launch Main form to access vars, but don't show it
            ProgressLoading.Invoke(new MethodInvoker(delegate { ProgressLoading.Value = 60; }));

            LoadLogs();
            CheckForCorruptions();
            ReadLogs(0);
            ProgressLoading.Invoke(new MethodInvoker(delegate { ProgressLoading.Value = 80; }));
            EnableTimer();
            UpdateDatabase();
            LoadLocalDatabase();
            ProgressLoading.Invoke(new MethodInvoker(delegate { ProgressLoading.Value = 100; }));
            LoadChangelog();

            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            Invoke(new MethodInvoker(delegate { CreateIconMenuStructure(); _mainForm.Ver = fvi.FileVersion; _mainForm.labelYoVersionLab.Text = _mainForm.Ver + @" " + Main.Build;
                _mainForm.TopMost = true; _mainForm.Show(); Hide(); Controls.Clear(); _mainForm.TopMost = false; Text = @"InfANT Helper"; })); //show the main form, hides this form and sets the name of it to "Helper"
        }

        private void CheckForCorruptions()
        {
            if (GetSquareBrackets(OkLogs[OkLogs.Count - 1],5).StartsWith("(S")) //If there's a start entry, but no end entry, do this:
            {   
                CreateLogEntry(4, "(EScan was rudely interrupted, was not finished correctly)|unknown|");
            }
        }
        //---------------------------------



        //TASKBAR ICO
        //--------------------------------
        public readonly NotifyIcon NotifyIcon1 = new NotifyIcon();
        private readonly ContextMenu _contextMenu1 = new ContextMenu();
        private void CreateIconMenuStructure()
        {
            //we create a contextMenu first
            _contextMenu1.MenuItems.Add("Open").Click += _mainForm.MenuOpen; //Triggers on menu-click
            _contextMenu1.MenuItems.Add("Fast-Scan").Click += _mainForm.MenuFast;
            _contextMenu1.MenuItems.Add("Exit").Click += _mainForm.MenuExit;

            NotifyIcon1.Visible = true;
            ChangeIco(); //We enable the ico and set the right appearance

            NotifyIcon1.Text = "Your computer is safe";
            NotifyIcon1.ContextMenu = _contextMenu1; //And then assign it to a notifyIcon
            NotifyIcon1.Click += _mainForm.MenuOpen; //This triggers on click of a taskbar ico
        }
        public void ChangeIco()
        {
            if (_mainForm.Infected == false) //if infected - set "bad" ico, if not - "good" ico
                NotifyIcon1.Icon = Resources.favicoOK;
            else
                NotifyIcon1.Icon = Resources.favicoinfected;
        }
        //--------------------------------
        //END TASKBAR ICO



        //LOGS
        //----------------------------------
        private void LoadLogs()
        {
            try
            {
                OkLogs         = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + @"\logsOKs.txt", Encoding.UTF8).ToList();

                if (OkLogs[0] == "firstlaunch")
                {
                    File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\logsViruses.txt", "");
                    File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\logsSuspicious.txt", "");
                    File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\logsErrors.txt", "");
                    OkLogs.Clear();
                    OkLogs.Add("[I][G][NORE]");
                }

                Viruseslogs    = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + @"\logsViruses.txt", Encoding.UTF8).ToList();
                Suspiciouslogs = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + @"\logsSuspicious.txt", Encoding.UTF8).ToList();
                _errorslogs     = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + @"\logsErrors.txt", Encoding.UTF8).ToList();     
                //loads logs into RAM     
            }
            catch
            {
                MessageBox.Show("Logs weren't found! \r\nPlease, don't delete them by yourself, do it using the 'Settings' tab.\r\nRelaunch the application!", "Fatal Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);

                File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\logsOKs.txt", @"firstlaunch");
                        
                Application.Exit(); 
            }
        }

        //0 - All
        //1 - Viruses
        //2 - Suspicious
        //3 - Errors //we don't need to read errors to the user, so no case for this
        //4 - Actions (Eg. Scans, Changes)
        int _nodesCountViruses;
        int _nodesCountActions;
        public readonly Dictionary<int, string> ActionsContainer = new Dictionary<int, string>();
        public readonly Dictionary<int, string> VirusesContainer = new Dictionary<int, string>();
        public void ReadLogs(int whatToRead)
        {
            switch (whatToRead)
            {
                case 0:
                    _mainForm.treeHistoryScans.Nodes.Clear();
                    _mainForm.treeHistoryViruses.Nodes.Clear();
                    ActionsContainer.Clear();
                    VirusesContainer.Clear();
                    ReadLogs(1);
                    ReadLogs(2);
                    ReadLogs(4);
                    break;

                case 1: 
                    if(Viruseslogs.Count > 0)
                    {
                        foreach (string str in Viruseslogs)
                        {
                            string date = GetSquareBrackets(str, 1);
                            string time = GetSquareBrackets(str, 3);
                            string path = GetSquareBrackets(str, 5);

                            TreeNode[] treeNodes = _mainForm.treeHistoryViruses.Nodes.Cast<TreeNode>().Where(r => r.Text == date).ToArray(); //https://stackoverflow.com/questions/12388249/is-there-a-method-for-searching-for-treenode-text-field-in-treeview-nodes-collec
                            if (treeNodes.Length > 0)
                            {
                                TreeNode node = _mainForm.treeHistoryViruses.Nodes.Cast<TreeNode>().Where(r => r.Text == date).ToArray()[0];
                                TreeNode tmp = node.Nodes.Add(time);
                                tmp.Name = _nodesCountViruses.ToString();
                                VirusesContainer.Add(_nodesCountViruses, $"[{path}][V]");
                                _nodesCountViruses++;
                            }
                            else
                            {
                                TreeNode node = _mainForm.treeHistoryViruses.Nodes.Add(date);
                                node.Name = "date";
                                TreeNode tmp = node.Nodes.Add(time);
                                tmp.Name = _nodesCountViruses.ToString();
                                VirusesContainer.Add(_nodesCountViruses, $"[{path}][V]");
                                _nodesCountViruses++;
                            }
                        }
                    }
                    break;

                case 2:
                    if(Suspiciouslogs.Count > 0)
                    {
                        foreach (string str in Suspiciouslogs)
                        {
                            string date = GetSquareBrackets(str, 1);
                            string time = GetSquareBrackets(str, 3);
                            string path = GetSquareBrackets(str, 5);

                            TreeNode[] treeNodes = _mainForm.treeHistoryViruses.Nodes.Cast<TreeNode>().Where(r => r.Text == date).ToArray(); //https://stackoverflow.com/questions/12388249/is-there-a-method-for-searching-for-treenode-text-field-in-treeview-nodes-collec
                            if (treeNodes.Length > 0)
                            {
                                TreeNode node = _mainForm.treeHistoryViruses.Nodes.Cast<TreeNode>().Where(r => r.Text == date).ToArray()[0];

                                TreeNode tmp = node.Nodes.Add(time);
                                tmp.Name = _nodesCountViruses.ToString();
                                VirusesContainer.Add(_nodesCountViruses, $"[{path}][S]");
                                _nodesCountViruses++;
                            }
                            else
                            {
                                TreeNode node = _mainForm.treeHistoryViruses.Nodes.Add(date);
                                node.Name = "date";
                                TreeNode tmp = node.Nodes.Add(time);
                                tmp.Name = _nodesCountViruses.ToString();
                                VirusesContainer.Add(_nodesCountViruses, $"[{path}][S]");
                                _nodesCountViruses++;
                            }
                        }          
                    }
                    break;

                case 4:
                    if(OkLogs.Count > 1)
                    {
                        foreach (string str in OkLogs)
                        { 
                            string date = GetSquareBrackets(str, 1);
                            string time = GetSquareBrackets(str, 3);
                            string path = GetSquareBrackets(str, 5);
                            if (path != "NORE")
                            {
                                string action = GetBrackets(path,1);
                                string body = GetSticks(path);

                                if (action.StartsWith("S"))
                                {
                                    TreeNode[] treeNodes = _mainForm.treeHistoryScans.Nodes.Cast<TreeNode>().Where(r => r.Text == date).ToArray(); //https://stackoverflow.com/questions/12388249/is-there-a-method-for-searching-for-treenode-text-field-in-treeview-nodes-collec
                                    if (treeNodes.Length > 0)
                                    {
                                        TreeNode node = _mainForm.treeHistoryScans.Nodes.Cast<TreeNode>().Where(r => r.Text == date).ToArray()[0];
                                        TreeNode tmp = node.Nodes.Add(time);

                                        ActionsContainer.Add(_nodesCountActions, $"[{action}][{body}]");
                                        tmp.Name = _nodesCountActions.ToString(); 
                                    }
                                    else
                                    {
                                        TreeNode node = _mainForm.treeHistoryScans.Nodes.Add(date);
                                        node.Name = "date";
                                        TreeNode tmp = node.Nodes.Add(time);

                                        ActionsContainer.Add(_nodesCountActions, $"[{action}][{body}]");
                                        tmp.Name = _nodesCountActions.ToString();
                                    }
                                }
                                else if (action.StartsWith("F"))
                                {
                                    string filestatus = GetBrackets(path, 3);
                                    TreeNode[] treeNodes = _mainForm.treeHistoryScans.Nodes.Cast<TreeNode>().Where(r => r.Text == date).ToArray(); //https://stackoverflow.com/questions/12388249/is-there-a-method-for-searching-for-treenode-text-field-in-treeview-nodes-collec
                                    if (treeNodes.Length > 0)
                                    {                                       
                                        TreeNode node = _mainForm.treeHistoryScans.Nodes.Cast<TreeNode>().Where(r => r.Text == date).ToArray()[0];
                                        TreeNode tmp = node.Nodes.Add(time);

                                        ActionsContainer.Add(_nodesCountActions, $"[{action}][{body}][{filestatus}]");
                                        tmp.Name = _nodesCountActions.ToString();
                                        _nodesCountActions++;   
                                    }
                                    else
                                    {
                                        TreeNode node = _mainForm.treeHistoryScans.Nodes.Add(date);
                                        node.Name = "date";
                                        TreeNode tmp = node.Nodes.Add(time);

                                        ActionsContainer.Add(_nodesCountActions, $"[{action}][{body}][{filestatus}]");
                                        tmp.Name = _nodesCountActions.ToString();
                                        _nodesCountActions++;
                                    }
                                }
                                else
                                {
                                    ActionsContainer[_nodesCountActions] += $"[{action.Remove(0, 1)}][{body}]";
                                    _nodesCountActions++;
                                }
                            }
                        }
                    }
                    break;
            }
        }
        public string GetSquareBrackets(string logentry, int which)
        {
            return logentry.Split('[', ']')[which];
        }

        private static string GetBrackets(string logentry, int which)
        {
            return logentry.Split('(', ')')[which];
        }

        private static string GetSticks(string logentry)
        {
            return logentry.Split('|', '|')[1];
        }

        public List<string> Viruseslogs = new List<string>(); //we make a list BEFORE appling a new one just in case we may have nothing inside (i.e. no errors)
        public List<string> Suspiciouslogs = new List<string>(); //so it's needed to get rid of all the  exceptions
        private List<string> _errorslogs = new List<string>();
        public List<string> OkLogs = new List<string>();
        public void CreateLogEntry(int wheretowrite, string events)
        {
            DateTime localDate = DateTime.Now;
            string date = $"{localDate.Day}/{localDate.Month}/{localDate.Year}";
            string time = $"{localDate.Hour}:{localDate.Minute}:{localDate.Second}"; //generate a timestamp for an entry
            
            switch (wheretowrite) //write it to a needed log
            {
                case 1: //VIRUSES
                    string tempfist = $"[{date}][{time}][{events}]";
                    string result1 = Viruseslogs.FirstOrDefault(stringToCheck => stringToCheck.Contains(events));
                    if (result1 == null)
                        Viruseslogs.Add(tempfist);
                    break;
                case 2: //SUSP
                    string tempSecond = $"[{date}][{time}][{events}]";
                    string result2 = Suspiciouslogs.FirstOrDefault(stringToCheck => stringToCheck.Contains(events));
                    if (result2 == null)
                        Suspiciouslogs.Add(tempSecond);
                    break;
                case 3: //ERRS
                    _errorslogs.Add($"[{date}][{time}][{events}]");
                    break;
                case 4: //ACTIONS / OK
                    OkLogs.Add($"[{date}][{time}][{events}]");
                    break;
            }
        }

        private void timerSaveLogs_Tick(object sender, EventArgs e) //continuously  saves logs to files. 
        {
            try
            {
                File.WriteAllLines(AppDomain.CurrentDomain.BaseDirectory + @"\logsOKs.txt", OkLogs);
                File.WriteAllLines(AppDomain.CurrentDomain.BaseDirectory + @"\logsViruses.txt", Viruseslogs);
                File.WriteAllLines(AppDomain.CurrentDomain.BaseDirectory + @"\logsSuspicious.txt", Suspiciouslogs);
                File.WriteAllLines(AppDomain.CurrentDomain.BaseDirectory + @"\logsErrors.txt", _errorslogs);
            }
            catch
            {
                _timerLogSaver.Enabled = false;
                MessageBox.Show("Can't save logs to disk!\r\nThey are probably in use or InfANT has no premissions to save there.", "Oops!",MessageBoxButtons.OK, MessageBoxIcon.Error);
                _timerLogSaver.Enabled = true;
                _timerLogSaver.Start();
            }
        }
        //----------------------------------
        //END LOGS



        //INI
        //--------------------------------------
        private System.Timers.Timer _timerLogSaver;
        private void EnableTimer()
        {
            _timerLogSaver = new System.Timers.Timer(1000) {Enabled = true};
            _timerLogSaver.Elapsed += timerSaveLogs_Tick;
            _timerLogSaver.Start();
        }
        //--------------------------------------
        //END INI


        //DATABASE
        //---------------------------------------
        public void UpdateDatabase() //updates the database from the web
        {
            string[] stringSeparators = { "\r\n" }; //that's an our separator //IDK why do I need an array to store ONE separator
            try
            {
                using (WebClient http = new WebClient())
                {
                    string database = http.DownloadString("http://bitva-pod-moskvoy.ru/_kaspersky/database.txt"); 

                    string[] lines            = database.Split(stringSeparators, StringSplitOptions.None); //as we download big STRING we have to split it into LINES
                    _mainForm.Hashes           = lines.ToList(); //loads that to RAM BEFORE saving

                    try
                    {
                        File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\database.txt", database, Encoding.UTF8); //tries to save that
                    }
                    catch
                    {
                        CreateLogEntry(3, "Can't save database");
                        MessageBox.Show("Couldn't write the database to disk! \r\nIt looks like you have no access to the folder or the file is in use.", "Oops.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    _mainForm.IsInternetConnected = true; // if everything's cool - sets the internet connectivity to true
                }
            }
            catch //if not:
            {
                if (_mainForm.IsInternetConnected)
                {
                    CreateLogEntry(3, "Can't establish an internet connection");
                    _mainForm.IsInternetConnected = false; //sets the internet connectivity to false
                    if (MessageBox.Show("Looks like you have no internet, databases and changelog weren't updated", "Can't connect to the Internet!", MessageBoxButtons.RetryCancel, MessageBoxIcon.Information) == DialogResult.Retry)
                    {
                        _mainForm.RetryInt(); //if user selects retry
                        return;
                    }
                }   
                LoadDatabase(); //if not it just skips and loads a local database
            }

            try
            {
                using (WebClient http = new WebClient())
                {
                    string databasesusp = http.DownloadString("http://bitva-pod-moskvoy.ru/_kaspersky/databasesusp.txt");

                    string[] lines      = databasesusp.Split(stringSeparators, StringSplitOptions.None); //as we download big STRING we have to split it into LINES
                    _mainForm.SuspHashes = lines.ToList(); //loads that to RAM BEFORE saving

                    try
                    {
                        File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\databasesusp.txt", databasesusp, Encoding.UTF8); //tries to save that
                    }
                    catch
                    {
                        CreateLogEntry(3, "Can't save database");
                        MessageBox.Show("Couldn't write the database to disk! \r\nIt looks like you have no access to the folder or the file is in use.", "Oops.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    _mainForm.IsInternetConnected = true; // if everything's cool - sets the internet connectivity to true
                }
            }
            catch //if not:
            {
                if (_mainForm.IsInternetConnected)
                {
                    CreateLogEntry(3, "Can't establish an internet connection");
                    _mainForm.IsInternetConnected = false; //sets the internet connectivity to false
                    if (MessageBox.Show("Looks like you have no internet, databases and changelog weren't updated", "Can't connect to the Internet!", MessageBoxButtons.RetryCancel, MessageBoxIcon.Information) == DialogResult.Retry)
                    {
                        _mainForm.RetryInt(); //if user selects retry
                        return;
                    }
                }
                LoadDatabaseSusp(); //if not it just skips and loads a local database
            }
        }
        private void LoadDatabase()
        {
            try
            {
                string[] lines  = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + @"/database.txt");
                _mainForm.Hashes = lines.ToList(); //tries to load databases from the disk
            }
            catch
            { // if no luck - closes. Why do I need an antivirus without databases?
                CreateLogEntry(3, "Can't load MAIN databases");
                MessageBox.Show("Looks like you have no internet connection and no cached databases on your PC.\r\nWithout them antivirus is completely useless!", "No databases found!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        private void LoadDatabaseSusp()
        {
            try
            {
                string[] lines = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + @"/databasesusp.txt");
                _mainForm.SuspHashes = lines.ToList(); //tries to load databases from the disk
            }
            catch
            { // if a user has main databases, but has no susp databases I don't need to force-close the app, as it still works
                CreateLogEntry(3, "Can't load suspicious databases");
            }
        }
        private void LoadLocalDatabase() //this loads your own database (added through database editor)
        {
            try
            {
                string[] lines = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + @"/localdatabase.txt"); //load it
                _mainForm.Hashes.AddRange(lines.ToList());
            }
            catch
            { /*createLogEntry(3, "Can't load local database");*/ return; } //we don't need to show an error to an user, coz it doesn't really matter for us

            try
            {
                string[] lines = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + @"/localdatabasesusp.txt"); //load it
                _mainForm.SuspHashes.AddRange(lines.ToList());
            }
            catch
            { /* not empty at all */ } //we don't need to show an error to an user, coz it doesn't really matter for us
        }
        //----------------------------------------
        //END DATABASE



        //-------------------------------------------
        //CHANGELOG 
        private string _changelog; //our temp changelog
        private void LoadChangelog()
        {
            try
            {// same as with databases
                _changelog = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"/changelog.txt");
                FormatChangelog();
            }
            catch
            {
                _changelog = "iNo internet connection and no local cached copy of the changelog were found.";
                CreateLogEntry(3, "Can't establish an internet connection");
                FormatChangelog();
            }
        }
        public void FormatChangelog()
        {
            _mainForm.textChangelog.Text = _changelog;
            List<string> newChangelog = new List<string>(); //we create a new (temp) log from our old temp log!
                                                            //this resolves string separators and makes a list of changes

            foreach (string str in _mainForm.textChangelog.Lines)
            {
                if (_mainForm.LogOnlyImportant == false)
                {
                    if (str.StartsWith("i")) //as we log not only important we add everything.
                    {                        //as we don't need there "i"s we remove them.
                        newChangelog.Add(str.Remove(0, 1));
                    }
                    else
                        newChangelog.Add(str);
                }
                else
                {
                    if (str.StartsWith("i"))
                    {
                        newChangelog.Add(str.Remove(0, 1)); //if we log only important - we add only important (still no "i"s)
                    }
                }
            _mainForm.textChangelog.Lines = newChangelog.ToArray(); //finally, we append the changelog
            }
        }
        //END CHANGELOG 
        //-------------------------------------
        private void loadingscreen_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void loadingscreen_Load(object sender, EventArgs e)
        {
            if (_usedLauncher) return;
            MessageBox.Show("Open \"_Launcher.exe\" instead!");
            Application.Exit();
        }

    }
}
