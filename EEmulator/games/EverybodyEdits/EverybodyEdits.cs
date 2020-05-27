using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using EEmulator.Messages;
using PlayerIO.DevelopmentServer;

namespace EEmulator
{
    public class EverybodyEdits : IGame
    {
        public EverybodyEditsVersion Version { get; }
        public BigDB BigDB { get; }
        public List<RoomInfo> Rooms { get; set; }
        public string Description { get; set; } = "No description set.";

        public DevServer DevelopmentServer { get; private set; }
        public bool IsRunning { get; private set; }

        public GameAssembly GameAssembly =>
            this.Version == EverybodyEditsVersion.v0500 ? new GameAssembly("FlixelWalker.dll", "FlixelWalker.pdb") :
            this.Version == EverybodyEditsVersion.v0700 ? new GameAssembly("FlixelWalkerFX3.dll", "FlixelWalkerFX3.pdb") :
            this.Version == EverybodyEditsVersion.v0800 ? new GameAssembly("EverybodyEdits01.dll", "EverybodyEdits01.pdb") :
            this.Version == EverybodyEditsVersion.v89 ? new GameAssembly("EverybodyEdits89.dll", "EverybodyEdits89.pdb") :
            this.Version == EverybodyEditsVersion.v188 ? new GameAssembly("EverybodyEdits188.dll", "EverybodyEdits188.pdb") :
            this.Version == EverybodyEditsVersion.v225 ? new GameAssembly("EverybodyEdits225.dll", "EverybodyEdits225.pdb")
            : throw new NotImplementedException($"The version of game specified '{ this.Version }' does not have a game assembly associated with it.");

        public string GameId =>
            this.Version == EverybodyEditsVersion.v0500 ? "everybody-edits-v5" :
            this.Version == EverybodyEditsVersion.v0700 ? "everybody-edits-v7" :
            this.Version == EverybodyEditsVersion.v0800 ? "everybody-edits-v8" :
            this.Version == EverybodyEditsVersion.v89 ? "everybody-edits-v89" :
            this.Version == EverybodyEditsVersion.v188 ? "everybody-edits-v188" :
            this.Version == EverybodyEditsVersion.v225 ? "everybody-edits-v225"
            : throw new NotImplementedException($"The version of game specified '{ this.Version }' does not have a game id associated with it.");


