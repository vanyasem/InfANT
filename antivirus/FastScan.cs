using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace InfANT
{
    static class FastScan
    {
        public static Main MainRef;

        private static readonly BackgroundWorker FilesCountingWorker = new BackgroundWorker();
        private static readonly BackgroundWorker ScanningWorker = new BackgroundWorker();
        public static CultureInfo CurrentCultureInfo;
        public static void Initialize()
        {
            FilesCountingWorker.WorkerSupportsCancellation = true;
            FilesCountingWorker.DoWork += (sender, args) =>
            {
                if (FilesCountingWorker.CancellationPending) return;
                CountFilesFast(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
                CountFilesFast(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                CountFilesFast(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
                CountFilesFast(Environment.GetFolderPath(Environment.SpecialFolder.InternetCache)); //TODO you are supposed to scan autorun.
            };

            FilesCountingWorker.RunWorkerCompleted += (sender, args) =>
            {
                AfterAbort();
                if (args.Error != null) // if an exception occurred during DoWork,
                    MessageBox.Show(args.Error.ToString());
            };

            ScanningWorker.WorkerSupportsCancellation = true;
            ScanningWorker.DoWork += (sender, args) =>
            {
                if (ScanningWorker.CancellationPending) return;
                TreeScan(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
                TreeScan(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                TreeScan(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
                TreeScan(Environment.GetFolderPath(Environment.SpecialFolder.InternetCache));
            };

            ScanningWorker.RunWorkerCompleted += (sender, args) =>
            {
                AfterAbort();
                if (args.Error != null) // if an exception occurred during DoWork,
                    MessageBox.Show(args.Error.ToString());
            };
        }

        public static void Start() 
        {
            FilesCountingWorker.RunWorkerAsync();
            ScanningWorker.RunWorkerAsync();
            MainRef.IsScanning = true;
        }

        private static bool _wasAfterAborted;
        public static void Abort()
        {
            ScanningWorker.CancelAsync();
            FilesCountingWorker.CancelAsync();
        }

        private static void AfterAbort()
        {
            //MessageBox.Show(_wasAfterAborted.ToString());
            if (_wasAfterAborted)
            {
                MainRef.IsScanning = false;
                MainRef.Loadings.CreateLogEntry(4, $"(E{LanguageResources.LOGS_fast_scan_aborted})|{ScannedFast}-{_overallfast}|");
                MainRef.EnableEverything();
                _wasAfterAborted = false;
            }
            else
            {
                _wasAfterAborted = true;
            }  
        }

        public static void Reset()
        {
            MainRef.progressFast.Maximum = _overallfast; //overall amount of files
            MainRef.progressFast.Value = 0;
            ScannedFast = 0;  //sets the amount of scanned files by the fast scanner to zero
            _filesCountFast = 0;
            _overallfast = 0;
        }

        private static int ScannedFast
        {
            get { return _scannedFast; } //determines how many files were scanned by the fast scanner, function, triggers on change
            set
            {
                _scannedFast = value;
                MainRef.labScannedFastNum.Invoke((new MethodInvoker(delegate { MainRef.labScannedFastNum.Text = value + @"/" + _overallfast; })));
            }
        }

        private static int _scannedFast; //determines how many files were scanned by the fast scanner, var
        private static int _filesCountFast; //temp filescount, used only in CountFiles. "overall" is used in other places. I don't remember why, but, I guess, there's a reason for this
        private static int _overallfast;
        private static void CountFilesFast(string dir2)
        {
            try
            {
                foreach (string file in Directory.GetFiles(dir2)) //gets all files from the folder
                {
                    if (FilesCountingWorker.CancellationPending) return;
                    _filesCountFast++; //increase the temp filescount with every file
                    _overallfast = _filesCountFast;
                    //as we just changed the temp overall files count, we have to set it to the global files count
                    MainRef.labScannedFastNum.Invoke(
                        new MethodInvoker(
                            delegate { MainRef.labScannedFastNum.Text = ScannedFast + @"/" + _overallfast; }));
                    //change the count label
                    MainRef.progressFast.Invoke(
                        new MethodInvoker(delegate { MainRef.progressFast.Maximum = _overallfast; }));
                    //set the maximum progressbar value to max files
                    MainRef.progressFast.Invoke(
                        new MethodInvoker(delegate { MainRef.progressFast.Value = ScannedFast; }));
                    //if the scan is running at the same this will prevent blinking of a prbar
                    MainRef.progressFast.Invalidate();
                }
            }
            catch
            { /*return;*/ } //we want to do nothing here, so nothing here. Do I need to log this? Don't think so

            try
            {
                foreach (string dir in Directory.GetDirectories(dir2)) //gets all folders from the folder and does the same for all of them
                {
                    if (!FilesCountingWorker.CancellationPending)
                    {
                        CountFilesFast(dir);
                    }
                }
            }
            catch
            { /*return;*/ } //we want to do nothing here, so nothing here. Do I need to log this? Don't think so
        }

        private static void SetThreadsCulture(Thread thread)
        {
            try
            {
                thread.CurrentCulture = CurrentCultureInfo;
                thread.CurrentUICulture = CurrentCultureInfo;
            }
            catch
            { /* ignored */ } //TODO handlers
        }

        private static void TreeScan(string folder) //wheretopass determines where should LogIt(whichlog,text,whichscan) pass it. 
        {                                                     //Wheretopass determines which scan is used. See more at the LogIt definition.
            SetThreadsCulture(Thread.CurrentThread);
            try
            {
                foreach (string file in Directory.GetFiles(folder)) //gets all files' filenames from the folder
                {
                    if (ScanningWorker.CancellationPending) return;
                    string temphash = MainRef.GetSHA1(file);
                    if (MainRef.Hashes.Contains(temphash)) //checks if this hash exists, should be probably replaced, too slow
                    {
                        MainRef.LogIt(1, file, LanguageResources.infected, 0);
                        MainRef.Infected = true;
                        ScannedFast++; //increases the OVERALL advanced folder scanned count
                        MainRef.progressFast.Invoke(new MethodInvoker(delegate { MainRef.progressFast.PerformStep(); }));
                    }
                    else
                    {
                        if (MainRef.SuspHashes.Contains(temphash)) //checks if this hash exists, should be probably replaced, too slow
                        {
                            MainRef.LogIt(2, file, LanguageResources.susp, 0);
                            if (MainRef.Infected != true)
                                MainRef.Infected = null;
                            ScannedFast++; //increases the OVERALL advanced folder scanned count
                            MainRef.progressFast.Invoke(new MethodInvoker(delegate { MainRef.progressFast.PerformStep(); }));
                        }
                        else
                        {
                            if (ScanningWorker.CancellationPending) return;
                            MainRef.LogIt(4, file, LanguageResources.LOGS_is_clear, 0);
                            ScannedFast++; //increases the OVERALL advanced folder scanned count
                            MainRef.progressFast.Invoke(new MethodInvoker(delegate { MainRef.progressFast.PerformStep(); }));
                        }
                    }
                }
            }
            catch (ThreadAbortException) //we don't want an "thread terminated" exception to log (coz we do it by ourselves) so we check for that
            { return; }
            catch (Exception e)
            {
                MainRef.LogIt(3, e.Message, 0);
            }

            try
            {
                foreach (string dir in Directory.GetDirectories(folder)) //gets all folders from the folder and does the same for all of them
                {
                    if (ScanningWorker.CancellationPending) return;
                    TreeScan(dir);
                }
            }
            catch (ThreadAbortException) { /* we don't want an "thread terminated" exception to log (coz we do it by ourselves) so we check for that */ }
            catch (Exception e)
            {
                MainRef.LogIt(3, e.Message, 0);
            }
        }
    }
}
