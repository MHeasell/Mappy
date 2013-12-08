namespace Mappy
{
    using System;
    using System.IO;
    using System.Windows.Forms;
    using UI.Forms;

    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            RemoveOldVersionSettings();

            Application.ThreadException += Program.OnGuiUnhandedException;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        private static void HandleUnhandledException(object o)
        {
            Exception e = o as Exception;
            if (e != null)
            {
                throw e;
            }
        }

        private static void OnGuiUnhandedException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            HandleUnhandledException(e.Exception);
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
