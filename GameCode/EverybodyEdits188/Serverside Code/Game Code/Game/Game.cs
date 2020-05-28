using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using EverybodyEdits.Game.ChatCommands;
using EverybodyEdits.Lobby;
using PlayerIO.GameLibrary;

namespace EverybodyEdits.Game
{
    // *******************************************************************************************
    // Game
    // *******************************************************************************************

    [RoomType("Everybodyedits188")]
    public class EverybodyEdits : Game<Player>
    {
        protected Dictionary<IPAddress, int> accessAttempts = new Dictionary<IPAddress, int>();

        protected string[] autotexts =
        {
            "Left.",
            "Hi.",
            "Goodbye.",
            "Help me!",
            "Thank you.",
            "Follow me.",
            "Stop!",
            "Yes.",
            "No.",
            "Right."
        };

        protected List<Ban> bans = new List<Ban>();
        protected World baseworld;
        protected BlockMap blockMap;

        protected bool blue = false;
        protected double bluetime;
        protected ChatMessage[] chatmessages = new ChatMessage[40];
        protected Dictionary<string, ChatCommand> cmds = new Dictionary<string, ChatCommand>();
        private DateTime coinanticheat = DateTime.Now;
        protected int coinscollected = 0;
        protected int crownid = -1;
        protected bool cyan = false;
        protected double cyantime;
        protected bool die = false;
        protected string editchar = ".";
        protected string editkey = "";
        protected bool green = false;
        protected double greentime;
        protected Dictionary<string, int> ips = new Dictionary<string, int>();
        protected bool isCheckingPotions = false;
        protected Boolean isTutorialRoom = false;
        protected bool isbetalevel = false;
        protected string levelOwnerName = "";
        protected bool lockedroom = false;
        protected bool magenta = false;
        protected double magentatime;
        protected int magiccollected = 0;

        protected double metaUpdateTime = 0;
        protected bool owned = false;
        protected double potionCheckTime = 0;
        protected DateTime potionswitched;
        protected Random rd = new Random();
        protected bool ready = false;
        protected bool red = false;
        protected double redtime;
        protected List<DatabaseObject> reports = new List<DatabaseObject>();
        //protected int plays = 0;
        protected int sessionplays = 0;
        protected Shop shop;
        protected SmileyMap smileyMap;
        protected string systemName = "* System";
        protected bool timedoor = false;
        protected double timedoortime;
        //protected int worldwoots = 0;
        protected bool upgrade = false;
        protected bool upgradeWarning = false;
        protected double upgradeWarningTime = 0;
        protected int version = Config.version;

        protected bool worldBanned = false; // Either owner is banned or world is banned
        protected string worldBannedReason = "";
        protected double worldWootCheckTime = 0;
        protected int worldWootLatest = 0;
        protected int worldWootOffset = 0;
        protected int worldWootReduction = 0;

        protected DateTime worldstarted = DateTime.Now;
        protected bool yellow = false;
        protected double yellowtime;

        public string SystemName
        {
            get { return this.systemName; }
        }

        public World BaseWorld
        {
            get { return this.baseworld; }
        }

        public string LevelOwnerName
        {
            get { return this.levelOwnerName; }
        }

        public override void GameStarted()
        {
            this.PreloadPayVaults = true;
            this.PreloadPlayerObjects = true;

            this.baseworld = new World(this.PlayerIO);

            var allowedChars = "acdefghijnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ";
            var rd = new Random();

            this.editchar = ".";
            for (var a = 0; a < rd.Next(1, 3); a++)
            {
                this.editchar += allowedChars[rd.Next(0, allowedChars.Length)].ToString();
            }

            this.isbetalevel = this.RoomId.Substring(0, 2) == "BW" || this.RoomId == "ChrisWorld";
            var isowned = this.isbetalevel || this.RoomId.Substring(0, 2) == "PW" || this.isTutorialRoom ||
                          this.RoomId == "toturialWorld";
            var isopen = this.RoomId.Substring(0, 2) == "OW";

            if (this.isbetalevel)
            {
                this.RoomData["beta"] = "true";
            }

            this.Visible = false;

            if (isowned || this.RoomData.ContainsKey("owned"))
            {
                this.owned = true;
                this.lockedroom = true;
                this.editkey = "23f23fdswvsdv24t24wrgwerg23t5h35h35h35h3x2f'23f5";
                this.RoomData["needskey"] = "yep";
                this.RoomData["plays"] = "0";
                this.RoomData["rating"] = "0";
                this.RoomData["name"] = "My World";
                this.RoomData["woots"] = "0";

                this.checkWorldBanAndLoad(this.RoomId);
            }
            else
            {
                this.Visible = true;
                if (this.RoomData.ContainsKey("editkey"))
                {
                    this.lockedroom = true;
                    this.editkey = this.RoomData["editkey"];
                    this.RoomData.Remove("editkey");
                }
                this.RoomData["openworld"] = "true";
                this.RoomData["plays"] = "0";
                this.RoomData["rating"] = "0";
                this.RoomData["woots"] = "0";
                this.RoomData["name"] = this.RoomData.ContainsKey("name")
                    ? this.removeBadChars(this.RoomData["name"])
                    : "Open world";

                this.baseworld.key = this.RoomId;
                this.baseworld.refreshWoots(delegate
                {
                    this.worldWootLatest = this.baseworld.woots;
                    this.BroadcastMetaData();
                });
            }

            this.RoomData.Save();

            this.startTimers();


            this.potionswitched = DateTime.Now.AddSeconds(-30);
            this.smileyMap = new SmileyMap();
            this.blockMap = new BlockMap();

            this.CreateChatCommands();
        }


        protected void startTimers()
        {
            this.AddTimer(this.checkVersion, 10000);
            this.checkVersion();

            this.AddTimer(delegate
            {
                if (this.red && this.getTime() - this.redtime > 5000)
                {
                    this.Broadcast("show", "red");
                    this.red = false;
                }
                if (this.green && this.getTime() - this.greentime > 5000)
                {
                    this.Broadcast("show", "green");
                    this.green = false;
                }
                if (this.blue && this.getTime() - this.bluetime > 5000)
                {
                    this.Broadcast("show", "blue");
                    this.blue = false;
                }
                if (this.cyan && this.getTime() - this.cyantime > 5000)
                {
                    this.Broadcast("show", "cyan");
                    this.cyan = false;
                }
                if (this.magenta && this.getTime() - this.magentatime > 5000)
                {
                    this.Broadcast("show", "magenta");
                    this.magenta = false;
                }
                if (this.yellow && this.getTime() - this.yellowtime > 5000)
                {
                    this.Broadcast("show", "yellow");
                    this.yellow = false;
                }
                if (this.getTime() - this.timedoortime > 5000)
                {
                    this.timedoortime = this.getTime();
                    this.timedoor = !this.timedoor;
                    this.Broadcast(this.timedoor ? "show" : "hide", "timedoor");
                }

                foreach (var p in this.Players)
                {
                    if (p.cheat > 0) p.cheat--;
                }
                if (this.die)
                {
                    foreach (var p in this.Players)
                    {
                        p.Disconnect();
                    }
                }

                if (this.upgrade)
                {
                    foreach (var p in this.Players)
                    {
                        p.SendMessage(Message.Create("upgrade"));
                        p.Disconnect();
                    }
                }


                if (!this.upgrade && this.upgradeWarning && (this.getTime() - this.upgradeWarningTime) > 30000)
                {
                    this.upgradeWarningTime = this.getTime();
                    foreach (var p in this.Players)
                    {
                        if (!p.isguest && p.initialized)
                        {
                            p.SendMessage(Message.Create("write", "SYSTEM",
                                "Everybody Edits is about to get updated. Please save your world now."));
                        }
                    }
                }

                if (this.getTime() - this.potionCheckTime > 1000)
                {
                    this.potionCheckTime = this.getTime();
                    this.checkPotions();
                }

                if (this.getTime() - this.metaUpdateTime > 10000)
                {
                    this.metaUpdateTime = this.getTime();
                    this.BroadcastMetaData();
                }

                if (this.getTime() - this.worldWootCheckTime > (Config.worldwoot_bucket_time / 2 * 1000))
                {
                    this.worldWootCheckTime = this.getTime();
                    this.worldWootLatest = this.baseworld.woots;
                    this.baseworld.refreshWoots(delegate { });
                }


                var nl = new List<Ban>();
                foreach (var b in this.bans)
                {
                    if ((b.timestamp - DateTime.Now).TotalMilliseconds > 0)
                    {
                        nl.Add(b);
                    }
                }
                this.bans = nl;

                if (this.coinscollected > 1)
                {
                    this.coinscollected -= 1;
                }
                else
                {
                    this.coinscollected = 0;
                }
            }, 250);

            this.AddTimer(this.checkAllOnlineStatus, 30000);
            //AddTimer(BroadcastDelayedPlayerUpdates, 25);
        }

        protected virtual void checkWorldBanAndLoad(string roomid)
        {
            Console.WriteLine("checkWorldBanAndLoad " + roomid);
            this.PlayerIO.BigDB.Load("WorldBans", roomid, delegate (DatabaseObject o)
            {
                var isWorldBanned = false;
                if (o != null && o.Contains("Bans"))
                {
                    var bans = o.GetArray("Bans");
                    foreach (DatabaseObject ban in bans)
                    {
                        if (ban.GetBool("Active", false))
                        {
                            isWorldBanned = true;
                            this.worldBannedReason = ban.GetString("Reason", "No reason given");
                            break;
                        }
                    }
                }

                Console.WriteLine("Found in  WorldBans. Banned? " + isWorldBanned);

                this.worldBanned = isWorldBanned;

                this.loadWorld(roomid);
            }, delegate
            {
                Console.WriteLine("Not found in WorldBans.");
                this.loadWorld(roomid);
            });
        }

        protected virtual void loadWorld(string roomid)
        {
            Console.WriteLine("LoadWorld");

            var t = this.getTime();


            this.PlayerIO.BigDB.LoadOrCreate("Worlds", roomid, delegate (DatabaseObject o)
            {
                //DateTime d = DateTime.Now;

                //ownedWorld = o;
                this.baseworld.fromDatabaseObject(o);

                //Console.WriteLine("Object unserialized " + DateTime.Now.Subtract(d).TotalMilliseconds);

                //width = ownedWorld.GetInt("width", width);
                //height = ownedWorld.GetInt("height", height);
                //levelOwner = ownedWorld.GetString("owner", "");
                //coinbanned = ownedWorld.GetBool("coinbanned", false);
                //wootupbanned = ownedWorld.GetBool("wootbanned", false);
                //plays = ownedWorld.GetInt("plays", 1);
                //worldwoots = ownedWorld.GetInt("woots", 0);
                //Visible = ownedWorld.GetBool("visible",true);

                this.Visible = this.baseworld.visible && !this.worldBanned;

                this.RoomData["plays"] = this.baseworld.plays.ToString();
                this.RoomData["name"] = this.removeBadChars(this.baseworld.name);
                this.RoomData["woots"] = this.baseworld.woots.ToString();
                //Console.WriteLine(baseworld.name + " is featured: " + baseworld.IsFeatured.ToString());
                this.RoomData["IsFeatured"] = this.baseworld.IsFeatured.ToString();
                this.RoomData.Save();

                if (this.baseworld.ownerid != "")
                {
                    Console.WriteLine("loading po");
                    this.PlayerIO.BigDB.Load("PlayerObjects", this.baseworld.ownerid, delegate (DatabaseObject oo)
                    {
                        if (oo.GetBool("banned", false) || oo.GetBool("tempbanned", false))
                        {
                            foreach (var p in this.Players)
                            {
                                p.disconnected = true;
                                p.SendMessage(Message.Create("info", "World not available",
                                    "The owner of this world has been banned."));
                                p.Disconnect();
                            }
                            return;
                        }

                        Console.WriteLine("World is READY");

                        this.levelOwnerName = oo.GetString("name", "");
                        this.ready = true;

                        foreach (var p in this.Players)
                        {
                            if (p.ready)
                                this.sendInitMessage(p);
                        }
                    });
                }
                else
                {
                    this.ready = true;

                    foreach (var p in this.Players)
                    {
                        if (p.ready)
                            this.sendInitMessage(p);
                    }
                }

                Console.WriteLine("Loaded world, in" + (this.getTime() - t));
                //this.baseworld.refreshWoots(delegate
                //{
                //    Console.WriteLine("Woots refreshed: " + this.baseworld.woots);
                //    this.worldWootLatest = this.baseworld.woots;
                //    this.BroadcastMetaData();
                //});


                //Console.WriteLine("Total work time " + DateTime.Now.Subtract(d).TotalMilliseconds);
            });
        }


