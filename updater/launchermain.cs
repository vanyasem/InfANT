using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using databaseworker;
using InfANT;

namespace launcher
{
    public partial class Mainupdater : Form
    {
        public Mainupdater()
        {
            InitializeComponent();
        }

        private void mainupdater_Load(object sender, EventArgs e)
        {
            Process[] processName = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName); //gets the count of processes with the same name
            if (processName.Length != 1) //if it's greater than 1 - close, we want only one instance running at a time
            {
                MessageBox.Show("An another instance of this application in currently running.\r\nYou can only run 1 instance at the same time.", "Oops.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Application.Exit();
                return;
            }
            
            Thread loadingThread = new Thread(LoadEverything);
            loadingThread.Start();
        }

        private void GetSizeAll()
        {
            try
            { //https://stackoverflow.com/questions/122853/get-http-file-size
                int size  = GetSize("http://bitva-pod-moskvoy.ru/_kaspersky/InfANT.exe");
                    size += GetSize("http://bitva-pod-moskvoy.ru/_kaspersky/databaseeditor.exe");
                    size += GetSize("http://bitva-pod-moskvoy.ru/_kaspersky/prgbar.dll");
                size = size / 1000;
                    labDownloadSizeActual.Invoke(new MethodInvoker(delegate { labDownloadSizeActual.Text = size + @" KB"; }));
            }
            catch 
            {
                labDownloadSizeActual.Invoke(new MethodInvoker(delegate { labDownloadSizeActual.Text = "no internet"; }));
                btnFix.Invoke(new MethodInvoker(delegate { btnFix.Enabled = false; }));
                MessageBox.Show("Looks like you have no internet connection, can't check for updates!", "Can't connect to the Internet!", MessageBoxButtons.OK, MessageBoxIcon.Error); 
            }
        }

        private int GetSize(string url)
        {
            WebRequest req = WebRequest.Create(url);
            req.Method = "HEAD";
            using (WebResponse resp = req.GetResponse())
            {
                int ContentLength;
                if (int.TryParse(resp.Headers.Get("Content-Length"), out ContentLength))
                {
                    return ContentLength;
                }
                return 0;
            }
        }

