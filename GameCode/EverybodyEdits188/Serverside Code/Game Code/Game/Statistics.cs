using System;
using PlayerIO.GameLibrary;

namespace EverybodyEdits.Game
{
    /**
     * History:
     * 
     * v.1.2:
     * Registering guestvisits.
     * Added history of worlds visited 
     * 
     * v.1.3:
     * Registering pageloads.
     * Use Gems more accurate.
     * Added history of gem purchase and useage.
     *
     **/

    internal class PlayerStats
    {
        public const String STATISTIC_TABLE = "PlayerStats";
        public const double STATISTIC_VERSION = 3;

        private readonly string[] DEPRECATED_STATS_VALUES = { "IsGuest" };
        private DatabaseObject abtest;

        public Client client;
        public string connectUserId;
        private DatabaseObject current_world;
        private string current_world_id = "";
        private string current_world_type = "";
        private DateTime editstart;
        private DateTime gamestart;
        private DatabaseObject gemstats;
        private bool inRoom;
        public bool isFirstVisit = false;
        public bool isReady = false;
        private DateTime sessionstart;

        private DatabaseObject shopstats;
        private DatabaseObject stats;
        private DatabaseObject wootstats;
        private DatabaseObject worldstats;

        public PlayerStats(Client c)
        {
            this.client = c;
        }

        public string Key
        {
            get { return this.stats.Key; }
        }

        public void activate(string key, Callback successcallback = null)
        {
            Console.WriteLine("Activate user: " + key);
            this.client.BigDB.LoadOrCreate(STATISTIC_TABLE, key, delegate (DatabaseObject result)
            {
                this.stats = result;

                this.connectUserId = result.Key;

                // If StatsDate is not set, the databaseobject is new. Init.
                if (!this.stats.Contains("StatsDate"))
                {
                    this.stats.Set("StatsDate", DateTime.Now);
                    this.stats.Set("Version", STATISTIC_VERSION);
                    this.stats.Set("Shop", new DatabaseObject());
                    this.stats.Set("Gems", new DatabaseObject());
                    this.stats.Set("Worlds", new DatabaseObject());

                    this.stats.Set("Type", this.isGuest() ? "guest" : "old");
                }
                else
                {
                    this.FixDeprecatedValues(this.stats);
                }

                if (!this.stats.Contains("Woots"))
                {
                    this.stats.Set("Woots", new DatabaseObject());
                }

                this.shopstats = this.stats.GetObject("Shop");
                this.gemstats = this.stats.GetObject("Gems");
                this.worldstats = this.stats.GetObject("Worlds");
                this.wootstats = this.stats.GetObject("Woots");

                this.isReady = true;
                Console.WriteLine("Tracking on: " + this.stats.Key);
                if (successcallback != null) successcallback.Invoke();
            });
        }

        public bool isGuest()
        {
            return
                !(this.connectUserId.StartsWith("fb") || this.connectUserId.StartsWith("simple") ||
                  this.connectUserId.StartsWith("kong") || this.connectUserId.StartsWith("armor"));
        }