        public void BanUser(Player player, string bannedBy, string reason)
        {
            player.PlayerObject.Set("banned", true);
            player.PlayerObject.Set("ban_reason", "Banned by " + bannedBy + ": " + reason);
            player.PlayerObject.Save();
        }

        public void KickUser(string userid, int minutes)
        {
            Console.WriteLine("addBan " + userid + " for " + minutes);
            this.bans.Add(new Ban(userid, DateTime.Now.AddMinutes(minutes)));
        }

        public List<Player> GetPlayersWithIp(IPAddress ip)
        {
            return this.Players.Where(p => p.IPAddress == ip).ToList();
        }

        protected void checkVersion()
        {
            this.PlayerIO.BigDB.Load("Config", "config", delegate (DatabaseObject o)
            {
                if (this.version < o.GetInt("version", this.version) ||
                    (this.isbetalevel && this.version < o.GetInt("betaversion", this.version)))
                {
                    this.upgrade = true;
                }
                ;
                if (this.version < o.GetInt("nextversion", this.version))
                {
                    this.upgradeWarning = true;
                }
                ;
            });
        }

        protected void checkAllOnlineStatus()
        {
            foreach (var p in this.Players)
            {
                if (!p.isguest && !p.disconnected && p.initialized) this.checkOnlineStatus(p);

                if (!p.initialized && (DateTime.Now - p.joinTime).TotalSeconds > 30)
                {
                    //p.SendMessage(Message.Create("info", "Server error",
                    //    "Your connection to the world timed out. Please try again."));
                    //p.Disconnect();
                }
            }
        }

        protected void checkOnlineStatus(Player p)
        {
            OnlineStatus.getOnlineStatus(this.PlayerIO, p.ConnectUserId, delegate (OnlineStatus os)
            {
                if (os.currentWorldName != "" && os.currentWorldId != "" && os.currentWorldId != this.RoomId &&
                    os.ipAddress != p.IPAddress.ToString())
                {
                    this.PlayerIO.ErrorLog.WriteError("User present in multiple worlds. ",
                        p.name + " is online in " + this.baseworld.name + "(" + this.RoomId + ") and " +
                        os.currentWorldName + "(" + os.currentWorldId + "). IpAdress Ingame: " + p.IPAddress + " vs " +
                        os.ipAddress + ". Already disconnected? " + p.disconnected, "Kicked User: " + p.name, null);
                    p.disconnected = true;
                    p.SendMessage(Message.Create("info", "Limit reached",
                        "To prevent abuse you can only be connected to one world at a time.\nYou are also logged into " +
                        os.currentWorldName + "."));
                    p.Disconnect();
                }
                else
                {
                    p.SaveOnlineStatus();
                }
            });
        }

        protected void BroadcastPlayerUpdate(Player player, Message msg, string type = null)
        {
            this.BroadcastMessage(msg);
            //return;

            //string t = type != null? type: msg.Type;

            //if (player.last_update.ContainsKey(t) && (DateTime.Now - ((DateTime)player.last_update[t])).TotalMilliseconds < 5)
            //{
            //    player.addUpdateMessage(msg, t);
            //}
            //else
            //{                
            //    Broadcast(msg);
            //    player.setUpdateMessageSend(t);
            //}
        }

        public void BroadcastMessage(Message msg)
        {
            foreach (var p in this.Players)
            {
                p.SendMessage(msg);
            }
        }

        //protected void BroadcastDelayedPlayerUpdates() {

        //    //Console.WriteLine("BroadcastPlayerUpdates");
        //    foreach (Player player in Players) {

        //        if (!player.hasUpdates || player.disconnected) continue;

        //        //Console.WriteLine("Player " + player.name + " has " + player.updates.Count + " updates");
        //        foreach (DictionaryEntry entry in player.updates) {
        //            //Console.WriteLine("\tmessage: " + ((Message)entry.Value).Type);
        //            player.setUpdateMessageSend((string)entry.Key);
        //            Broadcast((Message)entry.Value);
        //        }

        //        player.clearUpdateMessages();
        //    }
        //}

        public override bool AllowUserJoin(Player player)
        {
            Console.WriteLine("Users joining");
            return true; // allow infinite amount of players in a world.

            // To prevent people joining anonymously 
            if (!player.PlayerObject.GetBool("created", false) || player.PlayerObject.GetString("name", "") == "")
            {
                return false;
            }

            if (this.PlayerCount > 50 && (!player.canbemod || !player.CanBeGuardian))
            {
                player.Send("Info", "Room is full", "Sorry this room is full, please try again later :)");
                return false;
            }

            if (player.PlayerObject.GetBool("banned", false))
            {
                player.Send("info", "You are banned", "This account is banned due to abuse or fraud.");
                return false;
            }

            if (player.PlayerObject.GetBool("yempbanned", false))
            {
                player.Send("info", "You are banned", "This account is temporarily banned due to abuse or fraud.");
                return false;
            }

            if (player.isguest)
            {
                return false; // Prevent guests from entering world, because guest bombing ruins stuff
                /*
                player.canWinEnergy = false;
                // When player is a guest we only allow one IP adress to be used as a guest account. This is to prevent guest bombing.
                if (IPAdressAlreadyPresentAsGuest(player))
                {
                    player.Disconnect();
                }
                */
            }
            if (this.editkey == "" ||
                (player.JoinData.ContainsKey("editkey") && player.JoinData["editkey"] == this.editkey))
            {
                player.canEdit = true;
            }

            if (this.owned && (this.RoomId == player.room0 || player.betaonlyroom == this.RoomId) &&
                player.haveSmileyPackage)
            {
                player.canEdit = true;
                player.owner = true;
                // removed. No reason to set baseworld ownerid. It should be loaded from the world anyway (and set on world creation time)
                //if (baseworld != null) baseworld.ownerid = player.ConnectUserId;
            }


            foreach (var b in this.bans)
            {
                if (b.userid == player.ConnectUserId && !player.owner && !player.canbemod && !player.CanBeGuardian)
                {
                    player.Send("info", "You are banned", "You have been banned from this world");
                    return false;
                }
            }

            var pKey = player.PlayerObject.Key;
            int pKeyInt;
            if (pKey.StartsWith("simple") && !int.TryParse(pKey.Substring(6, 1), out pKeyInt))
            {
                player.Send("info", "Technical issue",
                    "There has been an error with you player data. Please try again later.");
                return false;
            }

            if (this.Players.Count(pl => pl.ConnectUserId == player.ConnectUserId) > 2)
            {
                player.Send("info", "Limit reached",
                    "To prevent abuse you can only be connected to the same world once.");
                return false;
            }

            return true;
            //   return base.AllowUserJoin(player);
        }

        private void CheckTempBanned(Player player)
        {
            this.PlayerIO.BigDB.Load("TempBans", player.ConnectUserId, delegate (DatabaseObject o)
            {
                if (o == null)
                {
                    return;
                }

                var tempBans = o.GetArray("Bans");
                if (tempBans != null)
                {
                    var orderedBans =
                        tempBans.OrderByDescending(x => ((DatabaseObject)x).GetDateTime("Date")).ToArray();

                    var lastExpiration = orderedBans[0] as DatabaseObject;

                    if (lastExpiration == null)
                    {
                        return;
                    }

                    if (lastExpiration.GetDateTime("Expires") > DateTime.Now)
                    {
                        player.Send("info", "Banned",
                            "You have been banned.\n\"" + lastExpiration.GetString("Reason", "") + "\"");
                        player.Disconnect();
                        player.disconnected = true;
                    }
                }
            });
        }

        private bool IPAdressAlreadyPresentAsGuest(Player player)
        {
            var playerIp = player.IPAddress;
            foreach (var p in this.Players)
            {
                // If the players IP is already in use in the world AND is used by a guest
                // The requirement is that two guests cannot use the same IP adress
                if (p.IPAddress.Equals(playerIp) && p.isguest)
                {
                    return true;
                }
            }
            return false;
        }

        // This method is called whenever a player joins the game
        public override void UserJoined(Player player)
        {
            if (!this.ips.ContainsKey(player.IPAddress.ToString()) && !player.isguest)
            {
                lock (this.RoomData)
                {
                    this.ips.Add(player.IPAddress.ToString(), -1);

                    this.baseworld.plays++;
                    this.sessionplays++;

                    if (this.sessionplays % 15 == 0)
                    {
                        if (this.owned && this.baseworld != null)
                        {
                            if (this.sessionplays > 10)
                                this.baseworld.save(false);
                        }
                        this.BroadcastMetaData();
                    }

                    this.RoomData["plays"] = this.baseworld.plays.ToString();
                    this.RoomData.Save();
                }
            }
        }

        public override void GameClosed()
        {
            if (this.owned && this.baseworld != null)
            {
                this.baseworld.save(false);
            }
        }

        // This method is called when a player leaves the game
        public override void UserLeft(Player player)
        {
            this.Broadcast("left", player.Id);
            player.currentWorldId = "";
            player.currentWorldName = "";
            player.Save();
            player.disconnected = true;

            Console.WriteLine("User fully left :)");
        }

        private void ResetPlayers()
        {
            var tele = Message.Create("tele", true);
            foreach (var p in this.Players)
            {
                if (!p.isgod && !p.ismod)
                {
                    p.x = 16;
                    p.y = 16;
                    p.checkpoint = null;
                    var d = this.baseworld.getSpawn();
                    p.x = d.x * 16;
                    p.y = d.y * 16;
                    tele.Add(p.Id, p.x, p.y);
                }
            }
            this.BroadcastMessage(tele);
        }

        private void RespawnPlayer(Player p)
        {
            if (!p.isgod && !p.ismod)
            {
                var zombie_potion = ItemManager.GetPotion(9);
                if (p.hasActivePotion(zombie_potion))
                {
                    p.removePotion(zombie_potion);
                    this.Broadcast("p", p.Id, zombie_potion.id, false, 0);
                }

                var tele = Message.Create("tele", false);
                this.AddRespawnToMessage(p, tele);
                Broadcast(tele);
            }
        }

        public void RespawnAllPlayers()
        {
            var tele = Message.Create("tele", false);
            foreach (var p in this.Players)
            {
                if (!p.isgod && !p.ismod)
                {
                    this.AddRespawnToMessage(p, tele);
                }
            }
            this.BroadcastMessage(tele);
        }

        private void AddRespawnToMessage(Player p, Message msg)
        {
            p.x = 16;
            p.y = 16;
            Item d = null;
            if (p.checkpoint != null &&
                this.baseworld.getBrickType(0, p.checkpoint.x, p.checkpoint.y) == (uint)ItemTypes.Checkpoint)
            {
                d = p.checkpoint;
            }
            else
            {
                d = this.baseworld.getSpawn();
            }
            p.x = d.x * 16;
            p.y = d.y * 16;

            msg.Add(p.Id, p.x, p.y);
        }

        // This method is called when a player sends a message into the server code

