using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using EverybodyEdits.Common;
using EverybodyEdits.Game.AntiCheat;
using EverybodyEdits.Game.Campaigns;
using EverybodyEdits.Game.Chat;
using EverybodyEdits.Game.Chat.Commands;
using EverybodyEdits.Game.Contest;
using EverybodyEdits.Game.CountWorld;
using EverybodyEdits.Game.Crews;
using EverybodyEdits.Lobby;
using PlayerIO.GameLibrary;

namespace EverybodyEdits.Game
{
    // *******************************************************************************************
    // Game
    // *******************************************************************************************

    [RoomType("Everybodyedits" + Config.VersionString)]
    public class EverybodyEdits : Game<Player>, IUpgradeRoom<Player>
    {
        public IEnumerable<Player> FilteredPlayers
        {
            get
            {
                return this.Players.Where(p => !p.Stealthy);
            }
        }

        private readonly Dictionary<string, int> ips = new Dictionary<string, int>();
        private readonly List<DatabaseObject> reports = new List<DatabaseObject>();

        private BasicAntiCheat antiCheat;
        private BlockMap blockMap;


        private Campaign campaign;
        public Crew Crew;
        private Player crewRequestSender;

        private ArtContest artContest;

        private bool die;
        private string editkey = "";
        private double effectCheckTime;
        private bool isbetalevel;
        private bool isCheckingEffects;

        private List<JoinBan> joinBans = new List<JoinBan>();
        private Keys keys;
        private bool lockedroom;
        private Magic magic;

        private double metaUpdateTime;

        private bool open;
        public readonly HashSet<int> OrangeSwitches = new HashSet<int>();

        private string pendingCrewRequest = "";
        private int pendingFavorites;
        private bool ready;
        private int sessionplays;
        private Shop shop;
        private SmileyMap smileyMap;
        private bool timedoor;
        private double timedoortime;
        private double upgradeWarningTime;
        private bool AllowVisibility = true;
        public Timer CheckIpBanTimer;

        public Random Random { get; private set; }

        public ChatHandler Chat { get; private set; }

        public bool Owned { get; private set; }

        public World BaseWorld { get; private set; }

        public string LevelOwnerName { get; private set; }

        private int _crownId;
        public int CrownId
        {
            get
            {
                return this._crownId;
            }
            set
            {
                this._crownId = value;
                this.BroadcastMessage("k", this._crownId);
            }
        }

        public bool IsCampaign
        {
            get { return this.BaseWorld.IsPartOfCampaign && this.campaign.Visible; }
        }

        public UpgradeChecker<Player> UpgradeChecker { get; private set; }

        public void BroadcastMessage(string type, params object[] parameters)
        {
            this.BroadcastMessage(Message.Create(type, parameters));
        }

        public void BroadcastMessage(Message message)
        {
            foreach (var player in this.Players)
            {
                player.SendMessage(message);
            }
        }

        [Obsolete("Use BroadcastMessage instead.")]
        public new void Broadcast(string type, params object[] parameters)
        {
            base.Broadcast(type, parameters);
        }

        [Obsolete("Use BroadcastMessage instead.")]
        public new void Broadcast(Message message)
        {
            base.Broadcast(message);
        }

        public override void GameStarted()
        {
            this.PreloadPayVaults = true;
            this.PreloadPlayerObjects = true;

            this.Random = new Random();
            this.CrownId = -1;

            this.BaseWorld = new World(this.PlayerIO);
            this.antiCheat = new BasicAntiCheat(this);
            this.magic = new Magic(this);
            this.keys = new Keys(this);
            this.Chat = new ChatHandler(this);
            this.campaign = new Campaign(this.PlayerIO);
            this.Crew = new Crew(this.PlayerIO);
            this.LevelOwnerName = "";

            this.UpgradeChecker = new UpgradeChecker<Player>(this);
            this.CheckIpBanTimer = this.AddTimer(this.CheckIpBans, 30000);

            this.isbetalevel = this.RoomId.Substring(0, 2) == "BW" || this.RoomId == "ChrisWorld";
            var isowned = this.isbetalevel || this.RoomId.Substring(0, 2) == "PW" || this.RoomId.Substring(0, 2) == "CW";
            this.open = this.RoomId.Substring(0, 2) == "OW";

            Dictionary<string, string> userRoomData = this.RoomData.ToDictionary(entry => entry.Key, entry => entry.Value);
            this.RoomData.Clear();
            if (this.isbetalevel)
            {
                this.RoomData["beta"] = "true";
            }

            this.Visible = false;

            if (isowned || userRoomData.ContainsKey("owned"))
            {
                this.Owned = true;
                this.lockedroom = true;
                this.editkey = Guid.NewGuid().ToString("N");
                this.RoomData["needskey"] = "yep";
                this.RoomData["plays"] = "0";
                this.RoomData["rating"] = "0";
                this.RoomData["name"] = "My World";
                this.RoomData["Favorites"] = "0";
                this.RoomData["Likes"] = "0";
                this.RoomData["description"] = "";

                this.LoadWorld(this.RoomId);
            }
            else
            {
                this.SetVisibility(true);
                if (userRoomData.ContainsKey("editkey"))
                {
                    this.lockedroom = true;
                    this.editkey = userRoomData["editkey"];
                }
                this.RoomData["openworld"] = "true";
                this.RoomData["plays"] = "0";
                this.RoomData["rating"] = "0";
                this.RoomData["Favorites"] = "0";
                this.RoomData["Likes"] = "0";
                this.RoomData["description"] = "";
                this.RoomData["name"] = (userRoomData.ContainsKey("name") && !string.IsNullOrWhiteSpace(userRoomData["name"]))
                    ? ChatUtils.RemoveBadCharacters(userRoomData["name"])
                    : "Open world";

                this.BaseWorld.Key = this.RoomId;
                this.BroadcastMetaData();
            }

            this.RoomData.Save();

            this.StartTimers();


            this.smileyMap = new SmileyMap();
            this.blockMap = new BlockMap();
        }


        private void StartTimers()
        {
            this.UpgradeChecker.CheckVersion();

            this.AddTimer(delegate
            {
                this.keys.Tick();

                if (this.GetTime() - this.timedoortime > 5000)
                {
                    this.timedoortime = this.GetTime();
                    this.timedoor = !this.timedoor;
                    this.BroadcastMessage(this.timedoor ? "show" : "hide", "timedoor");
                }

                foreach (var p in this.FilteredPlayers.Where(p => p.Cheat > 0))
                {
                    p.Cheat--;
                }

                if (this.die)
                {
                    foreach (var p in this.Players)
                    {
                        p.SendMessage("info", "Room killed.", "This room was killed.");
                        p.Disconnect();
                    }
                }

                if (!this.UpgradeChecker.Upgrade && this.UpgradeChecker.UpgradeWarning && !this.UpgradeChecker.SentWarning)
                {
                    this.upgradeWarningTime = this.GetTime();
                    foreach (var p in this.FilteredPlayers.Where(p => !p.IsGuest && p.Initialized))
                    {
                        this.UpgradeChecker.SendUpdateMessage(p);
                    }
                    this.UpgradeChecker.SentWarning = true;
                }

                if (!this.UpgradeChecker.Upgrade && this.UpgradeChecker.RepeatWarning &&
                    this.GetTime() - this.upgradeWarningTime > 60000)
                {
                    this.upgradeWarningTime = this.GetTime();
                    foreach (var p in this.FilteredPlayers.Where(p => !p.IsGuest && p.Initialized))
                    {
                        p.SendMessage(Message.Create("write", ChatUtils.SystemName,
                            "Everybody Edits is about to get updated. Please save your world now."));
                    }
                }


                if (this.GetTime() - this.effectCheckTime > 1000)
                {
                    this.effectCheckTime = this.GetTime();
                    this.CheckEffects();
                }

                if (this.GetTime() - this.metaUpdateTime > 10000)
                {
                    this.metaUpdateTime = this.GetTime();
                    this.BroadcastMetaData();
                }

                this.joinBans = this.joinBans.Where(b => (b.Timestamp - DateTime.Now).TotalMilliseconds > 0).ToList();
            }, 250);

            this.AddTimer(this.CheckAllOnlineStatus, 30000);
            this.AddTimer(() => this.NotifyWorld(), 30000);
            this.AddTimer(() => this.CheckAntiSpam(), 10000);
        }

        private void LoadWorld(string roomid)
        {
            Console.WriteLine("LoadWorld");

            var t = this.GetTime();

            this.PlayerIO.BigDB.LoadOrCreate("Worlds", roomid, delegate (DatabaseObject o)
            {
                this.BaseWorld.FromDatabaseObject(o);

                this.SetVisibility(this.BaseWorld.Visible && !this.BaseWorld.HideLobby);

                this.RoomData["plays"] = this.BaseWorld.Plays.ToString();
                this.RoomData["name"] = ChatUtils.RemoveBadCharacters(this.BaseWorld.Name);
                this.RoomData["Favorites"] = this.BaseWorld.Favorites.ToString();
                this.RoomData["Likes"] = this.BaseWorld.Likes.ToString();
                this.RoomData["IsFeatured"] = this.BaseWorld.IsFeatured.ToString();
                this.RoomData["IsCampaign"] = this.IsCampaign.ToString();
                this.RoomData["description"] = this.BaseWorld.WorldDescription;
                this.RoomData["LobbyPreviewEnabled"] = this.BaseWorld.LobbyPreviewEnabled.ToString();
                this.RoomData.Save();

                if (this.BaseWorld.OwnerId != "")
                {
                    Console.WriteLine("Loading campaign");
                    this.campaign.Load(this.BaseWorld.Campaign, () =>
                    {
                        this.RoomData["IsCampaign"] = this.IsCampaign.ToString();
                        this.RoomData.Save();

                        Console.WriteLine("Loading crew");
                        this.Crew.Load(this.BaseWorld.Crew, () =>
                        {
                            if (this.BaseWorld.IsPartOfCrew && this.Crew.DatabaseObject == null)
                            {
                                this.BaseWorld.Crew = "";
                                this.BaseWorld.Status = WorldStatus.NonCrew;
                            }
                            else if (!this.BaseWorld.CrewVisibleInLobby)
                            {
                                this.SetVisibility(false);
                            }

                            Console.WriteLine("loading po");
                            this.PlayerIO.BigDB.Load("PlayerObjects", this.BaseWorld.OwnerId,
                                delegate (DatabaseObject oo)
                                {
                                    Console.WriteLine("World is READY");

                                    if (oo == null)
                                    {
                                        Console.WriteLine("World owner PlayerObject is null");
                                        this.LevelOwnerName = "eemu";
                                    }
                                    else
                                    {
                                        //this.artContest = new ArtContest(this.PlayerIO, this.Crew.Id);
                                        this.LevelOwnerName = oo.GetString("name", "");
                                    }


                                    this.SetWorldReady();
                                });
                        });
                    });
                }
                else
                {
                    this.SetWorldReady();
                }

                Console.WriteLine("Loaded world, in" + (this.GetTime() - t));
                this.BroadcastMetaData();
            });
        }

        private void SetWorldReady()
        {
            Console.WriteLine("SetWorldReady 1");

            this.ready = true;
            if (this.pendingFavorites != 0)
            {
                this.AddFavoritesToWorld(this.pendingFavorites);
            }

            Console.WriteLine("SetWorldReady 2");

            foreach (var p in this.FilteredPlayers.Where(p => p.Ready))
            {
                this.SendInitMessage(p);
            }

            Console.WriteLine("SetWorldReady 3");
        }

        public void KickUser(Player player, int minutes)
        {
            Console.WriteLine("addBan " + player.ConnectUserId + " for " + minutes);
            this.joinBans.Add(new JoinBan(player, DateTime.Now.AddMinutes(minutes)));
        }
        public void KickUser(string userid, int minutes)
        {
            Console.WriteLine("addBan " + userid + " for " + minutes);
            this.joinBans.Add(new JoinBan(userid, DateTime.Now.AddMinutes(minutes)));
        }

        public bool ForgiveUser(string username)
        {
            Console.WriteLine("forgiven " + username);
            return this.joinBans.RemoveAll(p => p.Username == username) > 0;
        }

        private List<Player> GetPlayersWithIp(IPAddress ip)
        {
            return this.FilteredPlayers.Where(p => p.IPAddress.Equals(ip)).ToList();
        }

        private void CheckAllOnlineStatus()
        {
            foreach (var p in this.Players)
            {
                if (!p.IsGuest && !p.Disconnected && p.Initialized)
                {
                    this.CheckOnlineStatus(p);
                }

                if (!p.Initialized && (DateTime.UtcNow - p.JoinTime).TotalSeconds > 90)
                {
                    p.SendMessage(Message.Create("info", "Server error",
                        "Your connection to the world timed out. Please try again."));
                    p.Disconnect();
                }
            }
        }

        private void CheckOnlineStatus(Player p)
        {
            OnlineStatus.GetOnlineStatus(this.PlayerIO, p.ConnectUserId, delegate (OnlineStatus os)
            {
                if (os.CurrentWorldName != "" && os.CurrentWorldId != "" && os.CurrentWorldId != this.RoomId &&
                    os.IpAddress != p.IPAddress.ToString())
                {
                    this.PlayerIO.ErrorLog.WriteError("User present in multiple worlds. ",
                        p.Name + " is online in " + this.BaseWorld.Name + "(" + this.RoomId + ") and " +
                        os.CurrentWorldName + "(" + os.CurrentWorldId + "). IpAdress Ingame: " + p.IPAddress + " vs " +
                        os.IpAddress + ". Already disconnected? " + p.Disconnected, "Kicked User: " + p.Name, null);

                    p.Disconnected = true;
                    p.SendMessage(Message.Create("info", "Limit reached",
                        "To prevent abuse you can only be connected to one world at a time.\nYou are also logged into " +
                        os.CurrentWorldName + "."));

                    p.Disconnect();
                }
                else
                {
                    p.SaveOnlineStatus();
                }
            });
        }


        // Note to a future developer:
        // Message types sent here must be whitelisted in player.SendMessage(msg)
        // Otherwise these messages will be ignored
        public override bool AllowUserJoin(Player player)
        {
            Console.WriteLine("Users joining");

            if (player.PlayerObject.Contains("linkedTo"))
            {
                player.SendMessage("linked");
                return false;
            }
            // Allow only owned and open worlds
            if (!this.Owned && !this.open)
            {
                return false;
            }

            // To prevent people joining anonymously 
            if (!player.IsGuest && player.PlayerObject.GetString("name", "") == "")
            {
                return false;
            }

            if (player.IsBanned)
            {
                player.SendMessage("info", "You are banned", "This account is banned due to abuse or fraud.");
                player.SendMessage("banned");
                return false;
            }

            if (player.IsTempBanned)
            {
                player.SendMessage("info", "You are banned", "This account is temporarily banned due to abuse or fraud.");
                player.SendMessage("banned");
                return false;
            }

            if (this.editkey == "" ||
                (player.JoinData.ContainsKey("editkey") && player.JoinData["editkey"] == this.editkey))
            {
                player.CanEdit = true;
            }

            if (this.Owned && (this.RoomId == player.Room0 || player.Betaonlyroom == this.RoomId) &&
                player.HasSmileyPackage)
            {
                player.CanEdit = true;
                player.Owner = true;
            }

            //if (this.PlayerCount > 45 && !(player.IsAdmin || player.IsModerator || player.Owner))
            //{
            //    player.SendMessage("Info", "Room is full", "Sorry this room is full, please try again later :)");
            //    return false;
            //}

            if (this.joinBans.Any(b => b.UserId == player.ConnectUserId && !player.Owner && !player.IsAdmin && !player.IsModerator && !this.Crew.IsMember(player)))
            {
                player.SendMessage("info", "You are banned", "You have been banned from this world");
                return false;
            }

            //if (this.GetPlayersWithIp(player.IPAddress).Count > 5 && (!player.IsAdmin || !player.IsModerator))
            //{
            //    player.SendMessage("info", "Too many connections",
            //        "You have been kicked from this world because you are connected more than 5 times.");
            //    return false;
            //}

            //if (this.FilteredPlayers.Count(pl => pl.ConnectUserId == player.ConnectUserId) > 2 && !player.IsAdmin)
            //{
            //    player.SendMessage("info", "Limit reached",
            //        "To prevent abuse you can only be connected to the same world once.");
            //    return false;
            //}

            if (player.PlayerObject.Contains("ChatColor"))
            {
                player.ChatColor = player.PlayerObject.GetUInt("ChatColor");
            }

            return true;
        }