        public bool HandleMessage(params object[] parameters)
        {
            var type = (string)parameters[0];
            Console.WriteLine("playerStats - HandleMessage: " + type);

            if (this.isReady)
            {
                switch (type)
                {
                    case "swfloaded":
                    {
                        this.isFirstVisit = (bool)parameters[1];

                        break;
                    }

                    case "register":
                    {
                        this.stats.Set("Register", this.GetStamp());
                        this.stats.Set("Type", "new");

                        var userid = (string)parameters[1];
                        Console.WriteLine("register new user. Key: " + this.stats.Key + ". Recieved key: " + userid);
                        if (this.stats.Key != userid) this.SaveAs(userid);
                        break;
                    }

                    case "sessionstart":
                    {
                        this.stats.Set("TotalSessions", this.stats.GetInt("TotalSessions", 0) + 1);
                        this.sessionstart = DateTime.Now;
                        break;
                    }

                    case "sessionend":
                    {
                        if (this.inRoom) this.HandleMessage("leaveroom");

                        if (!this.stats.Contains("FirstSession"))
                        {
                            this.stats.Set("FirstSession", this.GetStamp());
                        }
                        this.stats.Set("LatestSession", this.GetStamp());
                        this.stats.Set("TotalSessionTime",
                            this.stats.GetDouble("TotalSessionTime", 0) +
                            (DateTime.Now - this.sessionstart).TotalSeconds);
                        break;
                    }

                    case "shop":
                    {
                        if (!this.shopstats.Contains("FirstView"))
                        {
                            this.shopstats.Set("FirstView", this.GetStamp());
                        }
                        this.shopstats.Set("TotalViews", this.shopstats.GetInt("TotalViews", 0) + 1);
                        this.shopstats.Set("LatestView", this.GetStamp());
                        break;
                    }

                    case "useenergy":
                    {
                        if (!this.shopstats.Contains("FirstUseEnergy"))
                        {
                            this.shopstats.Set("FirstUseEnergy", this.GetStamp());
                        }
                        this.shopstats.Set("TotalUseEnergy", this.shopstats.GetInt("TotalUseEnergy", 0) + 1);
                        this.shopstats.Set("LatestUseEnergy", this.GetStamp());
                        break;
                    }

                    case "gems":
                    {
                        if (!this.gemstats.Contains("FirstView"))
                        {
                            this.gemstats.Set("FirstView", this.GetStamp());
                        }
                        this.gemstats.Set("TotalViews", this.gemstats.GetInt("TotalViews", 0) + 1);
                        this.gemstats.Set("LatestView", this.GetStamp());
                        break;
                    }

                    case "buygems":
                    {
                        var amount = (int)parameters[1];
                        if (!this.gemstats.Contains("FirstBuyGems"))
                        {
                            this.gemstats.Set("FirstBuyGems", this.GetStamp());
                        }
                        if (!this.gemstats.Contains("BuyHistory"))
                        {
                            this.gemstats.Set("BuyHistory", new DatabaseArray());
                        }
                        var purchase = this.GetStamp();
                        purchase.Set("Amount", amount);
                        this.gemstats.GetArray("BuyHistory").Add(purchase);

                        this.gemstats.Set("TotalBuyGemsAmount", this.gemstats.GetInt("TotalBuyGemsAmount", 0) + amount);
                        this.gemstats.Set("TotalBuyGems", this.gemstats.GetInt("TotalBuyGems", 0) + 1);
                        this.gemstats.Set("LatestBuyGems", this.GetStamp());
                        break;
                    }

                    case "usegems":
                    {
                        var itemname = (string)parameters[1];
                        var amount = (int)parameters[2];

                        if (!this.gemstats.Contains("FirstUseGems"))
                        {
                            this.gemstats.Set("FirstUseGems", this.GetStamp());
                        }
                        if (!this.gemstats.Contains("UseHistory"))
                        {
                            this.gemstats.Set("UseHistory", new DatabaseArray());
                        }
                        var gemuse = this.GetStamp();
                        gemuse.Set("Amount", amount);
                        gemuse.Set("Item", itemname);
                        this.gemstats.GetArray("UseHistory").Add(gemuse);

                        this.gemstats.Set("TotalUseGemsAmount", this.gemstats.GetInt("TotalUseGemsAmount", 0) + amount);
                        this.gemstats.Set("TotalUseGems", this.gemstats.GetInt("TotalUseGems", 0) + 1);
                        this.gemstats.Set("LatestUseGems", this.GetStamp());
                        break;
                    }

                    case "joinroom":
                    {
                        this.inRoom = true;
                        var canEdit = (bool)parameters[1];
                        var owner = (bool)parameters[2];
                        this.current_world_id = (string)parameters[3];
                        this.current_world_type = owner ? "own" : canEdit ? "open" : "locked";
                        this.worldstats.Set("TotalEnterWorld", this.worldstats.GetInt("TotalEnterWorld", 0) + 1);

                        this.current_world = new DatabaseObject();
                        this.current_world.Set("Enter", DateTime.Now);
                        this.current_world.Set("Sessions", this.stats.GetInt("TotalSessions", 1));
                        this.current_world.Set("Id", this.current_world_id);

                        this.gamestart = DateTime.Now;
                        if (canEdit) this.editstart = DateTime.Now;

                        switch (this.current_world_type)
                        {
                            case "own":
                            {
                                if (!this.worldstats.Contains("FirstEnterOwnWorld"))
                                {
                                    this.worldstats.Set("FirstEnterOwnWorld", this.GetStamp());
                                }
                                this.worldstats.Set("LatestEnterOwnWorld", this.GetStamp());
                                break;
                            }

                            case "locked":
                            {
                                if (!this.worldstats.Contains("FirstEnterLockedWorld"))
                                {
                                    this.worldstats.Set("FirstEnterLockedWorld", this.GetStamp());
                                }
                                this.worldstats.Set("LatestEnterLockedWorld", this.GetStamp());
                                break;
                            }

                            case "open":
                            {
                                if (!this.worldstats.Contains("FirstEnterOpenWorld"))
                                {
                                    this.worldstats.Set("FirstEnterOpenWorld", this.GetStamp());
                                }
                                this.worldstats.Set("LatestEnterOpenWorld", this.GetStamp());
                                break;
                            }
                        }

                        break;
                    }

                    case "enterlobby":
                    {
                        if (this.inRoom) this.HandleMessage("leaveroom");

                        break;
                    }

                    case "leaveroom":
                    {
                        Console.WriteLine("LeaveRoom. current_world_type: " + this.current_world_type);

                        this.inRoom = false;

                        if (this.current_world != null)
                        {
                            this.current_world.Set("Leave", DateTime.Now);
                            if (!this.worldstats.Contains("History"))
                            {
                                this.worldstats.Set("History", new DatabaseArray());
                            }
                            this.worldstats.GetArray("History").Add(this.current_world);
                            this.current_world = null;
                        }

                        switch (this.current_world_type)
                        {
                            case "own":
                            {
                                this.worldstats.Set("TotalOwnWorldTime",
                                    this.worldstats.GetDouble("TotalOwnWorldTime", 0) +
                                    (DateTime.Now - this.gamestart).TotalSeconds);
                                break;
                            }

                            case "locked":
                            {
                                this.worldstats.Set("TotalLockedWorldTime",
                                    this.worldstats.GetDouble("TotalLockedWorldTime", 0) +
                                    (DateTime.Now - this.gamestart).TotalSeconds);
                                break;
                            }

                            case "open":
                            {
                                this.worldstats.Set("TotalOpenWorldTime",
                                    this.worldstats.GetDouble("TotalOpenWorldTime", 0) +
                                    (DateTime.Now - this.gamestart).TotalSeconds);
                                break;
                            }
                        }
                        this.worldstats.Set("TotalGameTime",
                            this.worldstats.GetDouble("TotalGameTime", 0) + (DateTime.Now - this.gamestart).TotalSeconds);

                        break;
                    }

                    case "woot":
                    {
                        var num_woot_today = (int)parameters[1];
                        var num_woot_total = (int)parameters[2];

                        var entry = this.GetStamp();
                        entry.Set("CurrentSessionTime", (DateTime.Now - this.sessionstart).TotalSeconds);
                        entry.Set("WootToday", num_woot_today);
                        entry.Set("WootTotal", num_woot_total);

                        if (!this.wootstats.Contains("WootHistory"))
                        {
                            this.wootstats.Set("WootHistory", new DatabaseArray());
                        }
                        this.wootstats.GetArray("WootHistory").Add(entry);

                        break;
                    }

                    case "level":
                    {
                        var level = (int)parameters[1];

                        var entry = this.GetStamp();
                        entry.Set("Level", level);

                        if (!this.wootstats.Contains("LevelHistory"))
                        {
                            this.wootstats.Set("LevelHistory", new DatabaseArray());
                        }
                        this.wootstats.GetArray("LevelHistory").Add(entry);

                        break;
                    }
                }
                return true;
            }

            // Events that can be saved in the view stats
            switch (type)
            {
                case "abtest":
                {
                    var testname = (string)parameters[1];
                    var scenario = (int)parameters[2];
                    var scenarioname = (string)parameters[3];

                    if (this.abtest == null) this.abtest = new DatabaseObject();

                    var currenttest = this.GetStamp();
                    currenttest.Set("TestName", testname);
                    currenttest.Set("Scenario", scenario);
                    currenttest.Set("ScenarioName", scenarioname);

                    this.abtest.Set(testname, currenttest);

                    return true;
                }

                default:
                {
                    return false;
                }
            }
        }