        public override void GotMessage(Player player, Message m)
        {
            if (!player.initialized && m.Type != "init")
            {
                // Ignore if player are not initialized (to prevent hacking)
                return;
            }

            //if (player.RepeatMessageCount(m.Type) > 100 && (DateTime.Now-player.lastMessageTimer).TotalSeconds < 1) {
            if (!player.AllowMessage(m.Type))
            {
                var msgtype = m.Type;
                if (msgtype.StartsWith(this.editchar))
                {
                    msgtype = msgtype.Remove(0, this.editchar.Length);
                }
                this.PlayerIO.ErrorLog.WriteError("Message Repeat Limit exeeded",
                    player.name + " has send '" + msgtype + "' to many times. IpAdress Ingame: " + player.IPAddress,
                    "Message type: " + msgtype, null);
                //player.Disconnect();
                return;
                //if(m.Type == editchar){
                //    player.Send("info", "Server Error", "");                    
                //}
            }

            switch (m.Type)
            {
                case "clear":
                {
                    if (this.owned && (player.owner || player.canbemod))
                    {
                        this.baseworld.reset();
                        this.Broadcast("clear", this.baseworld.width, this.baseworld.height, this.baseworld.borderType,
                            this.baseworld.fillType);
                    }
                    break;
                }

                case "say":
                {
                    this.handleSay(player, m);
                    break;
                }

                case "autosay":
                {
                    this.handleAutoSay(player, m);
                    break;
                }

                case "diamondtouch":
                {
                    var xp = m.GetInt(0);
                    var yp = m.GetInt(1);

                    if (this.baseworld.getBrickType(0, xp, yp) == (uint)ItemTypes.Diamond)
                    {
                        player.face = 31;

                        //Broadcast("face", player.Id, 31);
                        this.BroadcastPlayerUpdate(player, Message.Create("face", player.Id, 31));
                    }
                    break;
                }

                case "caketouch":
                {
                    var xp = m.GetInt(0);
                    var yp = m.GetInt(1);

                    if (this.baseworld.getBrickType(0, xp, yp) == (uint)ItemTypes.Cake)
                    {
                        var partyfaces = new List<int>();
                        for (var i = 72; i <= 75; i++)
                        {
                            if (i != player.face) partyfaces.Add(i);
                        }
                        player.face = partyfaces[this.rd.Next(0, partyfaces.Count)];

                        this.BroadcastPlayerUpdate(player, Message.Create("face", player.Id, player.face));
                    }

                    break;
                }

                case "hologramtouch":
                {
                    var xp = m.GetInt(0);
                    var yp = m.GetInt(1);

                    if (this.baseworld.getBrickType(0, xp, yp) == (uint)ItemTypes.Hologram)
                    {
                        player.face = 100;
                        this.BroadcastPlayerUpdate(player, Message.Create("face", player.Id, player.face));
                    }

                    break;
                }

                case "death":
                {
                    this.RespawnPlayer(player);
                    break;
                }

                case "checkpoint":
                {
                    var xp = m.GetInt(0);
                    var yp = m.GetInt(1);

                    if (this.baseworld.getBrickType(0, xp, yp) == (uint)ItemTypes.Checkpoint)
                    {
                        //Console.WriteLine("checkpoint set: " + xp + "/" + yp );
                        player.checkpoint = new Item((int)ItemTypes.Checkpoint, xp, yp);
                    }
                    break;
                }

                case "levelcomplete":
                {
                    if (!player.hasCompleted)
                        this.Broadcast("write", "* WORLD", player.name.ToUpper() + " completed this world!");

                    player.hasSilverCrown = true;
                    player.hasCompleted = true;

                    this.BroadcastPlayerUpdate(player, Message.Create("ks", player.Id));

                    break;
                }

                case "save":
                {
                    if (this.owned && (player.owner || player.canbemod))
                    {
                        //if (sessionplays > 10) ownedWorld.Set("plays", plays);
                        //ownedWorld.Set("woots", worldwoots);
                        //ownedWorld.Set("name", removeBadChars( RoomData["name"] ));
                        this.baseworld.save(true, delegate { player.SendMessage(Message.Create("saved")); });
                        if (this.baseworld.ownerid != "")
                        {
                            this.PlayerIO.BigDB.Load("PlayerObjects", this.baseworld.ownerid,
                                delegate (DatabaseObject oo)
                                {
                                    if (!oo.Contains("myworldnames"))
                                    {
                                        oo.Set("myworldnames", new DatabaseObject());
                                    }
                                    oo.GetObject("myworldnames")
                                        .Set(this.RoomId, this.removeBadChars(this.baseworld.name));
                                    oo.Save();
                                });
                        }
                    }
                    break;
                }
                case "name":
                {
                    if (this.owned && (player.owner || player.canbemod) && this.baseworld != null)
                    {
                        var newname = this.removeBadChars(m.GetString(0));
                        if (newname.Length > 20)
                        {
                            newname = newname.Substring(0, 20);
                        }
                        if (newname.Length == 0)
                        {
                            return;
                        }

                        this.RoomData["name"] = newname;
                        this.RoomData.Save();

                        this.BroadcastMetaData();

                        this.baseworld.name = newname;

                        if (this.baseworld.ownerid != "")
                        {
                            this.PlayerIO.BigDB.Load("PlayerObjects", this.baseworld.ownerid,
                                delegate (DatabaseObject oo)
                                {
                                    if (!oo.Contains("myworldnames"))
                                    {
                                        oo.Set("myworldnames", new DatabaseObject());
                                    }
                                    oo.GetObject("myworldnames").Set(this.RoomId, this.baseworld.name);
                                    oo.Save();
                                });
                        }
                        foreach (var p in this.Players)
                        {
                            p.currentWorldId = this.RoomId;
                            p.currentWorldName = this.removeBadChars(this.baseworld.name);
                        }
                    }
                    break;
                }
                case "key":
                {
                    if (player.owner)
                    {
                        this.editkey = m.GetString(0);
                        foreach (var p in this.Players)
                        {
                            if (p.owner == false)
                            {
                                this.setEditRigths(p, false);
                            }
                        }
                    }
                    break;
                }

                case "touch":
                {
                    var touchedId = m.GetInt(0);
                    var potionId = m.GetInt(1);

                    if (!this.baseworld.isPotionEnabled(potionId.ToString())) break;
                    foreach (var p in this.Players)
                    {
                        if (p.Id == touchedId && !p.isgod && !p.ismod) // we could add guests here? // *zombieguests
                        {
                            var potion = ItemManager.GetPotion(potionId);
                            if (player.hasActivePotion(potion) && !p.hasActivePotion(potion))
                            {
                                if (potionId == 6)
                                {
                                    var removed = player.removePotion(potion);
                                    p.addPotion(removed, delegate
                                    {
                                        // Player lost potion
                                        this.Broadcast("p", player.Id, removed.id, false, removed.timeleft);
                                        // p got it
                                        this.Broadcast("p", p.Id, removed.id, true, removed.timeleft);
                                    });
                                }
                                else
                                {
                                    p.activatePotion(potion,
                                        delegate { this.Broadcast("p", p.Id, potion.id, true, potion.timeleft); });
                                }
                            }
                            break;
                        }
                    }
                    break;
                }

                case "kill":
                {
                    if (player.ismod)
                    {
                        this.die = true;
                    }
                    break;
                }
                case "mod":
                {
                    if (player.canbemod)
                    {
                        player.ismod = !player.ismod;
                        this.setEditRigths(player, player.ismod);
                        this.Broadcast("mod", player.Id, player.ismod);
                    }
                    break;
                }

                case "guardian":
                {
                    if (player.CanBeGuardian)
                    {
                        player.isInGuardianMode = !player.isInGuardianMode;
                        this.Broadcast("guardian", player.Id, player.isInGuardianMode);
                    }
                    break;
                }


                case "god":
                {
                    if (this.lockedroom && player.canEdit)
                    {
                        this.Broadcast("god", player.Id, m.GetBoolean(0));
                        player.isgod = m.GetBoolean(0);
                    }
                    break;
                }
                case "time":
                {
                    player.SendMessage(Message.Create("time", m.GetDouble(0), this.getTime()));
                    break;
                }
                case "access":
                {
                    if (!player.isguest && m.Count != 0 && m.GetString(0).Length >= 4)
                    {
                        if (m.GetString(0) == this.editkey)
                        {
                            this.setEditRigths(player, true);
                        }
                    }
                    break;
                }
                case "allowpotions":
                {
                    if (!player.owner) break;

                    if (this.onChangePotionSettings(player))
                    {
                        this.baseworld.allowPotions = m.GetBoolean(0);
                        this.broadcastPotionSettings();
                        this.Broadcast("write", this.systemName,
                            "Potions are now: " + (this.baseworld.allowPotions ? "on" : "off"));

                        if (this.baseworld.allowPotions)
                        {
                            foreach (var p in this.Players)
                            {
                                this.broadcastActivePotions(p);
                            }
                        }
                    }
                    break;
                }
                case "wootup":
                {
                    if (!this.baseworld.wootupbanned && !player.owner)
                    {
                        //WootUpPlayer.giveWootUp(this.PlayerIO, player.ConnectUserId, player.IPAddress.ToString(),
                        //    delegate (bool success)
                        //    {
                        //        if (success)
                        //        {
                        //            this.addWootToWorld();
                        //            this.BroadcastMetaData(player);
                        //            this.BroadcastMessage(Message.Create("wu", player.Id));
                        //            this.BroadcastMessage(Message.Create("write", "* WORLD",
                        //                player.name.ToUpper() + " gave a woot."));
                        //        }
                        //        else
                        //        {
                        //            player.SendMessage(Message.Create("wu", player.Id));
                        //            player.SendMessage(Message.Create("write", "* WORLD",
                        //                player.name.ToUpper() + " gave a woot."));
                        //        }
                        //    }, player.timezoneoffset);
                    }
                    break;
                }

                case "init":
                {
                    player.ready = true;
                    player.initialized = true;

                    this.sendInitMessage(player);

                    // To prevent people (hacking) sending in too many init messages
                    //if (player.initialized || player.isInitializing)
                    //{
                    //    player.cheat++;
                    //    break;
                    //}

                    //Callback cb = delegate
                    //{
                    //    player.Init(this.PlayerIO, delegate
                    //    {
                    //        player.joinTime = DateTime.Now;
                    //        player.currentWorldId = this.RoomId;
                    //        player.currentWorldName = this.RoomData.ContainsKey("name")
                    //            ? this.RoomData["name"]
                    //            : "Untitled World";

                    //        player.Save();

                    //        //Old code
                    //        if (!player.haveSmileyPackage && this.isbetalevel)
                    //        {
                    //            player.SendMessage(Message.Create("betaonly"));
                    //            player.Disconnect();
                    //        }

                    //        Console.WriteLine("Player ready " + player.name + " world? " + this.ready);
                    //        if (this.owned && !this.ready)
                    //        {
                    //            player.ready = true;
                    //            //if (worldBanned)
                    //            //{
                    //            //    if (player.canbemod || player.CanBeGuardian || player.owner)
                    //            //    {
                    //            //        player.SendMessage(Message.Create("info", "World not available", "This world has been banned.\n"+worldBannedReason));
                    //            //        sendInitMessage(player);
                    //            //    }
                    //            //    else
                    //            //    {
                    //            //        player.disconnected = true;
                    //            //        player.SendMessage(Message.Create("info", "World not available", "This world has been banned."));
                    //            //        player.Disconnect();
                    //            //    }
                    //            //}
                    //        }
                    //        else
                    //        {
                    //            this.sendInitMessage(player);
                    //        }
                    //    });
                    //};

                    // Initialize shop since we need to check certain player requests against the payvault (which is the shop)
                    //if (this.shop == null)
                    //{
                    //    this.shop = new Shop(this.PlayerIO, delegate { cb(); });
                    //}
                    //else
                    //{
                    //    cb();
                    //}

                    /*
                        player.Init(PlayerIO, delegate() {

                            player.joinTime = DateTime.Now;
                            player.currentWorldId = RoomId;
                            player.currentWorldName = RoomData.ContainsKey("name") ? RoomData["name"] : "Untitled World";

                            player.Save();

                            //Old code
                            if (!player.haveSmileyPackage && isbetalevel) {
                                player.Send("betaonly");
                                player.Disconnect();
                            }

                            if (owned && !ready) {
                                player.ready = true;
                            } else {
                                sendInitMessage(player);
                            }

                        });
                     * */
                    break;
                }

                case "init2":
                {
                    //player.initialized = true;

                    // Adding the existing players to the new player screen
                    foreach (var p in this.Players)
                    {
                        if (p != player)
                        {
                            var friends = player.hasFriend(p.ConnectUserId);
                            player.SendMessage(Message.Create("add", p.Id, p.name, p.face, p.x, p.y, p.isgod, p.ismod,
                                p.canchat, p.coins, p.bcoins, friends, false, 1 /* woot level */, p.isClubMember,
                                p.isInGuardianMode));
                            player.SendMessage(Message.Create("face", p.Id, p.face));

                            if (p.initialized)
                            {
                                var potions = p.getActivePotions();
                                foreach (var potion in potions)
                                {
                                    player.SendMessage(Message.Create("p", p.Id, potion.id, true, potion.timeleft));
                                }
                            }
                            if (p.hasSilverCrown) player.SendMessage(Message.Create("ks", p.Id));
                        }
                    }


                    // Send the previous 10 chat messages to the player (if she has chat or messages are from a friend) 
                    // for (int a = 0; a < chatmessages.Length; a++) {
                    for (var a = 10; a < this.chatmessages.Length; a++)
                    // Chat messages length increased to 100, but we still only want to deliver the last 10 to the user
                    {
                        if (this.chatmessages[a] != null)
                        {
                            if (this.chatmessages[a].connectUserId == player.ConnectUserId ||
                                (player.canchat && this.chatmessages[a].canchat) ||
                                player.hasFriend(this.chatmessages[a].connectUserId))
                            {
                                player.SendMessage(Message.Create("say_old", this.chatmessages[a].name,
                                    this.chatmessages[a].text, player.hasFriend(this.chatmessages[a].connectUserId)));
                            }
                        }
                    }
                    //}

                    player.SendMessage(Message.Create("k", this.crownid));

                    break;
                }

                // Player collect coin
                case "c":
                {
                    player.coins = m.GetInt(0);
                    player.bcoins = m.GetInt(1);
                    this.coinscollected++;

                    if (m.Count < 4) return;

                    var cx = m.GetInt(2);
                    var cy = m.GetInt(3);

                    //Console.WriteLine("collect coin at " + cx + "/" + cy + ". Already collected? " + player.isCoinCollected(cx, cy));

                    if (
                        // There is a coin on the position
                        (this.baseworld.getBrickType(0, cx, cy) == (uint)ItemTypes.Coin) &&
                        // Player has not collected all more coins than is possible
                        player.coins <= this.baseworld.coinCount() &&
                        // Player is moving
                        (DateTime.Now - player.lastmove).TotalSeconds < 15 &&
                        // Some time has passed since last coin pickup
                        (DateTime.Now - this.coinanticheat).TotalMilliseconds > 100 &&
                        // world has not been banned for woot coins
                        !this.baseworld.coinbanned &&
                        // It is not the players own world
                        !player.owner &&
                        // The player is not able to edit
                        (!player.canEdit || player.ismod) &&
                        // Player has not already picked up a coin at that position
                        !player.isCoinCollected(cx, cy)
                        )
                    {
                        // Set the coin as collected by the player
                        player.setCoinCollected(cx, cy);

                        // Check against percentage of world that is coins
                        if ((this.baseworld.coinPercentage() * 2) * 100 < this.rd.Next(0, 100))
                        {
                            //var playerlevel = player.level;
                            player.doCoinCollect(delegate (bool success)
                            {
                                if (success)
                                {
                                    this.magiccollected++;
                                    this.BroadcastMessage(Message.Create("w", player.Id));

                                    //if (player.level > playerlevel)
                                    //{
                                    //    this.BroadcastMessage(Message.Create("levelup", player.Id, 1 /* woot level */));
                                    //}
                                }
                            });
                        }
                    }


                    if (
                        player.canWinEnergy &&
                        (DateTime.Now - player.lastCoin).TotalMinutes > 20 &&
                        (DateTime.Now > player.coinTimer) &&
                        this.coinscollected < 4 * this.PlayerCount &&
                        this.coinscollected < 40 &&
                        (DateTime.Now - this.coinanticheat).TotalMilliseconds > 100 &&
                        (DateTime.Now - player.lastmove).TotalSeconds < 15 &&
                        !this.baseworld.coinbanned
                        )
                    {
                        // 1.3 % chance
                        if (this.rd.Next(0, 75) == 1)
                        {
                            // 4 % chance
                            if (this.rd.Next(0, 50) <= 2)
                            {
                                // 25% chance each
                                if (this.rd.Next(0, 100) < 25)
                                {
                                    // Smileys
                                    if (!player.PayVault.Has("smileywizard"))
                                    {
                                        player.SendMessage(Message.Create("info", "Congratulations!",
                                            "You found an extra magical coin! Upon touching the coin you found that you had a wizard hat and beard..."));
                                        this.Broadcast("write", "* MAGIC",
                                            player.name.ToUpper() + " just became a wizard!");
                                        player.PayVault.Give(new[] { new BuyItemInfo("smileywizard") }, delegate { });
                                        player.lastCoin = DateTime.Now;
                                        player.canWinEnergy = false;
                                        player.face = 22;
                                        this.BroadcastPlayerUpdate(player, Message.Create("face", player.Id, 22));
                                        player.SendMessage(Message.Create("givemagicsmiley", "smileywizard"));
                                    }
                                    else if (!player.PayVault.Has("smileywizard2"))
                                    {
                                        // Energy Coins
                                        var rand = rd.Next(0, 100);

                                        if (rand <= 5)
                                        {
                                            player.SendMessage(Message.Create("info", "Congratulations!", "You found a magical coin! Upon touching the coin you were awarded 15 total Energy and a full Energy bar!"));
                                            Broadcast("write", "* MAGIC", player.name.ToUpper() + " just found a magical coin and was awarded 15 total energy.");
                                            player.lastCoin = DateTime.Now;
                                            player.canWinEnergy = false;
                                            player.PlayerObject.Set("maxEnergy", player.PlayerObject.GetInt("maxEnergy") + 15);
                                            var full = new TimeSpan(0, 0, 150 * player.maxEnergy);
                                            player.PlayerObject.Set("shopDate", DateTime.Now + full);
                                            player.PlayerObject.Save();
                                        }
                                        else if (rand <= 35)
                                        {
                                            player.SendMessage(Message.Create("info", "Congratulations!", "You found a magical coin! Upon touching the coin you found 10 total Energy and a full Energy bar!"));
                                            Broadcast("write", "* MAGIC", player.name.ToUpper() + " just found a magical coin and was awarded 10 total energy.");
                                            player.lastCoin = DateTime.Now;
                                            player.canWinEnergy = false;
                                            player.PlayerObject.Set("maxEnergy", player.PlayerObject.GetInt("maxEnergy") + 10);
                                            var full = new TimeSpan(0, 0, 150 * player.maxEnergy);
                                            player.PlayerObject.Set("shopDate", DateTime.Now + full);
                                            player.PlayerObject.Save();
                                        }
                                        else if (rand <= 53)
                                        {
                                            player.SendMessage(Message.Create("info", "Congratulations!", "You found a magical coin! Upon touching the coin you found 5 total Energy!"));
                                            Broadcast("write", "* MAGIC", player.name.ToUpper() + " just found a magical coin and was awarded 5 total energy.");
                                            player.lastCoin = DateTime.Now;
                                            player.canWinEnergy = false;
                                            player.PlayerObject.Set("maxEnergy", player.PlayerObject.GetInt("maxEnergy") + 5);
                                            player.PlayerObject.Save();
                                        }
                                        else if (rand <= 100)
                                        {
                                            player.SendMessage(Message.Create("info", "Congratulations!", "You found a magical coin! Upon touching the coin you found 1 total Energy!"));
                                            Broadcast("write", "* MAGIC", player.name.ToUpper() + " just found a magical coin and was awarded 1 total energy.");
                                            player.lastCoin = DateTime.Now;
                                            player.canWinEnergy = true;
                                            player.PlayerObject.Set("maxEnergy", player.PlayerObject.GetInt("maxEnergy") + 1);
                                            player.PlayerObject.Save();
                                        }
                                    }
                                }
                                else // 75% Chance
                                {
                                    // Energy Coins
                                    var rand = this.rd.Next(0, 100);

                                    if (rand <= 5)
                                    {
                                        player.SendMessage(Message.Create("info", "Congratulations!",
                                            "You found a magical coin! Upon touching the coin you were awarded 15 total Energy and a full Energy bar!"));
                                        this.Broadcast("write", "* MAGIC",
                                            player.name.ToUpper() +
                                            " just found a magical coin and was awarded 15 total energy.");
                                        player.lastCoin = DateTime.Now;
                                        player.canWinEnergy = false;
                                        player.PlayerObject.Set("maxEnergy",
                                            player.PlayerObject.GetInt("maxEnergy") + 15);
                                        player.PlayerObject.Set("Energy", player.PlayerObject.GetInt("maxEnergy"));
                                        player.PlayerObject.Save();
                                    }
                                    else if (rand <= 35)
                                    {
                                        player.SendMessage(Message.Create("info", "Congratulations!",
                                            "You found a magical coin! Upon touching the coin you found 10 total Energy and a full Energy bar!"));
                                        this.Broadcast("write", "* MAGIC",
                                            player.name.ToUpper() +
                                            " just found a magical coin and was awarded 10 total energy.");
                                        player.lastCoin = DateTime.Now;
                                        player.canWinEnergy = false;
                                        player.PlayerObject.Set("maxEnergy",
                                            player.PlayerObject.GetInt("maxEnergy") + 10);
                                        player.PlayerObject.Set("Energy", player.PlayerObject.GetInt("maxEnergy"));
                                        player.PlayerObject.Save();
                                    }
                                    else if (rand <= 53)
                                    {
                                        player.SendMessage(Message.Create("info", "Congratulations!",
                                            "You found a magical coin! Upon touching the coin you found 5 total Energy!"));
                                        this.Broadcast("write", "* MAGIC",
                                            player.name.ToUpper() +
                                            " just found a magical coin and was awarded 5 total energy.");
                                        player.lastCoin = DateTime.Now;
                                        player.canWinEnergy = false;
                                        player.PlayerObject.Set("maxEnergy", player.PlayerObject.GetInt("maxEnergy") + 5);
                                        player.PlayerObject.Save();
                                    }
                                    else if (rand <= 100)
                                    {
                                        player.SendMessage(Message.Create("info", "Congratulations!",
                                            "You found a magical coin! Upon touching the coin you found 1 total Energy!"));
                                        this.Broadcast("write", "* MAGIC",
                                            player.name.ToUpper() +
                                            " just found a magical coin and was awarded 1 total energy.");
                                        player.lastCoin = DateTime.Now;
                                        player.canWinEnergy = true;
                                        player.PlayerObject.Set("maxEnergy", player.PlayerObject.GetInt("maxEnergy") + 1);
                                        player.PlayerObject.Save();
                                    }
                                }
                            }

                            player.coinTimer = DateTime.Now.AddMinutes(1);
                        }
                    }

                    this.coinanticheat = DateTime.Now;

                    this.BroadcastPlayerUpdate(player, Message.Create("c", player.Id, player.coins, player.bcoins));
                    //Broadcast("c", player.Id, player.coins);
                    break;
                }

                // Player move
                case "m":
                {
                    player.x = m.GetDouble(0);
                    player.y = m.GetDouble(1);
                    //player.coins = m.GetInt(8);

                    var xo = (int)(player.x + 8) >> 4;
                    var yo = (int)(player.y + 8) >> 4;


                    if (xo < 0 || yo < 0 || xo > this.baseworld.width - 1 || yo > this.baseworld.height - 1)
                    {
                        player.Disconnect();
                        return;
                    }

                    if (!player.canEdit && !player.ismod)
                    {
                        var cur = this.baseworld.getBrickType(0, xo, yo);

                        // Check speed against maxspeed
                        var maxspeed = 16 * Config.physics_variable_multiplyer;
                        if (Math.Abs(m.GetDouble(2)) > maxspeed || Math.Abs(m.GetDouble(3)) > maxspeed)
                        {
                            player.cheat++;
                        }

                        // Check gravity
                        if (m.Count < 8 || m.GetDouble(8) != this.baseworld.gravityMultiplier)
                        {
                            player.cheat++;
                        }

                        if (player.cheat > 4)
                        {
                            player.SendMessage(Message.Create("info", "Synchronization Error",
                                "Data recieved from your client does not match data from server. You have been disconnected."));
                            player.Disconnect();
                            return;
                        }
                    }

                    try
                    {
                        var horizontal = m.GetInt(6);
                        var vertical = m.GetInt(7);

                        if (player.horizontal != horizontal || player.vertical != vertical)
                        {
                            player.lastmove = DateTime.Now;
                        }


                        player.horizontal = horizontal;
                        player.vertical = vertical;

                        //BroadcastPlayerUpdate(player, Message.Create("m", player.Id,
                        //    m.GetDouble(0),
                        //    m.GetDouble(1),
                        //    m.GetDouble(2),
                        //    m.GetDouble(3),
                        //    m.GetDouble(4),
                        //    m.GetDouble(5),
                        //    horizontal,
                        //    vertical,
                        //    player.coins)
                        //);

                        var spacedown = m.GetBoolean(9);

                        this.Broadcast("m", player.Id,
                            m.GetDouble(0),
                            m.GetDouble(1),
                            m.GetDouble(2),
                            m.GetDouble(3),
                            m.GetDouble(4),
                            m.GetDouble(5),
                            horizontal,
                            vertical,
                            player.coins,
                            spacedown
                            );
                    }
                    catch (Exception e)
                    {
                        this.PlayerIO.ErrorLog.WriteError("We got invalid data from " + player.ConnectUserId, e);
                    }
                    break;
                }
                default:
                {
                    if (m.Type == this.editchar)
                    {
                        this.placeBrick(player, m);
                    }
                    if (m.Type == this.editchar + "k")
                    {
                        this.crownid = player.Id;
                        //Broadcast("k", player.Id);
                        this.BroadcastPlayerUpdate(player, Message.Create("k", player.Id));
                    }
                    else if (m.Type == this.editchar + "p")
                    {
                        this.handlePotion(player, m);
                    }
                    else if (m.Type == this.editchar + "r")
                    {
                        this.redtime = this.getTime();
                        this.Broadcast("hide", "red", player.Id);
                        this.red = true;
                    }
                    else if (m.Type == this.editchar + "g")
                    {
                        this.greentime = this.getTime();
                        this.Broadcast("hide", "green", player.Id);
                        this.green = true;
                    }
                    else if (m.Type == this.editchar + "b")
                    {
                        this.bluetime = this.getTime();
                        this.Broadcast("hide", "blue", player.Id);
                        this.blue = true;
                    }
                    else if (m.Type == this.editchar + "c")
                    {
                        this.cyantime = this.getTime();
                        this.Broadcast("hide", "cyan", player.Id);
                        this.cyan = true;
                    }
                    else if (m.Type == this.editchar + "m")
                    {
                        this.magentatime = this.getTime();
                        this.Broadcast("hide", "magenta", player.Id);
                        this.magenta = true;
                    }
                    else if (m.Type == this.editchar + "y")
                    {
                        this.yellowtime = this.getTime();
                        this.Broadcast("hide", "yellow", player.Id);
                        this.yellow = true;
                    }
                    else if (m.Type == this.editchar + "f")
                    {
                        var face = m.GetInt(0);


                        if (this.smileyMap.smileyIsLegit(player, face, this.shop))
                        {
                            this.setPlayerFace(player, face);
                        }
                        else
                        {
                            player.SendMessage(Message.Create("info", "Synch Error",
                                "Data recieved from your client does not match data from server."));
                        }
                    }
                    break;
                }
            }
        }