        // This method is called whenever a player joins the game
        public override void UserJoined(Player player)
        {
            this.AllowVisibility = true;
            this.SetVisibility(!this.BaseWorld.HideLobby && this.BaseWorld.Visible && this.BaseWorld.CrewVisibleInLobby && !this.BaseWorld.FriendsOnly);
            if (player.PlayerInsight != null)
            {
                player.PlayerInsight.Refresh(() =>
                {
                    ClientType c;
                    if (Enum.TryParse(player.PlayerInsight.GetSegment("clientapi"), true, out c))
                        player.ClientType = c;

                }, delegate (PlayerIOError e)
                {
                    this.PlayerIO.ErrorLog.WriteError("[ClientApi] Something went wrong with PlayerInsight", e);
                });
            }
            else
            {
                this.PlayerIO.ErrorLog.WriteError("[ClientApi] PlayerInsight is null");
            }

            if (!this.ips.ContainsKey(player.IPAddress.ToString()) && !player.IsGuest)
            {
                lock (this.RoomData)
                {
                    this.ips.Add(player.IPAddress.ToString(), -1);
                    this.BaseWorld.Plays++;
                    this.sessionplays++;

                    if (this.sessionplays % 15 == 0)
                    {
                        if (this.Owned && this.BaseWorld != null)
                        {
                            if (this.sessionplays > 10)
                            {
                                this.BaseWorld.Save(false);
                            }
                        }
                        this.BroadcastMetaData();
                    }


                    this.RoomData["plays"] = this.BaseWorld.Plays.ToString();
                    this.RoomData.Save();
                }
            }

            if (this.PerformQuickAction(player))
            {
                player.Disconnect();
            }

            player.JoinTime = DateTime.UtcNow;
            player.CampaignJoinTime = player.JoinTime;
        }

        private bool PerformQuickAction(Player player)
        {
            if (!player.JoinData.ContainsKey("QuickAction"))
            {
                return false;
            }

            var action = player.JoinData["QuickAction"];
            if (action == "unfavorite")
            {
                this.UnfavoriteWorld(player);
                return true;
            }
            return false;
        }

        public override void GameClosed()
        {
            if (this.Owned && this.BaseWorld != null)
            {
                this.BaseWorld.Save(false);
            }
        }

        // This method is called when a player leaves the game
        public override void UserLeft(Player player)
        {
            if (!player.Stealthy)
                this.BroadcastMessage("left", player.Id);

            if (player.TempStealth)
            {
                player.Stealthy = false;
            }

            player.CurrentWorldId = "";
            player.CurrentWorldName = "";
            player.Save();
            player.Disconnected = true;

            if (player.IsInCampaignMode &&
                !player.HasCompleted &&
                !(player.IsBot ?? true))
            {
                CampaignPlayer.Load(this.PlayerIO, player.ConnectUserId, campPlayer =>
                {
                    var progress = new CampaignProgressBackup(campPlayer, player);

                    if (player.HasBeenKicked || !player.BackupCampaign)
                    {
                        progress.Remove();
                    }
                    else
                    {
                        progress.Save(this.RoomId, this.BaseWorld);
                    }
                });
            }

            Console.WriteLine("User fully left :)");
        }

        public void ResetPlayers()
        {
            var tele = Message.Create("tele", true, true);
            foreach (var p in this.Players)
            {
                this.AddPlayerResetToMessage(tele, p);
            }
            this.BroadcastMessage(tele);

            this.OrangeSwitches.Clear();
        }

        public void ResetOrangeSwitches()
        {
            this.OrangeSwitches.Clear();
            this.BroadcastMessage("resetGlobalSwitches");
        }

        public bool AddPlayerResetToMessage(Message tele, Player player)
        {
            if (player.IsInGodMode || player.IsInModeratorMode || player.IsInAdminMode)
            {
                return false;
            }

            var spawn = this.BaseWorld.GetSpawn();
            player.X = spawn.X * 16;
            player.Y = spawn.Y * 16;
            player.Checkpoint = spawn;
            player.Deaths = 0;
            player.HasSilverCrown = false;
            player.HasCompleted = false;

            if (this.CrownId == player.Id)
            {
                this.CrownId = -1;
            }

            tele.Add(player.Id, player.X, player.Y, player.Deaths);

            player.Switches.Clear();

            if (player.RevertTemporarySmiley())
            {
                this.BroadcastMessage("face", player.Id, player.Smiley);
            }

            var activeEffects = player.GetEffects();
            foreach (var effect in activeEffects)
            {
                player.RemoveEffect(effect.Id);
                this.BroadcastMessage("effect", player.Id, (int)effect.Id, false);
            }

            if (player.Team != 0)
            {
                player.Team = 0;
                this.BroadcastMessage("team", player.Id, 0);
            }

            return true;
        }

        private void RespawnPlayer(Player p)
        {
            if (p.IsInGodMode || p.IsInModeratorMode || p.IsInAdminMode)
            {
                return;
            }

            if (p.HasActiveEffect(EffectId.Zombie))
            {
                p.RemoveEffect(EffectId.Zombie);
                this.BroadcastMessage("effect", p.Id, (int)EffectId.Zombie, false);
            }
            if (p.HasActiveEffect(EffectId.Curse))
            {
                p.RemoveEffect(EffectId.Curse);
                this.BroadcastMessage("effect", p.Id, (int)EffectId.Curse, false);
            }
            if (p.HasActiveEffect(EffectId.Fire))
            {
                p.RemoveEffect(EffectId.Fire);
                this.BroadcastMessage("effect", p.Id, (int)EffectId.Fire, false);
            }

            var tele = Message.Create("tele", false, false);
            this.AddRespawnToMessage(p, tele);
            this.BroadcastMessage(tele);
        }

        public void RespawnAllPlayers()
        {
            var tele = Message.Create("tele", false, false);
            foreach (var p in this.FilteredPlayers.Where(p => !p.IsInGodMode && !p.IsInModeratorMode && !p.IsInAdminMode))
            {
                this.AddRespawnToMessage(p, tele);
            }
            this.BroadcastMessage(tele);
        }

        public void AddRespawnToMessage(Player p, Message msg)
        {
            var d = this.BaseWorld.GetBrickType(0, p.Checkpoint.X, p.Checkpoint.Y) == (uint)ItemTypes.Checkpoint
                ? p.Checkpoint
                : this.BaseWorld.GetSpawn();
            p.X = d.X * 16;
            p.Y = d.Y * 16;
            p.SpeedX = 0;
            p.SpeedY = 0;

            msg.Add(p.Id, p.X, p.Y, p.Deaths);
        }

