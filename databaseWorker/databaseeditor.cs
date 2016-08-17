using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace databaseworker
{
    public partial class Databaseeditor : Form
    {
        private readonly bool _usedLauncher;
        public Databaseeditor(bool usedLauncherPool)
        {
            string temp = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"\lang.ini");
            var currentCulture = new CultureInfo(temp);
            Thread.CurrentThread.CurrentCulture = currentCulture;
            Thread.CurrentThread.CurrentUICulture = currentCulture;
            InitializeComponent();
            _usedLauncher = usedLauncherPool;
        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog {Title = LanguageResources.select_file};
            open.ShowDialog();

            if (open.FileName == "") return;
            text_VirusPath.Text = open.FileName;
            textSHA1.Text = GetSHA1(text_VirusPath.Text);
            btnAddIt.Enabled = true;
        }

        private string _SHA;
        private string GetSHA1(string filename)
        {
            using (var sha1 = SHA1.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    _SHA = BitConverter.ToString(sha1.ComputeHash(stream)).Replace("-", string.Empty);
                    return _SHA;
                }
            }
        }


        private string _path;
        private bool _contains;
        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fold = new FolderBrowserDialog {Description = LanguageResources.select_installation_folder};
            fold.ShowDialog();
            _contains = false;

            if (fold.SelectedPath == "") return;
            _path = fold.SelectedPath;
            foreach (string file in Directory.GetFiles(_path))
            {
                if (file.Contains("InfANT.exe"))
                {
                    textPathToAntivirus.Text = fold.SelectedPath;
                    btnSelectAntivirus.Enabled = false;
                    btnSelectFile.Enabled      = true;
                    _contains = true;
                    File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\lastpath.txt", _path);
                }
            }

            if(_contains == false)
            {
                MessageBox.Show(LanguageResources.cant_find_infant_in_folder);
            }
        }

        private List<string> _localDatabase;
        private List<string> _localDatabaseSusp;
        private void btnAddIt_Click(object sender, EventArgs e)
        {
            if (File.Exists(_path + @"\localdatabase.txt"))
                _localDatabase = File.ReadAllLines(_path + @"\localdatabase.txt", Encoding.UTF8).ToList<string>();
            else
                _localDatabase = new List<string>();

            if (File.Exists(_path + @"\localdatabasesusp.txt"))
                _localDatabaseSusp = File.ReadAllLines(_path + @"\localdatabasesusp.txt", Encoding.UTF8).ToList<string>();
            else
                _localDatabaseSusp = new List<string>();

            if(radioSusp.Checked == false)
            {
                try
                {
                    if (!_localDatabase.Contains(_SHA) & !_localDatabaseSusp.Contains(_SHA))
                    {
                        _localDatabase.Add(_SHA);
                        File.WriteAllLines(_path + @"\localdatabase.txt", _localDatabase);
                    }
                    else
                    {
                        MessageBox.Show(LanguageResources.this_hash_already_exists);
                        return;
                    }
                }
                catch
                {
                    MessageBox.Show(LanguageResources.access_denied);
                    return;
                }
            }
            else
            {
                try
                {
                    if (!_localDatabase.Contains(_SHA) & !_localDatabaseSusp.Contains(_SHA))
                    {
                        _localDatabase.Add(_SHA);
                        File.WriteAllLines(_path + @"\localdatabasesusp.txt", _localDatabase);
                    }
                    else
                    {
                        MessageBox.Show(LanguageResources.this_hash_already_exists);
                        return;
                    }
                }
                catch
                {
                    MessageBox.Show(LanguageResources.access_denied);
                    return;
                }
            }
            MessageBox.Show(LanguageResources.done);
            text_VirusPath.Text = LanguageResources.select_file_text;
            textSHA1.Text = string.Empty;
            btnAddIt.Enabled = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!_usedLauncher)
            {
                MessageBox.Show(LanguageResources.open_launcher_instead);
                Application.Exit();
            }

            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\InfANT.exe"))
            {
                _path = Directory.GetCurrentDirectory();
                textPathToAntivirus.Text = _path;
                btnSelectFile.Enabled = true;
                _contains = true;
                return;
            }
            
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\lastpath.txt")) return;
            _path = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"\lastpath.txt");

            try
            {
                foreach (string file in Directory.GetFiles(_path))
                {
                    if (!file.Contains("InfANT.exe")) continue;
                    textPathToAntivirus.Text = _path;
                    btnSelectFile.Enabled = true;
                    _contains = true;
                }
            }
            catch
            {
                File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\lastpath.txt");
                Application.Restart();
                return;
            }

            if(_contains == false)
            {
                File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\lastpath.txt");
                Application.Restart();
                return;
            }
            _contains = false;
        }

        private void databaseeditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
