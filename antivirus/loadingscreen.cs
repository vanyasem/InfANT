using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Net;
using System.Timers;
using InfANT.Properties;

namespace antivirus
{
    public partial class loadingscreen : Form
    {
        private main mainform; //we use that to access main form :3

        bool usedlauncher;
        public loadingscreen(bool usedlauncherbool)
        {
            InitializeComponent();
            usedlauncher = usedlauncherbool;
        } 

        System.Windows.Forms.Timer myTimer;
        private void loadingscreen_Shown(object sender, EventArgs e) //happens right after it's showm BUT NOT FULLY DREWN!!! Load happend BEFORE, so it makes a blank gap.
        {
            myTimer  = new System.Windows.Forms.Timer();
            myTimer.Interval = 200;
            myTimer.Tick += kostil;
            myTimer.Start(); //as it's not fully drewn but we WANT it to we set a small timer. The timer works in an another thread so the UI thread will be able to finish drawing.
        }
        private void kostil(object sender, EventArgs e)
        {
            myTimer.Stop(); //we don't want to fire it more than I time, so immediatly stop it.

            Thread th = new Thread(kostil2); //we need an another thread so the main thread will remain responsible.
            th.IsBackground = true; //we want our thread to close with the program and not run apart, this will do it
            th.Start();
            ProgressLoading.Invoke(new MethodInvoker(delegate { ProgressLoading.Value = 20; }));
        }
        private void kostil2()
        {
            ProgressLoading.Invoke(new MethodInvoker(delegate { ProgressLoading.Value = 40; }));
            mainform = new main(this); //launch main form to access vars, but don't show it
            mainform.labelYoVersionLab.Text = mainform.ver + " "+ mainform.build;
            ProgressLoading.Invoke(new MethodInvoker(delegate { ProgressLoading.Value = 60; }));

            loadLogs();
            CheckForCorruptions();
            readLogs(0);
            ProgressLoading.Invoke(new MethodInvoker(delegate { ProgressLoading.Value = 80; }));
            enabletimer();
            updatedatabase();
            loadlocaldatabase();
            ProgressLoading.Invoke(new MethodInvoker(delegate { ProgressLoading.Value = 100; }));
            loadchangelog();

            this.Invoke(new MethodInvoker(delegate { createIconMenuStructure(); mainform.TopMost = true; mainform.Show(); this.Hide(); this.Controls.Clear(); mainform.TopMost = false; this.Text = "InfANT Helper"; })); //show the main form, hides this form and sets the name of it to "Helper"
        }

        private void CheckForCorruptions()
        {
            if (getSquareBrackets(OKlogs[OKlogs.Count - 1],5).StartsWith("(S")) //If there's a start entry, but no end entry, do this:
            {   
                createLogEntry(4, "(EScan was rudely interrupted, was not finished correctly)|unknown|");
            }
        }
        //---------------------------------



        //TASKBARICO
        //--------------------------------
        public  NotifyIcon notifyIcon1 = new NotifyIcon();
        private ContextMenu contextMenu1 = new ContextMenu();
        private void createIconMenuStructure()
        {
            //we create a contextMenu fisrst
            contextMenu1.MenuItems.Add("Open").Click += new EventHandler(mainform.MenuOpen); //Triggers on menu-click
            contextMenu1.MenuItems.Add("Fast-Scan").Click += new EventHandler(mainform.MenuFast);
            contextMenu1.MenuItems.Add("Exit").Click += new EventHandler(mainform.MenuExit);

            notifyIcon1.Visible = true;
            changeIco(); //We enable the ico and set the right appearance

            notifyIcon1.Text = "Your computer is safe";
            notifyIcon1.ContextMenu = contextMenu1; //And then assign it to a notifyIcon
            notifyIcon1.Click += new EventHandler(mainform.MenuOpen); //This triggers on click of a taskbar ico
        }
        public void changeIco()
        {
            if (mainform.infected == false) //if infected - set "bad" ico, if not - "good" ico
            {
                notifyIcon1.Icon = Resources.favicoOK;
            }
            else
            {
                notifyIcon1.Icon = Resources.favicoinfected;
            }
        }
        //--------------------------------
        //ENDTASKBARICO



