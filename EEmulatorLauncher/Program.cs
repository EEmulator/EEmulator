using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using EEWorldArchive;

namespace EEmulatorLauncher
{
    static class Program
    {
        public static WorldArchive WorldArchive { get; internal set; }
        public static Dictionary<string, string> UsernameToConnectUserId { get; internal set; }
            = new Dictionary<string, string>();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            foreach (var line in File.ReadAllLines(@"inc/usernames.txt"))
            {
                var split = line.Split(':');

                if (UsernameToConnectUserId.ContainsKey(split[1]))
                    continue;

                UsernameToConnectUserId.Add(split[1], split[0]);
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
