using System;
using System.Diagnostics;
using System.IO;
using PlayerIO.DevelopmentServer;

namespace EEmulatorV3
{
    public class EverybodyEdits : IGame
    {
        public EverybodyEditsVersion Version { get; }
        public DevServer DevelopmentServer { get; private set; }
        public bool IsRunning { get; private set; }

        public GameAssembly GameAssembly =>
            this.Version == EverybodyEditsVersion.v0500 ? new GameAssembly("FlixelWalker.dll", "FlixelWalker.pdb")
            : throw new NotImplementedException($"The version of game specified '{ this.Version }' does not have a game assembly associated with it.");

        public string GameId =>
            this.Version == EverybodyEditsVersion.v0500 ? "everybody-edits-v5"
            : throw new NotImplementedException($"The version of game specified '{ this.Version }' does not have a game id associated with it.");


        public EverybodyEdits(EverybodyEditsVersion version)
        {
            this.Version = version;
        }

        public void Run()
        {
            if (this.IsRunning)
                throw new Exception("Unable to start a game that is already running.");

            this.DevelopmentServer = new DevServer("http://localhost:80/api", null);
            this.DevelopmentServer.SetClusterAccessKey("clusterAccessKey", "Username", true);
            this.DevelopmentServer.TryStart();
            this.IsRunning = true;
            this.DevelopmentServer.Console += this.DevelopmentServerConsoleOutput;
            this.DevelopmentServer.SetDll(this.GameId, this.GameAssembly.Dll);

            Process.Start(Path.Combine("inc", "flashplayer_32_sa_debug.exe"), $"\"" + "http://localhost:80/clients/" + this.Version.ToString() + "\"").WaitForExit();
        }

        private void DevelopmentServerConsoleOutput(string sourceAppDomain, string text, bool isLine, bool isErrorStream)
        {
            if (isErrorStream)
                Console.ForegroundColor = ConsoleColor.Red;

            Console.Title = sourceAppDomain;
            Console.Write(text);

            if (isLine)
                Console.WriteLine();
            
            Console.ResetColor();
        }
    }
}
