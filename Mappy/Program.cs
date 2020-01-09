namespace Mappy
{
    using System;
    using System.IO;
    using System.Windows.Forms;

    using Mappy.IO;
    using Mappy.Models;
    using Mappy.Services;
    using Mappy.UI.Forms;
    using Mappy.Util;

    public static class Program
    {
        private const string AuthorEmail = "armouredfish@gmail.com";

        private static readonly string CrashLogDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Mappy",
            "Crashes");

        private static Bugsnag.Client bugsnagClient;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            bugsnagClient = new Bugsnag.Client(new Bugsnag.Configuration
            {
                ApiKey = "fa43381b116de659fcf1cfda14884d98",
                AppVersion = Application.ProductVersion,
                AutoNotify = false
            });

            // install custom handler for fatal exceptions
            AppDomain.CurrentDomain.UnhandledException += UnhandledException;

            // route UI thread exceptions to the fatal exception handler
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);

            RemoveOldVersionSettings();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var mainForm = new MainForm();
            var tileCache = new BitmapCache();
            var dialogService = new DialogService(mainForm);
            var featureService = new FeatureService();
            var sectionsService = new SectionService();
            var sectionFactory = new SectionFactory(tileCache);
            var sectionBitmapService = new SectionBitmapService(sectionFactory);
            var mapModelFactory = new MapModelFactory(tileCache);
            var mapLoadingService = new MapLoadingService(sectionFactory, mapModelFactory);
            var imageImportingService = new ImageImportService(tileCache);
            var model = new CoreModel();

            // Unsure if I should get the SectionViews to register themselves, or pass the dispatcher the SectionViews as params
            mainForm.SectionView.SetModel(new SectionViewViewModel(sectionsService));
            mainForm.FeatureView.SetModel(new FeatureViewViewModel(featureService));

            var dispatcher = new Dispatcher(
                model,
                dialogService,
                sectionsService,
                sectionBitmapService,
                featureService,
                mapLoadingService,
                imageImportingService,
                tileCache,
                mainForm);
            mainForm.SetModel(new MainFormViewModel(model, dispatcher));

            mainForm.MapViewPanel.SetModel(new MapViewViewModel(model, dispatcher, featureService));

            var minimapForm = new MinimapForm();
            minimapForm.Owner = mainForm;
            minimapForm.SetModel(new MinimapFormViewModel(model, dispatcher));

            Application.Run(mainForm);
        }

        public static void HandleUnexpectedException(Exception ex)
        {
            string fileName = string.Format(
                "{0}Crash_{1}.log",
                Application.ProductName,
                DateTime.Now.ToString("yyyy-MM-dd_HH.mm.ss"));
            string fullPath = Path.Combine(CrashLogDir, fileName);

            try
            {
                if (!Directory.Exists(CrashLogDir))
                {
                    Directory.CreateDirectory(CrashLogDir);
                }

                using (TextWriter t = new StreamWriter(File.Create(fullPath)))
                {
                    t.WriteLine("Mappy Crash Log");
                    t.WriteLine("For assistance, email this file to {0}", AuthorEmail);
                    t.WriteLine();
                    t.WriteLine("Version: " + Application.ProductVersion);
                    t.WriteLine("Timestamp: " + DateTime.UtcNow.ToString("u"));
                    t.WriteLine();
                    t.WriteLine("Exception details follow.");
                    t.WriteLine();
                    t.WriteLine(ex);
                }
            }
            catch (IOException e)
            {
                string msg = "A fatal error has occurred which could not be logged. Reason: "
                    + e.Message + "\n\n"
                    + "Technical details follow:\n\n"
                    + ex;

                MessageBox.Show(
                    null,
                    msg,
                    "Fatal Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            string errString = "A fatal error has occurred. "
                + "Technical details have been logged to:\n\n" + fullPath + "\n\n"
                + "You may optionally send a crash report to the developer of Mappy. "
                + "This will help them to fix this problem in a future version.\n\n"
                + "If you choose to send a report, the technical details in the log file will be uploaded to the Internet.\n\n"
                + "Would you like to send a crash report?";

            var result = MessageBox.Show(
                errString,
                "Fatal Error",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Error);
            if (result == DialogResult.Yes)
            {
                // Technically doesn't send the report,
                // but rather queues it to be sent on Bugsnag's worker thread.
                // Unfortunately there's no way to know whether this succeeded.
                bugsnagClient.Notify(ex);
                MessageBox.Show(
                    "Attempted to send the report. Thanks for your help to improve Mappy.",
                    "Report Sent",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleUnexpectedException((Exception)e.ExceptionObject);
        }

        private static void RemoveOldVersionSettings()
        {
            string appDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string oldDir = Path.Combine(appDir, @"Armoured_Fish");

            try
            {
                if (Directory.Exists(oldDir))
                {
                    Directory.Delete(oldDir, true);
                }
            }
            catch (IOException)
            {
                // we don't care if this fails
            }
        }
    }
}
