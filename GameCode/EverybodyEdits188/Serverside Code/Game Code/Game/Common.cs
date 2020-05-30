using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PlayerIO.GameLibrary;

namespace EverybodyEdits.Game
{
    // *********************************************************************************
    //  Config
    // *********************************************************************************

    public class Config
    {
        // Tile dimensions
        public const int TILE_WIDTH = 16;
        public const int TILE_HEIGHT = 16;

        // versions
        public const int termsVersion = 1;

        // Values should match Config.as
        public const int version = 188;
        public const int physics_gravity = 2;
        public const int physics_jump_height = 26;
        public const double physics_variable_multiplyer = 7.752;
        public const int max_potions = 10;
        public const int max_level = 12;

        // in seconds
        public const int worldwoot_bucket_time = 30 * 60;
        // When changing this, also update WootWorldStatus.getRoundedDate()

        public const int worldwoot_decay_time = 6 * 60 * 60; //2 * 24 * 60 * 60;

        public const bool WOOT_DECAY_ENABLED = false;

        public static double[] level_woot_decay
        {
            get
            {
                // in seconds
                return new[]
                {
                    (24 * 60 * 60) / 1,
                    (24 * 60 * 60) / 1,
                    (24 * 60 * 60) / 1,
                    (24 * 60 * 60) / 1,
                    (24 * 60 * 60) / 1.5,
                    (24 * 60 * 60) / 1.5,
                    (24 * 60 * 60) / 1.5,
                    (24 * 60 * 60) / 2,
                    (24 * 60 * 60) / 2,
                    (24 * 60 * 60) / 3,
                    (24 * 60 * 60) / 4,
                    (24 * 60 * 60) / 5
                };
            }
        }

        public static int[] levelcap
        {
            get { return new[] { 0, 5, 12, 20, 30, 40, 55, 80, 115, 165, 230, 320, int.MaxValue }; }
        }

        public static int[] levelmaxenergy
        {
            get { return new[] { 200, 205, 212, 220, 230, 240, 255, 275, 300, 330, 360, 400 }; }
        }

        public static string[] leveltitles
        {
            get
            {
                return new[]
                {
                    "Amateur",
                    "Novice",
                    "Student",
                    "Apprentice",
                    "Regular",
                    "Journeyman",
                    "Adept",
                    "Scholar",
                    "Mentor",
                    "Expert",
                    "Master",
                    "GrandMaster"
                };
            }
        }
    }

    public class Player : BasePlayer
    {
        private readonly Dictionary<string, int> MessageCounter = new Dictionary<string, int>();
        private OnlineStatus OnlineStatusObject;
        public PotionStatus PotionStatusObject;

        public int Timestamp = 0;
        public int bcoins = 0;
        //public int face = 0;
        // Used for smileys that are not saved
        public bool canEdit = false;
        public bool canWinEnergy = true;
        public int chatCoolDown = 500;
        public int cheat = 0;
        public Item checkpoint = null;
        public DateTime coinTimer = DateTime.Now.AddMinutes(1);
        // Coinmap is used to make sure players can only collect magic from a coin once (so replacing it will not work)
        public Hashtable coinmap = new Hashtable();
        public int coins = 0;
        public bool disconnected = false;

        public Hashtable friends;

        public bool hasCompleted = false;
        public bool hasSilverCrown = false;
        public int horizontal = 0;
        public bool initialized = false;

        public bool isInGuardianMode = false;
        // "isGuardian" means that the player is in guardian mode. You cannot enter this mode unless you are "isGuardian" in the database

        public bool isInitializing = false;
        public bool isgod = false;

        public bool ismod = false;
        // "ismod" means that the player is in mod mode. You cannot enter this mode unless you are "isModerator" in the database

        public DateTime joinTime;

        public DateTime lastChat = DateTime.Now;

        public string lastChatMessage = "";
        public int lastChatRepeat = 0;
        public DateTime lastEdit = DateTime.Now;
        public int lastMessageRepeat = 0;
        public DateTime lastMessageTimer = DateTime.Now;
        public string lastMessageType = "";
        public DateTime lastmove = DateTime.Now;
        public DateTime lastreport = DateTime.Now.AddHours(-1);

        private int messageLimit = 100;
        public List<string> mutedUsers = new List<string>();
        public bool owner = false;
        public bool ready = false;
        public int temporary_face = -1;
        public int threshold = 150;
        public int vertical = 0;
        public double x = 16;
        public double y = 16;

        public List<Potion> ActivePotions { get; private set; }
            = new List<Potion>();
        //public void addUpdateMessage(Message msg, string type = null)
        //{
        //    string t = type != null? type: msg.Type;
        //    updates[t] = msg;
        //    hasUpdates = true;
        //}

