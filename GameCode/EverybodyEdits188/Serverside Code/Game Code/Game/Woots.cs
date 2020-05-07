using System;
using System.Collections.Generic;
using PlayerIO.GameLibrary;

namespace EverybodyEdits.Game
{
    public class Level
    {
        public static int GetLevel(int woot)
        {
            var cap = Config.levelcap;

            for (var i = cap.Length - 1; i >= 0; i--)
            {
                if (woot >= cap[i])
                {
                    return i + 1;
                }
            }
            return 1;
        }

        public static double GetWootDecay(int level)
        {
            var decay = Config.level_woot_decay;
            return decay[level - 1];
        }
    }

    public class WootStatus
    {
        public const String WOOT_STATUS_TABLE = "WootStatus";

        private const int COOLDOWN_TIME_BASE = 45;
        private const double COOLDOWN_MULTIPLIER = 1.02;

        private const double WOOT_GAMBLE_MULTIPLIER = 1.015;
        private const int WOOT_GAMBLE_DIVIDER = 3;

        private readonly Client client;
        private readonly String key;

        private readonly Random ran = new Random();
        private readonly DatabaseObject status;

        private bool allowSave;

        //private int worldCoinCount = 100;
        //private int coinsToCollect = 10;
        private double nextwootchange;
        private DateTime nextwootdate;
        //private int coinscollected = 0;


        public WootStatus(Client c, DatabaseObject existing, Callback<WootStatus> callback, bool enablesave)
        {
            this.client = c;
            this.key = existing.Key;
            this.status = existing;
            if (!this.status.Contains("recieved"))
            {
                this.status.Set("recieved", new DatabaseObject());
            }
            Console.WriteLine("INIT WOOT. Latest: " + this.getLatestWootDate() + " loadcounter: " + this.loadcounter);

            this.nextwootdate = this.getLatestWootDate();
            try
            {
                this.nextwootdate = this.nextwootdate.AddSeconds(this.getSecondsToNextWoot());
                if (this.nextwootdate < DateTime.Now)
                    this.nextwootdate = DateTime.Now.AddSeconds(this.ran.Next(10, 20));
            }
            catch (ArgumentOutOfRangeException e)
            {
                this.nextwootdate = this.nextwootdate.AddHours(1);
                c.ErrorLog.WriteError("Error making next woot date for " + this.key + ". woots: " + this.getWootsToday() +
                                      " - Errormessage: " + e.Message);
            }
            this.updateNextWootChance();

            if (enablesave)
            {
                if (this.loadcounter == int.MaxValue) this.loadcounter = 0;
                this.loadcounter += 1;
                this.Save(delegate
                {
                    this.allowSave = true;
                    Console.WriteLine("INIT WOOT. Has " + this.getWootsToday() + ". Next woot can be collected at: " +
                                      this.nextwootdate + " with " + (this.nextwootchange * 100) + "% chance");
                    //decayWoots();
                    callback(this);
                });
            }
            else
            {
                //decayWoots();
                callback(this);
            }
        }

        public int total
        {
            get { return this.status.GetInt("total", 0); }
            set { this.status.Set("total", value); }
        }

        public int timezone
        {
            get { return this.status.GetInt("timezone", 0); }
            set
            {
                if (this.status.Contains("timezone")) return;
                this.status.Set("timezone", value);
            }
        }

        public int current
        {
            get { return this.status.GetInt("current", 0); }
            set { this.status.Set("current", value); }
        }

        //public int getDailyWoots(int offset = 0) {
        //    Console.WriteLine("Daily: " + DateTime.Today + " with offset: " + DateTime.Today.AddHours(offset));
        //    return getWootsSince(DateTime.Today.AddHours(offset));
        //}

        //public int daily {

        //    get {
        //        //DateTime now = DateTime.Now;
        //        //return getWootsSince(new DateTime(now.Year, now.Month, now.Day));
        //        return getWootsSince(DateTime.Today.AddHours(timezone));

        //    }
        //}

        public int level
        {
            get { return Level.GetLevel(this.current); }
        }

        public DateTime nextdecay
        {
            get
            {
                return this.status.GetDateTime("nextdecay", DateTime.Now.AddSeconds(Level.GetWootDecay(this.level)));
            }
            set { this.status.Set("nextdecay", value); }
        }

        public int loadcounter
        {
            get { return this.status.GetInt("loadcounter", 0); }
            set { this.status.Set("loadcounter", value); }
        }

        //public void setWorldCoinCount(int count) {
        //    worldCoinCount = count;
        //    updateCoinsToCollect();
        //    Console.WriteLine("WORLD COIN COUNT. Next woot can be collected at: " + nextwoot + " after " + coinsToCollect + " coins (" + worldCoinCount +")");
        //}