        public override void GotMessage(Player player, Message m)
        {
            if (!player.Initialized && m.Type != "init")
            {
                // Ignore if player are not initialized (to prevent hacking)
                return;
            }
            if (player.Stealthy && !new[] { "say", "init", "init2", "kill" }.Contains(m.Type))
            {
                Console.WriteLine("[{0}] received message from stealth player but ignored", player.Name);
                return;
            }

            if (!player.AllowMessage(m.Type))
            {
                this.PlayerIO.ErrorLog.WriteError("Message Repeat Limit exeeded",
                    player.Name + " has send '" + m.Type + "' to many times. IpAdress Ingame: " + player.IPAddress,
                    "Message type: " + m.Type, null);
                return;
            }

            switch (m.Type)
            {
                case "clear":
                {
                    if (this.Owned && (player.CanChangeWorldOptions || player.IsAdmin))
                    {
                        this.BaseWorld.Reset();
                        this.BroadcastMessage("clear", this.BaseWorld.Width, this.BaseWorld.Height,
                            this.BaseWorld.BorderType,
                            this.BaseWorld.FillType);
                    }
                    break;
                }

                case "say":
                {
                    this.Chat.HandleSay(player, m);
                    break;
                }

                case "autosay":
                {
                    this.Chat.HandleAutoSay(player, m);
                    break;
                }

                case "diamondtouch":
                {
                    var xp = m.GetUInt(0);
                    var yp = m.GetUInt(1);

                    if (this.BaseWorld.GetBrickType(0, xp, yp) != (uint)ItemTypes.Diamond)
                    {
                        break;
                    }

                    player.Smiley = 31;
                    this.BroadcastMessage("face", player.Id, 31);

                    break;
                }

                case "caketouch":
                {
                    var xp = m.GetUInt(0);
                    var yp = m.GetUInt(1);

                    if (this.BaseWorld.GetBrickType(0, xp, yp) != (uint)ItemTypes.Cake)
                    {
                        break;
                    }

                    var partyfaces = new List<int>();
                    for (var i = 72; i <= 75; i++)
                    {
                        if (i != player.Smiley)
                        {
                            partyfaces.Add(i);
                        }
                    }
                    player.Smiley = partyfaces[this.Random.Next(0, partyfaces.Count)];

                    this.BroadcastMessage("face", player.Id, player.Smiley);

                    break;
                }

                case "hologramtouch":
                {
                    var xp = m.GetUInt(0);
                    var yp = m.GetUInt(1);

                    if (this.BaseWorld.GetBrickType(0, xp, yp) != (uint)ItemTypes.Hologram)
                    {
                        break;
                    }

                    player.Smiley = 100;
                    this.BroadcastMessage("face", player.Id, player.Smiley);

                    break;
                }

                case "godblocktouch":
                {
                    var xp = m.GetUInt(0);
                    var yp = m.GetUInt(1);

                    if (this.BaseWorld.GetBrickType(0, xp, yp) != (uint)ItemTypes.GodBlock)
                    {
                        break;
                    }
                    if (!player.CanToggleGodMode)
                    {
                        player.CanToggleGodMode = true;
                        this.BroadcastMessage("toggleGod", player.Id, player.CanToggleGodMode);
                        this.SendSystemMessage(player, "You may now use god mode.");
                    }
                    break;
                }

                case "death":
                {
                    player.Deaths += 1;
                    this.RespawnPlayer(player);
                    break;
                }

                case "checkpoint":
                {
                    var xp = m.GetUInt(0);
                    var yp = m.GetUInt(1);

                    if (this.BaseWorld.GetBrickType(0, xp, yp) == (uint)ItemTypes.Checkpoint)
                    {
                        player.Checkpoint = new Item(new ForegroundBlock(), xp, yp);
                    }
                    break;
                }

                case "levelcomplete":
                {
                    if (!player.Owner || this.IsCampaign)
                    {
                        var xp = m.GetUInt(0);
                        var yp = m.GetUInt(1);

                        if (this.BaseWorld.GetBrickType(0, xp, yp) != (uint)ItemTypes.Complete)
                        {
                            //CHEATER!!
                            this.KickAndLogCheatingUser(player, "BadTrophy");
                            return;
                        }
                    }

                    if (this.IsCampaign &&
                        player.IsInCampaignMode &&
                        this.antiCheat.OnTrophy(player))
                    {
                        return;
                    }

                    if (!player.HasCompleted)
                    {
                        if (player.TimeSinceReset.TotalSeconds > 8 && !player.IsGuest)
                        {
                            this.BroadcastMessage("write", "* WORLD", player.Name.ToUpper() + " completed this world!");
                        }
                        player.HasSilverCrown = true;
                        player.HasCompleted = true;

                        if (this.IsCampaign)
                        {
                            this.campaign.CompleteWorld(player, this.RoomId, this.shop);
                            this.BaseWorld.AntiCheatData.StatsMinMoves.Add(player.TotalMovements + player.Name);
                        }
                        else
                        {
                            player.SendMessage("completedLevel");
                        }

                        this.AwardOwnerWithGod(player, true);
                    }

                    this.BroadcastMessage("ks", player.Id);
                    break;
                }
                case "reset":
                {
                    if (!player.Owner || this.BaseWorld.IsPartOfCampaign)
                    {
                        var xp = m.GetUInt(0);
                        var yp = m.GetUInt(1);

                        if (this.BaseWorld.GetBrickType(0, xp, yp) != (uint)ItemTypes.ResetPoint)
                        {
                            this.KickAndLogCheatingUser(player, "BadReset");
                            return;
                        }
                    }

                    var tele = Message.Create("tele", true, false);
                    if (this.AddPlayerResetToMessage(tele, player))
                    {
                        this.BroadcastMessage(tele);
                    }

                    player.ResetTime = DateTime.UtcNow;
                    break;
                }
                case "save":
                {
                    if (this.Owned && ((player.CanChangeWorldOptions && !this.IsCampaign) || player.IsAdmin || player.IsModerator))
                    {
                        this.BaseWorld.Save(true, delegate { player.SendMessage(Message.Create("saved")); });
                        if (this.BaseWorld.OwnerId != "")
                        {
                            this.PlayerIO.BigDB.Load("PlayerObjects", this.BaseWorld.OwnerId,
                                delegate (DatabaseObject oo)
                                {
                                    if (!oo.Contains("myworldnames"))
                                    {
                                        oo.Set("myworldnames", new DatabaseObject());
                                    }
                                    oo.GetObject("myworldnames")
                                        .Set(this.RoomId, ChatUtils.RemoveBadCharacters(this.BaseWorld.Name));
                                    oo.Save();
                                });
                        }
                    }
                    break;
                }
                case "name":
                {
                    if (this.Owned && (player.CanChangeWorldOptions || player.IsAdmin) && this.BaseWorld != null &&
                        !this.IsCampaign)
                    {
                        this.SetWorldName(m.GetString(0));
                    }
                    break;
                }
                case "key":
                {
                    if (player.CanChangeWorldOptions && !this.IsCampaign && !this.BaseWorld.IsArtContest)
                    {
                        this.editkey = m.GetString(0);
                        foreach (var p in this.FilteredPlayers.Where(p => !p.CanChangeWorldOptions))
                        {
                            this.TrySetEditRights(p, false);
                        }
                    }
                    break;
                }

                case "touch":
                {
                    var touchedId = m.GetInt(0);
                    var id = m.GetInt(1);
                    var effectId = (EffectId)id;

                    if (!this.IsTouchEffect(effectId))
                    {
                        break;
                    }

                    foreach (
                        var p in
                            this.FilteredPlayers.Where(
                                p => p.Id == touchedId && !p.IsInGodMode && !p.IsInModeratorMode && !p.IsInAdminMode))
                    {
                        if (player.HasActiveEffect(effectId) && !p.HasActiveEffect(effectId))
                        {
                            var effect = player.GetEffect(effectId);
                            if (effectId == EffectId.Curse)
                            {
                                player.RemoveEffect(effectId);
                                p.AddEffect(effect);
                                this.BroadcastMessage("effect", player.Id, id, false);
                                this.BroadcastMessage("effect", p.Id, id, true, effect.TimeLeft, effect.Duration);
                            }
                            else if (effectId == EffectId.Protection)
                            {
                                p.RemoveEffect(EffectId.Curse);
                                p.RemoveEffect(EffectId.Zombie);
                                this.BroadcastMessage("effect", p.Id, (int)EffectId.Curse, false);
                                this.BroadcastMessage("effect", p.Id, (int)EffectId.Zombie, false);
                                this.BroadcastMessage("effect", p.Id, (int)EffectId.Fire, false);
                            }
                            else if (effectId == EffectId.Zombie)
                            {
                                var limit = this.BaseWorld.ZombieLimit;
                                if (limit > 0)
                                {
                                    var numZombie = 0;
                                    foreach (var pl in this.Players)
                                    {
                                        if (pl.HasActiveEffect(EffectId.Zombie))
                                        {
                                            numZombie++;
                                        }
                                        if (numZombie == limit)
                                        {
                                            break;
                                        }
                                    }
                                    if (numZombie == limit)
                                    {
                                        break;
                                    }
                                }

                                var newEffect = new Effect(id, effect.Duration);
                                newEffect.Activate();
                                p.AddEffect(newEffect);
                                this.BroadcastMessage("effect", p.Id, id, true, newEffect.TimeLeft,
                                    newEffect.Duration);
                            }
                            else
                            {
                                var newEffect = new Effect(id, effect.Duration);
                                newEffect.Activate();
                                p.AddEffect(newEffect);
                                this.BroadcastMessage("effect", p.Id, id, true, newEffect.TimeLeft,
                                    newEffect.Duration);
                            }
                        }
                        break;
                    }
                    break;
                }

                case "kill":
                {
                    if (player.IsAdmin || player.IsModerator)
                    {
                        this.die = true;
                    }
                    break;
                }

                case "admin":
                {
                    if (player.IsAdmin)
                    {
                        this.LeaveCampaignMode(player);
                        player.IsInAdminMode = !player.IsInAdminMode;
                        player.StaffAuraOffset = m.Count > 0 ? m.GetInt(0) != -1 ? m.GetInt(0) : 0 : 0;

                        this.BroadcastMessage("admin", player.Id, player.IsInAdminMode, player.StaffAuraOffset);
                    }
                    break;
                }

                case "mod":
                {
                    if (player.IsModerator)
                    {
                        this.LeaveCampaignMode(player);
                        player.IsInModeratorMode = !player.IsInModeratorMode;
                        player.StaffAuraOffset = m.Count > 0 ? m.GetInt(0) != -1 ? m.GetInt(0) : 0 : 0;

                        this.BroadcastMessage("mod", player.Id, player.IsInModeratorMode, player.StaffAuraOffset);
                    }
                    break;
                }

                case "god":
                {
                    if (this.lockedroom && player.CanToggleGodMode)
                    {
                        this.BroadcastMessage("god", player.Id, m.GetBoolean(0));
                        player.IsInGodMode = m.GetBoolean(0);
                    }
                    break;
                }
                case "time":
                {
                    player.SendMessage(Message.Create("time", m.GetDouble(0), this.GetTime()));
                    break;
                }
                case "access":
                {
                    if (!player.IsGuest && m.Count != 0 && !this.IsCampaign)
                    {
                        if (m.GetString(0) == this.editkey)
                        {
                            this.TrySetEditRights(player, true);
                        }
                    }
                    break;
                }
                case "setRoomVisible":
                {
                    if (this.BaseWorld.IsArtContest && !player.IsAdmin)
                    {
                        player.SendMessage("info", "Disabled",
                            "Sorry, you cannot change this in a contest world.");
                        return;
                    }

                    if (!player.CanChangeWorldOptions)
                    {
                        break;
                    }

                    this.BaseWorld.Visible = m.GetBoolean(0);
                    this.BaseWorld.Save(false);
                    this.SetVisibility(!this.BaseWorld.HideLobby && this.BaseWorld.Visible && this.BaseWorld.CrewVisibleInLobby && !this.BaseWorld.FriendsOnly);
                    this.BroadcastMessage("roomVisible", this.BaseWorld.Visible, this.BaseWorld.FriendsOnly);
                    break;
                }
                case "setHideLobby":
                {
                    if (this.BaseWorld.IsArtContest && !player.IsAdmin)
                    {
                        player.SendMessage("info", "Disabled",
                            "Sorry, you cannot change this in a contest world.");
                        return;
                    }

                    if (!player.CanChangeWorldOptions)
                    {
                        break;
                    }
                    this.BaseWorld.HideLobby = m.GetBoolean(0);
                    this.BaseWorld.Save(false);
                    this.SetVisibility(!this.BaseWorld.HideLobby && this.BaseWorld.Visible && this.BaseWorld.CrewVisibleInLobby && !this.BaseWorld.FriendsOnly);
                    this.BroadcastMessage("hideLobby", this.BaseWorld.HideLobby);
                    break;
                }
                case "setAllowSpectating":
                {
                    if (!player.CanChangeWorldOptions)
                    {
                        break;
                    }
                    this.BaseWorld.AllowSpectating = m.GetBoolean(0);
                    this.BaseWorld.Save(false);
                    this.BroadcastMessage("allowSpectating", this.BaseWorld.AllowSpectating);
                    break;
                }
                case "setRoomDescription":
                {
                    if (!player.CanChangeWorldOptions)
                    {
                        break;
                    }
                    var desc = m.GetString(0);

                    this.BaseWorld.WorldDescription = desc;
                    this.BaseWorld.Save(false);
                    this.RoomData["description"] = desc;
                    this.RoomData.Save();

                    this.BroadcastMessage("roomDescription", this.BaseWorld.WorldDescription);
                    break;
                }
                case "setStatus":
                {
                    if (this.BaseWorld.IsArtContest && !player.IsAdmin)
                    {
                        player.SendMessage("info", "Disabled",
                            "Sorry, you cannot change the status of a contest world. Please contact an administrator if you are ready to release your world!");
                        return;
                    }

                    if (!player.CanChangeWorldOptions || !this.BaseWorld.IsPartOfCrew
                        || this.BaseWorld.Status == WorldStatus.Released)
                    {
                        break;
                    }

                    var status = m.GetInt(0);
                    if (status < 0 || status > 2)
                    {
                        break;
                    }

                    this.BaseWorld.Status = (WorldStatus)status;
                    this.BaseWorld.Save(false);
                    /*
                    switch ((WorldStatus)status)
                    {
                        case WorldStatus.Wip:
                        {
                            this.SetVisibility(false);

                            foreach (var p in this.Players)
                            {
                                if (!this.Crew.IsMember(p) && !(p.IsAdmin || p.IsModerator || p.Owner))
                                {
                                    p.SendMessage("info", "World locked",
                                        "World was set to only allow crew members. Come back later.");
                                    p.Disconnect();
                                }
                                p.SendMessage("write", ChatUtils.SystemName,
                                    "World status changed to 'Work In Progress'. Only crew members can now join this world.");
                            }
                            break;
                        }
                        case WorldStatus.Open:
                        {
                            this.SetVisibility(!this.BaseWorld.HideLobby && this.BaseWorld.Visible);

                            this.BroadcastMessage("write", ChatUtils.SystemName,
                                "World status changed to 'Open'. Everyone can now join this world.");
                            break;
                        }
                        case WorldStatus.Released:
                        {
                            this.Crew.PublishReleaseNotification(this.RoomId);

                            foreach (var p in this.Players)
                            {
                                p.SendMessage("write", ChatUtils.SystemName,
                                    "Congratulations. World has been released to the public!");
                                p.CanChangeWorldOptions = false;
                                p.SendMessage("worldReleased");
                            }
                            break;
                        }
                    }*/
                    break;
                }
                case "setCurseLimit":
                {
                    if (!player.CanChangeWorldOptions)
                    {
                        break;
                    }
                    var limit = m.GetInt(0);
                    if (limit < 0 || limit > 75)
                    {
                        break;
                    }
                    this.BaseWorld.CurseLimit = limit;
                    this.BaseWorld.Save(false);
                    foreach (var p in this.FilteredPlayers.Where(p => p.HasActiveEffect(EffectId.Curse)))
                    {
                        p.RemoveEffect(EffectId.Curse);
                        this.BroadcastMessage("effect", p.Id, (int)EffectId.Curse, false);
                    }
                    this.BroadcastMessage(Message.Create("effectLimits", this.BaseWorld.CurseLimit,
                        this.BaseWorld.ZombieLimit));
                    break;
                }
                case "setZombieLimit":
                {
                    if (!player.CanChangeWorldOptions)
                    {
                        break;
                    }
                    var limit = m.GetInt(0);
                    if (limit < 0 || limit > 75)
                    {
                        break;
                    }
                    this.BaseWorld.ZombieLimit = limit;
                    this.BaseWorld.Save(false);
                    foreach (var p in this.FilteredPlayers.Where(p => p.HasActiveEffect(EffectId.Zombie)))
                    {
                        p.RemoveEffect(EffectId.Zombie);
                        this.BroadcastMessage("effect", p.Id, (int)EffectId.Zombie, false);
                    }
                    this.BroadcastMessage(Message.Create("effectLimits", this.BaseWorld.CurseLimit,
                        this.BaseWorld.ZombieLimit));
                    break;
                }
                case "setMinimapEnabled":
                {
                    if (!player.CanChangeWorldOptions)
                    {
                        break;
                    }
                    this.BaseWorld.MinimapEnabled = m.GetBoolean(0);
                    this.BroadcastMessage("minimapEnabled", this.BaseWorld.MinimapEnabled);
                    break;
                }
                case "setLobbyPreviewEnabled":
                {
                    if (!player.CanChangeWorldOptions)
                    {
                        break;
                    }
                    this.BaseWorld.LobbyPreviewEnabled = m.GetBoolean(0);
                    this.RoomData["LobbyPreviewEnabled"] = this.BaseWorld.LobbyPreviewEnabled.ToString();
                    this.RoomData.Save();
                    this.BroadcastMessage("lobbyPreviewEnabled", this.BaseWorld.LobbyPreviewEnabled);
                    break;
                }
                case "like":
                {
                    if (this.BaseWorld.IsArtContest)
                    {
                        player.SendMessage("info", "Disabled",
                            "Sorry, you cannot like a contest world.");
                        return;
                    }

                    if (player.Owner || player.IsGuest)
                    {
                        break;
                    }

                    if (player.HasUnliked)
                    {
                        player.SendMessage("write", ChatUtils.SystemName, "You must rejoin to like this world again.");
                        return;
                    }
                    player.HasLiked = true;

                    var likes = player.PlayerObject.GetObject("likes");
                    if (likes == null)
                    {
                        likes = new DatabaseObject();
                        player.PlayerObject.Set("likes", likes);
                    }
                    else if (likes.Contains(this.RoomId))
                    {
                        player.SendMessage("write", ChatUtils.SystemName, "You have already liked this world before.");
                        return;
                    }

                    likes.Set(this.RoomId, true);
                    player.PlayerObject.Save();

                    player.SendMessage("liked");

                    this.AddLikesToWorld(1);
                    break;
                }
                case "unlike":
                {
                    if (player.Owner || player.IsGuest)
                    {
                        break;
                    }

                    if (player.HasLiked)
                    {
                        player.HasUnliked = true;
                    }

                    var likes = player.PlayerObject.GetObject("likes");
                    if (likes == null || !likes.Contains(this.RoomId))
                    {
                        player.SendMessage("write", ChatUtils.SystemName, "This world is not in your likes.");
                        return;
                    }
                    likes.Remove(this.RoomId);
                    player.PlayerObject.Save();

                    player.SendMessage("unliked");

                    this.AddLikesToWorld(-1);
                    break;
                }
                case "favorite":
                {
                    if (this.BaseWorld.IsArtContest)
                    {
                        player.SendMessage("info", "Disabled",
                            "Sorry, you cannot favourite a contest world.");
                        return;
                    }

                    if (player.Owner || player.IsGuest)
                    {
                        break;
                    }

                    if (player.HasUnfavorited)
                    {
                        player.SendMessage("write", ChatUtils.SystemName,
                            "You must rejoin to favourite this world again.");
                        return;
                    }
                    player.HasFavorited = true;

                    var favorites = player.PlayerObject.GetObject("favorites");
                    if (favorites == null)
                    {
                        favorites = new DatabaseObject();
                        player.PlayerObject.Set("favorites", favorites);
                    }
                    else if (favorites.Contains(this.RoomId))
                    {
                        player.SendMessage("write", ChatUtils.SystemName, "This world is already in your favorites.");
                        return;
                    }

                    favorites.Set(this.RoomId, this.BaseWorld.Name);
                    player.PlayerObject.Save();

                    player.SendMessage("favorited");

                    this.AddFavoritesToWorld(1);
                    break;
                }
                case "unfavorite":
                {
                    this.UnfavoriteWorld(player);
                    break;
                }
                case "init":
                {
                    // To prevent people (hacking) sending in too many init messages
                    if (player.Initialized || player.IsInitializing || player.IsInitializingDone)
                    {
                        player.Cheat++;
                        break;
                    }

                    Callback cb = delegate
                    {
                        player.Init(this.PlayerIO, delegate
                        {
                            player.CurrentWorldId = this.RoomId;
                            player.CurrentWorldName = this.RoomData.ContainsKey("name")
                                ? this.RoomData["name"]
                                : "Untitled World";

                            if (!this.smileyMap.SmileyIsLegit(player, player.Smiley, this.shop))
                            {
                                player.Smiley = 0;
                            }
                            if (!this.smileyMap.AuraIsLegit(player, player.Aura, player.AuraColor, this.shop))
                            {
                                player.AuraColor = 0;
                                player.Aura = 0;
                            }

                            player.Save();
                            if (!player.HasBeta && this.isbetalevel)
                            {
                                //player.SendMessage("info", "Beta only!", "Sorry, but this level is only accessible by Beta members.");
                                //player.Disconnect();
                            }

                            if (this.BaseWorld.IsPartOfCrew && this.BaseWorld.IsArtContest)
                            {
                                if (!this.Crew.IsMember(player) && !player.IsAdmin && !player.IsModerator && !player.IsJudge && this.BaseWorld.Status != WorldStatus.Released)
                                {
                                    //player.SendMessage("info", "Crew only",
                                    //    "This contest world is only accesible to crew members.");
                                    //player.Disconnect();
                                }
                                if (player.IsJudge)
                                    this.TrySetEditRights(player, true);
                                this.ScheduleCallback(delegate ()
                                {
                                    SendSystemMessage(player, "WARNING: If you are not using a web browser, contest assets may not load correctly due to Flash security policy.");
                                }, 15000);

                            }

                            Console.WriteLine("Player ready " + player.Name + " world? " + this.ready);
                            if (this.Owned && !this.ready)
                            {
                                player.Ready = true;
                            }
                            else
                            {
                                this.SendInitMessage(player);
                            }
                        });
                    };
                    // Initialize shop since we need to check certain player requests against the payvault (which is the shop)
                    if (this.shop == null)
                    {
                        this.shop = new Shop(this.PlayerIO, delegate { cb(); });
                    }
                    else
                    {
                        cb();
                    }
                    break;
                }
                case "init2":
                {
                    if (this.BaseWorld.IsArtContest)
                        this.BroadcastContestItems(this.Crew.Id);

                    if (player.IsAdmin)
                    {
                        this.TrySetEditRights(player, true);
                    }

                    // Adding the existing players to the new player screen
                    foreach (var p in (player.IsAdmin || player.IsModerator) ? this.Players : this.FilteredPlayers)
                    {
                        if (p == player)
                        {
                            continue;
                        }

                        this.SendAddMessage(p, player);

                        if (p.Initialized)
                        {
                            foreach (var effect in p.GetEffects())
                            {
                                if (effect.CanExpire)
                                {
                                    player.SendMessage(Message.Create("effect", p.Id, (int)effect.Id, true,
                                        effect.TimeLeft, effect.Duration));
                                }
                                else if (effect.Id == EffectId.Gravity)
                                {
                                    player.SendMessage(Message.Create("effect", p.Id, (int)effect.Id, true, effect.Duration));
                                }
                                else
                                {
                                    player.SendMessage(Message.Create("effect", p.Id, (int)effect.Id, true));
                                }
                            }
                        }
                        if (p.HasSilverCrown)
                        {
                            player.SendMessage(Message.Create("ks", p.Id));
                        }
                    }

                    this.Chat.SendOldChat(player);

                    player.SendMessage(Message.Create("k", this.CrownId));
                    player.SendMessage(Message.Create("init2"));

                    this.AwardOwnerWithGod(player);
                    this.NotifyWorld();
                    break;
                }

                // Player collect coin
                case "c":
                {
                    var oldCoins = player.Coins;
                    var oldBCoins = player.BlueCoins;
                    player.Coins = m.GetInt(0);
                    player.BlueCoins = m.GetInt(1);

                    if (m.Count < 4)
                    {
                        return;
                    }

                    var cx = m.GetUInt(2);
                    var cy = m.GetUInt(3);

                    var increasedCoins = (oldCoins + 1 == player.Coins &&
                                          oldBCoins == player.BlueCoins) ||
                                         (oldCoins == player.Coins &&
                                          oldBCoins + 1 == player.BlueCoins);

                    var block = this.BaseWorld.GetBrickType(0, cx, cy);

                    if (this.IsCampaign)
                    {
                        if (!increasedCoins)
                        {
                            this.LogCheatingUser(player, "NoIncreaseInCoins");
                            return;
                        }
                        if (block != 100 && block != 101)
                        {
                            this.LogCheatingUser(player, "BadCoin");
                            return;
                        }
                    }

                    if ((block == 100 || block == 101) && increasedCoins)
                    {
                        this.magic.OnCoin(player,
                            block == 101,
                            this.BaseWorld.CoinCount,
                            this.BaseWorld.BlueCoinCount);
                    }

                    player.SetCoinCollected(block, cx, cy);

                    this.BroadcastMessage("c", player.Id, player.Coins, player.BlueCoins, cx, cy);
                    break;
                }

                // Player move
                case "m":
                {
                    try
                    {
                        var horizontal = m.GetInt(6);
                        var vertical = m.GetInt(7);

                        if (player.Horizontal != horizontal || player.Vertical != vertical)
                        {
                            player.LastMove = DateTime.UtcNow;
                        }

                        player.TotalMovements++;
                        player.X = m.GetDouble(0);
                        player.Y = m.GetDouble(1);
                        player.SpeedX = m.GetDouble(2);
                        player.SpeedY = m.GetDouble(3);
                        player.ModifierX = m.GetDouble(4);
                        player.ModifierY = m.GetDouble(5);
                        player.Horizontal = horizontal;
                        player.Vertical = vertical;
                        player.SpaceDown = m.GetBoolean(9);
                        player.SpaceJustPressed = m.GetBoolean(10);
                        player.TickId = m.GetInt(11);
                        this.BroadcastMessage("m", player.Id, player.X, player.Y, player.SpeedX, player.SpeedY,
                            player.ModifierX, player.ModifierY, player.Horizontal, player.Vertical, player.SpaceDown,
                            player.SpaceJustPressed);
                        this.antiCheat.OnMove(player);

                        var blockX = ((int)player.X + 8) >> 4;
                        var blockY = ((int)player.Y + 8) >> 4;
                        if ((blockX < 0 || blockY < 0 || blockX > this.BaseWorld.Width || blockY > this.BaseWorld.Height) && !player.Owner)
                        {
                            player.Disconnect();
                        }
                    }
                    catch (Exception e)
                    {
                        this.PlayerIO.ErrorLog.WriteError("We got invalid data from " + player.ConnectUserId, e);
                    }
                    break;
                }

                case "ps":
                {
                    var posX = m.GetUInt(0);
                    var posY = m.GetUInt(1);
                    var switchType = m.GetUInt(2);
                    var id = m.GetInt(3);
                    var enabled = m.GetBoolean(4);

                    if (id > 999 || switchType > 1)
                    {
                        break;
                    }

                    var blockType = this.BaseWorld.GetForeground(posX, posY).Type;

                    if (switchType == 0)
                    {
                        if (blockType != (uint)ItemTypes.SwitchPurple)
                            break;

                        if (enabled)
                            player.Switches.Add(id);
                        else
                            player.Switches.Remove(id);
                    }
                    else if (switchType == 1)
                    {
                        if (blockType != (uint)ItemTypes.SwitchOrange)
                            break;

                        if (enabled)
                            this.OrangeSwitches.Add(id);
                        else
                            this.OrangeSwitches.Remove(id);
                    }

                    this.BroadcastMessage("ps", player.Id, switchType, id, enabled);
                    break;
                }

                case "effect":
                {
                    var xp = m.GetUInt(0);
                    var yp = m.GetUInt(1);
                    var id = m.GetInt(2);

                    var effectId = (EffectId)id;
                    var block = this.BaseWorld.GetForeground(xp, yp);

                    // If user sent invalid effect id for this block we stop processing
                    if (!this.IsEffectBlock(block, effectId))
                    {
                        break;
                    }

                    var arg = this.GetEffectArg(block);

                    var effectToGive = new Effect(id, (int)arg);
                    this.ApplyEffectToPlayer(player, effectToGive);
                    break;
                }

                case "team":
                {
                    var xp = m.GetUInt(0);
                    var yp = m.GetUInt(1);

                    var block = this.BaseWorld.GetForeground(xp, yp);
                    if (!this.IsEffectBlock(block, EffectId.Team))
                    {
                        break;
                    }

                    var team = (int)block.Number;
                    if (player.Team == team)
                    {
                        break;
                    }

                    player.Team = team;
                    this.BroadcastMessage("team", player.Id, player.Team);
                    break;
                }

                case "cheatDetected":
                {
                    var cheatType = m.GetString(0);
                    if (cheatType == "")
                    {
                        cheatType = "Unknown";
                    }

                    this.KickAndLogCheatingUser(player, cheatType);
                    break;
                }
                case "requestAddToCrew":
                {
                    if (player.AddToCrewRequests++ > 2)
                    {
                        player.SendMessage("crewAddRequestFailed",
                            "The owner has already rejected your request twice. Maybe it's time to stop annoying him.");
                        break;
                    }

                    if (this.BaseWorld.IsPartOfCrew)
                    {
                        player.SendMessage("crewAddRequestFailed", "This world is already part of Crew.");
                        break;
                    }
                    if (this.pendingCrewRequest != "")
                    {
                        player.SendMessage("crewAddRequestFailed", "This world already has pending crew add request.");
                        break;
                    }

                    var crewId = m.GetString(0).ToLower();

                    var c = new Crew(this.PlayerIO);
                    c.Load(crewId, () =>
                    {
                        if (c.DatabaseObject != null && c.isContest)
                        {
                            player.SendMessage("info", "Error", "You cannot add worlds to contest crews.");
                            return;
                        }
                        if (c.DatabaseObject != null && c.HasPower(player, CrewPower.WorldsManagement))
                        {
                            if (player.Owner)
                            {
                                this.AddToCrew(c, player);
                            }
                            else
                            {
                                var owner = this.FilteredPlayers.FirstOrDefault(p => p.Owner);
                                if (owner != null)
                                {
                                    this.pendingCrewRequest = c.Id;
                                    this.crewRequestSender = player;
                                    owner.SendMessage("crewAddRequest", player.Name.ToUpper(), c.Name);
                                    player.SendMessage("info2", "Request pending",
                                        "Waiting for response from world owner...");
                                }
                                else
                                {
                                    player.SendMessage("crewAddRequestFailed",
                                        "World owner has to be online in the world to accept request.");
                                }
                            }
                        }
                        else
                        {
                            player.SendMessage("crewAddRequestFailed",
                                "Crew not found or you don't have rights to add worlds.");
                        }
                    });
                    break;
                }
                case "addToCrew":
                {
                    if (!player.Owner || this.pendingCrewRequest == "")
                    {
                        break;
                    }

                    var c = new Crew(this.PlayerIO);
                    c.Load(this.pendingCrewRequest, () =>
                    {
                        if (c.DatabaseObject != null && c.isContest)
                        {
                            player.SendMessage("info", "Error", "You cannot add worlds to contest crews.");
                            return;
                        }
                        this.AddToCrew(c, player);
                    });
                    break;
                }
                case "rejectAddToCrew":
                {
                    if (!player.Owner || this.pendingCrewRequest == "")
                    {
                        break;
                    }

                    this.pendingCrewRequest = "";
                    if (this.crewRequestSender != null)
                    {
                        this.crewRequestSender.SendMessage("info2", "Request response",
                            "Request rejected by world owner.");
                        this.crewRequestSender = null;
                    }
                    break;
                }
                case "aura":
                {
                    var aura = m.GetInt(0);
                    var color = m.GetInt(1);

                    if (player.IsAdmin)
                    {
                        this.SetPlayerAura(player, aura, color);
                        return;
                    }

                    player.PayVault.Refresh(delegate
                    {
                        if (this.smileyMap.AuraIsLegit(player, aura, color, this.shop))
                        {
                            this.SetPlayerAura(player, aura, color);
                        }
                        else
                        {
                            this.SetPlayerAura(player, 0, 0);
                        }
                    });
                    break;
                }
                case "changeBadge":
                {
                    var badgeId = m.GetString(0);

                    player.Achievements.Refresh(() =>
                    {
                        var badge = player.Achievements.Get(badgeId);
                        player.Badge = badge != null && badge.Completed ? badge.Id : "";
                        player.PlayerObject.Save();
                        this.BroadcastMessage("badgeChange", player.Id, player.Badge);
                    });
                    break;
                }
                case "smileyGoldBorder":
                {
                    if (player.HasGoldMembership)
                    {
                        var smileyGoldBorder = m.GetBoolean(0);
                        player.SmileyGoldBorder = smileyGoldBorder;
                        player.PlayerObject.Save();
                        this.BroadcastMessage("smileyGoldBorder", player.Id, player.SmileyGoldBorder);
                    }
                    break;
                }

                case "b":
                {
                    this.PlaceBrick(player, m);
                    break;
                }
                case "crown":
                {
                    if (!player.Owner)
                    {
                        var xp = m.GetUInt(0);
                        var yp = m.GetUInt(1);

                        if (this.BaseWorld.GetBrickType(0, xp, yp) != (uint)ItemTypes.Crown)
                        {
                            break;
                        }
                    }

                    this.CrownId = player.Id;
                    break;
                }
                case "pressKey":
                {
                    this.keys.Handle(player, m.GetUInt(0), m.GetUInt(1), m.GetString(2));
                    break;
                }
                case "smiley":
                {
                    var smiley = m.GetInt(0);
                    if (player.IsAdmin)
                    {
                        this.SetPlayerFace(player, smiley);
                        this.antiCheat.OnFace(player);
                        return;
                    }

                    player.PayVault.Refresh(delegate
                    {
                        if (this.smileyMap.SmileyIsLegit(player, smiley, this.shop))
                        {
                            this.SetPlayerFace(player, smiley);
                            this.antiCheat.OnFace(player);
                        }
                        else
                        {
                            this.SetPlayerFace(player, 0);
                        }
                    });
                    break;
                }
            }
        }

