using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace databaseworker
{
    public partial class databaseeditor : Form
    {
        private readonly bool _usedLauncher;
        public databaseeditor(bool usedLauncherPool)
        {
            InitializeComponent();
            _usedLauncher = usedLauncherPool;
        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog {Title = "Select the file you want to add"};
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
            FolderBrowserDialog fold = new FolderBrowserDialog {Description = "Select the InfANT's installation folder"};
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
                MessageBox.Show("Can't find InfANT in that folder! Try again!");
            }
        }

        List<string> _localDatabase;
        List<string> _localDatabaseSusp;
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
                        MessageBox.Show("This entry already exists in one of the databases, no need to add it again.");
                        return;
                    }
                }
                catch
                {
                    MessageBox.Show("Access Denied! Try again!");
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
                        MessageBox.Show("This entry already exists in one of the databases, no need to add it again.");
                        return;
                    }
                }
                catch
                {
                    MessageBox.Show("Access Denied! Try again!");
                    return;
                }
            }
            MessageBox.Show("Done!");
            text_VirusPath.Text = "Select the file                                     ---------------------->"; //this looks bad
            textSHA1.Text = string.Empty;
            btnAddIt.Enabled = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!_usedLauncher)
            {
                MessageBox.Show("Open \"_Launcher.exe\" instead!");
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