        protected void setPlayerFace(Player player, int face)
        {
            player.face = face;
            this.BroadcastPlayerUpdate(player, Message.Create("face", player.Id, face));
        }

        protected void handleAutoSay(Player player, Message m)
        {
            var offset = 1;
            try
            {
                offset = m.GetInt(0);
            }
            catch (PlayerIOError error)
            {
                this.PlayerIO.ErrorLog.WriteError(error.ToString(),
                    "HandleAutoSay did not recieve int. Got: " + m.GetString(0), "", new Dictionary<string, string>());
            }

            if (offset < 0 || offset >= this.autotexts.Length) return;
            if (player.isguest)
            {
                player.SendMessage(Message.Create("info", "Not logged in",
                    "You must be a registered user to use the quick chat."));
                return;
            }
            if (DateTime.Now.Subtract(player.lastChat).TotalMilliseconds < 500)
            {
                player.SendMessage(Message.Create("write", "SYSTEM",
                    "You are trying to chat way to fast, spamming the chat is not nice!"));
                return;
            }
            player.lastChat = DateTime.Now;


            this.Broadcast("autotext", player.Id, this.autotexts[offset]);
        }

        protected string removeBadChars(String text)
        {
            //Limit range to 31 - 255
            text = Regex.Replace(text, @"[^\x1F-\xFF]", "").Trim();

            //Remove del char 127
            text = Regex.Replace(text, @"[\x7F]", "").Trim();

            return text;
        }