        private void AddContestItemsToMessage(Message message, string crewId, Action<Message> action)
        {
            this.PlayerIO.BigDB.Load("ContestAssets", crewId, contestAsset =>
            {
                message.Add("cs");
                foreach (var kvp in contestAsset.GetArray("BlockAssets").IndexesAndValues)
                {
                    var index = kvp.Key;
                    var blockAsset = (DatabaseObject)kvp.Value;

                    message.Add((uint)index);
                    message.Add(blockAsset.GetBytes("Bitmap", new byte[0]));
                    message.Add(blockAsset.GetUInt("Layer", 0));
                    message.Add(blockAsset.GetBool("HasShadow", false));
                    message.Add(blockAsset.GetBool("IsAbove", false));
                }
                message.Add("ce");

                action.Invoke(message);
            });
        }

        private void BroadcastContestItems(string crewId)
        {
            var message = Message.Create("contestItemPack");
            this.AddContestItemsToMessage(message, crewId, this.BroadcastMessage);
        }

        public void MarkArtContestWorld(int maximumBlockAssets = 21)
        {
            this.BaseWorld.IsArtContest = true;

            this.BaseWorld.Save(false, new Callback(() =>
            {
                this.PlayerIO.BigDB.CreateObject("ContestAssets", this.Crew.Id, new DatabaseObject(), new Callback<DatabaseObject>((obj) =>
                {
                    obj.Set("MaximumBlockAssets", maximumBlockAssets);
                    obj.Set("BlockAssets", new DatabaseArray());

                    obj.Save();
                }));
            }));
        }

        public void NotifyWorld(bool forceNotify = false)
        {
            this.PlayerIO.BigDB.LoadOrCreate("Config", "Notification", value =>
            {
                if (value.GetDateTime("EndDate", DateTime.MinValue).Subtract(DateTime.UtcNow).TotalSeconds > 0)
                {
                    var header = value.GetString("Header", "Notification");
                    var body = value.GetString("Body", "NULL");

                    foreach (var p in this.FilteredPlayers.Where(player => !player.GotNotified || forceNotify))
                    {
                        p.SendMessage("notice", header, body);
                        p.SendMessage("info2", header, body);
                        p.GotNotified = true;
                    }
                }
            });
        }

        public void CheckAntiSpam()
        {
            //this.PlayerIO.BigDB.LoadOrCreate("Config", "AntiSpam", value =>
            //{
            //    var worldPrefix = value.GetString("WorldKillPrefix", "OW");
            //    if (value.GetBool("KillWorld", false))
            //    {
            //        if (this.RoomId.StartsWith(worldPrefix))
            //        {
            //            foreach (var player in this.Players)
            //            {
            //                player.Disconnect();
            //            }
            //        }
            //    }
            //});
        }

        private void AddToCrew(Crew c, Player player)
        {
            this.BaseWorld.Crew = c.Id;
            this.BaseWorld.Status = WorldStatus.Open;
            this.BaseWorld.Save(false);
            this.Crew = c;

            player.SendMessage("info2", "Success", "Added world to " + this.Crew.Name + "!");
            this.BroadcastMessage("write", ChatUtils.SystemName, "Added world to " + this.Crew.Name + ".");

            this.BroadcastMessage("addedToCrew", this.Crew.Id, this.Crew.Name);
        }