        public EverybodyEdits(EverybodyEditsVersion version)
        {
            this.Version = version;
            this.BigDB = new BigDB(this);
            this.Rooms = new List<RoomInfo>();
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
            Directory.CreateDirectory(Path.Combine("games", "EverybodyEdits", "config", this.GameId));

            this.BigDB.CreateTable("PlayerObjects");
            this.BigDB.CreateTable("Worlds");
            this.BigDB.CreateTable("PayVaultItems");

            // Server description
            if (File.Exists(Path.Combine("games", "EverybodyEdits", "config", this.GameId, "description.txt")))
                this.Description = File.ReadAllText(Path.Combine("games", "EverybodyEdits", "config", this.GameId, "description.txt"));

            // A default world.
            var PW01_DESTINATION = Path.Combine("games", "EverybodyEdits", "bigdb", this.GameId, "Worlds", "PW01.tson");
            if (!File.Exists(PW01_DESTINATION))
                File.Copy(Path.Combine("games", "EverybodyEdits", "includes", "PW01.tson"), PW01_DESTINATION);

            // A default collection of PayVaultItems
            foreach (var item in Directory.GetFiles(Path.Combine("games", "EverybodyEdits", "includes", "PayVaultItems"), "*.tson"))
            {
                var destination = Path.Combine("games", "EverybodyEdits", "bigdb", this.GameId, "PayVaultItems", Path.GetFileName(item));
                if (!File.Exists(destination))
                    File.Copy(item, destination);
            }

            switch (this.Version)
            {
                #region v0800
                case EverybodyEditsVersion.v0800:
                    this.BigDB.CreateTable("Config");
                    this.BigDB.CreateTable("Worlds");

                    this.BigDB.CreateObject("PlayerObjects", "guest", new DatabaseObject()
                        .Set("haveSmileyPackage", true));

                    this.BigDB.CreateObject("PlayerObjects", "user", new DatabaseObject()
                        .Set("owner", "simpleTest")
                        .Set("haveSmileyPackage", true)
                        .Set("isModerator", true)
                        .Set("canchat", true)
                        .Set("name", "eemu")
                        .Set("chatbanned", false)
                        .Set("shopDate", DateTime.Now));
                    break;
                #endregion
                #region v89
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

                    this.BigDB.CreateObject("PlayerObjects", "user", new DatabaseObject()
                        .Set("owner", "simpleTest")
                        .Set("haveSmileyPackage", true)
                        .Set("isModerator", true)
                        .Set("canchat", true)
                        .Set("name", "eemu")
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
                #endregion

                #region v188
                case EverybodyEditsVersion.v188:
                    this.BigDB.CreateTable("Config");
                    this.BigDB.CreateTable("Worlds");
                    this.BigDB.CreateTable("Usernames");
                    this.BigDB.CreateTable("ClubMembers");
                    this.BigDB.CreateTable("TempBans");
                    this.BigDB.CreateTable("OnlineStatus");

                    this.BigDB.CreateObject("PlayerObjects", "user", new DatabaseObject()
                        .Set("owner", "simpleTest")
                        .Set("haveSmileyPackage", true)
                        .Set("isModerator", true)
                        .Set("canchat", true)
                        .Set("name", "eemu")
                        .Set("chatbanned", false)
                        .Set("maxEnergy", 200)
                        .Set("shopDate", DateTime.Now));

                    this.BigDB.CreateObject("PlayerObjects", "guest", new DatabaseObject()
                        .Set("owner", "simpleTest")
                        .Set("haveSmileyPackage", true)
                        .Set("isModerator", true)
                        .Set("canchat", true)
                        .Set("name", "eemu")
                        .Set("chatbanned", false)
                        .Set("maxEnergy", 200)
                        .Set("shopDate", DateTime.Now));

                    this.BigDB.CreateObject("PlayerObjects", "simpleguest", new DatabaseObject()
                        .Set("owner", "simpleTest")
                        .Set("haveSmileyPackage", true)
                        .Set("isModerator", true)
                        .Set("canchat", true)
                        .Set("name", "eemu")
                        .Set("chatbanned", false)
                        .Set("maxEnergy", 200)
                        .Set("shopDate", DateTime.Now));

                    this.BigDB.CreateObject("Config", "config", new DatabaseObject()
                        .Set("version", 188)
                        .Set("betaversion", 188));
                    
                    // base worlds (home worlds)
                    this.BigDB.CreateObject("Worlds", "PWTXSVWxb3cUI", new DatabaseObject());
                    this.BigDB.CreateObject("Worlds", "PWQe-HH_N2bUI", new DatabaseObject());
                    this.BigDB.CreateObject("ClubMembers", "membernumber", new DatabaseObject().Set("latest_trial", 0));
                    break;
                #endregion

                case EverybodyEditsVersion.v225:
                    this.BigDB.CreateTable("Config");
                    this.BigDB.CreateTable("Worlds");
                    this.BigDB.CreateTable("Usernames");
                    this.BigDB.CreateTable("ClubMembers");
                    this.BigDB.CreateTable("TempBans");
                    this.BigDB.CreateTable("OnlineStatus");
                    this.BigDB.CreateTable("Campaigns");
                    this.BigDB.CreateTable("Coupons");
                    this.BigDB.CreateTable("Friends");
                    this.BigDB.CreateTable("IPBans");

                    this.BigDB.CreateObject("PlayerObjects", "user", new DatabaseObject()
                        .Set("owner", "simpleTest")
                        .Set("haveSmileyPackage", true)
                        .Set("isModerator", true)
                        .Set("canchat", true)
                        .Set("name", "eemu")
                        .Set("chatbanned", false)
                        .Set("maxEnergy", 200)
                        .Set("shopDate", DateTime.Now));

                    this.BigDB.CreateObject("PlayerObjects", "guest", new DatabaseObject()
                        .Set("owner", "simpleTest")
                        .Set("haveSmileyPackage", true)
                        .Set("isModerator", true)
                        .Set("canchat", true)
                        .Set("name", "eemu")
                        .Set("chatbanned", false)
                        .Set("maxEnergy", 200)
                        .Set("shopDate", DateTime.Now));

                    this.BigDB.CreateObject("PlayerObjects", "simpleguest", new DatabaseObject()
                        .Set("owner", "simpleTest")
                        .Set("haveSmileyPackage", true)
                        .Set("isModerator", true)
                        .Set("canchat", true)
                        .Set("name", "eemu")
                        .Set("chatbanned", false)
                        .Set("maxEnergy", 200)
                        .Set("shopDate", DateTime.Now));

                    this.BigDB.CreateObject("Config", "config", new DatabaseObject()
                        .Set("version", 188)
                        .Set("betaversion", 188));

                    this.BigDB.CreateObject("Config", "staff", new DatabaseObject()
                        .Set("eemu", "Admin"));

                    // base worlds (home worlds)
                    this.BigDB.CreateObject("Worlds", "PWQe-HH_N2bUI", new DatabaseObject());
                    this.BigDB.CreateObject("Worlds", "PWTXSVWxb3cUI", new DatabaseObject());

                    // use PW01 as home world template
                    if (!File.Exists(Path.Combine("games", "EverybodyEdits", "bigdb", this.GameId, "Worlds", "PWvmGW1Jl3bUI.tson")))
                        File.Copy(Path.Combine("games", "EverybodyEdits", "includes", "PW01.tson"), Path.Combine("games", "EverybodyEdits", "bigdb", this.GameId, "Worlds", "PWvmGW1Jl3bUI.tson"));

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