        //public void setUpdateMessageSend(string type)
        //{
        //    last_update[type] = DateTime.Now;
        //}

        //public void clearUpdateMessages()
        //{
        //    updates = new Hashtable();
        //    hasUpdates = false;
        //}

        public string name
        {
            get { return this.PlayerObject.GetString("name", "Guest-" + this.Id); }
        }

        public string currentWorldName
        {
            get { return this.PlayerObject.GetString("currentWorldName", ""); }
            set
            {
                if (this.OnlineStatusObject != null) this.OnlineStatusObject.currentWorldName = value;
                if (this.PlayerObject != null) this.PlayerObject.Set("currentWorldName", value);
            }
        }

        public string currentWorldId
        {
            get { return this.PlayerObject.GetString("currentWorldId", ""); }
            set
            {
                if (this.OnlineStatusObject != null) this.OnlineStatusObject.currentWorldId = value;
                if (this.PlayerObject != null) this.PlayerObject.Set("currentWorldId", value);
            }
        }

        public string room0
        {
            get { return this.PlayerObject.GetString("room0", ""); }
        }

        public string betaonlyroom
        {
            get { return this.PlayerObject.GetString("betaonlyroom", ""); }
        }

        /*
        Modified on 24-04-2013, all registred users can now chat. Which means everybody that are not chatbanned and are not guests
         * Modified again on 01-10-2014 Only people with chat can chat. To much drama otherwise.
         * Modified again on 03-02-2015 All people can chat again.
        */

        public bool canchat
        {
            get { return !this.PlayerObject.GetBool("chatbanned", false) && !this.isguest; }
        }

        public bool hasBeta
        {
            get { return this.PayVault.Has("pro") || this.PlayerObject.GetBool("haveSmileyPackage", false); }
        }

        public DateTime lastCoin
        {
            get
            {
                return PlayerObject.GetDateTime("lastcoin", DateTime.Now.AddHours(-24));
            }
            set
            {
                if (PlayerObject != null) PlayerObject.Set("lastcoin", value);
            }
        }

        public int face
        {
            get
            {
                return temporary_face > -1 ? temporary_face : PlayerObject.GetInt("smiley", 0);
            }
            set
            {
                Console.WriteLine("Setting face: " + value + " - " + allowSaveSmiley(value));
                if (!allowSaveSmiley(value))
                {
                    temporary_face = value;
                }
                else
                {
                    temporary_face = -1;
                    if (PlayerObject != null) PlayerObject.Set("smiley", value);
                    if (OnlineStatusObject != null) OnlineStatusObject.smiley = value;
                }
            }
        }


        public int maxEnergy
        {
            get
            {
                if (!PlayerObject.Contains("maxEnergy"))
                {
                    PlayerObject.Set("maxEnergy", 200);
                    PlayerObject.Save();
                }

                return PlayerObject.GetInt("maxEnergy", 200);
                //return Math.Max(PlayerObject.GetInt("maxEnergy", 200),200);
            }
            set
            {
                if ((value - maxEnergy) >= 10) PlayerObject.Remove("shopDate");
                if (PlayerObject != null) PlayerObject.Set("maxEnergy", value);
            }
        }

        /**
         * "canbemod" means the player is able to switch to mod mode (only user with the isModerator property in the database can do this) 
         * A guardian cannot be "canbemod"
         */

        public bool canbemod
        {
            get { return this.PlayerObject.GetBool("isModerator", false); }
        }

        public bool haveSmileyPackage
        {
            get
            {
                return this.PlayerObject.GetBool("haveSmileyPackage", false) || this.PayVault.Has("pro") ||
                       this.isClubMember;
            }
        }

        public bool isguest
        {
            get { return this.ConnectUserId == "simpleguest"; }
        }

        public int timezoneoffset
        {
            get { return this.PlayerObject.GetInt("timezone", 0); }
        }

        public bool isClubMember
        {
            get
            {
                return this.PlayerObject.Contains("club_expire") &&
                       this.PlayerObject.GetDateTime("club_expire") > DateTime.Now;
            }
        }

        public bool CanBeGuardian
        {
            get { return this.PlayerObject.GetBool("isGuardian", false); }
        }

        public int RepeatChatCount(string chat)
        {
            if (chat == this.lastChatMessage)
            {
                this.lastChatRepeat++;
            }
            else
            {
                this.lastChatRepeat = 0;
                this.lastChatMessage = chat;
            }
            return this.lastChatRepeat;
        }

        public bool AllowMessage(string messageType)
        {
            if ((DateTime.Now - this.lastMessageTimer).TotalSeconds > 1)
            {
                this.MessageCounter.Clear();
                this.lastMessageTimer = DateTime.Now;
            }

            if (!this.MessageCounter.ContainsKey(messageType))
            {
                this.MessageCounter.Add(messageType, 0);
            }
            this.MessageCounter[messageType] = this.MessageCounter[messageType] + 1;
            if (this.MessageCounter[messageType] > this.messageLimit)
            {
                return false;
            }
            return true;
        }