        private bool ReachedEffectLimit(int limit, EffectId effect)
        {
            if (limit <= 0)
            {
                return false;
            }

            var numActive = 0;
            foreach (var p in this.Players)
            {
                if (p.HasActiveEffect(effect))
                {
                    numActive++;
                }
                if (numActive == limit)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsTimedEffect(EffectId id)
        {
            switch (id)
            {
                case EffectId.Curse:
                case EffectId.Zombie:
                case EffectId.Fire:
                    return true;

                default:
                    return false;
            }
        }

        private bool IsTouchEffect(EffectId id)
        {
            switch (id)
            {
                case EffectId.Curse:
                case EffectId.Zombie:
                case EffectId.Protection:
                    return true;

                default:
                    return false;
            }
        }

        private uint GetEffectArg(ForegroundBlock block)
        {
            switch (block.Type)
            {
                case (uint)ItemTypes.Lava:
                    return 2;

                case (uint)ItemTypes.EffectCurse:
                case (uint)ItemTypes.EffectZombie:
                case (uint)ItemTypes.EffectTeam:
                case (uint)ItemTypes.EffectFly:
                case (uint)ItemTypes.EffectJump:
                case (uint)ItemTypes.EffectProtection:
                case (uint)ItemTypes.EffectRun:
                case (uint)ItemTypes.EffectLowGravity:
                case (uint)ItemTypes.EffectMultijump:
                case (uint)ItemTypes.EffectGravity:
                    return block.Number;

                default:
                    return 0;
            }
        }

        private bool IsEffectBlock(ForegroundBlock block, EffectId effectId)
        {
            switch (effectId)
            {
                case EffectId.Curse:
                    return block.Type == (uint)ItemTypes.EffectCurse;
                case EffectId.Zombie:
                    return block.Type == (uint)ItemTypes.EffectZombie;
                case EffectId.Team:
                    return block.Type == (uint)ItemTypes.EffectTeam;
                case EffectId.Fly:
                    return block.Type == (uint)ItemTypes.EffectFly;
                case EffectId.Jump:
                    return block.Type == (uint)ItemTypes.EffectJump;
                case EffectId.Protection:
                    return block.Type == (uint)ItemTypes.EffectProtection;
                case EffectId.Run:
                    return block.Type == (uint)ItemTypes.EffectRun;
                case EffectId.LowGravity:
                    return block.Type == (uint)ItemTypes.EffectLowGravity;
                case EffectId.Fire:
                    return block.Type == (uint)ItemTypes.Lava || block.Type == (uint)ItemTypes.Water ||
                           block.Type == (uint)ItemTypes.Mud;
                case EffectId.Multijump:
                    return block.Type == (uint)ItemTypes.EffectMultijump;
                case EffectId.Gravity:
                    return block.Type == (uint)ItemTypes.EffectGravity;

                default:
                    return false;
            }
        }

        public void SetPlayerFace(Player player, int face)
        {
            player.Smiley = face;
            this.BroadcastMessage("face", player.Id, face);
        }

        private void SetPlayerAura(Player player, int aura, int color)
        {
            player.Aura = aura;
            player.AuraColor = color;
            this.BroadcastMessage("aura", player.Id, aura, color);
        }

        public DatabaseObject GetLocalCopyOfReport(string offender)
        {
            return this.reports.FirstOrDefault(report => report.GetString("ReportedUsername", "") == offender);
        }

        public void SaveLocalCopyOfReport(DatabaseObject report)
        {
            this.reports.Add(report);
            if (this.reports.Count > 50)
            {
                this.reports.RemoveAt(0);
            }
        }

        public void CopyLevel(Player player, string key2 = "")
        {
            this.GetUniqueId(this.isbetalevel,
                delegate (string key)
                {
                    this.PlayerIO.BigDB.CreateObject("Worlds", key2 == "" ? key : key2,
                        this.BaseWorld.GetDatabaseObject(),
                        delegate (DatabaseObject o)
                        {
                            player.SendMessage(Message.Create("write", ChatUtils.SystemName,
                                "Room has been saved as: " + o.Key));
                        });
                });
        }

        private void BroadcastMetaData(Player avoid = null)
        {
            foreach (var p in this.FilteredPlayers.Where(p => avoid == null || p != avoid))
            {
                p.SendMessage(Message.Create("updatemeta", this.LevelOwnerName, this.RoomData["name"],
                    this.BaseWorld.Plays, this.BaseWorld.Favorites, this.BaseWorld.Likes));
            }
        }

        // Copied from Lobby
        private void GetUniqueId(bool isbetaonly, Callback<string> myCallback)
        {
            string newid;
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
                    this.GetUniqueId(isbetaonly, myCallback);
                }
                else
                {
                    myCallback(newid);
                }
            });
        }

        public bool TrySetEditRights(Player p, bool editrights)
        {
            if (p.CanEdit == editrights)
            {
                return false;
            }

            p.CanEdit = editrights;

            if (!editrights)
            {
                p.CanToggleGodMode = false;
                if (p.IsInGodMode)
                {
                    p.IsInGodMode = false;
                    this.BroadcastMessage("god", p.Id, false);
                }
            }
            else
            {
                this.LeaveCampaignMode(p);
            }
            p.SendMessage(Message.Create(editrights ? "access" : "lostaccess"));

            foreach (var player in this.Players)
            {
                if (player.Id != p.Id && (player.IsAdmin || player.IsModerator || player.Owner))
                {
                    player.SendMessage("editRights", p.Id, editrights);
                }
            }
            return true;
        }

        public void LeaveCampaignMode(Player player)
        {
            if (player.IsInCampaignMode)
            {
                player.IsInCampaignMode = false;
                player.SendMessage("lockCampaign", this.BaseWorld.Campaign);
                player.SendMessage("write", ChatUtils.SystemName, "You left campaign mode.");
            }
        }

        private bool IsBrickAllowed(uint id, int layer)
        {
            return id <= 0 || layer == this.blockMap.GetBlockLayerById((int)id);
        }