        public void Save()
        {
            Console.WriteLine("Save Stats. stats? " + (this.stats != null) + " - FirstVisit? " + this.isFirstVisit);
            if (this.stats != null)
            {
                if (this.abtest != null) this.stats.Set("ABTest", this.abtest);
                this.stats.Save();
                Console.WriteLine("Stats saved as " + this.stats.Key);
            }
            else if (this.isFirstVisit)
            {
                // User was never activated. 
                Console.WriteLine("Saving pageview");

                var view = new DatabaseObject();
                view.Set("Version", STATISTIC_VERSION);
                view.Set("IsPageView", true);
                view.Set("StatsDate", DateTime.Now);
                if (this.abtest != null) view.Set("ABTest", this.abtest);
                this.client.BigDB.CreateObject(STATISTIC_TABLE, null, view,
                    delegate (DatabaseObject d) { Console.WriteLine("Saved viewed stats as " + d.Key); });
            }
        }

        public void SaveAs(string key)
        {
            Console.WriteLine("Save Stats as: " + key);
            this.isReady = false;

            this.client.BigDB.Load(STATISTIC_TABLE, key, delegate (DatabaseObject result)
            {
                Console.WriteLine("load " + result);
                if (result != null)
                {
                    // Returning user. Has logged in as guest earlier.
                    this.client.ErrorLog.WriteError("Error saving guest data to new key: " + key +
                                                    ". Key already exists.");
                    return;
                }

                this.stats.Set("GuestSessions", this.stats.GetInt("TotalSessions", 1));
                this.client.BigDB.CreateObject(STATISTIC_TABLE, key, this.stats, delegate (DatabaseObject newstat)
                {
                    Console.WriteLine("Success! Created stats as " + newstat.Key);
                    this.client.BigDB.DeleteKeys(STATISTIC_TABLE, new[] { this.stats.Key });
                    this.activate(key);
                });
            });
        }

        private DatabaseObject GetStamp()
        {
            var stamp = new DatabaseObject();
            stamp.Set("Date", DateTime.Now);
            stamp.Set("Sessions", this.stats != null ? this.stats.GetInt("TotalSessions", 1) : 0);
            stamp.Set("SessionTime",
                this.stats != null
                    ? this.stats.GetDouble("TotalSessionTime", 0) + (DateTime.Now - this.sessionstart).TotalSeconds
                    : 0);
            return stamp;
        }

        private void FixDeprecatedValues(DatabaseObject statsobj)
        {
            for (var i = 0; i < this.DEPRECATED_STATS_VALUES.Length; i++)
            {
                if (statsobj.Contains(this.DEPRECATED_STATS_VALUES[i]))
                {
                    statsobj.Remove(this.DEPRECATED_STATS_VALUES[i]);
                }
            }
        }
    }
}