        public void SendMessage(Message msg)
        {
            if (this.initialized)
            {
                base.Send(msg);
            }
        }

        public int RepeatMessageCount(string messagetype)
        {
            if (messagetype == this.lastMessageType)
            {
                this.lastMessageRepeat++;
            }
            else
            {
                this.lastMessageRepeat = 0;
                this.lastMessageTimer = DateTime.Now;
                this.lastMessageType = messagetype;
            }
            return this.lastMessageRepeat;
        }

        public void setCoinCollected(int x, int y)
        {
            var key = x + "_" + y;
            if (!this.coinmap.Contains(x + "_" + y))
            {
                this.coinmap.Add(key, true);
            }
        }

        public bool isCoinCollected(int x, int y)
        {
            return this.coinmap.Contains(x + "_" + y);
        }

        public bool hasFriend(String friendConnectionId)
        {
            //Console.WriteLine(ConnectUserId + " is friends with " + friendConnectionId + "? " + friends.Contains(friendConnectionId));
            return this.friends != null && this.friends.Contains(friendConnectionId);
        }

        public bool hasBrickPack(String payvaultid)
        {
            switch (payvaultid)
            {
                case "brickhwtrophy":
                {
                    return this.PayVault.Has(payvaultid);
                }
                default:
                {
                    return this.isClubMember || this.PayVault.Has(payvaultid);
                }
            }
        }

        public int getBrickPackCount(String payvaultid)
        {
            switch (payvaultid)
            {
                case "brickdiamond":
                {
                    return this.PayVault.Count(payvaultid);
                }
                default:
                {
                    return this.isClubMember ? 4000000 : this.PayVault.Count(payvaultid);
                }
            }
        }

        public bool hasSmileyPack(String payvaultid)
        {
            return this.PayVault.Has(payvaultid);
        }

        public void activatePotion(Potion potion, Callback<List<Potion>> callback)
        {
            //potion.activate();
            //this.addPotion(potion, callback);
        }

        public void addPotion(Potion potion, Callback<List<Potion>> callback)
        {
            this.ActivePotions.Add(potion);
            callback(this.ActivePotions);

            //this.PotionStatusObject.addPotion(potion, callback);
            //SavePotionStatus("add");
        }

        public Potion removePotion(Potion potion)
        {
            this.getActivePotions().RemoveAll(x => x.payvatultid == potion.payvatultid);

            //var removed = this.PotionStatusObject.removePotion(potion);
            //SavePotionStatus("remove");
            //return removed;
            return null;
        }

        public List<Potion> getActivePotions()
        {
            return this.ActivePotions;
            //return this.PotionStatusObject.getPotions();
        }

        public bool hasActivePotion(Potion potion)
        {
            return this.getActivePotions().Any(x => x.payvatultid == potion.payvatultid);

            //return false;
            //return this.PotionStatusObject.hasActivePotion(potion);
        }

        public void doCoinCollect(Callback<bool> callback)
        {
            callback(true);
        }

        public void Init(Client c, Callback successCallback)
        {
            //if (this.isInitializing) return;
            //this.isInitializing = true;

            //Callback checkInit = delegate
            //{
            //    Console.WriteLine("check player init: " + (this.OnlineStatusObject != null) + ", " +
            //                      (this.friends != null) + ", " + (this.PotionStatusObject != null) + ", " +
            //                      (this.WootStatusObject != null));
            //    if (this.OnlineStatusObject != null && this.friends != null && this.PotionStatusObject != null &&
            //        this.WootStatusObject != null)
            //    {
            //        if (!this.allowSaveSmiley(this.face)) this.face = 0;

            //        this.initialized = true;
            //        this.isInitializing = false;
            //        successCallback.Invoke();
            //    }
            //};

            successCallback.Invoke();
            return;

            //OnlineStatus.getOnlineStatus(c, this.ConnectUserId, delegate(OnlineStatus os)
            //{
            //    this.OnlineStatusObject = os;
            //    this.OnlineStatusObject.name = this.name;
            //    this.OnlineStatusObject.ipAddress = this.IPAddress.ToString();
            //    this.OnlineStatusObject.lastUpdate = DateTime.Now;
            //    checkInit.Invoke();
            //});

            //c.BigDB.LoadOrCreate("Friends", this.ConnectUserId, delegate(DatabaseObject friendslist)
            //{
            //    this.friends = new Hashtable();
            //    foreach (var key in friendslist.Properties)
            //    {
            //        if (friendslist.GetBool(key)) this.friends.Add(key, true);
            //    }
            //    checkInit.Invoke();
            //});

            //PotionStatus.getPotioneStatus(c, this.ConnectUserId, delegate(PotionStatus ps)
            //{
            //    this.PotionStatusObject = ps;
            //    checkInit.Invoke();
            //});

            //WootStatus.getWootStatus(c, this.ConnectUserId, delegate(WootStatus ws)
            //{
            //    this.WootStatusObject = ws;
            //    checkInit.Invoke();

            //    if (this.canbemod)
            //    {
            //        this.WootStatusObject.nextdecay = DateTime.Now.AddYears(1);
            //        return;
            //    }
            //    if (this.isClubMember)
            //    {
            //        this.WootStatusObject.nextdecay = this.PlayerObject.GetDateTime("club_expire", DateTime.Now);
            //    }
            //});
        }