        protected void handleSay(Player player, Message m)
        {
            var text = this.removeBadChars(m.GetString(0));

            if (text.Length > 80)
            {
                return;
            }

            if (text.StartsWith("/"))
            {
                var words = text.Split(' ');

                if (words[0] == "/reportabuse" || words[0] == "/report" || words[0] == "/rep")
                {
                    this.cmds["/report"].resolve(player, words);
                    return;
                }

                if (words[0] == "/mute")
                {
                    this.cmds["/mute"].resolve(player, words);
                    return;
                }

                if (words[0] == "/unmute")
                {
                    this.cmds["/unmute"].resolve(player, words);
                    return;
                }

                if (words[0] == "/pm")
                {
                    if (!this.checkforSpam(text, player)) return;
                    this.cmds["/pm"].resolve(player, words);
                    return;
                }

                if (words[0] == "/refillmyenergy" && player.canbemod)
                {
                    var full = new TimeSpan(0, 0, 150 * player.maxEnergy);
                    player.PlayerObject.Set("shopDate", DateTime.Now - full);
                    return;
                }

                if (player.CanBeGuardian || player.canbemod)
                {
                    if (words[0] == "/tempban")
                    {
                        this.cmds["/tempban"].resolve(player, words);
                        return;
                    }

                    //Kick from world (repeated later in the code for owners)
                    if (words[0] == "/kick" && words.Length >= 2)
                    {
                        this.cmds["/kick"].resolve(player, words);
                        return;
                    }

                    if (words[0] == "/checkip")
                    {
                        this.cmds["/checkip"].resolve(player, words);
                        return;
                    }
                }

                if (player.owner || player.canbemod)
                {
                    if (words[0] == "/kickguests")
                    {
                        this.cmds["/kickguests"].resolve(player, words);
                        return;
                    }

                    //Reset world
                    if (words[0] == "/loadlevel")
                    {
                        if (this.owned && this.baseworld != null)
                        {
                            this.baseworld.reload();

                            var mess = Message.Create("reset");

                            this.baseworld.addToMessageAsComplexList(mess);
                            this.BroadcastMessage(mess);

                            this.ResetPlayers();
                            return;
                        }
                    }

                    if (words[0] == "/stats" && player.canbemod)
                    {
                        player.SendMessage(Message.Create("write", "SYSTEM",
                            "\ncoinscollected: " + this.coinscollected + "\nmagiccollected: " + this.magiccollected +
                            "\ncoinbanned: " + this.baseworld.coinbanned + "\nwootupbanned: " +
                            this.baseworld.wootupbanned));
                        return;
                    }

                    if (words[0] == "/bancoins" && player.canbemod)
                    {
                        if (this.owned)
                        {
                            this.magiccollected = 0;
                            this.baseworld.coinbanned = true;
                            this.baseworld.save(false,
                                delegate
                                {
                                    player.SendMessage(Message.Create("write", "SYSTEM",
                                        "World can no longer award coins or magic"));
                                });
                        }
                        return;
                    }

                    if (words[0] == "/banwootups" && player.canbemod)
                    {
                        if (this.owned)
                        {
                            this.baseworld.wootupbanned = true;
                            this.baseworld.save(false,
                                delegate
                                {
                                    player.SendMessage(Message.Create("write", "SYSTEM",
                                        "World can no longer recieve wootups"));
                                });
                        }
                        return;
                    }

                    if (words[0] == "/reset")
                    {
                        if (this.owned)
                        {
                            this.ResetPlayers();
                            return;
                        }
                    }

                    if (words[0] == "/respawnall")
                    {
                        if (this.owned)
                        {
                            this.cmds["/respawnall"].resolve(player, words);
                            return;
                        }
                    }

                    if (words[0] == "/clearandsave")
                    {
                        if (this.owned && player.canbemod)
                        {
                            //clearAndClose(player/*, m*/);
                            this.cmds["/clearandsave"].resolve(player, words);
                            return;
                        }
                    }

                    if (words[0] == "/banuser" && player.canbemod)
                    {
                        /*
                        banUser(player, m);
                        */
                        this.cmds["/banuser"].resolve(player, words);
                        return;
                    }

                    if (words[0] == "/copylevel" && player.canbemod)
                    {
                        //copyLevel(player);
                        this.cmds["/copylevel"].resolve(player, words);
                        return;
                    }

                    if (words[0] == "/visible" && words.Length >= 2 /*&& player.canbemod*/)
                    {
                        this.cmds["/visible"].resolve(player, words);
                        return;
                    }

                    if ((words[0] == "/bgcolor" || words[0] == "/bgcolour") && words.Length >= 2)
                    {
                        this.cmds["/bgcolor"].resolve(player, words);
                        return;
                    }


                    if ((words[0] == "/giveedit" || words[0] == "/removeedit") && words.Length >= 2)
                    {
                        this.cmds["/giveedit"].resolve(player, words);
                        return;
                    }

                    //Kick from world
                    if (words[0] == "/kick" && words.Length >= 2)
                    {
                        this.cmds["/kick"].resolve(player, words);
                        return;
                    }

                    /*
                    Handle the "/kill [playername]" command, this kills the player with the specified playername
                    */
                    if (words[0] == "/kill")
                    {
                        this.cmds["/kill"].resolve(player, words);
                        return;
                    }
                    /*
                    Handle the "/killemall" command, this kills all players except mods
                    */
                    if (words[0] == "/killemall")
                    {
                        this.cmds["/killemall"].resolve(player, words);
                        return;
                    }

                    /*
                    Handle "teleport" command
                        Usage: "/teleport #playername# #x# #y#
                        Arguments is a playername and optional x and y coordinates. If x and y are ommitted playername is teleported to the command issuer
                        Coordinates are specified in tiles, not pixels
                    */
                    if (words[0] == "/teleport" || words[0] == "/setpos")
                    {
                        this.cmds["/teleport"].resolve(player, words);
                        return;
                    }

                    /*
                   Handle /getpos #playername#, get position in tiles of playername, if playername is omitted the owners postion is returned. The position is in tiles
                   */
                    if (words[0] == "/getpos")
                    {
                        this.cmds["/getpos"].resolve(player, words);
                        return;
                    }

                    /*
                     * Handle /potionon and /potionoff
                     */
                    if (words[0] == "/potionson" || words[0] == "/potionsoff")
                    {
                        this.cmds["/potionson"].resolve(player, words);
                        return;
                    }

                    if (words[0] == "/listportals")
                    {
                        this.cmds["/listportals"].resolve(player, words);
                        return;
                    }

                    if (words[0] == "/help")
                    {
                        this.cmds["/help"].resolve(player, words);
                        return;
                    }


                    // Command 
                    player.SendMessage(Message.Create("info", "System Message", "Unknown command"));
                }
                else
                {
                    player.SendMessage(Message.Create("info", "System Message",
                        "You do not have command access in this room!"));
                }
                return;
            }

            //if (player.canchat){


            if (text.Trim() == "") return;

            if (!this.checkforSpam(text, player)) return;

            var friendsonline = false;
            var modonline = false;
            var guardianonline = false;


            // Handle all other players chat roles
            foreach (var p in this.Players)
            {
                var isFriend = p.hasFriend(player.ConnectUserId);
                friendsonline = friendsonline || isFriend;
                modonline = modonline || p.canbemod;
                guardianonline = guardianonline || p.CanBeGuardian;
                if (p.Id != player.Id)
                {
                    if ((player.canchat && p.canchat) || isFriend || p.canbemod || player.canbemod || p.CanBeGuardian ||
                        player.CanBeGuardian)
                    {
                        // Check if player is in muted list of p, if he is we dont want to send the message
                        if (!p.mutedUsers.Contains(player.ConnectUserId))
                        {
                            p.SendMessage(Message.Create("say", player.Id, text, isFriend));
                        }
                    }
                    else if (!p.canchat)
                    {
                        // The empty string is sent so that the UI can display a small bubble indicating that the user says something, but not what he is saying
                        p.SendMessage(Message.Create("say", player.Id, "", false));
                    }
                }
            }

            // Handle current players chat roles
            if (player.isguest)
            {
                player.SendMessage(Message.Create("info", "Sorry, you can not chat!",
                    "Guests are not verified for chatting."));
            }
            else if (!player.canchat && !friendsonline && !modonline && !guardianonline)
            {
                player.SendMessage(Message.Create("info", "Sorry, this account is not verified for chatting."));
            }
            else
            {
                player.SendMessage(Message.Create("say", player.Id, text, false));

                // Store the chat messages to serve a newly logged in player the previous messages 
                for (var a = 0; a < this.chatmessages.Length - 1; a++)
                {
                    this.chatmessages[a] = this.chatmessages[a + 1]; // shift messages one to the left
                }
                // add the latest chat message
                this.chatmessages[this.chatmessages.Length - 1] = new ChatMessage(player.name, text, player.canchat,
                    player.ConnectUserId);
            }
        }

        private Boolean checkforSpam(String text, Player player)
        {
            var repeatCount = player.RepeatChatCount(text);
            if (repeatCount > 3)
            {
                player.SendMessage(Message.Create("write", "SYSTEM",
                    "Final Warning. You have said the same thing " + repeatCount + " times."));
                player.chatCoolDown = 60000;
                return false;
            }

            if (DateTime.Now.Subtract(player.lastChat).TotalMilliseconds < player.chatCoolDown)
            {
                player.SendMessage(Message.Create("write", "SYSTEM",
                    "You are trying to chat way to fast, spamming the chat room is not nice!"));
                return false;
            }

            player.lastChat = DateTime.Now;
            return true;
        }

