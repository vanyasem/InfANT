using System;
using System.Diagnostics;
using System.Globalization;
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
        private bool _isInternetConnected;
        public Mainupdater()
        {
            if (File.Exists("lang.ini"))
            {
                string temp = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"\lang.ini");
                Thread.CurrentThread.CurrentCulture = new CultureInfo(temp);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(temp);
            }
            InitializeComponent();
        }

        private void mainupdater_Load(object sender, EventArgs e)
        {
            Process[] processName = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName); //gets the count of processes with the same name
            if (processName.Length != 1) //if it's greater than 1 - close, we want only one instance running at a time
            {
                MessageBox.Show(LanguageResources.an_another_is_running_only_one_allowed, LanguageResources.oops, MessageBoxButtons.OK, MessageBoxIcon.Information);
                Application.Exit();
                return;
            }

            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\lang.ini"))
            {
                CultureInfo currentCulture = Thread.CurrentThread.CurrentUICulture;
                if (currentCulture.ToString().StartsWith("ru"))
                    File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\lang.ini", @"ru");
                else
                    File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\lang.ini", @"en");
            }

            Thread loadingThread = new Thread(LoadEverything) {IsBackground = true};
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
                labDownloadSizeActual.Invoke(new MethodInvoker(delegate { labDownloadSizeActual.Text = LanguageResources.no_internet_sign; }));
                btnFix.Invoke(new MethodInvoker(delegate { btnFix.Enabled = false; }));
                MessageBox.Show(LanguageResources.no_internet_cant_check_updates, LanguageResources.cant_connect_to_internet, MessageBoxButtons.OK, MessageBoxIcon.Error); 
            }
        }

        private int GetSize(string url)
        {
            WebRequest req = WebRequest.Create(url);
            req.Method = "HEAD";
            using (WebResponse resp = req.GetResponse())
            {
                int contentLength;
                if (int.TryParse(resp.Headers.Get("Content-Length"), out contentLength))
                {
                    return contentLength;
                }
                return 0;
            }
        }

        private string _changelog;
        private string _lastVersion;
        private void LoadEverything()
        {
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\logsOKs.txt"))
                File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\logsOKs.txt", @"firstlaunch");
           
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\databaseeditor.exe"))
                btnDatabaseEditor.Invoke(new MethodInvoker(delegate { btnDatabaseEditor.Enabled = true; }));

            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\InfANT.exe"))
                btnLaunch.Invoke(new MethodInvoker(delegate { btnLaunch.Enabled = true; }));

            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\ru\_Launcher.resources.dll"))
                btnLang.Invoke(new MethodInvoker(delegate { btnLang.Enabled = true; }));

            

            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\lastversion.txt"))
            {
                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\InfANT.exe"))
                    labInstalledActual.Invoke(new MethodInvoker(delegate { labInstalledActual.Text = LanguageResources.corrupted_sign; }));
                else
                    labInstalledActual.Invoke(new MethodInvoker(delegate { labInstalledActual.Text = LanguageResources.not_installed; }));
                labIsOutdatedTextActual.Invoke(new MethodInvoker(delegate { labIsOutdatedTextActual.Text = LanguageResources.yes_exc; }));
                btnUpdate.Invoke(new MethodInvoker(delegate { btnUpdate.Text = LanguageResources.install; }));
            }
            else
                labInstalledActual.Invoke(new MethodInvoker(delegate { labInstalledActual.Text = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"\lastversion.txt"); }));

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
                    MessageBox.Show(LanguageResources.cant_write_logs_no_access, LanguageResources.oops, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                using (WebClient http = new WebClient())
                {
                    _lastVersion = http.DownloadString("http://bitva-pod-moskvoy.ru/_kaspersky/lastversion.txt"); //tries to download it
                    labLastActual.Invoke(new MethodInvoker(delegate { labLastActual.Text = _lastVersion; }));
                }

                btnChangelog.Invoke(new MethodInvoker(delegate { btnChangelog.Enabled = true; }));
                _isInternetConnected = true;
            }
            catch
            {
                MessageBox.Show(LanguageResources.no_internet_changelog_wasnt_updated, LanguageResources.cant_connect_to_internet, MessageBoxButtons.OK, MessageBoxIcon.Error);
                _isInternetConnected = false;
            }

            if (!_isInternetConnected) return;
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\InfANT.exe"))
            {
                btnUninstall.Invoke(new MethodInvoker(delegate { btnUninstall.Enabled = true; }));
                btnFix.Invoke(new MethodInvoker(delegate { btnFix.Enabled = true; }));

                if (labInstalledActual.Text != labLastActual.Text)
                {
                    btnUpdate.Invoke(new MethodInvoker(delegate { btnUpdate.Enabled = true; btnUpdate.Text = LanguageResources.update; }));
                    GetSizeAll();
                }
            }
            else
            {
                GetSizeAll();
                File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\lastversion.txt");
                btnUpdate.Invoke(new MethodInvoker(delegate { btnUpdate.Enabled = true; }));
            }

            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\databaseeditor.exe"))
                btnUpdate.Invoke(new MethodInvoker(delegate { btnUpdate.Enabled = true; }));
            btnLang.Invoke(new MethodInvoker(delegate { btnLang.Enabled = true; }));

            try
            {
                if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\lastversion.txt"))
                {           
                    btnUpdate.Invoke(new MethodInvoker(delegate { btnUpdate.Enabled = true; }));
                    GetSizeAll();
                }
                else
                {     
                    if (labInstalledActual.Text != labLastActual.Text)
                    {
                        labIsOutdatedTextActual.Invoke(new MethodInvoker(delegate { labIsOutdatedTextActual.Text = LanguageResources.yes_exc; }));
                        btnUpdate.Invoke(new MethodInvoker(delegate { btnUpdate.Enabled = true; btnUpdate.Text = LanguageResources.update; }));
                        GetSizeAll();
                    }
                    else
                    { labIsOutdatedTextActual.Invoke(new MethodInvoker(delegate { labIsOutdatedTextActual.Text = LanguageResources.no; })); }
                }
            }
            catch { /* movie selection in Russian NetFlix */ }
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
                MessageBox.Show(LanguageResources.cant_laucnch_busy_corrupt,LanguageResources.fatal_error,MessageBoxButtons.OK,MessageBoxIcon.Error);
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
                MessageBox.Show(LanguageResources.no_internet_cant_update_install, LanguageResources.cant_connect_to_internet, MessageBoxButtons.OK, MessageBoxIcon.Error);
                _isBusy = false;
                return;
            }
            MessageBox.Show(LanguageResources.installing_updating_finished, LanguageResources.yay, MessageBoxButtons.OK, MessageBoxIcon.Information);
            btnLaunch.Enabled = true;
            btnDatabaseEditor.Enabled = true;
            btnFix.Enabled = true;
            btnUninstall.Enabled = true;
            btnUpdate.Enabled = false;
            btnDatabaseEditor.Enabled = true;
            labInstalledActual.Text = _lastVersion;
            labIsOutdatedTextActual.Text = LanguageResources.no;
            labDownloadSizeActual.Text = @"0 KB";
            _isBusy = false;    
        }

        private void btnUninstall_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(LanguageResources.action_will_uninstall_launcher_wont_be_deleted, LanguageResources.are_u_sure, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                _isBusy = true;
                btnLaunch.Enabled = false;
                btnFix.Enabled = false;
                btnUninstall.Enabled = false;
                btnDatabaseEditor.Enabled = false;
                progressBar.Value = 0;
                try
                {
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\changelog.txt");
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\lang.ini");
                }
                catch { /* */ }
                progressBar.Value = 10;
                try
                {
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\databaseeditor.exe");
                }
                catch { /* */ }
                progressBar.Value = 20;
                try
                {
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\database.txt");
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\databasesusp.txt");
                }
                catch { /* */ }       
                progressBar.Value = 30;
                try
                {
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\localdatabase.txt");
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\localdatabasesusp.txt");
                }
                catch { /* */ }
                progressBar.Value = 40;
                try
                {
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\prgbar.dll");
                }
                catch { /* */ }
                progressBar.Value = 50;
                try
                {
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\InfANT.exe");
                }
                catch { /* */ }
                progressBar.Value = 60;
                try
                {
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\lastversion.txt");
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\lastpath.txt");
                }
                catch { /* */ }
                progressBar.Value = 70;
                try
                {
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\logsErrors.txt");
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\logsOKs.txt");
                }
                catch { /* */ }
                progressBar.Value = 80;
                try
                {
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\logsSuspicious.txt");
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\logsViruses.txt");
                }
                catch { /* */ }
                progressBar.Value = 90;
                try
                {
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"ru\databaseeditor.resources.dll");
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"ru\InfANT.resources.dll");
                }
                catch { /* */ }
                progressBar.Value = 100;
                MessageBox.Show(LanguageResources.hope_u_had_good_experience_will_close_now, LanguageResources.uninstall_finished_successfully);
                _isBusy = false;
                Application.Exit();
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
                if (MessageBox.Show(LanguageResources.this_will_reset_settings_delete_logs_renew_files, LanguageResources.are_u_sure, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
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
                        File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\logsOKs.txt", @"firstlaunch");
                        File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\lang.ini", @"en");
                        File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"ru\databaseeditor.resources.dll");
                        File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"ru\InfANT.resources.dll");
                        _lastVersion = http.DownloadString("http://bitva-pod-moskvoy.ru/_kaspersky/lastversion.txt"); //tries to download it
                        File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\lastversion.txt", _lastVersion, Encoding.UTF8);
                    }
                    btnLaunch.Enabled = true;
                    btnFix.Enabled = true;
                    btnUninstall.Enabled = true;
                    labInstalledActual.Text = _lastVersion;
                    labIsOutdatedTextActual.Text = LanguageResources.no;
                    labDownloadSizeActual.Text = @"0 KB";
                    btnUpdate.Enabled = false;
                    _isBusy = false;
                    MessageBox.Show(LanguageResources.was_fixed_successfully, LanguageResources.done, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Application.Restart();
                }
            }
            catch
            {
                MessageBox.Show(LanguageResources.something_wrong_but_nothing_bad, LanguageResources.oops);
                Application.Restart();
            }
        }

        private void btnDatabaseEditor_Click(object sender, EventArgs e)
        {
            Databaseeditor database = new Databaseeditor(true);
            database.Show();
            Controls.Clear();
            Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"ru\_Launcher.resources.dll"))
            {
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"\ru");
                using (WebClient http = new WebClient())
                    http.DownloadFile("http://bitva-pod-moskvoy.ru/_kaspersky/_Launcher.resources.dll", @"ru\_Launcher.resources.dll"); //tries to download it
            }

            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"ru\databaseeditor.resources.dll"))
            {
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"\ru");
                using (WebClient http = new WebClient())
                    http.DownloadFile("http://bitva-pod-moskvoy.ru/_kaspersky/databaseeditor.resources.dll", @"ru\databaseeditor.resources.dll"); //tries to download it
            }

            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"ru\InfANT.resources.dll"))
            {
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"\ru");
                using (WebClient http = new WebClient())
                    http.DownloadFile("http://bitva-pod-moskvoy.ru/_kaspersky/InfANT.resources.dll", @"ru\InfANT.resources.dll"); //tries to download it
            }

            {
                try
                {
                    string lang = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"\lang.ini");
                    if (lang == "en")
                    {
                        Thread.CurrentThread.CurrentCulture = new CultureInfo("ru");
                        Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru");
                        File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\lang.ini", @"ru");
                    }
                    else
                    {
                        Thread.CurrentThread.CurrentCulture = new CultureInfo("en");
                        Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
                        File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\lang.ini", @"en");
                    }
                }
                catch (Exception)
                {
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\lang.ini");
                }
            }
            Application.Restart();
        }
    }
}
