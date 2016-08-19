using System;
using System.Windows.Forms;

namespace InfANT
{
    internal class FullScan : Scan
    {
        private static int ScannedFull
        {
            get { return _scannedFull; } //determines how many files were scanned by the fast scanner, function, triggers on change
            set
            {
                _scannedFull = value;
                MainRef.labScannedFullNum.Invoke((new MethodInvoker(delegate { MainRef.labScannedFullNum.Text = value + @"/" + _overallFull; })));
            }
        }
        private static int _scannedFull; //determines how many files were scanned by the fast scanner, var
        private static int _filesCountFull; //temp filescount, used only in CountFiles. "overall" is used in other places. I don't remember why, but, I guess, there's a reason for this
        private static int _overallFull;
        protected override void OnCount()
        {
            _filesCountFull++; //increase the temp filescount with every file
            _overallFull = _filesCountFull; //as we just changed the temp overall files count, we have to set it to the global files count

            MainRef.labScannedFullNum.Invoke( //change the count label
                new MethodInvoker(delegate { MainRef.labScannedFullNum.Text = ScannedFull + @"/" + _overallFull; }));

            MainRef.progressFull.Invoke( //set the maximum progressbar value to max files
                new MethodInvoker(delegate { MainRef.progressFull.Maximum = _overallFull; }));

            MainRef.progressFull.Invoke( //if the scan is running at the same this will prevent blinking of a prbar
                new MethodInvoker(delegate { MainRef.progressFull.Value = ScannedFull; }));

            MainRef.progressFull.Invalidate();
        }

        protected override void ScanWork()
        {
            TreeScan(MainRef.FullDrivePath);
        }

        protected override void CountWork()
        {
            CountFiles(MainRef.FullDrivePath);
        }

        protected override void HandleErrorDetection(Exception e)
        {
            MainRef.LogIt(3, e.Message, 1);
        }

        protected override void HandleVirusDetection(string filePath)
        {
            MainRef.LogIt(1, filePath, LanguageResources.infected, 1);
            MainRef.Infected = true;
            ScannedStep();
        }

        protected override void HandleSuspDetection(string filePath)
        {
            MainRef.LogIt(2, filePath, LanguageResources.susp, 1);
            if (MainRef.Infected != true)
                MainRef.Infected = null;
            ScannedStep();
        }

        protected override void HandleOKDetection(string filePath)
        {
            MainRef.LogIt(4, filePath, LanguageResources.LOGS_is_clear, 1);
            ScannedStep();
        }

        protected override void AfterAbortActions()
        {
            MainRef.Loadings.CreateLogEntry(4, $"(E{LanguageResources.LOGS_drive_scan_aborted})|{ScannedFull}-{_overallFull}|");
            MainRef.LogIt(0, LanguageResources.LOGS_drive_scan_aborted, 1);
            MainRef.Loadings.timerSaveLogs_Tick(null, null);

            MainRef.btnFullScan.Invoke(
                new MethodInvoker(
                    delegate
                    {
                        MainRef.btnFullScan.Enabled = true; MainRef.btnFullScan.Text = LanguageResources.IFBTN_SCAN; }));
        }

        private static void ScannedStep()
        {
            ScannedFull++; //increases the OVERALL advanced folder scanned count
            MainRef.progressFull.Invoke(new MethodInvoker(delegate { MainRef.progressFull.PerformStep(); }));
        }

        public void Reset()
        {
            MainRef.progressFull.Maximum = _overallFull; //overall amount of files
            MainRef.progressFull.Value = 0;
            ScannedFull = 0;  //sets the amount of scanned files by the fast scanner to zero
            _filesCountFull = 0;
            _overallFull = 0;
        }  
    }
}
