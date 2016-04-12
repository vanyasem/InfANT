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
        bool usedlauncher;
        public databaseeditor(bool usedlauncherpool)
        {
            InitializeComponent();
            usedlauncher = usedlauncherpool;
        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Title = "Select the file you want to add";
            open.ShowDialog();

            if(open.FileName != "")
            {
                text_VirusPath.Text = open.FileName;
                textSHA1.Text = GetSHA1(text_VirusPath.Text);
                btnAddIt.Enabled = true;
            }
        }

        string SHA;
        private string GetSHA1(string filename)
        {
            using (var sha1 = SHA1.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    SHA = BitConverter.ToString(sha1.ComputeHash(stream)).Replace("-", string.Empty);
                    return SHA;
                }
            }
        }


        string path;
        bool contains = false;
        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fold = new FolderBrowserDialog();
            fold.Description = "Select the InfANT's installation folder";
            fold.ShowDialog();

            if (fold.SelectedPath != "")
            {
                path = fold.SelectedPath;
                foreach (string file in Directory.GetFiles(path))
                {
                    if (file.Contains("InfANT.exe"))
                    {
                        textPathToAntivirus.Text = fold.SelectedPath;

                        btnSelectAntivirus.Enabled = false;
                        btnSelectFile.Enabled      = true;
                        contains = true;
                    }
                }

                if(contains == false)
                {
                    MessageBox.Show("Can't find the database in that folder! Try again!");
                }
                File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\lastpath.txt",path);
                contains = false;
            }
        }

        List<string> localdatabase;
        List<string> localdatabasesusp;
        private void btnAddIt_Click(object sender, EventArgs e)
        {
            if (File.Exists(path + @"\localdatabase.txt"))
                    localdatabase = File.ReadAllLines(path + @"\localdatabase.txt", Encoding.UTF8).ToList<string>();
                else
                    localdatabase = new List<string>();

                if (File.Exists(path + @"\localdatabasesusp.txt"))
                    localdatabasesusp = File.ReadAllLines(path + @"\localdatabasesusp.txt", Encoding.UTF8).ToList<string>();
                else
                    localdatabasesusp = new List<string>();

            if(radioSusp.Checked == false)
            {
                try
                {
                    if (!localdatabase.Contains(SHA) & !localdatabasesusp.Contains(SHA))
                    {
                        localdatabase.Add(SHA);
                        File.WriteAllLines(path + @"\localdatabase.txt", localdatabase);
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
                    if (!localdatabase.Contains(SHA) & !localdatabasesusp.Contains(SHA))
                    {
                        localdatabase.Add(SHA);
                        File.WriteAllLines(path + @"\localdatabasesusp.txt", localdatabase);
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
            textSHA1.Text = String.Empty;
            btnAddIt.Enabled = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!usedlauncher)
            {
                MessageBox.Show("Open \"_Launcher.exe\" instead!");
                Application.Exit();
            }    

            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\lastpath.txt"))
            {
                path = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"\lastpath.txt");

                try
                {
                    foreach (string file in Directory.GetFiles(path))
                    {
                        if (file.Contains("InfANT.exe"))
                        {
                            textPathToAntivirus.Text = path;
                            btnSelectFile.Enabled = true;
                            contains = true;
                        }
                    }
                }
                catch
                {
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\lastpath.txt");
                    Application.Restart();
                    return;
                }

                if(contains == false)
                {
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\lastpath.txt");
                    Application.Restart();
                    return;
                }
                contains = false;
            }
        }

        private void databaseeditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
