using System;
using System.Windows.Forms;

namespace InfANT
{
    internal class FastScan: Scan
    {
        private static int ScannedFast
        {
            get { return _scannedFast; } //determines how many files were scanned by the fast scanner, function, triggers on change
            set
            {
                _scannedFast = value;
                MainRef.labScannedFastNum.Invoke((new MethodInvoker(delegate { MainRef.labScannedFastNum.Text = value + @"/" + _overallFast; })));
            }
        }
        private static int _scannedFast; //determines how many files were scanned by the fast scanner, var
        private static int _filesCountFast; //temp filescount, used only in CountFiles. "overall" is used in other places. I don't remember why, but, I guess, there's a reason for this
        private static int _overallFast;
        protected override void OnCount()
        {
            _filesCountFast++; //increase the temp filescount with every file
            _overallFast = _filesCountFast;  //as we just changed the temp overall files count, we have to set it to the global files count

            MainRef.labScannedFastNum.Invoke( //change the count label
                new MethodInvoker(delegate { MainRef.labScannedFastNum.Text = ScannedFast + @"/" + _overallFast; }));

            MainRef.progressFast.Invoke( //set the maximum progressbar value to max files
                new MethodInvoker(delegate { MainRef.progressFast.Maximum = _overallFast; }));

            MainRef.progressFast.Invoke( //if the scan is running at the same this will prevent prbar glitching
                new MethodInvoker(delegate { MainRef.progressFast.Value = ScannedFast; }));

            MainRef.progressFast.Invalidate();
        }

        protected override void ScanWork()
        {
            TreeScan(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
            TreeScan(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            TreeScan(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            TreeScan(Environment.GetFolderPath(Environment.SpecialFolder.InternetCache));
        }

        protected override void CountWork()
        {
            CountFiles(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
            CountFiles(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            CountFiles(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            CountFiles(Environment.GetFolderPath(Environment.SpecialFolder.InternetCache)); //TODO you are supposed to scan autorun.
        }

        protected override void HandleErrorDetection(Exception e)
        {
            MainRef.LogIt(3, e.Message, 0);
        }

        protected override void HandleVirusDetection(string filePath)
        {
            MainRef.LogIt(1, filePath, LanguageResources.infected, 0);
            MainRef.Infected = true;
            ScannedStep();
        }

        protected override void HandleSuspDetection(string filePath)
        {
            MainRef.LogIt(2, filePath, LanguageResources.susp, 0);
            if (MainRef.Infected == false)
                MainRef.Infected = null;
            ScannedStep();
        }

        protected override void HandleOKDetection(string filePath)
        {
            MainRef.LogIt(4, filePath, LanguageResources.LOGS_is_clear, 0);
            ScannedStep();
        }

        protected override void AfterAbortActions()
        {
            MainRef.Loadings.CreateLogEntry(4, $"(E{LanguageResources.LOGS_fast_scan_aborted})|{ScannedFast}-{_overallFast}|");
            MainRef.LogIt(0, LanguageResources.LOGS_fast_scan_aborted, 0);
            MainRef.Loadings.timerSaveLogs_Tick(null, null);
            
            MainRef.btnFastScan.Invoke( 
                new MethodInvoker(
                    delegate
                    {
                        MainRef.btnFastScan.Enabled = true; MainRef.btnFastScan.Text = LanguageResources.IFBTN_SCAN; }));
        }

        private static void ScannedStep()
        {
            ScannedFast++; //increases the OVERALL advanced folder scanned count
            MainRef.progressFast.Invoke(new MethodInvoker(delegate { MainRef.progressFast.PerformStep(); }));
        }

        public void Reset()
        {
            MainRef.progressFast.Maximum = _overallFast; //overall amount of files
            MainRef.progressFast.Value = 0;
            ScannedFast = 0;  //sets the amount of scanned files by the fast scanner to zero
            _filesCountFast = 0;
            _overallFast = 0;
        }   
    }
}