        private void PlaceBrick(Player player, Message m)
        {
            if (!player.CanEdit)
            {
                return;
            }

            if (!player.IsAdmin && !player.Owner)
            {
                var totalMiliseconds = (DateTime.Now - player.LastEdit).TotalMilliseconds;
                var totalMessageTicks = (int)Math.Floor(totalMiliseconds / (1000.0 / Player.MessageLimit));
                if (totalMessageTicks < 1)
                {
                    player.Threshold -= 10;
                }
                else
                {
                    player.Threshold += 10 * totalMessageTicks;
                    player.Threshold = Math.Min(150, player.Threshold);
                }

                if (player.Threshold < 0)
                {
                    player.Threshold = 0;
                    return;
                }
            }

            player.LastEdit = DateTime.Now;

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

            if (!this.IsBrickAllowed(brick, layerNum))
            {
                if (this.BaseWorld.IsArtContest && brick < 2500)
                    return;
                if (!this.BaseWorld.IsArtContest)
                    return;
            }

            if (!player.IsAdmin)
            {
                // General bounds check on allowed brick id
                // For open worlds, check if block is part of Open Worlds block subset
                if (!this.lockedroom && !this.blockMap.BlockIsLegitOpenWorld(player, (int)brick))
                {
                    return;
                }

                if (!this.IsBrickAllowed(brick, layerNum))
                {
                    return;
                }
            }
            //var margin = 1;

            //if (brick > 0)
            //    margin = 0;

            if (cx >= 0 && cx < this.BaseWorld.Width && cy < this.BaseWorld.Height &&
                (this.editkey != "" ? cy >= 0 : cy > 4))
            {
                switch (brick)
                {
                    case (int)ItemTypes.Piano:
                    {
                        if (this.Owned && player.HasBrickPack("bricknode"))
                        {
                            this.SetBrickSound(ItemTypes.Piano, cx, cy, m.GetInt(4), (uint)player.Id);
                        }
                        break;
                    }
                    case (int)ItemTypes.Drums:
                    {
                        if (this.Owned && player.HasBrickPack("brickdrums"))
                        {
                            this.SetBrickSound(ItemTypes.Drums, cx, cy, m.GetInt(4), (uint)player.Id);
                        }
                        break;
                    }
                    //Admin label
                    case 1000:
                    {
                        if (this.Owned && (player.IsAdmin || player.IsModerator || player.IsStaff))
                        {
                            this.SetBrickLabel(cx, cy, m.GetString(4), m.GetString(5), m.GetUInt(6), (uint)player.Id);
                        }
                        break;
                    }


                    case 255:
                    {
                        // Spawn point
                        if (this.Owned)
                        {
                            this.SetBrick(layerNum, cx, cy, brick, (uint)player.Id);
                        }
                        break;
                    }

                    case (int)ItemTypes.Checkpoint:
                    {
                        if (this.Owned)
                        {
                            this.SetBrick(layerNum, cx, cy, brick, (uint)player.Id);
                        }
                        break;
                    }
                    case (int)ItemTypes.Fire:
                    {
                        if (this.Owned && player.HasBrickPack("brickfire"))
                        {
                            this.SetBrick(layerNum, cx, cy, brick, (uint)player.Id);
                        }
                        break;
                    }
                    case (int)ItemTypes.ResetPoint:
                    {
                        if (this.Owned)
                        {
                            this.SetBrick(layerNum, cx, cy, brick, (uint)player.Id);
                        }
                        break;
                    }
                    case (int)ItemTypes.Spike:
                    {
                        if (this.Owned && player.HasBrickPack("brickspike"))
                        {
                            var rotation = (uint)m.GetInt(4);
                            if (rotation >= 4)
                            {
                                rotation = 0;
                            }
                            this.SetBrickRotateable(layerNum, cx, cy, brick, rotation, (uint)player.Id);
                        }
                        break;
                    }

                    case (int)ItemTypes.GlowyLineBlueStraight:
                    case (int)ItemTypes.GlowyLineBlueSlope:
                    case (int)ItemTypes.GlowyLineGreenSlope:
                    case (int)ItemTypes.GlowyLineGreenStraight:
                    case (int)ItemTypes.GlowyLineYellowSlope:
                    case (int)ItemTypes.GlowyLineYellowStraight:
                    case (int)ItemTypes.GlowyLineRedSlope:
                    case (int)ItemTypes.GlowyLineRedStraight:
                    {
                        if (player.HasBrickPack("brickscifi"))
                        {
                            var rotation = (uint)m.GetInt(4);
                            if (rotation >= 4)
                            {
                                rotation = 0;
                            }
                            this.SetBrickRotateable(layerNum, cx, cy, brick, rotation, (uint)player.Id);
                        }
                        break;
                    }
                    case (int)ItemTypes.OnewayCyan:
                    case (int)ItemTypes.OnewayOrange:
                    case (int)ItemTypes.OnewayYellow:
                    case (int)ItemTypes.OnewayPink:
                    case (int)ItemTypes.OnewayGray:
                    case (int)ItemTypes.OnewayBlue:
                    case (int)ItemTypes.OnewayRed:
                    case (int)ItemTypes.OnewayGreen:
                    case (int)ItemTypes.OnewayBlack:
                    case (int)ItemTypes.OnewayWhite:
                    {
                        if (player.HasBrickPack("brickoneway"))
                        {
                            var rotation = (uint)m.GetInt(4);
                            if (rotation >= 4)
                            {
                                rotation = 0;
                            }
                            this.SetBrickRotateable(layerNum, cx, cy, brick, rotation, (uint)player.Id);
                        }
                        break;
                    }
                    case (int)ItemTypes.MedievalAxe:
                    case (int)ItemTypes.MedievalBanner:
                    case (int)ItemTypes.MedievalCoatOfArms:
                    case (int)ItemTypes.MedievalShield:
                    case (int)ItemTypes.MedievalSword:
                    {
                        if (player.HasBrickPack("brickmedieval"))
                        {
                            var rotation = (uint)m.GetInt(4);
                            if (rotation >= 4)
                            {
                                rotation = 0;
                            }
                            this.SetBrickRotateable(layerNum, cx, cy, brick, rotation, (uint)player.Id);
                        }
                        break;
                    }
                    case (int)ItemTypes.MedievalTimber:
                    {
                        if (player.HasBrickPack("brickmedieval"))
                        {
                            var rotation = (uint)m.GetInt(4);
                            if (rotation >= 6)
                            {
                                rotation = 0;
                            }
                            this.SetBrickRotateable(layerNum, cx, cy, brick, rotation, (uint)player.Id, 6);
                        }
                        break;
                    }
                    case (int)ItemTypes.ToothBig:
                    case (int)ItemTypes.ToothSmall:
                    case (int)ItemTypes.ToothTriple:
                    {
                        if (player.HasBrickPack("brickmonster"))
                        {
                            var rotation = (uint)m.GetInt(4);
                            if (rotation >= 4)
                            {
                                rotation = 0;
                            }
                            this.SetBrickRotateable(layerNum, cx, cy, brick, rotation, (uint)player.Id);
                        }
                        break;
                    }
                    case (int)ItemTypes.DojoLightLeft:
                    case (int)ItemTypes.DojoLightRight:
                    case (int)ItemTypes.DojoDarkLeft:
                    case (int)ItemTypes.DojoDarkRight:
                    {
                        if (player.HasBrickPack("brickninja"))
                        {
                            var rotation = (uint)m.GetInt(4);
                            if (rotation >= 3)
                            {
                                rotation = 0;
                            }
                            this.SetBrickRotateable(layerNum, cx, cy, brick, rotation, (uint)player.Id, 3);
                        }
                        break;
                    }
                    case (int)ItemTypes.DomesticLightBulb:
                    case (int)ItemTypes.DomesticTap:
                    case (int)ItemTypes.DomesticPainting:
                    case (int)ItemTypes.DomesticVase:
                    case (int)ItemTypes.DomesticTv:
                    case (int)ItemTypes.DomesticWindow:
                    case (int)ItemTypes.HalfBlockDomesticBrown:
                    case (int)ItemTypes.HalfBlockDomesticWhite:
                    case (int)ItemTypes.HalfBlockDomesticYellow:
                    {
                        if (player.HasBrickPack("brickdomestic"))
                        {
                            var rotation = (uint)m.GetInt(4);
                            if (rotation >= 4)
                            {
                                rotation = 0;
                            }

                            this.SetBrickRotateable(layerNum, cx, cy, brick, rotation, (uint)player.Id);
                        }
                        break;
                    }
                    case (int)ItemTypes.HalfBlockWhite:
                    case (int)ItemTypes.HalfBlockGray:
                    case (int)ItemTypes.HalfBlockBlack:
                    case (int)ItemTypes.HalfBlockRed:
                    case (int)ItemTypes.HalfBlockOrange:
                    case (int)ItemTypes.HalfBlockYellow:
                    case (int)ItemTypes.HalfBlockGreen:
                    case (int)ItemTypes.HalfBlockCyan:
                    case (int)ItemTypes.HalfBlockBlue:
                    case (int)ItemTypes.HalfBlockPurple:
                    {
                        if (player.HasBrickPack("brickhalfblocks"))
                        {
                            var rotation = (uint)m.GetInt(4);
                            if (rotation >= 4)
                            {
                                rotation = 0;
                            }

                            this.SetBrickRotateable(layerNum, cx, cy, brick, rotation, (uint)player.Id);
                        }
                        break;
                    }
                    case (int)ItemTypes.Halloween2015WindowRect:
                    case (int)ItemTypes.Halloween2015WindowCircle:
                    case (int)ItemTypes.Halloween2015Lamp:
                    {
                        if (player.HasBrickPack("brickhalloween2015"))
                        {
                            var rotation = (uint)m.GetInt(4);
                            if (rotation >= 2)
                            {
                                rotation = 0;
                            }

                            this.SetBrickRotateable(layerNum, cx, cy, brick, rotation, (uint)player.Id, 2);
                        }
                        break;
                    }
                    case (int)ItemTypes.NewYear2015Balloon:
                    case (int)ItemTypes.NewYear2015Streamer:
                    {
                        if (player.HasBrickPack("bricknewyear2015"))
                        {
                            var rotation = (uint)m.GetInt(4);
                            if (rotation >= 5)
                            {
                                rotation = 0;
                            }

                            this.SetBrickRotateable(layerNum, cx, cy, brick, rotation, (uint)player.Id, 5);
                        }
                        break;
                    }

                    case (int)ItemTypes.FairytaleFlowers:
                    case (int)ItemTypes.HalfBlockFairytaleOrange:
                    case (int)ItemTypes.HalfBlockFairytaleGreen:
                    case (int)ItemTypes.HalfBlockFairytaleBlue:
                    case (int)ItemTypes.HalfBlockFairytalePink:
                    {
                        if (player.HasBrickPack("brickfairytale"))
                        {
                            var maxRotation = (brick == (uint)ItemTypes.FairytaleFlowers ? 3 : 4);
                            var rotation = (uint)m.GetInt(4);
                            if (rotation >= maxRotation)
                            {
                                rotation = 0;
                            }

                            this.SetBrickRotateable(layerNum, cx, cy, brick, rotation, (uint)player.Id);
                        }
                        break;
                    }

                    case (int)ItemTypes.SpringDaisy:
                    case (int)ItemTypes.SpringTulip:
                    case (int)ItemTypes.SpringDaffodil:
                    {
                        if (player.HasBrickPack("brickspring2016"))
                        {
                            var rotation = (uint)m.GetInt(4);
                            if (rotation >= 3)
                            {
                                rotation = 0;
                            }

                            this.SetBrickRotateable(layerNum, cx, cy, brick, rotation, (uint)player.Id, 3);
                        }
                        break;
                    }

                    case (uint)ItemTypes.SummerFlag:
                    case (uint)ItemTypes.SummerAwning:
                    {
                        if (player.HasBrickPack("bricksummer2016"))
                        {
                            var rotation = (uint)m.GetInt(4);
                            if (rotation >= 6)
                            {
                                rotation = 0;
                            }

                            this.SetBrickRotateable(layerNum, cx, cy, brick, rotation, (uint)player.Id, 6);
                        }
                        break;
                    }

                    case (uint)ItemTypes.SummerIceCream:
                    {
                        if (player.HasBrickPack("bricksummer2016"))
                        {
                            var rotation = (uint)m.GetInt(4);
                            if (rotation >= 4)
                            {
                                rotation = 0;
                            }

                            this.SetBrickRotateable(layerNum, cx, cy, brick, rotation, (uint)player.Id, 4);
                        }
                        break;
                    }

                    case (int)ItemTypes.Portal:
                    {
                        if (this.Owned && player.HasBrickPack("brickportal"))
                        {
                            var rotation = (uint)m.GetInt(4);
                            var id = (uint)m.GetInt(5);
                            var target = (uint)m.GetInt(6);
                            if (rotation >= 4)
                            {
                                rotation -= 4;
                            }

                            this.SetBrick(cx, cy, brick, rotation, id, target, (uint)player.Id);
                        }
                        break;
                    }

                    case (int)ItemTypes.WorldPortal:
                    {
                        if (this.Owned && (player.Owner || player.IsAdmin) &&
                        (player.GetBrickPackCount("brickworldportal") > this.BaseWorld.WorldPortalCount ||
                         this.BaseWorld.GetBrickType(0, cx, cy) == (int)ItemTypes.WorldPortal))
                        {
                            var target = m.GetString(4);
                            this.SetBrickWorldPortal(cx, cy, brick, target, (uint)player.Id);
                        }
                        break;
                    }

                    case (int)ItemTypes.TextSign:
                    {
                        var signText = ChatUtils.RemoveBadCharacters(m.GetString(4));
                        var signType = (uint)m.GetInt(5);

                        if (this.Owned && player.HasBrickPack("bricksign"))
                        {
                            if (signType > (player.HasGoldMembership ? 3 : 2))
                                signType = 0;

                            this.SetBrickTextSign(cx, cy, signText, signType, (uint)player.Id);
                        }
                        break;
                    }

                    case (int)ItemTypes.PortalInvisible:
                    {
                        if (this.Owned && player.HasBrickPack("brickinvisibleportal"))
                        {
                            var rotation = (uint)m.GetInt(4);
                            var id = (uint)m.GetInt(5);
                            var target = (uint)m.GetInt(6);

                            if (rotation >= 4)
                            {
                                rotation -= 4;
                            }

                            this.SetBrick(cx, cy, brick, rotation, id, target, (uint)player.Id);
                        }
                        break;
                    }

                    case 241:
                    {
                        if (this.Owned && (player.Owner || player.IsAdmin) &&
                            player.GetBrickPackCount("brickdiamond") > this.BaseWorld.DiamondCount)
                        {
                            this.SetBrick(layerNum, cx, cy, brick, (uint)player.Id);
                        }
                        break;
                    }

                    case (int)ItemTypes.Cake:
                    {
                        if (this.Owned && (player.Owner || player.IsAdmin) &&
                            player.GetBrickPackCount("brickcake") > this.BaseWorld.CakesCount)
                        {
                            this.SetBrick(layerNum, cx, cy, brick, (uint)player.Id);
                        }

                        break;
                    }

                    case (int)ItemTypes.Hologram:
                    {
                        if (this.Owned && (player.Owner || player.IsAdmin) &&
                            player.GetBrickPackCount("brickhologram") > this.BaseWorld.HologramsCount)
                        {
                            this.SetBrick(layerNum, cx, cy, brick, (uint)player.Id);
                        }

                        break;
                    }

                    case (uint)ItemTypes.Lava:
                    {
                        if (this.Owned && player.HasBrickPack("bricklava"))
                        {
                            this.SetBrick(layerNum, cx, cy, brick, (uint)player.Id);
                        }
                        break;
                    }

                    case (uint)ItemTypes.CoinGate:
                    case (uint)ItemTypes.CoinDoor:
                    {
                        var count = (uint)m.GetInt(4);
                        if (count > 999)
                        {
                            count = 999;
                        }
                        if (this.Owned)
                        {
                            this.SetBrick(cx, cy, brick, count, (uint)player.Id);
                        }
                        break;
                    }

                    case (uint)ItemTypes.BlueCoinGate:
                    case (uint)ItemTypes.BlueCoinDoor:
                    {
                        var count = (uint)m.GetInt(4);
                        if (count > 999)
                        {
                            count = 999;
                        }
                        if (this.Owned)
                        {
                            this.SetBrick(cx, cy, brick, count, (uint)player.Id);
                        }
                        break;
                    }

                    case (uint)ItemTypes.DeathGate:
                    case (uint)ItemTypes.DeathDoor:
                    {
                        var count = (uint)m.GetInt(4);
                        if (count > 999)
                        {
                            count = 999;
                        }
                        if (this.Owned && player.HasBrickPack("brickdeathdoor"))
                        {
                            this.SetBrick(cx, cy, brick, count, (uint)player.Id);
                        }
                        break;
                    }


                    case (uint)ItemTypes.Complete:
                    {
                        if (this.Owned)
                        {
                            this.SetBrick(layerNum, cx, cy, brick, (uint)player.Id);
                        }
                        break;
                    }
                    case (uint)ItemTypes.TimeGate:
                    case (uint)ItemTypes.TimeDoor:
                    {
                        if (this.Owned && player.HasBrickPack("bricktimeddoor"))
                        {
                            this.SetBrick(layerNum, cx, cy, brick, (uint)player.Id);
                        }
                        break;
                    }
                    case (uint)ItemTypes.ZombieGate:
                    case (uint)ItemTypes.ZombieDoor:
                    {
                        if (this.Owned && player.HasBrickPack("brickeffectzombie"))
                        {
                            this.SetBrick(layerNum, cx, cy, brick, (uint)player.Id);
                        }
                        break;
                    }
                    case (uint)ItemTypes.SwitchPurple:
                    case (uint)ItemTypes.GatePurple:
                    case (uint)ItemTypes.DoorPurple:
                    {
                        var count = (uint)m.GetInt(4);
                        if (count > 999)
                        {
                            count = 999;
                        }
                        if (this.Owned && player.HasBrickPack("brickswitchpurple"))
                        {
                            this.SetBrick(cx, cy, brick, count, (uint)player.Id);
                        }
                        break;
                    }
                    case (uint)ItemTypes.SwitchOrange:
                    case (uint)ItemTypes.GateOrange:
                    case (uint)ItemTypes.DoorOrange:
                    {
                        var count = (uint)m.GetInt(4);
                        if (count > 999)
                        {
                            count = 999;
                        }
                        if (this.Owned && player.HasBrickPack("brickswitchorange"))
                        {
                            this.SetBrick(cx, cy, brick, count, (uint)player.Id);
                        }
                        break;
                    }
                    case (uint)ItemTypes.EffectTeam:
                    {
                        var count = (uint)m.GetInt(4);
                        if (count > 6)
                        {
                            count = 6;
                        }
                        if (this.Owned && player.HasBrickPack("brickeffectteam"))
                        {
                            this.SetBrick(cx, cy, brick, count, (uint)player.Id);
                        }
                        break;
                    }
                    case (uint)ItemTypes.TeamDoor:
                    case (uint)ItemTypes.TeamGate:
                    {
                        var count = (uint)m.GetInt(4);
                        if (count > 6)
                        {
                            count = 6;
                        }
                        if (this.Owned && player.HasBrickPack("brickeffectteam"))
                        {
                            this.SetBrick(cx, cy, brick, count, (uint)player.Id);
                        }
                        break;
                    }
                    case (uint)ItemTypes.EffectCurse:
                    case (uint)ItemTypes.EffectZombie:
                    {
                        var count = (uint)m.GetInt(4);
                        if (count > 999)
                        {
                            count = 999;
                        }
                        if (this.Owned &&
                             ((brick == (int)ItemTypes.EffectCurse && player.HasBrickPack("brickeffectcurse")) ||
                              (brick == (int)ItemTypes.EffectZombie && player.HasBrickPack("brickeffectzombie"))))
                        {
                            this.SetBrick(cx, cy, brick, count, (uint)player.Id);
                        }
                        break;
                    }
                    case (uint)ItemTypes.EffectFly:
                    case (uint)ItemTypes.EffectJump:
                    case (uint)ItemTypes.EffectProtection:
                    case (uint)ItemTypes.EffectRun:
                    case (uint)ItemTypes.EffectLowGravity:
                    {
                        var count = (uint)m.GetInt(4);
                        if (count > 1)
                        {
                            count = 1;
                        }
                        if (this.Owned &&
                             (brick == (int)ItemTypes.EffectFly && player.HasBrickPack("brickeffectfly")) ||
                             (brick == (int)ItemTypes.EffectJump && player.HasBrickPack("brickeffectjump")) ||
                             (brick == (int)ItemTypes.EffectProtection && player.HasBrickPack("brickeffectprotection")) ||
                             (brick == (int)ItemTypes.EffectRun && player.HasBrickPack("brickeffectspeed")) ||
                             (brick == (int)ItemTypes.EffectLowGravity && player.HasBrickPack("brickeffectlowgravity")))
                        {
                            this.SetBrick(cx, cy, brick, count, (uint)player.Id);
                        }
                        break;
                    }

                    case (uint)ItemTypes.EffectGravity:
                    {
                        var count = (uint)m.GetInt(4);
                        if (count > 4)
                            count = 4;

                        if (this.Owned && (brick == (int)ItemTypes.EffectGravity && player.HasBrickPack("brickeffectgravity")))
                        {
                            this.SetBrickRotateable(0, cx, cy, brick, count, (uint)player.Id, 5);
                        }
                        break;
                    }
                    case (uint)ItemTypes.EffectMultijump:
                    {
                        var count = (uint)m.GetInt(4);
                        if (count > 999)
                            count = 1000;

                        if (this.Owned && brick == (int)ItemTypes.EffectMultijump && player.PayVault.Has("brickeffectmultijump"))
                        {
                            this.SetBrick(cx, cy, brick, count, (uint)player.Id);
                        }
                        break;
                    }

                    case 1065:
                    case 1066:
                    case 1067:
                    case 1068:
                    case 1069:
                    case 709:
                    case 710:
                    case 711:
                    {
                        if ((player.Owner || player.IsAdmin) && player.HasGoldMembership)
                        {
                            this.SetBrick(layerNum, cx, cy, brick, (uint)player.Id);
                        }
                        break;
                    }

                    case (int)ItemTypes.CaveCrystal:
                    {
                        if (player.HasBrickPack("brickmine"))
                        {
                            var rotation = (uint)m.GetInt(4);
                            if (rotation >= 6)
                            {
                                rotation = 0;
                            }

                            this.SetBrickRotateable(layerNum, cx, cy, brick, rotation, (uint)player.Id, 6);
                        }
                        break;
                    }
                    case (int)ItemTypes.RestaurantBowl:
                    case (int)ItemTypes.RestaurantPlate:
                    case (int)ItemTypes.RestaurantCup:
                    {
                        if (player.HasBrickPack("brickrestaurant"))
                        {
                            var rotation = (uint)m.GetInt(4);
                            if (rotation >= 4)
                            {
                                rotation = 0;
                            }

                            this.SetBrickRotateable(layerNum, cx, cy, brick, rotation, (uint)player.Id, 4);
                        }
                        break;
                    }

                    case (uint)ItemTypes.Halloween2016Eyes:
                    case (uint)ItemTypes.Halloween2016Rotatable:
                    {
                        if (player.HasBrickPack("brickhalloween2016"))
                        {
                            var rotation = (uint)m.GetInt(4);
                            if (rotation >= 4)
                            {
                                rotation = 0;
                            }

                            this.SetBrickRotateable(layerNum, cx, cy, brick, rotation, (uint)player.Id, 4);
                        }
                        break;
                    }

                    case (uint)ItemTypes.Halloween2016Pumpkin:
                    {
                        if (player.HasBrickPack("brickhalloween2016"))
                        {
                            var rotation = (uint)m.GetInt(4);
                            if (rotation >= 2)
                            {
                                rotation = 0;
                            }

                            this.SetBrickRotateable(layerNum, cx, cy, brick, rotation, (uint)player.Id, 2);
                        }
                        break;
                    }

                    case (int)ItemTypes.Christmas2016LightsUp:
                    case (int)ItemTypes.Christmas2016LightsDown:
                    {
                        if (player.HasBrickPack("brickchristmas2016"))
                        {
                            var rotation = (uint)m.GetInt(4);
                            if (rotation >= 5)
                            {
                                rotation = 0;
                            }

                            this.SetBrickRotateable(layerNum, cx, cy, brick, rotation, (uint)player.Id, 5);
                        }
                        break;
                    }

                    case (int)ItemTypes.HalfBlockChristmas2016PresentRed:
                    case (int)ItemTypes.HalfBlockChristmas2016PresentGreen:
                    case (int)ItemTypes.HalfBlockChristmas2016PresentWhite:
                    case (int)ItemTypes.HalfBlockChristmas2016PresentBlue:
                    case (int)ItemTypes.HalfBlockChristmas2016PresentYellow:
                    {
                        if (player.HasBrickPack("brickchristmas2016"))
                        {
                            var rotation = (uint)m.GetInt(4);
                            if (rotation != 1)
                            {
                                rotation = 1;
                            }

                            this.SetBrickRotateable(layerNum, cx, cy, brick, rotation, (uint)player.Id);
                        }
                        break;
                    }

                    case (int)ItemTypes.GodBlock:
                    {
                        if (this.Owned && (player.Owner || player.IsAdmin) && player.HasBrickPack("brickgodblock"))
                        {
                            this.SetBrick(layerNum, cx, cy, brick, (uint)player.Id);
                        }
                        break;
                    }

                    case (int)ItemTypes.Guitar:
                    {
                        if (this.Owned && player.HasBrickPack("brickguitar"))
                        {
                            this.SetBrickSound(ItemTypes.Guitar, cx, cy, m.GetInt(4), (uint)player.Id);
                        }
                        break;
                    }

                    case (uint)ItemTypes.IndustrialPipeThick:
                    case (uint)ItemTypes.IndustrialPipeThin:
                    {
                        if (player.HasBrickPack("brickindustrial"))
                        {
                            var rotation = (uint)m.GetInt(4);
                            if (rotation >= 2)
                            {
                                rotation = 0;
                            }

                            this.SetBrickRotateable(layerNum, cx, cy, brick, rotation, (uint)player.Id);
                        }
                        break;
                    }

                    case (uint)ItemTypes.IndustrialTable:
                    {
                        if (player.HasBrickPack("brickindustrial"))
                        {
                            var rotation = (uint)m.GetInt(4);
                            if (rotation >= 3)
                            {
                                rotation = 0;
                            }

                            this.SetBrickRotateable(layerNum, cx, cy, brick, rotation, (uint)player.Id);
                        }
                        break;
                    }

                    case (uint)ItemTypes.DomesticPipeStraight:
                    {
                        if (player.HasBrickPack("brickdomestic"))
                        {
                            var rotation = (uint)m.GetInt(4);
                            if (rotation >= 2)
                            {
                                rotation = 0;
                            }

                            this.SetBrickRotateable(layerNum, cx, cy, brick, rotation, (uint)player.Id);
                        }
                        break;
                    }

                    case (uint)ItemTypes.DomesticPipeT:
                    {
                        if (player.HasBrickPack("brickdomestic"))
                        {
                            var rotation = (uint)m.GetInt(4);
                            if (rotation >= 4)
                            {
                                rotation = 0;
                            }

                            this.SetBrickRotateable(layerNum, cx, cy, brick, rotation, (uint)player.Id);
                        }
                        break;
                    }

                    case (uint)ItemTypes.DomesticFrameBorder:
                    {
                        if (player.HasBrickPack("brickdomestic"))
                        {
                            var rotation = (uint)m.GetInt(4);
                            if (rotation >= 11)
                                rotation = 0;

                            this.SetBrickRotateable(layerNum, cx, cy, brick, rotation, (uint)player.Id, 11);
                        }
                        break;
                    }

                    default:
                    {
                        if (brick >= 2500 && brick <= 2500 + artContest.MaximumBlockAssets && this.BaseWorld.IsArtContest)
                        {
                            this.SetBrick(layerNum, cx, cy, brick, (uint)player.Id);
                            break;
                        }

                        if (this.blockMap.BlockIsLegit(player, (int)brick))
                        {
                            this.SetBrick(layerNum, cx, cy, brick, (uint)player.Id);
                        }

                        break;
                    }
                }
            }
        }