        private void CreateChatCommands()
        {
            this.cmds["/teleport"] = new SetPosCommand(this);
            this.cmds["/killemall"] = new KillEmAllCommand(this);
            this.cmds["/help"] = new HelpCommand(this);
            this.cmds["/kill"] = new KillCommand(this);
            this.cmds["/getpos"] = new GetPosCommand(this);
            this.cmds["/potionson"] = new PotionsOnOffCommand(this);
            this.cmds["/kick"] = new KickCommand(this);
            this.cmds["/giveedit"] = new EditToggleCommand(this);
            this.cmds["/visible"] = new VisibilityCommand(this);
            this.cmds["/copylevel"] = new CopyWorldCommand(this);
            this.cmds["/banuser"] = new BanUserCommand(this);
            this.cmds["/clearandsave"] = new ClearAndSaveCommand(this);
            this.cmds["/respawnall"] = new RespawnAllPlayersCommand(this);
            this.cmds["/listportals"] = new ListPortalsCommand(this);
            this.cmds["/report"] = new ReportCommand(this);
            this.cmds["/kickguests"] = new KickGuestsCommand(this);
            this.cmds["/mute"] = new MuteCommand(this);
            this.cmds["/unmute"] = new UnMuteCommand(this);
            this.cmds["/tempban"] = new TempBan(this);
            this.cmds["/checkip"] = new CheckIPCommand(this);
            this.cmds["/bgcolor"] = new BgColorCommand(this);
            this.cmds["/pm"] = new PrivateMessageCommand(this);
        }

        public DatabaseObject GetLocalCopyOfReport(string offender)
        {
            foreach (var report in this.reports)
            {
                if (report.GetString("ReportedUsername", "") == offender)
                {
                    return report;
                }
            }
            return null;
        }

        public void SaveLocalCopyOfReport(DatabaseObject report)
        {
            this.reports.Add(report);
            if (this.reports.Count > 50)
            {
                this.reports.RemoveAt(0);
            }
        }

        /*
                protected void banUser(Player player, Message m) {
                    if (!player.canbemod) return;

                    string text = m.GetString(0);
                    string[] words = text.ToLower().Split(' ');


                    if (words.Length < 2) {
                        player.Send("write", "SYSTEM", "You must define a user to ban");
                    }


                    PlayerIO.BigDB.Load("usernames", words[1], delegate(DatabaseObject o) {
                        if (o == null || o.GetString("owner", null) == null) {
                            player.Send("write", "SYSTEM", "User " + words[1] + " not found");
                            return;
                        }

                        PlayerIO.BigDB.Load("PlayerObjects", o.GetString("owner", "waggag"), delegate(DatabaseObject user) {
                            if (o == null) {
                                player.Send("write", "SYSTEM", "Crap, something went horriably wrong!... tell chris!");
                                return;
                            }

                            if (user.GetBool("isModerator", false)) {
                                player.Send("write", "SYSTEM", "Dude, stop that!");
                                return;
                            } else {
                                user.Set("banned", true);
                                user.Set("ban_reason", "Banned from chat by " + player.name + " [" + text + "]");
                                user.Save(delegate() {
                                    player.Send("write", "SYSTEM", "Player " + words[1] + " has been banned!");
                                });
                            }
                        });

                    });



                }
         */

        public void copyLevel(Player player)
        {
            this.getUniqueId(this.isbetalevel, player,
                delegate (string key)
                {
                    this.PlayerIO.BigDB.CreateObject("Worlds", key, this.baseworld.getDatabaseObject(),
                        delegate (DatabaseObject o)
                        {
                            player.SendMessage(Message.Create("write", "SYSTEM", "Room has been saved as: " + o.Key));
                        });
                });
        }

        protected void BroadcastMetaData(Player avoid = null)
        {
            foreach (var p in this.Players)
            {
                if (avoid == null || p != avoid)
                {
                    p.SendMessage(Message.Create("updatemeta", this.levelOwnerName, this.RoomData["name"],
                        this.baseworld.plays, this.getPrettyWoot(), this.baseworld.totalwoots));
                }
            }
            //Broadcast("updatemeta", levelOwnerName, removeBadChars(RoomData["name"]), baseworld.plays, getPrettyWoot());
        }

        // Method to even out the woot updates, so there isn't a big loss in woots every hour.
        protected int getPrettyWoot()
        {
            // progress between bucket updates.
            var progress = ((this.getTime() - this.worldWootCheckTime) / (Config.worldwoot_bucket_time * 1000));
            var difference = this.baseworld.woots - this.worldWootLatest;
            return Convert.ToInt32(this.worldWootLatest + (progress * difference));
        }

        // Copied from Lobby
        private void getUniqueId(Boolean isbetaonly, Player player, Callback<string> myCallback)
        {
            var newid = "";
            if (isbetaonly)
            {
                newid = "BW" +
                        Convert.ToBase64String(
                            BitConverter.GetBytes((DateTime.Now - new DateTime(1981, 3, 25)).TotalMilliseconds))
                            .Replace("=", "")
                            .Replace("+", "_")
                            .Replace("/", "-");
            }
            else
            {
                newid = "PW" +
                        Convert.ToBase64String(
                            BitConverter.GetBytes((DateTime.Now - new DateTime(1981, 3, 25)).TotalMilliseconds))
                            .Replace("=", "")
                            .Replace("+", "_")
                            .Replace("/", "-");
            }

            this.PlayerIO.BigDB.Load("Worlds", newid, delegate (DatabaseObject o)
            {
                if (o != null)
                {
                    this.getUniqueId(isbetaonly, player, myCallback);
                }
                else
                {
                    myCallback(newid);
                }
            });
        }

        /*
        protected void clearAndClose(Player player, Message m) {
            if (player.canbemod) {
                baseworld.reset();
                Broadcast("clear", baseworld.width, baseworld.height);

                baseworld.save(true, delegate {
                    player.Send("write", "SYSTEM", "Room has been reset and saved");
                });

            }
        }
        */

        protected virtual bool canEdit(Player player, Message m)
        {
            return player.canEdit;
        }

        public void setEditRigths(Player p, bool editrights)
        {
            p.canEdit = editrights;

            if (!editrights && p.isgod)
            {
                p.isgod = false;
                this.Broadcast("god", p.Id, false);
            }
            p.SendMessage(Message.Create(editrights ? "access" : "lostaccess"));
        }

        private bool isBrickAllowed(uint id, int layer)
        {
            // General bounds check on allowed brick id
            // Checking if layer is correct
            if ( /*id < 0 &&*/id > 0 && layer != this.blockMap.getBlockLayerById((int)id))
            {
                return false;
            }
            /*
            if (layer == 0) {
                //Forground
                if ((id < 0 || id > 374) && id != 1000) { 
                    return false; 
                }

                //Background
                if (id > 207 && id < 218) { 
                    return false; 
                }
            } else {
                //Decorations
                if ((id < 500 || id > 630) && id != 0)
                {
                    return false;
                }
            }
            */
            return true;
        }