        public void doCoinCollect(Callback<bool> callback)
        {
            var random = this.ran.NextDouble();
            Console.WriteLine("doCoinCollect: " + this.nextwootdate + " < " + DateTime.Now + " AND " + (random * 100) +
                              " < " + (this.nextwootchange * 100));

            if (DateTime.Now > this.nextwootdate && this.nextwootchange > random)
            {
                this.tryAddWoot(callback);
            }
            else
            {
                callback(false);
            }
        }

        public void tryAddWoot(Callback<bool> callback)
        {
            Console.WriteLine("tryAddWoot. allowSave? " + this.allowSave);
            this.checkStatus(delegate
            {
                if (!this.allowSave)
                {
                    Console.WriteLine("not allowed to save");
                    callback(false);
                }
                else
                {
                    this.addWoot(callback);
                }
            });
        }

        private void addWoot(Callback<bool> callback)
        {
            Console.WriteLine("addWoot!");

            if (this.current == 0)
            {
                this.nextdecay = DateTime.Now.AddSeconds(Level.GetWootDecay(this.level));
            }

            this.total++;
            this.current++;
            var received = this.status.GetObject("recieved");
            received.Set(this.total.ToString(), DateTime.Now);

            this.Save(delegate { callback(true); });

            this.updateNextWootDate();
            this.updateNextWootChance();

            Console.WriteLine("That was " + this.getWootsToday() + " (today). Next woot can be collected at: " +
                              this.nextwootdate + " with " + (this.nextwootchange * 100) + "% chance");
        }

        private void updateNextWootDate()
        {
            this.nextwootdate = DateTime.Now;
            try
            {
                this.nextwootdate = this.nextwootdate.AddSeconds(this.getSecondsToNextWoot());
            }
            catch (ArgumentOutOfRangeException e)
            {
                this.nextwootdate = this.nextwootdate.AddHours(1);
                this.client.ErrorLog.WriteError("Error making next woot date for " + this.key + ". woots: " +
                                                this.getWootsToday() + " - Errormessage: " + e.Message);
            }
        }


        private double getSecondsToNextWoot()
        {
            var count = this.getWootsToday();

            var cooldown = COOLDOWN_TIME_BASE * Math.Pow(COOLDOWN_MULTIPLIER, count * count);

            // if user has less than 10 woots, the time can be shorter than default cooldown. 
            // but most of the time it should be near the default cooldown.
            if (count < 10)
            {
                var r = 1 - (0.1 / (0.1 + this.ran.NextDouble() / 2));
                cooldown *= r;
                cooldown = Math.Max(cooldown, 5);
            }
            Console.WriteLine("cooldown: " + cooldown);
            return cooldown;
        }

        private void updateNextWootChance()
        {
            var count = this.getWootsToday();
            this.nextwootchange = (1 / Math.Pow(WOOT_GAMBLE_MULTIPLIER, count * count)) / WOOT_GAMBLE_DIVIDER;
        }

        private void decayWoots()
        {
            Console.WriteLine("Decay woots. Next: " + this.nextdecay + " Now: " + DateTime.Now + ". woots: " +
                              this.current);

            if (Config.WOOT_DECAY_ENABLED)
            {
                while (this.nextdecay <= DateTime.Now && this.current > 0)
                {
                    this.current--;
                    this.nextdecay = this.nextdecay.AddSeconds(Level.GetWootDecay(this.level));
                    Console.WriteLine("Decayed. Next: " + this.nextdecay + " Now: " + DateTime.Now + ". woots: " +
                                      this.current);
                }
            }
            this.nextdecay = DateTime.Now.AddDays(1);
        }

        //private void addWootToCurrentBucket

        private void checkStatus(Callback callback)
        {
            this.client.BigDB.LoadOrCreate(WOOT_STATUS_TABLE, this.key, delegate (DatabaseObject latest)
            {
                this.allowSave = latest.GetInt("loadcounter") == this.loadcounter;
                callback.Invoke();
            });
        }

        // Returns the woots collected today (incl. timezone offset)
        public int getWootsToday()
        {
            var today = DateTime.Today.AddHours(this.timezone);
            if (today > DateTime.Now)
            {
                today = today.AddHours(-24);
            }
            else if ((DateTime.Now - today).Hours > 24)
            {
                today = today.AddHours(24);
            }
            return this.getWootsSince(today);
            ;
        }

        public int getWootsSince(DateTime since)
        {
            if (!this.status.Contains("recieved")) return 0;

            var received = this.status.GetObject("recieved");

            var count = 0;
            foreach (var number in received.Properties)
            {
                if (since < received.GetDateTime(number))
                {
                    count++;
                }
            }
            return count;
        }