        private void SetBrick(int layerNum, uint x, uint y, uint brick, uint playerid, bool broadcast = true)
        {
            if (!this.BaseWorld.SetNormal(layerNum, x, y, brick))
            {
                return;
            }

            if (broadcast)
            {
                this.BroadcastMessage(Message.Create("b", layerNum, x, y, brick, playerid));
            }

            if (brick == (uint)ItemTypes.Checkpoint)
            {
                this.RemoveCheckpoint(x, y);
            }
        }

        private void RemoveCheckpoint(uint x, uint y)
        {
            foreach (var player in this.Players)
            {
                if (player.Checkpoint.X == x && player.Checkpoint.Y == y)
                {
                    player.Checkpoint = this.BaseWorld.GetSpawn();
                }
            }
        }

        private void SetBrick(uint x, uint y, uint brick, uint goal, uint playerid)
        {
            // Coin doors and gates
            if (brick == (uint)ItemTypes.CoinDoor && this.BaseWorld.SetBrickCoindoor(x, y, goal))
            {
                this.BroadcastMessage(Message.Create("bc", x, y, brick, goal, playerid));
            }
            if (brick == (uint)ItemTypes.CoinGate && this.BaseWorld.SetBrickCoingate(x, y, goal))
            {
                this.BroadcastMessage(Message.Create("bc", x, y, brick, goal, playerid));
            }
            if (brick == (uint)ItemTypes.BlueCoinDoor && this.BaseWorld.SetBrickBlueCoindoor(x, y, goal))
            {
                this.BroadcastMessage(Message.Create("bc", x, y, brick, goal, playerid));
            }
            if (brick == (uint)ItemTypes.BlueCoinGate && this.BaseWorld.SetBrickBlueCoingate(x, y, goal))
            {
                this.BroadcastMessage(Message.Create("bc", x, y, brick, goal, playerid));
            }

            // Death doors and gates
            if (brick == (uint)ItemTypes.DeathDoor && this.BaseWorld.SetBrickDeathDoor(x, y, goal))
            {
                this.BroadcastMessage(Message.Create("bc", x, y, brick, goal, playerid));
            }
            if (brick == (uint)ItemTypes.DeathGate && this.BaseWorld.SetBrickDeathGate(x, y, goal))
            {
                this.BroadcastMessage(Message.Create("bc", x, y, brick, goal, playerid));
            }

            // Purple switches with ids
            if (brick == (uint)ItemTypes.DoorPurple && this.BaseWorld.SetBrickDoorPurple(x, y, goal))
            {
                this.BroadcastMessage(Message.Create("bc", x, y, brick, goal, playerid));
            }
            if (brick == (uint)ItemTypes.GatePurple && this.BaseWorld.SetBrickGatePurple(x, y, goal))
            {
                this.BroadcastMessage(Message.Create("bc", x, y, brick, goal, playerid));
            }
            if (brick == (uint)ItemTypes.SwitchPurple && this.BaseWorld.SetBrickSwitchPurple(x, y, goal))
            {
                this.BroadcastMessage(Message.Create("bc", x, y, brick, goal, playerid));
            }

            // Orange switches with ids
            if (brick == (uint)ItemTypes.DoorOrange && this.BaseWorld.SetBrickDoorOrange(x, y, goal))
            {
                this.BroadcastMessage(Message.Create("bc", x, y, brick, goal, playerid));
            }
            if (brick == (uint)ItemTypes.GateOrange && this.BaseWorld.SetBrickGateOrange(x, y, goal))
            {
                this.BroadcastMessage(Message.Create("bc", x, y, brick, goal, playerid));
            }
            if (brick == (uint)ItemTypes.SwitchOrange && this.BaseWorld.SetBrickSwitchOrange(x, y, goal))
            {
                this.BroadcastMessage(Message.Create("bc", x, y, brick, goal, playerid));
            }

            // Team effect, doors and gates
            if (brick == (uint)ItemTypes.EffectTeam && this.BaseWorld.SetBrickTeamEffect(x, y, goal))
            {
                this.BroadcastMessage(Message.Create("bc", x, y, brick, goal, playerid));
            }
            if (brick == (uint)ItemTypes.TeamDoor && this.BaseWorld.SetBrickTeamDoor(x, y, goal))
            {
                this.BroadcastMessage(Message.Create("bc", x, y, brick, goal, playerid));
            }
            if (brick == (uint)ItemTypes.TeamGate && this.BaseWorld.SetBrickTeamGate(x, y, goal))
            {
                this.BroadcastMessage(Message.Create("bc", x, y, brick, goal, playerid));
            }

            if ((brick == (uint)ItemTypes.EffectCurse || brick == (uint)ItemTypes.EffectZombie) &&
                this.BaseWorld.SetBrickWithDuration(brick, x, y, goal))
            {
                this.BroadcastMessage(Message.Create("bc", x, y, brick, goal, playerid));
            }

            if ((brick == (uint)ItemTypes.EffectFly ||
                 brick == (uint)ItemTypes.EffectJump ||
                 brick == (uint)ItemTypes.EffectProtection ||
                 brick == (uint)ItemTypes.EffectRun ||
                 brick == (uint)ItemTypes.EffectLowGravity) &&
                this.BaseWorld.SetBrickWithOnStatus(brick, x, y, goal))
            {
                this.BroadcastMessage(Message.Create("bc", x, y, brick, goal, playerid));
            }

            if (brick == (uint)ItemTypes.EffectMultijump && this.BaseWorld.SetBrickMultijump(brick, x, y, goal))
            {
                this.BroadcastMessage("bc", x, y, brick, goal, playerid);
            }
        }

        private void SetBrickSound(ItemTypes type, uint x, uint y, int offset, uint playerid)
        {
            if (this.BaseWorld.SetBrickSound(type, x, y, offset))
            {
                this.BroadcastMessage(Message.Create("bs", x, y, (uint)type, offset, playerid));
            }
        }

        private void SetBrickLabel(uint x, uint y, string text, string color, uint wrapLength, uint playerid)
        {
            if (this.BaseWorld.SetBrickLabel(x, y, text, color, wrapLength))
            {
                this.BroadcastMessage(Message.Create("lb", x, y, 1000, text, color, playerid));
            }
        }

        private void SetBrickTextSign(uint x, uint y, string text, uint type, uint playerid)
        {
            var str = ChatUtils.RemoveBadCharacters(text);
            if (str.Trim() == "")
            {
                return;
            }
            if (str.Length > 140)
            {
                str = str.Remove(140);
            }

            if (this.BaseWorld.SetBrickTextSign(x, y, str, type))
            {
                this.BroadcastMessage(Message.Create("ts", x, y, (uint)ItemTypes.TextSign, str, type, playerid));
            }
        }

        private void SetBrickWorldPortal(uint x, uint y, uint brick, string target, uint playerid)
        {
            if (this.BaseWorld.SetBrickWorldPortal(x, y, target))
            {
                this.BroadcastMessage(Message.Create("wp", x, y, brick, target, playerid));
            }
        }

        // Portals
        private void SetBrick(uint x, uint y, uint brick, uint rotation, uint id, uint target, uint playerid)
        {
            if (this.BaseWorld.SetBrickPortal(brick, x, y, rotation, id, target))
            {
                this.BroadcastMessage(Message.Create("pt", x, y, brick, rotation, id, target, playerid));
            }
        }

        private void SetBrickRotateable(int layerNum, uint x, uint y, uint brick, uint rotation, uint playerid,
            uint rotations = 4)
        {
            if (this.BaseWorld.SetBrickRotateable(layerNum, x, y, brick, rotation, rotations))
            {
                this.BroadcastMessage(Message.Create("br", x, y, brick, rotation, layerNum, playerid));
            }
        }

        private void SendInitMessage(Player player)
        {
            Console.WriteLine("sendInitMessage 1 ");
            if (player.Disconnected)
            {
                return;
            }

            Console.WriteLine("sendInitMessage 2");
            Console.WriteLine("player is stealth: " + player.Stealthy);
            var d = this.BaseWorld.GetSpawn();
            player.X = d.X * 16;
            player.Y = d.Y * 16;
            player.Checkpoint = d;

            var isCampaignRoom = this.IsCampaign;
            var crewWorldStatus = this.BaseWorld.Status;
            var isCrewMember = this.Crew.IsMember(player);

            if (isCampaignRoom)
            {
                player.CanEdit = false;
                player.Owner = false;
            }
            else if (player.ConnectUserId == this.BaseWorld.OwnerId)
            {
                player.CanEdit = true;
                player.Owner = true;
            }
            else if (isCrewMember)
            {
                var powers = this.Crew.GetPowersForPlayer(player);
                if (this.BaseWorld.IsCrewLogo)
                {
                    if (powers.Contains(CrewPower.LogoWorldAccess))
                    {
                        player.CanEdit = true;
                        player.CanChangeWorldOptions = true;
                    }
                }
                else
                {
                    if (powers.Contains(CrewPower.AutoEdit))
                    {
                        player.CanEdit = true;
                    }
                    if (powers.Contains(CrewPower.WorldSettingsAccess) && crewWorldStatus != WorldStatus.Released)
                    {
                        player.CanChangeWorldOptions = true;
                    }
                }
            }

            var owners = new List<Player>();
            var count = 0;
            foreach (var p in this.Players)
            {
                if (p.ConnectUserId == player.ConnectUserId)
                {
                    count++;
                }
                if (p.Owner)
                {
                    owners.Add(p);
                }
            }

            if (!this.BaseWorld.CrewVisibleInLobby && !isCrewMember && !player.IsAdmin && !player.IsModerator && !player.Owner)
            {
                if (!this.BaseWorld.IsArtContest || (this.BaseWorld.IsArtContest && !player.IsJudge))
                {
                    //player.Send("info", "World not available", "The requested world can only be accessed by members of " + this.Crew.Name + ".");
                    //player.Disconnect();
                    //foreach (var p in owners)
                    //{
                    //    p.Send("write", ChatUtils.SystemName, player.Name.ToUpper() + " tried to join.");
                    //}
                    //return;
                }
            }

            if (player.Stealthy)
            {
                player.ChatColor = 1337420;
                player.Send("write", ChatUtils.SystemName, "You're in Stealthy Mode!");
            }

            if (this.BaseWorld.FriendsOnly && !player.Owner)
            {
                if (player.IsAdmin || player.IsModerator)
                {
                    player.Send("write", ChatUtils.SystemName, "Joined friends only world.");
                }
                else
                {
                    if (!player.HasFriend(this.BaseWorld.OwnerId))
                    {
                        //player.Send("info", "World not available", "The requested world is set to Friends Only.");
                        //player.Disconnect();

                        return;
                    }
                }
            }
            else
            {
                if (!this.BaseWorld.Visible && !player.Owner && !isCrewMember)
                {
                    if (player.IsAdmin || player.IsModerator)
                    {
                        player.Send("write", ChatUtils.SystemName, "Joined invisible world.");
                    }
                    else
                    {
                        player.SendMessage(Message.Create("info", "World not available",
                            "The requested world is not set to visible."));

                        player.Disconnect();

                        foreach (var p in owners)
                        {
                            p.SendMessage("write", ChatUtils.SystemName, player.Name.ToUpper() + " tried to join.");
                        }
                        return;
                    }
                }
            }

            if (count >= 2 && !player.IsAdmin)
            {
                if (player.Owner)
                {
                    if (count >= 3)
                    {
                        //player.Send(Message.Create("info", "Limit reached", "To prevent abuse you can only be connected to your own world twice."));
                        //player.Disconnect();
                        return;
                    }
                }
                else if (player.IsGuest)
                {
                    if (count > 5)
                    {
                        //player.Send(Message.Create("info", "Limit reached", "Sorry, only 5 guests can be connected to the same world at the same time!"));
                        //player.Disconnect();
                        return;
                    }
                }
                else
                {
                    //player.Send(Message.Create("info", "Limit reached", "To prevent abuse you can only be connected to the same world once."));
                    //player.Disconnect();
                    return;
                }
            }

            if (!this.smileyMap.AuraIsLegit(player, player.Aura, player.AuraColor, this.shop))
            {
                player.Aura = 0;
                player.AuraColor = 0;
            }

            // Tell all connected clients to add the new player (if the new player isn't in stealth mode)
            if (!player.Stealthy)
                this.SendAddPlayer(player);

            var roomname = "Untitled World";
            if (this.RoomData.ContainsKey("name"))
            {
                roomname = this.RoomData["name"];
            }

            Callback initCallback = () =>
            {
                // Serialize world data and send it
                var initmessage = Message.Create("init",
                    this.LevelOwnerName ?? "",
                    roomname,
                    this.BaseWorld.Plays,
                    this.BaseWorld.Favorites,
                    this.BaseWorld.Likes,
                    player.Id,
                    player.Smiley,
                    player.Aura,
                    player.AuraColor,
                    player.SmileyGoldBorder,
                    player.X,
                    player.Y,
                    player.ChatColor,
                    player.Name,
                    player.CanEdit,
                    player.Owner,
                    player.HasInFavorites(this.RoomId),
                    player.Liked(this.RoomId),
                    this.BaseWorld.Width,
                    this.BaseWorld.Height,
                    this.BaseWorld.GravityMultiplier,
                    this.BaseWorld.BackgroundColor,
                    this.BaseWorld.Visible,
                    this.BaseWorld.HideLobby,
                    this.BaseWorld.AllowSpectating,
                    this.BaseWorld.WorldDescription,
                    this.BaseWorld.CurseLimit,
                    this.BaseWorld.ZombieLimit,
                    isCampaignRoom,
                    this.BaseWorld.Crew, this.Crew.Name,
                    player.CanChangeWorldOptions,
                    (int)crewWorldStatus,
                    player.Badge,
                    isCrewMember,
                    this.BaseWorld.MinimapEnabled,
                    this.BaseWorld.LobbyPreviewEnabled,
                    VarintConverter.GetVarintBytes(this.OrangeSwitches),
                    this.BaseWorld.FriendsOnly,
                    this.BaseWorld.IsArtContest
                    );

                this.BaseWorld.AddToMessageAsComplexList(initmessage);
                if (this.BaseWorld.IsArtContest)
                {
                    this.AddContestItemsToMessage(initmessage, this.BaseWorld.Crew, m =>
                    {
                        player.SendMessage(m);
                        player.Initialized = true;
                    });
                }
                else
                {
                    player.SendMessage(initmessage);
                    player.Initialized = true;
                }
            };

            this.CheckAllOnlineStatus();

            TempBanCommand.CheckTempBanned(this.PlayerIO, player, tempBanned =>
            {
                if (tempBanned)
                {
                    player.Disconnect();
                    player.Disconnected = true;
                }
                else
                {
                    BanIpCommand.CheckIpBanned(this.PlayerIO, player, ipBanned =>
                    {
                        if (ipBanned)
                        {
                            player.Disconnect();
                            player.Disconnected = true;
                        }
                    });
                }
            });

            if (isCampaignRoom)
            {
                this.campaign.SendJoinMessage(player, this.RoomId, this.BroadcastMessage, initCallback);
            }
            else
            {
                initCallback();
            }

            if (this.UpgradeChecker.SentWarning)
            {
                this.UpgradeChecker.SendUpdateMessage(player);
            }

            // List crews that user might want to add this world to
            if (!this.BaseWorld.IsPartOfCrew && !this.IsCampaign)
            {
                this.PlayerIO.BigDB.Load("CrewMembership", player.ConnectUserId, membership =>
                {
                    if (membership != null && membership.Count > 0)
                    {
                        this.PlayerIO.BigDB.LoadKeys("Crews", membership.Properties.ToArray(), crews =>
                        {
                            var rtn = Message.Create("canAddToCrews");
                            foreach (var c in crews)
                            {
                                if (c == null)
                                {
                                    continue;
                                }

                                var members = c.GetObject("Members");
                                var memberObj = members.GetObject(player.ConnectUserId);
                                if (memberObj != null && this.BaseWorld.OwnerId != "" &&
                                    members.Contains(this.BaseWorld.OwnerId))
                                {
                                    var rank = memberObj.GetInt("Rank");
                                    var ranks = c.GetArray("Ranks");
                                    if (ranks.Count > rank)
                                    {
                                        var canManageWorlds = rank == 0 || ranks.GetObject(rank)
                                            .GetString("Powers", "")
                                            .Split(',')
                                            .Any(it => it != "" && int.Parse(it) == (int)CrewPower.WorldsManagement);

                                        if (canManageWorlds)
                                        {
                                            rtn.Add(c.Key, c.GetString("Name"));
                                        }
                                    }
                                }
                            }
                            if (rtn.Count > 0)
                            {
                                player.SendMessage(rtn);
                            }
                        });
                    }
                });
            }
        }