        //LOGS
        //----------------------------------
        private void loadLogs()
        {
            try
            {
                OKlogs         = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + @"\logsOKs.txt", Encoding.UTF8).ToList<string>();

                if (OKlogs[0] == "firstlaunch")
                {
                    File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\logsViruses.txt", "");
                    File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\logsSuspicious.txt", "");
                    File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\logsErrors.txt", "");
                    OKlogs.Clear();
                    OKlogs.Add("[I][G][NORE]");
                }

                Viruseslogs    = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + @"\logsViruses.txt", Encoding.UTF8).ToList<string>();
                Suspiciouslogs = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + @"\logsSuspicious.txt", Encoding.UTF8).ToList<string>();
                Errorslogs     = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + @"\logsErrors.txt", Encoding.UTF8).ToList<string>();     
                //loads logs into RAM     
            }
            catch
            {
                MessageBox.Show("Logs weren't found! \r\nPlease, don't delete them by yourself, do it using the 'Settings' tab.\r\nRelaunch the application!", "Fatal Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);

                File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\logsOKs.txt", "firstlaunch");
                        
                Application.Exit(); 
            }
        }

        //0 - All
        //1 - Viruses
        //2 - Suspicious
        //3 - Errors //we don't need to read errors to the user, so no case for this
        //4 - Actions (Eg. Scans, Changes)
        int nodescountviruses = 0;
        int nodescountactions = 0;
        public Dictionary<int, string> actionscontainer = new Dictionary<int, string>();
        public Dictionary<int, string> virusescontainer = new Dictionary<int, string>();
        public void readLogs(int whattoread)
        {
            switch (whattoread)
            {
                case 0:
                    mainform.treeHistoryScans.Nodes.Clear();
                    mainform.treeHistoryViruses.Nodes.Clear();
                    actionscontainer.Clear();
                    virusescontainer.Clear();
                    readLogs(1);
                    readLogs(2);
                    readLogs(4);
                    break;

                case 1: 
                    if(Viruseslogs.Count > 0)
                    {
                        foreach (string str in Viruseslogs)
                        {
                            string date = getSquareBrackets(str, 1);
                            string time = getSquareBrackets(str, 3);
                            string path = getSquareBrackets(str, 5);

                            TreeNode[] treeNodes = mainform.treeHistoryViruses.Nodes.Cast<TreeNode>().Where(r => r.Text == date).ToArray(); //https://stackoverflow.com/questions/12388249/is-there-a-method-for-searching-for-treenode-text-field-in-treeview-nodes-collec
                            if (treeNodes.Length > 0)
                            {
                                TreeNode node = mainform.treeHistoryViruses.Nodes.Cast<TreeNode>().Where(r => r.Text == date).ToArray()[0];
                                TreeNode tmp = node.Nodes.Add(time);
                                tmp.Name = nodescountviruses.ToString();
                                virusescontainer.Add(nodescountviruses, String.Format("[{0}][V]", path));
                                nodescountviruses++;
                            }
                            else
                            {
                                TreeNode node = mainform.treeHistoryViruses.Nodes.Add(date);
                                node.Name = "date";
                                TreeNode tmp = node.Nodes.Add(time);
                                tmp.Name = nodescountviruses.ToString();
                                virusescontainer.Add(nodescountviruses, String.Format("[{0}][V]", path));
                                nodescountviruses++;
                            }
                        }
                    }
                    break;

                case 2:
                    if(Suspiciouslogs.Count > 0)
                    {
                        foreach (string str in Suspiciouslogs)
                        {
                            string date = getSquareBrackets(str, 1);
                            string time = getSquareBrackets(str, 3);
                            string path = getSquareBrackets(str, 5);

                            TreeNode[] treeNodes = mainform.treeHistoryViruses.Nodes.Cast<TreeNode>().Where(r => r.Text == date).ToArray(); //https://stackoverflow.com/questions/12388249/is-there-a-method-for-searching-for-treenode-text-field-in-treeview-nodes-collec
                            if (treeNodes.Length > 0)
                            {
                                TreeNode node = mainform.treeHistoryViruses.Nodes.Cast<TreeNode>().Where(r => r.Text == date).ToArray()[0];

                                TreeNode tmp = node.Nodes.Add(time);
                                tmp.Name = nodescountviruses.ToString();
                                virusescontainer.Add(nodescountviruses, String.Format("[{0}][S]", path));
                                nodescountviruses++;
                            }
                            else
                            {
                                TreeNode node = mainform.treeHistoryViruses.Nodes.Add(date);
                                node.Name = "date";
                                TreeNode tmp = node.Nodes.Add(time);
                                tmp.Name = nodescountviruses.ToString();
                                virusescontainer.Add(nodescountviruses, String.Format("[{0}][S]", path));
                                nodescountviruses++;
                            }
                        }          
                    }
                    break;

                case 4:
                    if(OKlogs.Count > 1)
                    {
                        foreach (string str in OKlogs)
                        { 
                            string date = getSquareBrackets(str, 1);
                            string time = getSquareBrackets(str, 3);
                            string path = getSquareBrackets(str, 5);
                            string action = "";
                            string body = "";
                            if (path != "NORE")
                            {
                                action = getBrackets(path,1);
                                body = getSticks(path); //path

                                if (action.StartsWith("S"))
                                {
                                    TreeNode[] treeNodes = mainform.treeHistoryScans.Nodes.Cast<TreeNode>().Where(r => r.Text == date).ToArray(); //https://stackoverflow.com/questions/12388249/is-there-a-method-for-searching-for-treenode-text-field-in-treeview-nodes-collec
                                    if (treeNodes.Length > 0)
                                    {
                                        TreeNode node = mainform.treeHistoryScans.Nodes.Cast<TreeNode>().Where(r => r.Text == date).ToArray()[0];
                                        TreeNode tmp = node.Nodes.Add(time);

                                        actionscontainer.Add(nodescountactions, String.Format("[{0}][{1}]", action, body));
                                        tmp.Name = nodescountactions.ToString(); 
                                    }
                                    else
                                    {
                                        TreeNode node = mainform.treeHistoryScans.Nodes.Add(date);
                                        node.Name = "date";
                                        TreeNode tmp = node.Nodes.Add(time);

                                        actionscontainer.Add(nodescountactions, String.Format("[{0}][{1}]", action, body));
                                        tmp.Name = nodescountactions.ToString();
                                    }
                                }
                                else if (action.StartsWith("F"))
                                {
                                    string filestatus = getBrackets(path, 3);
                                    TreeNode[] treeNodes = mainform.treeHistoryScans.Nodes.Cast<TreeNode>().Where(r => r.Text == date).ToArray(); //https://stackoverflow.com/questions/12388249/is-there-a-method-for-searching-for-treenode-text-field-in-treeview-nodes-collec
                                    if (treeNodes.Length > 0)
                                    {                                       
                                        TreeNode node = mainform.treeHistoryScans.Nodes.Cast<TreeNode>().Where(r => r.Text == date).ToArray()[0];
                                        TreeNode tmp = node.Nodes.Add(time);

                                        actionscontainer.Add(nodescountactions, String.Format("[{0}][{1}][{2}]", action, body, filestatus));
                                        tmp.Name = nodescountactions.ToString();
                                        nodescountactions++;   
                                    }
                                    else
                                    {
                                        TreeNode node = mainform.treeHistoryScans.Nodes.Add(date);
                                        node.Name = "date";
                                        TreeNode tmp = node.Nodes.Add(time);

                                        actionscontainer.Add(nodescountactions, String.Format("[{0}][{1}][{2}]", action, body, filestatus));
                                        tmp.Name = nodescountactions.ToString();
                                        nodescountactions++;
                                    }
                                }
                                else
                                {
                                    actionscontainer[nodescountactions] += String.Format("[{0}][{1}]", action.Remove(0,1), body);
                                    nodescountactions++;
                                }
                            }
                        }
                    }
                    break;
            }
        }
        public string getSquareBrackets(string logentry, int which)
        {
            return logentry.Split('[', ']')[which];
        }
        public string getBrackets(string logentry, int which)
        {
            return logentry.Split('(', ')')[which];
        }
        public string getSticks(string logentry)
        {
            return logentry.Split('|', '|')[1];
        }

        public List<string> Viruseslogs = new List<string>(); //we make a list BEFORE appling a new one just in case we may have nothing inside (i.e. no errors)
        public List<string> Suspiciouslogs = new List<string>(); //so it's needed to get rid of all the  exceptions
        public List<string> Errorslogs = new List<string>();
        public List<string> OKlogs = new List<string>();
        public void createLogEntry(int wheretowrite, string events)
        {
            DateTime localDate = DateTime.Now;
            string date = String.Format("{0}/{1}/{2}", localDate.Day.ToString(), localDate.Month.ToString(), localDate.Year.ToString());
            string time = String.Format("{0}:{1}:{2}", localDate.Hour.ToString(), localDate.Minute.ToString(), localDate.Second.ToString()); //generate a timestamp for an entry
            
            switch (wheretowrite) //write it to a needed log
            {
                case 1: //VIRUSES
                    string tempfist = String.Format("[{0}][{1}][{2}]", date, time, events);
                    string result1 = Viruseslogs.FirstOrDefault(stringToCheck => stringToCheck.Contains(events));
                    if (result1 == null)
                        Viruseslogs.Add(tempfist);
                    break;
                case 2: //SUSP
                    string tempscnd = String.Format("[{0}][{1}][{2}]", date, time, events);
                    string result2 = Suspiciouslogs.FirstOrDefault(stringToCheck => stringToCheck.Contains(events));
                    if (result2 == null)
                        Suspiciouslogs.Add(tempscnd);
                    break;
                case 3: //ERRS
                    Errorslogs.Add(String.Format("[{0}][{1}][{2}]", date, time, events));
                    break;
                case 4: //ACTIONS / OK
                    OKlogs.Add(String.Format("[{0}][{1}][{2}]", date, time, events));
                    break;
            }
        }

        private void timerSaveLogs_Tick(object sender, EventArgs e) //continuously  saves logs to files. 
        {
            try
            {
                File.WriteAllLines(AppDomain.CurrentDomain.BaseDirectory + @"\logsOKs.txt", OKlogs);
                File.WriteAllLines(AppDomain.CurrentDomain.BaseDirectory + @"\logsViruses.txt", Viruseslogs);
                File.WriteAllLines(AppDomain.CurrentDomain.BaseDirectory + @"\logsSuspicious.txt", Suspiciouslogs);
                File.WriteAllLines(AppDomain.CurrentDomain.BaseDirectory + @"\logsErrors.txt", Errorslogs);
            }
            catch
            {
                timerLogSaver.Enabled = false;
                MessageBox.Show("Can't save logs to disk!\r\nThey are probably in use or InfANT has no premissions to save there.", "Oops!",MessageBoxButtons.OK, MessageBoxIcon.Error);
                timerLogSaver.Enabled = true;
                timerLogSaver.Start();
            }
        }
        //----------------------------------
        //END LOGS



        //INI
        //--------------------------------------
        System.Timers.Timer timerLogSaver;
        private void enabletimer()
        {
            timerLogSaver = new System.Timers.Timer(1000);
            timerLogSaver.Enabled = true;
            timerLogSaver.Elapsed += new ElapsedEventHandler(timerSaveLogs_Tick);
            timerLogSaver.Start();
        }
        //--------------------------------------
        //END INI


        //DATABASE
        //---------------------------------------
        public void updatedatabase() //updates the database from the web
        {
            string[] stringSeparators = new string[] { "\r\n" }; //that's an our seperator //IDK why do I need an array to store ONE seperator, lol
            try
            {
                using (WebClient http = new WebClient())
                {
                    string database = http.DownloadString("http://bitva-pod-moskvoy.ru/_kaspersky/database.txt"); 

                    string[] lines            = database.Split(stringSeparators, StringSplitOptions.None); //as we download big STRING we have to split it into LINES
                    mainform.hashes           = lines.ToList<string>(); //loads that to RAM BEFORE saving

                    try
                    {
                        File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\database.txt", database, Encoding.UTF8); //tries to save that
                    }
                    catch
                    {
                        createLogEntry(3, "Can't save database");
                        MessageBox.Show("Couldn't write the database to disk! \r\nIt looks like you have no access to the folder or the file is in use.", "Oops.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    mainform.isinternetconnected = true; // if everything's cool - sets the internet connectivity to true
                }
            }
            catch //if not:
            {
                if (mainform.isinternetconnected != false)
                {
                    createLogEntry(3, "Can't establish an internet connection");
                    mainform.isinternetconnected = false; //sets the internet connectivity to false
                    if (MessageBox.Show("Looks like you have no internet, databases and changelog weren't updated", "Can't connect to the Internet!", MessageBoxButtons.RetryCancel, MessageBoxIcon.Information) == DialogResult.Retry)
                    {
                        mainform.RetryInt(); //if user selects retry
                        return;
                    }
                }   
                loaddatabase(); //if not it just skips and loads a local database
            }

            try
            {
                using (WebClient http = new WebClient())
                {
                    string databasesusp = http.DownloadString("http://bitva-pod-moskvoy.ru/_kaspersky/databasesusp.txt");

                    string[] lines      = databasesusp.Split(stringSeparators, StringSplitOptions.None); //as we download big STRING we have to split it into LINES
                    mainform.susphashes = lines.ToList<string>(); //loads that to RAM BEFORE saving

                    try
                    {
                        File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\databasesusp.txt", databasesusp, Encoding.UTF8); //tries to save that
                    }
                    catch
                    {
                        createLogEntry(3, "Can't save database");
                        MessageBox.Show("Couldn't write the database to disk! \r\nIt looks like you have no access to the folder or the file is in use.", "Oops.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    mainform.isinternetconnected = true; // if everything's cool - sets the internet connectivity to true
                }
            }
            catch //if not:
            {
                if (mainform.isinternetconnected != false)
                {
                    createLogEntry(3, "Can't establish an internet connection");
                    mainform.isinternetconnected = false; //sets the internet connectivity to false
                    if (MessageBox.Show("Looks like you have no internet, databases and changelog weren't updated", "Can't connect to the Internet!", MessageBoxButtons.RetryCancel, MessageBoxIcon.Information) == DialogResult.Retry)
                    {
                        mainform.RetryInt(); //if user selects retry
                        return;
                    }
                }
                loaddatabasesusp(); //if not it just skips and loads a local database
            }
        }
        private void loaddatabase()
        {
            try
            {
                string[] lines  = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + @"/database.txt");
                mainform.hashes = lines.ToList<string>(); //trys to load databases from the disk
            }
            catch
            { // if no luck - closes. Why do I need an antivirus without databases?
                createLogEntry(3, "Can't load MAIN databases");
                MessageBox.Show("Looks like you have no internet connection and no cached databases on your PC.\r\nWithout them antivirus is completely useless!", "No databases found!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        private void loaddatabasesusp()
        {
            try
            {
                string[] lines = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + @"/databasesusp.txt");
                mainform.susphashes = lines.ToList<string>(); //trys to load databases from the disk
            }
            catch
            { // if a user has main databases, but has no susp databases I don't need to force-close the app, as it still works
                createLogEntry(3, "Can't load suspicious databases");
            }
        }
        private void loadlocaldatabase() //this loads your own database (added through database editor)
        {
            try
            {
                string[] lines = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + @"/localdatabase.txt"); //load it
                mainform.hashes.AddRange(lines.ToList<string>());
            }
            catch
            { /*createLogEntry(3, "Can't load local database");*/ return; } //we don't need to show an error to an user, coz it doesn't really matter for us

            try
            {
                string[] lines = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + @"/localdatabasesusp.txt"); //load it
                mainform.susphashes.AddRange(lines.ToList<string>());
            }
            catch
            { /*CreateLogEntry(3, "Can't load local database");*/ return; } //we don't need to show an error to an user, coz it doesn't really matter for us
        }
        //----------------------------------------
        //END DATABASE



        //-------------------------------------------
        //CHANGELOG 
        private string changelog; //our temp changelog
        private void loadchangelog()
        {
            try
            {// same as with databases
                changelog = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"/changelog.txt");
                formatchangelog();
            }
            catch
            {
                changelog = "iNo internet connection and no local cached copy of the changelog were found.";
                createLogEntry(3, "Can't establish an internet connection");
                formatchangelog();
            }
        }
        public void formatchangelog()
        {
            mainform.textChangelog.Text = changelog;
            List<string> newchangelog = new List<string>(); //we create a new (temp) log from our old temp log!
                                                            //this resolves string seperators and makes a list of changes

            foreach (string str in mainform.textChangelog.Lines)
            {
                if (mainform.logonlyimportant == false)
                {
                    if (str.StartsWith("i")) //as we log not only important we add everything.
                    {                        //as we don't need there "i"s we remove them.
                        newchangelog.Add(str.Remove(0, 1));
                    }
                    else
                        newchangelog.Add(str);
                }
                else
                {
                    if (str.StartsWith("i"))
                    {
                        newchangelog.Add(str.Remove(0, 1)); //if we log only inportant - we add only important (still no "i"s)
                    }
                }
            mainform.textChangelog.Lines = newchangelog.ToArray(); //finally, we append the changelog
            }
        }
        //END CHANGELOG 
        //-------------------------------------
        private void loadingscreen_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
            return; //this is needed to stop all running functions
        }

        private void loadingscreen_Load(object sender, EventArgs e)
        {
            if (!usedlauncher)
            {
                MessageBox.Show("Open \"_Launcher.exe\" instead!");
                Application.Exit();
            }    
        }

    }
}