        private void placeBrick(Player player, Message m)
        {
            if (!this.canEdit(player, m)) return;

            if ((DateTime.Now - player.lastEdit).TotalMilliseconds < 15)
            {
                player.threshold -= 10;
            }
            else if (player.threshold < 250)
            {
                player.threshold += 25;
            }

            if (player.threshold < 0)
            {
                player.threshold = 0;
                return;
            }

            player.lastEdit = DateTime.Now;

            int layerNum;
            uint cx;
            uint cy;
            uint brick;

            try
            {
                layerNum = m.GetInt(0);
                cx = (uint)m.GetInt(1);
                cy = (uint)m.GetInt(2);
                brick = (uint)m.GetInt(3);
            }
            catch (Exception e)
            {
                this.PlayerIO.ErrorLog.WriteError("Unable to deserialize world info from client", e);
                player.Disconnect();
                return;
            }
            // General bounds check on allowed brick id
            // For open worlds, check if block is part of Open Worlds block subset
            if (!this.lockedroom && !this.blockMap.blockIsLegitOpenWorld(player, (int)brick))
            {
                return;
            }
            /*
            Console.WriteLine("<------>");
            Console.WriteLine("-> Game.cs, placeBrick, id: " + brick + ", layerNum: " + layerNum);
            Console.WriteLine("isBrickAllowed(brick, layerNum) = " + isBrickAllowed(brick, layerNum));
            Console.WriteLine("<------>");
            */
            if (!this.isBrickAllowed(brick, layerNum))
                return;

            var margin = 1;

            if (player.canbemod)
            {
                margin = 0;
            }

            if (brick == 44)
            {
                margin = 0;
            }

            if (brick > 8 && brick < 16)
            {
                margin = 0;
            }

            if (brick == 182)
            {
                margin = 0;
            }

            if (cx >= margin && cx <= this.baseworld.width - (1 + margin) && cy <= this.baseworld.height - (1 + margin) &&
                (this.editkey != "" ? cy >= (margin) : cy >= 5))
            {
                switch (brick)
                {
                    case (int)ItemTypes.Piano:
                    {
                        if (this.owned && player.hasBrickPack("bricknode"))
                        {
                            this.setBrickSound(ItemTypes.Piano, cx, cy, (uint)m.GetInt(4), (uint)player.Id);
                        }
                        break;
                    }
                    case (int)ItemTypes.Drums:
                    {
                        if (this.owned && player.hasBrickPack("brickdrums"))
                        {
                            this.setBrickSound(ItemTypes.Drums, cx, cy, (uint)m.GetInt(4), (uint)player.Id);
                        }
                        break;
                    }
                    //Admin label
                    case 1000:
                    {
                        if (this.owned && player.canbemod)
                        {
                            this.setBrickLabel(cx, cy, m.GetString(4), m.GetString(5), (uint)player.Id);
                        }
                        break;
                    }


                    case 255:
                    {
                        // Spawn point
                        if (this.owned && (player.owner || player.canbemod) &&
                            player.getBrickPackCount("brickspawn") > this.baseworld.spawnCount())
                        {
                            setBrick(layerNum, cx, cy, brick, (uint)player.Id);
                        }
                        break;
                    }

                    case (int)ItemTypes.Checkpoint:
                    {
                        // Spawn point
                        if (this.owned && (player.owner || player.canbemod) &&
                            player.getBrickPackCount("brickcheckpoint") > this.baseworld.checkpointCount())
                        {
                            setBrick(layerNum, cx, cy, brick, (uint)player.Id);
                        }
                        break;
                    }
                    case (int)ItemTypes.Fire:
                    {
                        if (this.owned && (player.owner || player.canbemod) &&
                            player.getBrickPackCount("brickfire") * 10 > this.baseworld.fireHazardCount())
                        {
                            setBrick(layerNum, cx, cy, brick, (uint)player.Id);
                        }
                        break;
                    }
                    case (int)ItemTypes.Spike:
                    {
                        if (this.owned && (player.owner || player.canbemod) &&
                            player.getBrickPackCount("brickspike") * 10 > this.baseworld.spikesCount() ||
                            this.baseworld.getBrickType(0, (int)cx, (int)cy) == (int)ItemTypes.Spike)
                        {
                            var rotation = (uint)m.GetInt(4);
                            if (rotation >= 4) rotation -= 4;
                            if (rotation < 0) rotation += 4;
                            this.setBrickRotateable(layerNum, cx, cy, brick, rotation, (uint)player.Id);
                        }
                        break;
                    }

                    case (int)ItemTypes.GlowyLineBlueStraight:
                    case (int)ItemTypes.GlowyLineBlueSlope:
                    case (int)ItemTypes.GlowyLineGreenSlope:
                    case (int)ItemTypes.GlowyLineGreenStraight:
                    case (int)ItemTypes.GlowyLineYellowSlope:
                    case (int)ItemTypes.GlowyLineYellowStraight:
                    {
                        if (player.hasBrickPack("brickscifi2013"))
                        {
                            var rotation = (uint)m.GetInt(4);
                            if (rotation >= 4) rotation -= 4;
                            if (rotation < 0) rotation += 4;
                            this.setBrickRotateable(layerNum, cx, cy, brick, rotation, (uint)player.Id);
                        }
                        break;
                    }
                    case (int)ItemTypes.OnewayCyan:
                    case (int)ItemTypes.OnewayRed:
                    case (int)ItemTypes.OnewayYellow:
                    case (int)ItemTypes.OnewayPink:
                    {
                        if (player.hasBrickPack("brickoneway"))
                        {
                            var rotation = (uint)m.GetInt(4);
                            if (rotation >= 4) rotation -= 4;
                            if (rotation < 0) rotation += 4;
                            this.setBrickRotateable(layerNum, cx, cy, brick, rotation, (uint)player.Id);
                        }
                        break;
                    }
                    case (int)ItemTypes.Portal:
                    {
                        // Portal
                        if (this.owned && (player.owner || player.canbemod) &&
                            (player.getBrickPackCount("brickportal") + 1 > this.baseworld.portalCount() ||
                             this.baseworld.getBrickType(0, (int)cx, (int)cy) == (int)ItemTypes.Portal))
                        {
                            var rotation = (uint)m.GetInt(4);
                            var id = (uint)m.GetInt(5);
                            var target = (uint)m.GetInt(6);
                            if (rotation >= 4) rotation -= 4;
                            if (rotation < 0) rotation += 4;
                            this.setBrick(cx, cy, brick, rotation, id, target, (uint)player.Id);
                        }
                        break;
                    }

                    case (int)ItemTypes.WorldPortal:
                    {
                        if (this.owned && (player.owner || player.canbemod) &&
                            (player.getBrickPackCount("brickworldportal") > this.baseworld.worldportalsCount() ||
                             this.baseworld.getBrickType(0, (int)cx, (int)cy) == (int)ItemTypes.WorldPortal))
                        {
                            var target = m.GetString(4);
                            this.setBrickWorldPortal(cx, cy, brick, target, (uint)player.Id);
                        }
                        break;
                    }

                    case (int)ItemTypes.TextSign:
                    {
                        Console.WriteLine(">> TextSign added, player has: " + player.getBrickPackCount("bricksign") +
                                          " bricksign items in vault. Total added in world: " +
                                          this.baseworld.textsignsCount());
                        if (this.owned && (player.owner || player.canbemod) &&
                            (player.getBrickPackCount("bricksign") > this.baseworld.textsignsCount() ||
                             this.baseworld.getBrickType(0, (int)cx, (int)cy) == (int)ItemTypes.TextSign))
                        // if ( (owned && /*player.isClubMember*/ (player.owner || player.canbemod) && (player.getBrickPackCount("bricksign") > baseworld.textsignsCount()) // TODO: Make text sign purchable
                        {
                            var signText = this.removeBadChars(m.GetString(4));
                            this.setBrickTextSign(cx, cy, signText, (uint)player.Id);
                        }
                        break;
                    }

                    case (int)ItemTypes.PortalInvisible:
                    {
                        if (this.owned && (player.owner || player.canbemod) &&
                            (player.getBrickPackCount("brickinvisibleportal") + 1 >
                             this.baseworld.invisibleportalsCount() ||
                             this.baseworld.getBrickType(0, (int)cx, (int)cy) == (int)ItemTypes.PortalInvisible))
                        {
                            var rotation = (uint)m.GetInt(4);
                            var id = (uint)m.GetInt(5);
                            var target = (uint)m.GetInt(6);

                            if (rotation >= 4) rotation -= 4;
                            if (rotation < 0) rotation += 4;

                            this.setBrick(cx, cy, brick, rotation, id, target, (uint)player.Id);
                        }
                        break;
                    }

                    case 241:
                    {
                        if (this.owned && (player.owner || player.canbemod) &&
                            player.getBrickPackCount("brickdiamond") > this.baseworld.diamondCount())
                        {
                            setBrick(layerNum, cx, cy, brick, (uint)player.Id);
                        }
                        break;
                    }

                    case (int)ItemTypes.Cake:
                    {
                        if (this.owned && (player.owner || player.canbemod) &&
                            player.getBrickPackCount("brickcake") > this.baseworld.cakesCount())
                        {
                            setBrick(layerNum, cx, cy, brick, (uint)player.Id);
                        }

                        break;
                    }

                    case (int)ItemTypes.Hologram:
                    {
                        if (this.owned && (player.owner || player.canbemod) &&
                            player.getBrickPackCount("brickhologram") > this.baseworld.hologramsCount())
                        {
                            setBrick(layerNum, cx, cy, brick, (uint)player.Id);
                        }

                        break;
                    }

                    case 243:
                    {
                        if (this.owned && (player.owner || player.canbemod) && player.hasBrickPack("bricksecret"))
                        {
                            setBrick(layerNum, cx, cy, brick, (uint)player.Id);
                        }
                        break;
                    }

                    case (uint)ItemTypes.CoinDoor:
                    {
                        var count = (uint)m.GetInt(4);
                        if (count > 99)
                            count = 99;
                        if (this.owned && ((player.owner || player.canbemod)) &&
                            player.getBrickPackCount("brickcoindoor") * 10 > this.baseworld.coindoorCount())
                        {
                            setBrick(cx, cy, brick, count, (uint)player.Id);
                        }
                        break;
                    }
                    case (uint)ItemTypes.BlueCoinDoor:
                    {
                        var count = (uint)m.GetInt(4);
                        if (count > 99)
                            count = 99;
                        if (this.owned && ((player.owner || player.canbemod)) &&
                            player.getBrickPackCount("brickbluecoindoor") * 10 > this.baseworld.bluecoindoorCount())
                        {
                            setBrick(cx, cy, brick, count, (uint)player.Id);
                        }
                        break;
                    }

                    case (uint)ItemTypes.CoinGate:
                    {
                        var count = (uint)m.GetInt(4);
                        if (count > 99)
                            count = 99;
                        if (this.owned && ((player.owner || player.canbemod)) &&
                            player.getBrickPackCount("brickcoingate") * 10 > this.baseworld.coingateCount())
                        {
                            setBrick(cx, cy, brick, count, (uint)player.Id);
                        }
                        break;
                    }
                    case (uint)ItemTypes.BlueCoinGate:
                    {
                        var count = (uint)m.GetInt(4);
                        if (count > 99)
                            count = 99;
                        if (this.owned && ((player.owner || player.canbemod)) &&
                            player.getBrickPackCount("brickbluecoingate") * 10 > this.baseworld.bluecoingateCount())
                        {
                            setBrick(cx, cy, brick, count, (uint)player.Id);
                        }
                        break;
                    }

                    case (uint)ItemTypes.DeathGate:
                    case (uint)ItemTypes.DeathDoor:
                    {
                        var count = (uint)m.GetInt(4);
                        if (count > 99)
                            count = 99;
                        if (this.owned && ((player.owner || player.canbemod)) &&
                            player.getBrickPackCount("brickdeathdoor") * 10 > this.baseworld.deathDoorsGatesCount())
                        {
                            setBrick(cx, cy, brick, count, (uint)player.Id);
                        }
                        break;
                    }


                    case (uint)ItemTypes.Complete:
                    {
                        if (this.owned && (player.owner || player.canbemod) && player.hasBrickPack("brickcomplete") &&
                            (this.baseworld.completeCount() < 1 ||
                             this.baseworld.getBrickType(0, (int)cx, (int)cy) == (int)ItemTypes.Complete))
                        {
                            setBrick(layerNum, cx, cy, brick, (uint)player.Id);
                        }
                        break;
                    }
                    case (uint)ItemTypes.TimeGate:
                    case (uint)ItemTypes.TimeDoor:
                    {
                        if (this.owned && ((player.owner || player.canbemod)) &&
                            player.getBrickPackCount("bricktimeddoor") * 10 > this.baseworld.timedoorCount() ||
                            ((this.baseworld.getBrickType(0, (int)cx, (int)cy) == (int)ItemTypes.TimeGate) ||
                             (this.baseworld.getBrickType(0, (int)cx, (int)cy) == (int)ItemTypes.TimeDoor)))
                        {
                            setBrick(layerNum, cx, cy, brick, (uint)player.Id);
                        }
                        break;
                    }
                    case (uint)ItemTypes.ZombieGate:
                    case (uint)ItemTypes.ZombieDoor:
                    {
                        if (this.owned && ((player.owner || player.canbemod)) &&
                            player.getBrickPackCount("brickzombiedoor") * 10 > this.baseworld.zombiedoorCount() ||
                            ((this.baseworld.getBrickType(0, (int)cx, (int)cy) == (int)ItemTypes.ZombieGate) ||
                             (this.baseworld.getBrickType(0, (int)cx, (int)cy) == (int)ItemTypes.ZombieDoor)))
                        {
                            setBrick(layerNum, cx, cy, brick, (uint)player.Id);
                        }
                        break;
                    }
                    case (uint)ItemTypes.SwitchPurple:
                    {
                        var count = (uint)m.GetInt(4);
                        if (count > 99)
                            count = 99;
                        if (this.owned && ((player.owner || player.canbemod)) &&
                            player.hasBrickPack("brickswitchpurple"))
                        {
                            setBrick(cx, cy, brick, count, (uint)player.Id);
                        }
                        break;
                    }
                    case (uint)ItemTypes.GatePurple:
                    case (uint)ItemTypes.DoorPurple:
                    {
                        var count = (uint)m.GetInt(4);
                        if (count > 99)
                            count = 99;
                        if (this.owned && ((player.owner || player.canbemod)) &&
                            player.getBrickPackCount("brickswitchpurple") * 10 > this.baseworld.purpleDoorGateCount() ||
                            ((this.baseworld.getBrickType(0, (int)cx, (int)cy) == (int)ItemTypes.DoorPurple) ||
                             (this.baseworld.getBrickType(0, (int)cx, (int)cy) == (int)ItemTypes.GatePurple)))
                        {
                            setBrick(cx, cy, brick, count, (uint)player.Id);
                        }
                        break;
                    }
                    //case (uint)ItemTypes.GateClub:
                    //case (uint)ItemTypes.DoorClub: {
                    //        if (owned && (player.owner || player.canbemod) && player.isClubMember) {
                    //            setBrick(layerNum, cx, cy, brick);
                    //        }
                    //        break;
                    //    }

                    default:
                    {
                        if (this.blockMap.blockIsLegit(player, (int)brick))
                        {
                            //setBrick(layerNum, cx, cy, brick);
                            setBrick(layerNum, cx, cy, brick, (uint)player.Id);
                        }

                        break;
                    }
                }
            }
        }

        private void setBrick(int layerNum, uint x, uint y, uint brick, uint playerid)
        {
            if (this.baseworld.setBrick(layerNum, x, y, brick))
            {
                this.BroadcastMessage(Message.Create("b", layerNum, x, y, brick, playerid));
            }
        }

        //Coin doors/gates
        private void setBrick(uint x, uint y, uint brick, uint goal, uint playerid)
        {
            if (brick == (uint)ItemTypes.CoinDoor && this.baseworld.setBrickCoindoor(x, y, goal, true))
            {
                this.BroadcastMessage(Message.Create("bc", x, y, brick, goal, playerid));
            }
            if (brick == (uint)ItemTypes.CoinGate && this.baseworld.setBrickCoingate(x, y, goal, true))
            {
                this.BroadcastMessage(Message.Create("bc", x, y, brick, goal, playerid));
            }
            if (brick == (uint)ItemTypes.BlueCoinDoor && this.baseworld.setBrickBlueCoindoor(x, y, goal, true))
            {
                this.BroadcastMessage(Message.Create("bc", x, y, brick, goal, playerid));
            }
            if (brick == (uint)ItemTypes.BlueCoinGate && this.baseworld.setBrickBlueCoingate(x, y, goal, true))
            {
                this.BroadcastMessage(Message.Create("bc", x, y, brick, goal, playerid));
            }

            // Death doors and gates
            if (brick == (uint)ItemTypes.DeathDoor && this.baseworld.setBrickDeathDoor(x, y, goal, true))
            {
                this.BroadcastMessage(Message.Create("bc", x, y, brick, goal, playerid));
            }
            if (brick == (uint)ItemTypes.DeathGate && this.baseworld.setBrickDeathGate(x, y, goal, true))
            {
                this.BroadcastMessage(Message.Create("bc", x, y, brick, goal, playerid));
            }

            // Purple switches with ids
            if (brick == (uint)ItemTypes.DoorPurple && this.baseworld.setBrickDoorPurple(x, y, goal, true))
            {
                this.BroadcastMessage(Message.Create("bc", x, y, brick, goal, playerid));
            }
            if (brick == (uint)ItemTypes.GatePurple && this.baseworld.setBrickGatePurple(x, y, goal, true))
            {
                this.BroadcastMessage(Message.Create("bc", x, y, brick, goal, playerid));
            }
            if (brick == (uint)ItemTypes.SwitchPurple && this.baseworld.setBrickSwitchPurple(x, y, goal, true))
            {
                this.BroadcastMessage(Message.Create("bc", x, y, brick, goal, playerid));
            }
        }

        private void setBrickSound(ItemTypes type, uint x, uint y, uint offset, uint playerid)
        {
            if (this.baseworld.setBrickSound(type, x, y, offset))
            {
                this.BroadcastMessage(Message.Create("bs", x, y, (uint)type, offset, playerid));
            }
        }

        private void setBrickLabel(uint x, uint y, string text, string color, uint playerid)
        {
            if (this.baseworld.setBrickLabel(x, y, text, color))
            {
                this.BroadcastMessage(Message.Create("lb", x, y, 1000, text, color, playerid));
            }
        }

        private void setBrickTextSign(uint x, uint y, string text, uint playerid)
        {
            var str = this.removeBadChars(text);
            if (str.Trim() == "")
            {
                return;
            }
            if (str.Length > 140)
            {
                str = str.Remove(140);
            }


            if (this.baseworld.setBrickTextSign(x, y, str, true))
            {
                this.BroadcastMessage(Message.Create("ts", x, y, (uint)ItemTypes.TextSign, str, playerid));
            }
        }