        public void Save()
        {
            if (!this.isguest && !this.disconnected)
            {
                //this.SaveOnlineStatus();
                //this.SavePotionStatus();
                
                lock (this)
                {
                    if (this.PlayerObject != null)
                        this.PlayerObject.Save(delegate { Console.WriteLine("PlayerObject saved"); });
                }
            }
        }

        public void SaveOnlineStatus()
        {
            if (this.OnlineStatusObject != null)
            {
                this.OnlineStatusObject.lastUpdate = DateTime.Now;
                lock (this)
                {
                    this.OnlineStatusObject.Save();
                }
            }
        }

        public void SavePotionStatus()
        {
            if (this.PotionStatusObject != null)
            {
                lock (this)
                {
                    this.PotionStatusObject.Save();
                }
            }
        }

        private bool allowSaveSmiley(int face)
        {
            // do not allow to save diamond smiley, party smileys, hologram smiley and zombie smiley
            return (face != 31 || face <= 72 || face >= 75 || face != 87 || face != 100);
        }
    }


    // *********************************************************************************
    //  ChatMessage
    // *********************************************************************************

    public class ChatMessage
    {
        public bool canchat = true;
        public string connectUserId = "";
        public string name = "";
        public string text = "";

        public ChatMessage(string name, string text, bool canchat, string connectUserId)
        {
            this.name = name;
            this.text = text;
            this.canchat = canchat;
            this.connectUserId = connectUserId;
        }
    }

    // *********************************************************************************
    //  ItemTypes
    // *********************************************************************************

    public enum ItemTypes : uint
    {
        Coin = 100,
        BlueCoin = 101,
        TimeDoor = 156,
        TimeGate = 157,
        CoinDoor = 43,
        CoinGate = 165,
        BlueCoinDoor = 213,
        BlueCoinGate = 214,
        Diamond = 241,
        Portal = 242,
        WorldPortal = 374,
        SpawnPoint = 255,
        Cake = 337,
        Hologram = 397,
        Label = 1000,
        Piano = 77,
        Drums = 83,
        Complete = 121,
        DoorPurple = 184,
        GatePurple = 185,
        SwitchPurple = 113,
        DoorClub = 200,
        GateClub = 201,
        Checkpoint = 360,
        Spike = 361,
        Fire = 368,
        ZombieGate = 206,
        ZombieDoor = 207,
        GlowyLineBlueSlope = 375,
        GlowyLineBlueStraight = 376,
        GlowyLineYellowSlope = 377,
        GlowyLineYellowStraight = 378,
        GlowyLineGreenSlope = 379,
        GlowyLineGreenStraight = 380,
        PortalInvisible = 381,
        TextSign = 385,
        OnewayCyan = 1001,
        OnewayRed = 1002,
        OnewayYellow = 1003,
        OnewayPink = 1004,
        DeathDoor = 1011,
        DeathGate = 1012
    }

    // *********************************************************************************
    //  Item
    // *********************************************************************************

    public class Item
    {
        public int type = 0;
        public int x = 0;
        public int y = 0;

        public Item(int type, int x, int y)
        {
            this.type = type;
            this.x = x;
            this.y = y;
        }
    }

    // *********************************************************************************
    //  Ban
    // ********************************************************************************

    public class Ban
    {
        public DateTime timestamp;
        public string userid;

        public Ban(string userid, DateTime timestamp)
        {
            this.userid = userid;
            this.timestamp = timestamp;
        }
    }

    // *********************************************************************************
    //  WorldTypes
    // *********************************************************************************

    public enum WorldTypes
    {
        Small = 0,
        Medium = 1,
        Large = 2,
        Massive = 3,
        Wide = 4,
        Great = 5,
        Tall = 6,
        UltraWide = 7,
        MoonLarge = 8,
        Tutorial = 9,
        HomeWorld = 10,
        Huge = 11
    }
}