        public DateTime getLatestWootDate()
        {
            var latest = new DateTime(0);
            var received = this.status.GetObject("recieved");
            foreach (var number in received.Properties)
            {
                if (latest.Ticks == 0 || latest < received.GetDateTime(number))
                {
                    latest = received.GetDateTime(number);
                }
            }
            if (latest.Ticks == 0) latest = DateTime.Now;
            return latest;
        }

        public void cleanRecieveList()
        {
            var received = this.status.GetObject("recieved");

            foreach (var number in received.Properties)
            {
                if ((DateTime.Now - received.GetDateTime(number)).TotalHours > 24)
                {
                    received.Remove(number);
                }
            }
        }

        public void Save(Callback successCallback = null, Callback<PlayerIOError> errorCallback = null)
        {
            this.cleanRecieveList();
            this.status.Save(true,
                delegate { if (successCallback != null) successCallback.Invoke(); },
                delegate (PlayerIOError error)
                {
                    if (error.ErrorCode == ErrorCode.StaleVersion)
                    {
                        Console.WriteLine("Stale version. Save not allowed.");
                        this.allowSave = false;
                    }
                    if (errorCallback != null) errorCallback.Invoke(error);
                });
        }

        public static void getWootStatus(Client c, String connectUserId, Callback<WootStatus> callback,
            bool enablesave = true)
        {
            c.BigDB.LoadOrCreate(WOOT_STATUS_TABLE, connectUserId,
                delegate (DatabaseObject result) { new WootStatus(c, result, callback, enablesave); });
        }
    }

    public class WootUpPlayer
    {
        public const String WOOTUP_STATUS_TABLE = "WootUpPlayer";
        public const int MAX_WOOTUPS = 5;
        public const int MAX_WOOTUPS_IP = 10;
        //public const int WOOTUP_COOLDOWN = (24*60*60);

        public static void giveWootUp(Client client, String connectUserId, String userIpAddress,
            Callback<bool> successCallback, int timezone)
        {
            DatabaseObject ipwootstatus = null;
            DatabaseObject playerwootstatus = null;

            Callback checkLoad = delegate
            {
                //Console.WriteLine("check wootup save: " + (playerwootstatus != null) + ", " + (ipwootstatus != null));
                if (playerwootstatus != null && ipwootstatus != null)
                {
                    var latestplayerwoot = getLargestPropNum(playerwootstatus) + 1;
                    var latestipwoot = getLargestPropNum(ipwootstatus) + 1;
                    removeExpiredWootUps(playerwootstatus, timezone);
                    removeExpiredWootUps(ipwootstatus, 0);

                    if (playerwootstatus.Count < MAX_WOOTUPS && ipwootstatus.Count < MAX_WOOTUPS_IP)
                    {
                        playerwootstatus.Set(latestplayerwoot.ToString(), DateTime.Now);
                        ipwootstatus.Set(latestipwoot.ToString(), DateTime.Now);

                        var playerstatussaved = false;
                        var ipstatussaved = false;
                        Callback checkSave = delegate
                        {
                            if (playerstatussaved && ipstatussaved)
                            {
                                successCallback(true);
                            }
                        };

                        playerwootstatus.Save(true, delegate
                        {
                            playerstatussaved = true;
                            checkSave();
                        }, delegate { successCallback(false); });

                        ipwootstatus.Save(true, delegate
                        {
                            ipstatussaved = true;
                            checkSave();
                        }, delegate { successCallback(false); });
                    }
                    else
                    {
                        // The player uses a woot even if the ip-adress has already maxed out the woots.
                        if (playerwootstatus.Count < MAX_WOOTUPS)
                            playerwootstatus.Set(latestplayerwoot.ToString(), DateTime.Now);
                        successCallback(false);
                    }
                }
            };

            client.BigDB.LoadOrCreate(WOOTUP_STATUS_TABLE, connectUserId, delegate (DatabaseObject result)
            {
                playerwootstatus = result;
                checkLoad();
            }, delegate { successCallback(false); });

            client.BigDB.LoadOrCreate(WOOTUP_STATUS_TABLE, userIpAddress, delegate (DatabaseObject result)
            {
                ipwootstatus = result;
                checkLoad();
            }, delegate { successCallback(false); });
        }

        private static int getLargestPropNum(DatabaseObject data)
        {
            var max = 0;
            foreach (var num in data.Properties)
            {
                var propnum = int.Parse(num);
                max = Math.Max(propnum, max);
            }
            return max;
        }