        private void setBrickWorldPortal(uint x, uint y, uint brick, string target, uint playerid)
        {
            if (this.baseworld.setBrickWorldPortal(x, y, target, true))
            {
                this.BroadcastMessage(Message.Create("wp", x, y, brick, target, playerid));
            }
        }


        // Portals
        private void setBrick(uint x, uint y, uint brick, uint rotation, uint id, uint target, uint playerid)
        {
            //bool wc = changeBrick(x,y,brick);
            //bool pt = baseworld.setPortalProperties(x,y,rotation, id, target);
            if (this.baseworld.setBrickPortal(brick, x, y, rotation, id, target, true))
            {
                this.BroadcastMessage(Message.Create("pt", x, y, brick, rotation, id, target, playerid));
            }
        }

        private void setBrickRotateable(int layerNum, uint x, uint y, uint brick, uint rotation, uint playerid)
        {
            if (this.baseworld.setBrickRotateable(layerNum, x, y, brick, rotation, true))
            {
                this.BroadcastMessage(Message.Create("br", x, y, brick, rotation, layerNum, playerid));
            }
        }

        protected void sendInitMessage(Player player)
        {
            if (player.disconnected) return;

            Console.WriteLine("sendInitMessage");
            var d = this.baseworld.getSpawn();
            player.x = d.x * 16;
            player.y = d.y * 16;
            //player.WootStatusObject.setWorldCoinCount(baseworld.coinCount());

            //if (player.ConnectUserId == this.baseworld.ownerid)
            //{
            player.canEdit = true;
            player.owner = true;
            //}

            var count = 0;
            foreach (var p in this.Players)
            {
                if (p.ConnectUserId == player.ConnectUserId && !player.isguest) count++;
            }

            //if (!this.Visible && !player.owner && !player.canbemod && !player.CanBeGuardian)
            //{
            //    player.SendMessage(Message.Create("info", "World not available",
            //        "The requested world is not set to visible."));
            //    player.Disconnect();
            //    return;
            //}

            //if (this.worldBanned)
            //{
            //    if (player.owner || player.canbemod || player.CanBeGuardian)
            //    {
            //        player.SendMessage(Message.Create("info", "World Banned",
            //            "This world is banned.\n" + this.worldBannedReason));
            //    }
            //    else
            //    {
            //        player.SendMessage(Message.Create("info", "World Banned", "The requested world is banned."));
            //        player.Disconnect();
            //    }
            //}

            //if (count >= 2)
            //{
            //    if (player.owner)
            //    {
            //        if (count >= 3)
            //        {
            //            player.SendMessage(Message.Create("info", "Limit reached",
            //                "To prevent abuse you can only be connected to your own world twice."));
            //            player.Disconnect();
            //            return;
            //        }
            //    }
            //    else
            //    {
            //        player.SendMessage(Message.Create("info", "Limit reached",
            //            "To prevent abuse you can only be connected to the same world once."));
            //        player.Disconnect();
            //        return;
            //    }
            //}

            // Tell all connected clients to add the new player			

            foreach (var p in this.Players)
            {
                if (p != player)
                {
                    var friend = p.hasFriend(player.ConnectUserId);
                    p.SendMessage(Message.Create("add", player.Id, player.name, player.face, player.x, player.y,
                        player.isgod, player.ismod, player.canchat, player.coins, player.bcoins, friend, false,
                        1 /* woot level */, player.isClubMember, player.isInGuardianMode));
                }
            }

            var roomname = "Untitled World";
            if (this.RoomData.ContainsKey("name"))
            {
                roomname = this.RoomData["name"];
            }

            //String plays = "";


            //if (!RoomData.TryGetValue("name", out roomname)) {
            //    roomname = "Untitled World";
            //}
            //if (!RoomData.TryGetValue("plays", out plays)) {
            //    plays = "0";
            //}			


            //if (roomname != "" && roomname != null) {
            //    roomname = removeBadChars(roomname);
            //} else roomname = "Untitled World";


            // Serialize world data and sendt it
            var initmessage = Message.Create("init",
                this.levelOwnerName == null ? "" : this.levelOwnerName,
                roomname,
                this.baseworld.plays,
                this.baseworld.woots,
                this.baseworld.totalwoots,
                this.Rot13(this.editchar),
                player.Id,
                player.x,
                player.y,
                player.name,
                player.canEdit,
                player.owner,
                this.baseworld.width,
                this.baseworld.height,
                this.isTutorialRoom,
                this.baseworld.gravityMultiplier,
                this.baseworld.allowPotions,
                this.baseworld.backgroundColor,
                this.baseworld.visible
                );


            this.baseworld.addToMessageAsComplexList(initmessage);
            ItemManager.AddPotionCountToMessage(player, initmessage);
            player.SendMessage(initmessage);

            this.broadcastPotionSettings(player);

            if (this.baseworld.allowPotions)
            {
                this.broadcastActivePotions(player);
            }

            this.checkAllOnlineStatus();

            this.CheckTempBanned(player);

            //Message doors = Message.Create("hide");
            //if (purple) doors.Add("purple");
            //player.Send(doors);
        }

        // adds a woot to the world
        protected void addWootToWorld()
        {
            lock (this.RoomData)
            {
                //TODO
                //ips.Add(player.IPAddress.ToString(), -1);

                this.worldWootLatest++;

                this.baseworld.addWoot();
                this.RoomData["woots"] = this.baseworld.woots.ToString();

                this.RoomData.Save();
            }
        }

        public void broadcastPotionSettings(Player player = null)
        {
            var msg = Message.Create("allowpotions");
            msg.Add(this.baseworld.allowPotions);
            var potions_disabled = this.baseworld.getPotionsEnabled(false);
            for (var i = 0; i < potions_disabled.Count; i++)
            {
                msg.Add(potions_disabled[i]);
            }
            if (player != null)
            {
                player.SendMessage(msg);
            }
            else
            {
                this.BroadcastMessage(msg);
            }
        }

        public void broadcastActivePotions(Player player)
        {
            var potions = player.getActivePotions();
            foreach (var potion in potions)
            {
                if (this.baseworld.isPotionEnabled(potion.id.ToString()))
                {
                    this.Broadcast("p", player.Id, potion.id, true, potion.timeleft);
                }
            }
        }

        // Check if players have expired potions or new potions (activated in other worlds)
        protected void checkPotions()
        {
            if (!this.isCheckingPotions)
            {
                this.isCheckingPotions = true;
                foreach (var p in this.Players)
                {
                    if ( /*!p.isguest &&*/ !p.disconnected && p.initialized)
                    {
                        // *zombieguests - remove !p.isguest here to process zombie potions for guests as well

                        //List<Potion> newpotions = p.getNewPotions();
                        //foreach (Potion potion in newpotions) {
                        //    Console.WriteLine(p.name + " NEW " + potion.name + ": " + potion.timeleft);
                        //    Broadcast("p", p.Id, potion.id, true);
                        //}

                        var all = p.getActivePotions();
                        foreach (var potion in all)
                        {
                            //Console.WriteLine(p.name + " has " + potion.name + ": " + potion.timeleft);
                            if (potion.expired)
                            {
                                p.removePotion(potion);
                                this.Broadcast("p", p.Id, potion.id, false, potion.timeleft);
                                Console.WriteLine(p.name + " expires " + potion.name);

                                if (potion.id == 6 && this.baseworld.isPotionEnabled(6))
                                {
                                    this.Broadcast("kill", p.Id);
                                    this.Broadcast("write", "* WORLD", p.name.ToUpper() + " was killed by a curse");
                                }

                                if (potion.id == 9 && this.baseworld.isPotionEnabled(9) && !p.isgod && !p.ismod)
                                {
                                    this.Broadcast("kill", p.Id);
                                }

                                if (potion.id == 10 && this.baseworld.isPotionEnabled(10) && !p.isgod && !p.ismod)
                                {
                                    this.Broadcast("kill", p.Id);
                                }
                            }
                        }
                    }
                }
                this.isCheckingPotions = false;
            }
        }

        // When a player uses a potion
        protected void handlePotion(Player player, Message message)
        {
            Console.WriteLine("Use potion: " + message.GetInt(0));

            var potionid = message.GetInt(0);
            if (!this.baseworld.isPotionEnabled(potionid.ToString()))
            {
                this.sendPotionCountUpdate(player);
                return;
            }

            var potion = ItemManager.GetPotion(potionid);
            if (potion != null && !player.hasActivePotion(potion))
            {
                var vault = player.PayVault;
                vault.Refresh(delegate
                {
                    var vaultpotion = vault.First(potion.payvatultid);
                    if (vaultpotion != null)
                    {
                        vault.Consume(new[] { vaultpotion }, delegate { this.activatePotion(player, potion); }, delegate
                          {
                              this.sendPotionCountUpdate(player);
                              player.SendMessage(Message.Create("info", "Potion Error",
                                  "There was an error using the potion."));
                              this.PlayerIO.ErrorLog.WriteError("Error consuming potion");
                          });
                    }
                });
            }
            else
            {
                this.sendPotionCountUpdate(player);
            }
        }

        protected void activatePotion(Player player, Potion potion)
        {
            player.activatePotion(potion, delegate (List<Potion> removed)
            {
                this.sendPotionCountUpdate(player);
                for (var i = 0; i < removed.Count; i++)
                {
                    this.Broadcast("p", player.Id, removed[i].id, false, potion.timeleft);
                }

                if (potion.id == 6)
                {
                    this.Broadcast("write", "* WORLD", player.name.ToUpper() + " drank a cursed potion!");
                }
                if (potion.id == 9)
                {
                    this.Broadcast("write", "* WORLD", player.name.ToUpper() + " drank a zombie potion!");
                }

                this.Broadcast("p", player.Id, potion.id, true, potion.timeleft);
            });
        }

        protected void sendPotionCountUpdate(Player player)
        {
            var potioncount = Message.Create("pc");
            ItemManager.AddPotionCountToMessage(player, potioncount);
            player.SendMessage(potioncount);
        }

        public bool onChangePotionSettings(Player player)
        {
            var since_switch = Convert.ToInt32((DateTime.Now - this.potionswitched).TotalSeconds);
            var canswitch = (since_switch > 30);
            if (!canswitch)
            {
                var timeleft = 30 - since_switch;
                player.SendMessage(Message.Create("write", this.systemName,
                    "Potion settings can be changed in " + timeleft + (timeleft == 1 ? " second" : " seconds") + "."));
            }
            else
            {
                this.potionswitched = DateTime.Now;
            }
            return canswitch;
        }

        protected string Rot13(string value)
        {
            var array = value.ToCharArray();
            for (var i = 0; i < array.Length; i++)
            {
                int number = array[i];

                if (number >= 'a' && number <= 'z')
                {
                    if (number > 'm')
                    {
                        number -= 13;
                    }
                    else
                    {
                        number += 13;
                    }
                }
                else if (number >= 'A' && number <= 'Z')
                {
                    if (number > 'M')
                    {
                        number -= 13;
                    }
                    else
                    {
                        number += 13;
                    }
                }
                array[i] = (char)number;
            }
            return new string(array);
        }

        protected double getTime()
        {
            return Math.Round((DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds);
        }

        public List<ChatMessage> GetLastChatMessages()
        {
            var chatmsg = new List<ChatMessage>();
            foreach (var message in this.chatmessages)
            {
                if (message != null)
                {
                    chatmsg.Add(message);
                }
            }
            return chatmsg;
        }

        [DebugAction("Broadcast woot", DebugAction.Icon.Add)]
        public void senddebugWoot()
        {
            foreach (var p in this.Players)
            {
                this.BroadcastMessage(Message.Create("w", p.Id));
                break;
            }
        }

        [DebugAction("Broadcast wootup", DebugAction.Icon.Add)]
        public void senddebugWootup()
        {
            foreach (var p in this.Players)
            {
                this.BroadcastMessage(Message.Create("wu", p.Id));
                break;
            }
        }

        [DebugAction("Broadcast levelup", DebugAction.Icon.Add)]
        public void senddebugLevelup()
        {
            foreach (var p in this.Players)
            {
                this.BroadcastMessage(Message.Create("levelup", p.Id, 1 /* woot level */ + 1));
                break;
            }
        }

        [DebugAction("Add Bomb", DebugAction.Icon.Add)]
        public void senddebugBomb()
        {
            foreach (var p in this.Players)
            {
                var potion = ItemManager.GetPotion(6);
                p.activatePotion(potion, delegate { this.broadcastActivePotions(p); });
                Console.WriteLine("Activating bomb for: " + p.name);
                break;
            }
        }
    } // BaseGame
} // Namespace