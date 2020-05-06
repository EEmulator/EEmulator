using System;
using System.Diagnostics;
using System.IO;
using PlayerIO.DevelopmentServer;

namespace EEmulatorV3
{
    public class EverybodyEdits : IGame
    {
        public EverybodyEditsVersion Version { get; }
        public BigDB BigDB { get; }

        public DevServer DevelopmentServer { get; private set; }
        public bool IsRunning { get; private set; }

        public GameAssembly GameAssembly =>
            this.Version == EverybodyEditsVersion.v0500 ? new GameAssembly("FlixelWalker.dll", "FlixelWalker.pdb") :
            this.Version == EverybodyEditsVersion.v0700 ? new GameAssembly("FlixelWalkerFX3.dll", "FlixelWalkerFX3.pdb") :
            this.Version == EverybodyEditsVersion.v0800 ? new GameAssembly("FlixelWalkerFX19.dll", "FlixelWalkerFX19.pdb") :
            this.Version == EverybodyEditsVersion.v89 ? new GameAssembly("EverybodyEdits89.dll", "EverybodyEdits89.pdb")
            : throw new NotImplementedException($"The version of game specified '{ this.Version }' does not have a game assembly associated with it.");

        public string GameId =>
            this.Version == EverybodyEditsVersion.v0500 ? "everybody-edits-v5" :
            this.Version == EverybodyEditsVersion.v0700 ? "everybody-edits-v7" :
            this.Version == EverybodyEditsVersion.v0800 ? "everybody-edits-v8" :
            this.Version == EverybodyEditsVersion.v89 ? "everybody-edits-v89"
            : throw new NotImplementedException($"The version of game specified '{ this.Version }' does not have a game id associated with it.");


        public EverybodyEdits(EverybodyEditsVersion version)
        {
            this.Version = version;
            this.BigDB = new BigDB(this);
        }

        public void Run()
        {
            if (this.IsRunning)
                throw new Exception("Unable to start a game that is already running.");

            this.DevelopmentServer = new DevServer("http://localhost:80/api", null);
            this.DevelopmentServer.SetClusterAccessKey("clusterAccessKey", "Username", false);
            this.DevelopmentServer.TryStart();
            this.IsRunning = true;
            this.DevelopmentServer.Console += this.DevelopmentServer_ConsoleOutput;
            this.DevelopmentServer.GameDllChange += this.DevelopmentServer_GameDllChange;
            this.DevelopmentServer.SetDll(this.GameId, this.GameAssembly.Dll);
            this.CreateDefault();

            Process.Start(Path.Combine("inc", "flashplayer_32_sa_debug.exe"), $"\"" + "http://localhost:80/clients/" + this.Version.ToString() + "\"").WaitForExit();
        }

        /// <summary>
        /// Create default database objects and tables.
        /// </summary>
        private void CreateDefault()
        {
            Directory.CreateDirectory(Path.Combine("games", "EverybodyEdits", "accounts", this.GameId));

            this.BigDB.CreateTable("PlayerObjects");
            this.BigDB.CreateTable("PayVaultItems");

            switch (this.Version)
            {
                case EverybodyEditsVersion.v0800:
                    this.BigDB.CreateTable("Config");
                    this.BigDB.CreateTable("Worlds");

                    this.BigDB.CreateObject("PlayerObjects", "guest", new DatabaseObject()
                        .Set("haveSmileyPackage", true));
                    break;

                case EverybodyEditsVersion.v89:
                    this.BigDB.CreateTable("Config");
                    this.BigDB.CreateTable("Worlds");
                    this.BigDB.CreateTable("Usernames");

                    this.BigDB.CreateObject("PlayerObjects", "guest", new DatabaseObject()
                        .Set("owner", "simpleGuest")
                        .Set("haveSmileyPackage", true)
                        .Set("isModerator", false)
                        .Set("canchat", true)
                        .Set("chatbanned", false)
                        .Set("shopDate", DateTime.Now));

                    this.BigDB.CreateObject("Config", "config", new DatabaseObject()
                        .Set("version", 89)
                        .Set("betaversion", 89));

                    this.BigDB.CreateObject("PayVaultItems", "canchat", new DatabaseObject()
                        .Set("PriceCoins", 1));

                    this.BigDB.CreateObject("PayVaultItems", "pro", new DatabaseObject()
                        .Set("PriceCoins", 1));
                    break;
            }
        }

        private void DevelopmentServer_GameDllChange(string heading, string dllPath, string[] errors)
        {
            if (errors != null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                foreach (var error in errors)
                    Console.WriteLine("ERROR: " + error);
                Console.ResetColor();
                return;
            }

            Console.WriteLine();
            Console.WriteLine("GameId: " + heading);
            Console.WriteLine("DLL: " + dllPath);
            Console.WriteLine();
        }

        private void DevelopmentServer_ConsoleOutput(string sourceAppDomain, string text, bool isLine, bool isErrorStream)
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