        public static void removeExpiredWootUps(DatabaseObject data, int timezone)
        {
            //DateTime now = DateTime.Now;
            var today = DateTime.Today.AddHours(timezone);

            if (today > DateTime.Now)
            {
                today = today.AddHours(-24);
            }
            else if ((DateTime.Now - today).Hours > 24)
            {
                today = today.AddHours(24);
            }

            foreach (var num in data.Properties)
            {
                var wootdate = data.GetDateTime(num);
                //Console.WriteLine("removeExpiredWootUps. Today: " + today + " loaded: " + wootdate + " passed: " + (wootdate < today) );
                if (wootdate < today)
                {
                    data.Remove(num);
                }
            }
        }

        public static void getWootStatusAmount(Client c, String connectUserId, Callback<int> callback, int timezone)
        {
            c.BigDB.LoadOrCreate(WOOTUP_STATUS_TABLE, connectUserId, delegate (DatabaseObject result)
            {
                removeExpiredWootUps(result, timezone);
                callback(MAX_WOOTUPS - result.Count);
                //new WootUpstatus(c, result, userIpAddress, callback);
            }, delegate { callback(0); });
        }
    }

    public class WootWorldStatus
    {
        public const String WOOTUPWORLD_STATUS_TABLE = "WootUpWorld";

        private readonly DatabaseObject buckets;
        private readonly DatabaseObject status;
        private Client client;
        private int current;
        private String key;

        public WootWorldStatus(Client c, DatabaseObject existing)
        {
            this.client = c;
            this.key = existing.Key;
            this.status = existing;
            if (!this.status.Contains("buckets"))
            {
                this.status.Set("buckets", new DatabaseObject());
            }
            this.buckets = this.status.GetObject("buckets");
            this.removeOldBuckets();
        }

        public int getWoots(bool refresh = false)
        {
            if (refresh) this.removeOldBuckets();
            return this.current;
        }

        public int getTotalWoots()
        {
            return this.status.GetInt("total", 0);
        }

        public void addWoot()
        {
            Console.WriteLine("addWoot");
            var bucket = this.getCurrentBucket();
            bucket.Set("woots", bucket.GetInt("woots", 0) + 1);
            this.current++;
            this.status.Set("total", this.status.GetInt("total", 0) + 1);
        }

        private DatabaseObject getCurrentBucket()
        {
            var date = this.getRoundedDate();
            var rounddate = date.Year + "-" + date.Month + "-" + date.Day + "-" + date.Hour + "-" + date.Minute;
            Console.WriteLine("getCurrentBucket: " + date + " (" + rounddate + ")");

            if (!this.buckets.Contains(rounddate))
            {
                Console.WriteLine("creating new bucket");
                var bucket = new DatabaseObject();
                bucket.Set("start", date);
                bucket.Set("woots", 0);
                this.buckets.Set(rounddate, bucket);
            }
            return this.buckets.GetObject(rounddate);
        }

        private void removeOldBuckets()
        {
            this.current = 0;
            Console.WriteLine("removeOldBuckets");
            var remove = new List<string>();
            foreach (var name in this.buckets.Properties)
            {
                var bucket = this.buckets.GetObject(name);
                Console.WriteLine("bucket " + name + ": " + (DateTime.Now - bucket.GetDateTime("start")).TotalHours);
                if ((DateTime.Now - bucket.GetDateTime("start")).TotalSeconds > Config.worldwoot_decay_time)
                {
                    //if ((DateTime.Now - bucket.GetDateTime("start")).TotalMinutes > 2) {
                    remove.Add(name);
                }
                else
                {
                    this.current += bucket.GetInt("woots");
                }
            }
            for (var i = 0; i < remove.Count; i++)
            {
                Console.WriteLine("removing: " + remove[i]);
                this.buckets.Remove(remove[i]);
            }
        }

        // The way a date is rounded determines how often a new bucket is created
        private DateTime getRoundedDate()
        {
            var now = DateTime.Now;
            return new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute < 30 ? 0 : 30, 0);
        }

        //private string getRoundedDateString() {
        //    DateTime round = getRoundedDate(); ;
        //    return round.Year + "-" + round.Month + "-" + round.Hour;
        //}

        public void save(Callback successCallback = null, Callback<PlayerIOError> errorCallback = null)
        {
            this.removeOldBuckets();
            this.status.Save(true,
                delegate { if (successCallback != null) successCallback.Invoke(); },
                delegate (PlayerIOError error) { if (errorCallback != null) errorCallback.Invoke(error); });
        }

        public static void getWorldWootStatus(Client c, String worldid, Callback<WootWorldStatus> callback)
        {
            //c.BigDB.LoadOrCreate(WOOTUPWORLD_STATUS_TABLE, worldid,
            //    delegate(DatabaseObject result) { callback(new WootWorldStatus(c, result)); },
            //    delegate
            //    {
            //        Console.WriteLine("There was an error loading " + worldid + " in " + WOOTUPWORLD_STATUS_TABLE);
            //    });
        }
    }
}