        private string _changelog;
        private string _lastVersion;
        private void LoadEverything()
        {
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\logsOKs.txt"))
            {
                File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\logsOKs.txt", @"firstlaunch");
            }

            if(File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\InfANT.exe"))
            {
                btnLaunch.Invoke(new MethodInvoker(delegate { btnLaunch.Enabled = true; }));
                btnUninstall.Invoke(new MethodInvoker(delegate { btnUninstall.Enabled = true; }));
                btnFix.Invoke(new MethodInvoker(delegate { btnFix.Enabled = true; }));
            }
            else
            {
                GetSizeAll();
                File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\lastversion.txt");
                btnUpdate.Invoke(new MethodInvoker(delegate { btnUpdate.Text = "Install"; }));
                btnUpdate.Invoke(new MethodInvoker(delegate { btnUpdate.Enabled = true; }));
            }

            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\databaseeditor.exe"))
            {
                btnDatabaseEditor.Invoke(new MethodInvoker(delegate { btnDatabaseEditor.Enabled = true; }));
            }
            else
            {
                btnUpdate.Invoke(new MethodInvoker(delegate { btnUpdate.Enabled = true; }));
            }

            try
            {
                using (WebClient http = new WebClient())
                {
                    _lastVersion = http.DownloadString("http://bitva-pod-moskvoy.ru/_kaspersky/lastversion.txt"); //tries to download it
                }

                labLastActual.Invoke(new MethodInvoker(delegate { labLastActual.Text = _lastVersion; }));

                if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\lastversion.txt"))
                {
                    if(File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\InfANT.exe"))
                        labInstalledActual.Invoke(new MethodInvoker(delegate { labInstalledActual.Text = "corrupted!"; }));    
                    else
                        labInstalledActual.Invoke(new MethodInvoker(delegate { labInstalledActual.Text = "not installed!"; }));    
                    labIsOutdatedTextActual.Invoke(new MethodInvoker(delegate { labIsOutdatedTextActual.Text = "yes!"; }));
                    btnUpdate.Enabled = true;
                    btnUpdate.Text = "Install";
                    GetSizeAll();
                }
                else
                {
                    labInstalledActual.Invoke(new MethodInvoker(delegate { labInstalledActual.Text = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"\lastversion.txt"); }));
                    if (labInstalledActual.Text != labLastActual.Text)
                    {
                        labIsOutdatedTextActual.Invoke(new MethodInvoker(delegate { labIsOutdatedTextActual.Text = "yes!"; }));
                        btnUpdate.Invoke(new MethodInvoker(delegate { btnUpdate.Enabled = true; btnUpdate.Text = "Update"; }));
                        GetSizeAll();
                    }
                    else
                    { labIsOutdatedTextActual.Invoke(new MethodInvoker(delegate { labIsOutdatedTextActual.Text = "no"; })); }
                }                    
            }
            catch { /* movie selection in Russian NetFlix */ }

            try
            {
                using (WebClient http = new WebClient())
                {
                    _changelog = http.DownloadString("http://bitva-pod-moskvoy.ru/_kaspersky/changelog.txt"); //tries to download it
                }

                try
                {
                    File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\changelog.txt", _changelog, Encoding.UTF8); //tries to save it
                }
                catch
                {
                    MessageBox.Show("Couldn't write the logs to disk! \r\nIt looks like you have no access to the folder or the file is in use.", "Oops.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                btnChangelog.Invoke(new MethodInvoker(delegate { btnChangelog.Enabled = true; }));
            }
            catch
            {
                MessageBox.Show("Looks like you have no internet connection, changelog wasn't updated", "Can't connect to the Internet!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


        }

        private void btnLaunch_Click(object sender, EventArgs e)
        {
            try
            {
                LoadingScreen loadings = new LoadingScreen(true);
                loadings.Show();
                Controls.Clear();
                Hide();
            }
            catch
            {
                MessageBox.Show("Can't launch InfANT! The file is busy or corrupt.","Fatal Error!",MessageBoxButtons.OK,MessageBoxIcon.Error);
                _isBusy = false;
                Application.Exit();
            }
        }

        private bool _isBusy;
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            progressBar.Value = 0;
            try
            {
                using (WebClient http = new WebClient())
                {
                    _isBusy = true;
                    btnLaunch.Enabled         = false;
                    btnDatabaseEditor.Enabled = false;
                    btnFix.Enabled            = false;
                    btnUninstall.Enabled      = false;
                    btnDatabaseEditor.Enabled = false;
                    progressBar.Value    = 20;
                    _lastVersion = http.DownloadString("http://bitva-pod-moskvoy.ru/_kaspersky/lastversion.txt"); //tries to download it
                    File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\lastversion.txt", _lastVersion, Encoding.UTF8);
                    progressBar.Value    = 40;
                    http.DownloadFile("http://bitva-pod-moskvoy.ru/_kaspersky/prgbar.dll", "prgbar.dll"); //tries to download it
                    progressBar.Value    = 60;
                    http.DownloadFile("http://bitva-pod-moskvoy.ru/_kaspersky/InfANT.exe", "InfANT.exe"); //tries to download it
                    progressBar.Value    = 80;
                    http.DownloadFile("http://bitva-pod-moskvoy.ru/_kaspersky/databaseeditor.exe", "databaseeditor.exe"); //tries to download it
                    File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\logsOKs.txt", @"firstlaunch");
                    progressBar.Value    = 100;
                }
            }
            catch
            {
                MessageBox.Show("Looks like you have no internet connection, can't install/update the app!", "Can't connect to the Internet!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _isBusy = false;
                return;
            }
            MessageBox.Show("Installing/updating finished!", "Yay!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            btnLaunch.Enabled = true;
            btnDatabaseEditor.Enabled = true;
            btnFix.Enabled = true;
            btnUninstall.Enabled = true;
            btnUpdate.Enabled = false;
            btnDatabaseEditor.Enabled = true;
            labInstalledActual.Text = _lastVersion;
            labIsOutdatedTextActual.Text = "no";
            labDownloadSizeActual.Text = @"0 KB";
            _isBusy = false;    
        }

        private void btnUninstall_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("This action will uninstall this app from your computer. Launcher will not be deleted.", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    _isBusy = true;
                    btnLaunch.Enabled = false;
                    btnFix.Enabled = false;
                    btnUninstall.Enabled = false;
                    btnDatabaseEditor.Enabled = false;
                    progressBar.Value = 0;     
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\changelog.txt");
                    progressBar.Value = 10;
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\databaseeditor.exe");
                    progressBar.Value = 20;
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\database.txt");
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\databasesusp.txt");
                    progressBar.Value = 30;
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\localdatabase.txt");
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\localdatabasesusp.txt");
                    progressBar.Value = 40;
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\prgbar.dll");
                    progressBar.Value = 50;
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\InfANT.exe");
                    progressBar.Value = 60;
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\lastversion.txt");
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\lastpath.txt");
                    progressBar.Value = 70;
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\logsErrors.txt");
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\logsOKs.txt");
                    progressBar.Value = 80;
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\logsSuspicious.txt");
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\logsViruses.txt");
                    progressBar.Value = 90;
                    progressBar.Value = 100;
                    MessageBox.Show("We hope you had a good experience using our software! \r\n Launcher will close now.", "Uninstall finished successfully!");
                    _isBusy = false;
                    Application.Exit();
                }
            }
            catch
            {
                MessageBox.Show("There was an error uninstalling this software! ", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _isBusy = false;
            }
            
        }

        private void btnChangelog_Click(object sender, EventArgs e)
        {
            string[] stringSeparators = { "\r\n" };
            string[] lines = _changelog.Split(stringSeparators, StringSplitOptions.None);

            for (int i = 0; i < lines.Length; i++)
            {
                if(lines[i].StartsWith("i"))
                {
                    lines[i] = lines[i].Remove(0, 1);
                }
            }
            MessageBox.Show(string.Format("{4}\r\n{3}\r\n{2}\r\n{1}\r\n{0}", lines[lines.Length - 1], lines[lines.Length - 2], lines[lines.Length - 3], lines[lines.Length - 4], lines[lines.Length - 5]));
            //this one makes my eyes bleed ^
        }

        private void mainupdater_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(_isBusy)
                e.Cancel = true;
            else
                e.Cancel = false;
        }

        private void btnFix_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("This action will RESET all of your settings and will CLEAN your logs. Also, it will renew some files.", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    _isBusy = true;
                    btnLaunch.Enabled = false;
                    btnFix.Enabled = false;
                    btnUninstall.Enabled = false;
                    using (WebClient http = new WebClient())
                    {
                        http.DownloadFile("http://bitva-pod-moskvoy.ru/_kaspersky/prgbar.dll", "prgbar.dll"); //tries to download it
                        http.DownloadFile("http://bitva-pod-moskvoy.ru/_kaspersky/databaseeditor.exe", "databaseeditor.exe"); //tries to download it
                        File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\logsOKs.txt");
                        File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\logsSuspicious.txt");
                        File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\logsViruses.txt");
                        File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\logsErrors.txt");
                        File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\logsOKs.txt", "firstlaunch");
                    }
                    btnLaunch.Enabled = true;
                    btnFix.Enabled = true;
                    btnUninstall.Enabled = true;
                    _isBusy = false;
                }
                MessageBox.Show("InfANT was fixed successfully!","Done!",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
            catch
            {
                MessageBox.Show("Something went wrong, but nothing bad happened. Just try again.","Oops.");
            }
        }

        private void btnDatabaseEditor_Click(object sender, EventArgs e)
        {
            databaseeditor database = new databaseeditor(true);
            database.Show();
            Controls.Clear();
            Hide();
        }
    }
}
