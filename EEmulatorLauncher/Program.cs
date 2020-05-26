using System;
using System.Windows.Forms;
using EEWorldArchive;

namespace EEmulatorLauncher
{
    static class Program
    {
        public static WorldArchive WorldArchive { get; internal set; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