        private void GivePlayerGodAccess(Player p)
        {
            this.BroadcastMessage("toggleGod", p.Id, true);
            p.CanToggleGodMode = true;
            p.SendMessage("write", ChatUtils.SystemName, "You may now use god mode.");
        }

        private void AwardOwnerWithGod(Player player, bool skipCampaignCheck = false)
        {
            if (this.LevelOwnerName != player.Name || !this.IsCampaign || this.BaseWorld.OwnerId == "")
            {
                return;
            }

            if (skipCampaignCheck)
            {
                this.GivePlayerGodAccess(player);
            }
            else
            {
                var w = this.campaign.GetWorld(this.RoomId);
                if (w == null)
                {
                    return;
                }

                CampaignPlayer.Load(this.PlayerIO, player.ConnectUserId, this.campaign.Data.Id, campPlayer =>
                {
                    if (campPlayer.GetStatus(w.Tier) == CampaignStatus.Completed)
                    {
                        this.GivePlayerGodAccess(player);
                    }
                });
            }
        }

        public void SendAddPlayer(Player player)
        {
            foreach (var p in this.Players)
            {
                if (p != player)
                {
                    if ((p.IsAdmin || p.IsModerator) && player.Stealthy)
                    {
                        this.SendAddMessage(player, p);
                        if (p.Stealthy)
                            player.Send("write", ChatUtils.SystemName, p.Name.ToUpper() + " is stealthy!");
                        continue;
                    }

                    if (!player.Stealthy)
                    {
                        this.SendAddMessage(player, p);
                    }
                }
            }
        }

        public void SendAddMessage(Player joiningPlayer, Player receivingPlayer)
        {
            var rtn = Message.Create("add",
                joiningPlayer.Id,
                joiningPlayer.Name,
                joiningPlayer.ConnectUserId,
                joiningPlayer.Smiley,
                joiningPlayer.X,
                joiningPlayer.Y,
                joiningPlayer.IsInGodMode,
                joiningPlayer.IsInAdminMode,
                joiningPlayer.CanChat,
                joiningPlayer.Coins,
                joiningPlayer.BlueCoins,
                joiningPlayer.Deaths,
                receivingPlayer.HasFriend(joiningPlayer.ConnectUserId),
                joiningPlayer.HasGoldMembership,
                joiningPlayer.SmileyGoldBorder,
                joiningPlayer.IsInModeratorMode,
                joiningPlayer.Team,
                joiningPlayer.Aura,
                joiningPlayer.AuraColor,
                joiningPlayer.ChatColor,
                joiningPlayer.Badge,
                this.Crew.IsMember(joiningPlayer),
                VarintConverter.GetVarintBytes(joiningPlayer.Switches),
                joiningPlayer.StaffAuraOffset,
                joiningPlayer.CanEdit,
                joiningPlayer.CanToggleGodMode);

            receivingPlayer.SendMessage(rtn);
        }

        public void SetWorldName(string name)
        {
            var newname = ChatUtils.RemoveBadCharacters(name);
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

            this.BaseWorld.Name = newname;
            this.BaseWorld.Save(false);

            if (this.BaseWorld.OwnerId != "")
            {
                this.PlayerIO.BigDB.Load("PlayerObjects", this.BaseWorld.OwnerId,
                    delegate (DatabaseObject oo)
                    {
                        if (!oo.Contains("myworldnames"))
                        {
                            oo.Set("myworldnames", new DatabaseObject());
                        }
                        oo.GetObject("myworldnames").Set(this.RoomId, this.BaseWorld.Name);
                        oo.Save();
                    });
            }
            foreach (var p in this.Players)
            {
                p.CurrentWorldId = this.RoomId;
                p.CurrentWorldName = ChatUtils.RemoveBadCharacters(this.BaseWorld.Name);
            }
        }

        private void AddLikesToWorld(int count)
        {
            lock (this.RoomData)
            {
                this.BaseWorld.Likes += count;
                this.RoomData["Likes"] = this.BaseWorld.Likes.ToString();

                this.RoomData.Save();
            }
        }

        private void UnfavoriteWorld(Player player)
        {
            if (player.Owner || player.IsGuest)
            {
                return;
            }

            if (player.HasFavorited)
            {
                player.HasUnfavorited = true;
            }

            var favorites = player.PlayerObject.GetObject("favorites");
            if (favorites == null || !favorites.Contains(this.RoomId))
            {
                player.Send("write", ChatUtils.SystemName, "This world is not in your favorites.");
                return;
            }

            favorites.Remove(this.RoomId);
            player.PlayerObject.Save();

            player.SendMessage("unfavorited");

            this.AddFavoritesToWorld(-1);
        }

        private void AddFavoritesToWorld(int count)
        {
            if (!this.ready)
            {
                this.pendingFavorites += count;
                return;
            }

            lock (this.RoomData)
            {
                this.BaseWorld.Favorites += count;
                this.RoomData["Favorites"] = this.BaseWorld.Favorites.ToString();

                Console.WriteLine("Updated world favorites: {0}", this.BaseWorld.Favorites);
                this.BaseWorld.Save(false);
                this.RoomData.Save();
            }
        }

        public void CheckEffects()
        {
            if (this.isCheckingEffects)
            {
                return;
            }

            this.isCheckingEffects = true;
            foreach (var player in this.Players)
            {
                if (player.Disconnected || !player.Initialized)
                {
                    continue;
                }

                var effects = player.GetEffects();
                foreach (var effect in effects)
                {
                    if (effect.CanExpire && effect.Expired)
                    {
                        player.RemoveEffect(effect.Id);
                        this.BroadcastMessage("effect", player.Id, (int)effect.Id, false);
                        if (effect.Id == EffectId.Curse && !player.IsInGodMode && !player.IsInAdminMode && !player.IsInModeratorMode)
                        {
                            this.BroadcastMessage("kill", player.Id);
                        }
                        if (effect.Id == EffectId.Zombie && !player.IsInGodMode && !player.IsInAdminMode && !player.IsInModeratorMode)
                        {
                            this.BroadcastMessage("kill", player.Id);
                        }
                        if (effect.Id == EffectId.Fire && !player.IsInGodMode && !player.IsInAdminMode && !player.IsInModeratorMode)
                        {
                            this.BroadcastMessage("kill", player.Id);
                        }
                    }

                    if (effect.Id == EffectId.Protection)
                    {
                        if (player.HasActiveEffect(EffectId.Curse))
                        {
                            player.RemoveEffect(EffectId.Curse);
                            this.BroadcastMessage("effect", player.Id, 4, false);
                        }
                        if (player.HasActiveEffect(EffectId.Zombie))
                        {
                            player.RemoveEffect(EffectId.Zombie);
                            this.BroadcastMessage("effect", player.Id, 5, false);
                        }
                        if (player.HasActiveEffect(EffectId.Fire))
                        {
                            player.RemoveEffect(EffectId.Fire);
                            this.BroadcastMessage("effect", player.Id, 8, false);
                        }
                    }
                }
            }
            this.isCheckingEffects = false;
        }

        public void ApplyEffectToPlayer(Player player, Effect effect)
        {
            var effectId = effect.Id;

            if (effectId == EffectId.Multijump)
            {
                if (effect.Duration == 1)
                {
                    player.RemoveEffect(effectId);
                    this.BroadcastMessage("effect", player.Id, (int)effect.Id, false);
                    return;
                }

                effect.CanExpire = false;
                player.AddEffect(effect);
                this.BroadcastMessage("effect", player.Id, (int)effect.Id, true, effect.Duration);
                return;
            }

            if (effectId == EffectId.Gravity)
            {
                player.FlipGravity = effect.Duration;
                if (effect.Duration == 0)
                {
                    player.RemoveEffect(effectId);
                    this.BroadcastMessage("effect", player.Id, (int)effect.Id, false);
                    return;
                }

                effect.CanExpire = false;
                player.AddEffect(effect);
                this.BroadcastMessage("effect", player.Id, (int)effect.Id, true, effect.Duration);
                return;
            }

            // Disable the effects
            if (effect.Duration == 0)
            {
                if (!player.HasActiveEffect(effectId))
                {
                    return;
                }

                player.RemoveEffect(effectId);
                this.BroadcastMessage("effect", player.Id, (int)effect.Id, false);
                return;
            }

            if (player.HasActiveEffect(effectId))
            {
                return;
            }

            if (effectId == EffectId.Protection)
            {
                if (player.HasActiveEffect(EffectId.Curse))
                {
                    player.RemoveEffect(EffectId.Curse);
                    this.BroadcastMessage("effect", player.Id, (int)EffectId.Curse, false);
                }
                if (player.HasActiveEffect(EffectId.Zombie))
                {
                    player.RemoveEffect(EffectId.Zombie);
                    this.BroadcastMessage("effect", player.Id, (int)EffectId.Zombie, false);
                }
                if (player.HasActiveEffect(EffectId.Fire))
                {
                    player.RemoveEffect(EffectId.Fire);
                    this.BroadcastMessage("effect", player.Id, (int)EffectId.Fire, false);
                }
            }

            if (effectId == EffectId.Curse || effectId == EffectId.Zombie || effectId == EffectId.Fire)
            {
                if (player.HasActiveEffect(EffectId.Protection))
                {
                    return;
                }

                if (effectId == EffectId.Curse &&
                    this.ReachedEffectLimit(this.BaseWorld.CurseLimit, EffectId.Curse))
                {
                    return;
                }
                if (effectId == EffectId.Zombie &&
                    this.ReachedEffectLimit(this.BaseWorld.ZombieLimit, EffectId.Zombie))
                {
                    return;
                }
            }

            if (this.IsTimedEffect(effectId))
            {
                player.AddEffect(effect);
                effect.Activate();
                this.BroadcastMessage("effect", player.Id, (int)effect.Id, true, effect.TimeLeft, effect.Duration);
            }
            else
            {
                effect.CanExpire = false;
                player.AddEffect(effect);
                this.BroadcastMessage("effect", player.Id, (int)effect.Id, true);
            }
        }

        public double GetTime()
        {
            return Math.Round((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds);
        }

        public void KickAndLogCheatingUser(Player player, string cheatType, DatabaseObject data = null)
        {
            this.KickCheatingUser(player);
            this.LogCheatingUser(player, cheatType, data);
        }

        public void LogCheatingUser(Player player, string cheatType, DatabaseObject data = null)
        {
            // Anticheat is disabled on any noncampaign worlds
            if (!this.IsCampaign || !player.IsInCampaignMode)
            {
                return;
            }

            this.PlayerIO.BigDB.LoadOrCreate("Cheaters", player.ConnectUserId, cheaterTable =>
            {
                cheaterTable.Set("Username", player.Name);
                cheaterTable.Set("LastUpdate", DateTime.UtcNow);
                cheaterTable.Set("Banned", "no");
                cheaterTable.Set("IP", player.IPAddress.ToString());

                if (!cheaterTable.Contains("CheatAttempts"))
                {
                    cheaterTable.Set("CheatAttempts", new DatabaseArray());
                }
                var arr = cheaterTable.GetArray("CheatAttempts");
                var db = new DatabaseObject();
                db.Set("Type", cheatType);
                db.Set("IP", player.IPAddress.ToString());
                db.Set("RoomId", this.RoomId);
                db.Set("Date", DateTime.UtcNow);
                db.Set("Movements", player.TotalMovements);
                db.Set("Coins", player.Coins);
                db.Set("BlueCoins", player.BlueCoins);
                db.Set("X", player.X);
                db.Set("Y", player.Y);
                db.Set("PlayTime", (DateTime.UtcNow - player.CampaignJoinTime).TotalMinutes);
                if (data != null)
                {
                    db.Set("Data", data);
                }
                arr.Add(db);

                var attempts = cheaterTable.GetInt(cheatType, 0) + 1;
                cheaterTable.Set(cheatType, attempts);
                cheaterTable.Save();
            });
        }

        private void KickCheatingUser(Player player)
        {
            if (this.IsCampaign && player.IsInCampaignMode)
            {
                player.HasBeenKicked = true;
                player.SendMessage("info", "Oops, beware!", "Looks like you tried to cheat but got kicked instead!");
                player.Disconnect();

                this.BroadcastMessage("write", ChatUtils.SystemName,
                    "Looks like " + player.Name.ToUpper() + " tried to cheat and got kicked!");
            }
        }

        [DebugAction("Chat Color", DebugAction.Icon.Add)]
        public void ChangeChatColor()
        {
            foreach (var p in this.Players)
            {
                var bytes = new byte[4];
                this.Random.NextBytes(bytes);
                p.ChatColor = BitConverter.ToUInt32(bytes, 0);
                break;
            }
        }

        public void SetVisibility(bool visible)
        {
            this.Visible = this.AllowVisibility && visible;
        }

        public void CheckIpBans()
        {
            this.ForEachPlayer(delegate (Player player)
            {
                BanIpCommand.CheckIpBanned(this.PlayerIO, player, isBanned =>
                {
                    if (isBanned)
                    {
                        player.Disconnect();
                    }
                });
            });
        }
        public void SendSystemMessage(Player player, string message)
        {
            player.SendMessage("write", ChatUtils.SystemName, message);
        }
    }
}