namespace Mappy
{
    using System;
    using System.IO;
    using System.Windows.Forms;
    using UI.Forms;

    public static class Program
    {
        private const string AuthorEmail = "armouredfish@gmail.com";

        private static readonly string CrashLogDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            @"Mappy/Crashes");

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            RemoveOldVersionSettings();

            // install custom handler for fatal exceptions
            AppDomain.CurrentDomain.UnhandledException += UnhandledException;

            // route UI thread exceptions to the fatal exception handler
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        private static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleUnexpectedException((Exception)e.ExceptionObject);
        }

        private static void HandleUnexpectedException(Exception ex)
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
                + "If you'd like to get this fixed, email the log file to:\n\n" + AuthorEmail + "\n\n"
                + "Include any relevant information, "
                + "such as what map you had open and what you were doing when the error occurred.";

            MessageBox.Show(
                errString,
                "Fatal Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
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
