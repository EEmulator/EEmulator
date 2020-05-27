using System;
using System.Collections;
using System.Collections.Generic;
using EverybodyEdits.Common;
using EverybodyEdits.Game.CountWorld;
using PlayerIO.GameLibrary;

namespace EverybodyEdits.Game
{
    public class Player : CommonPlayer
    {
        public const int MessageLimit = 100;
        public const int TotalMessageLimit = 300;
        public readonly HashSet<string> LastChatChannels = new HashSet<string>();

        private readonly Dictionary<string, string> lastChatMessages = new Dictionary<string, string>();
        private readonly Dictionary<string, int> lastChatRepeat = new Dictionary<string, int>();
        private readonly Dictionary<string, int> messageCounter = new Dictionary<string, int>();
        public readonly List<string> MutedUsers = new List<string>();
        private Dictionary<EffectId, Effect> activeEffects = new Dictionary<EffectId, Effect>();
        public int BlueCoins = 0;

        private bool canChangeWorldOptions;
        public bool CanEdit = false;

        private bool canToggleGodMode;
        public bool CanWinEnergy = true;
        public uint ChatColor = 0x0;
        public int ChatCoolDown = 500;
        public int Cheat = 0;
        public HashSet<Item> Coinmap = new HashSet<Item>();
        public int Coins = 0;
        public DateTime CoinTimer = DateTime.Now.AddMinutes(1);
        public int Deaths;
        public bool Disconnected = false;
        public bool HasBeenKicked = false;

        private Hashtable friends;

        public DateTime ResetTime = DateTime.MinValue;

        public bool HasCompleted = false;
        public bool HasSilverCrown = false;
        public bool Initialized;

        public bool GotNotified { get; set; }

        public int StaffAuraOffset { get; set; }
        public bool IsInAdminMode = false;
        public bool IsInGodMode = false;

        public bool IsInCampaignMode;
        public bool InCampaign;

        public bool IsInitializing;
        public bool IsInitializingDone;

        public bool IsInModeratorMode = false;

        public DateTime JoinTime;
        public DateTime CampaignJoinTime;

        public DateTime LastChat = DateTime.Now;
        public DateTime LastEdit = DateTime.Now;
        private DateTime lastMessageTimer = DateTime.Now;
        public DateTime LastMove = DateTime.Now;
        public DateTime LastReport = DateTime.Now.AddHours(-1);
        private OnlineStatus onlineStatusObject;
        public bool Owner = false;
        public bool Ready = false;

        public HashSet<int> Switches = new HashSet<int>();

        public int Team;
        private int temporaryFace = -1;
        public int Threshold = 150;
        private int totalMessageCounter;

        public int TotalMovements = 0;
        public Item Checkpoint { get; set; }

        public TimeSpan TimeSinceReset {
            get {
                return DateTime.UtcNow - ResetTime;
            }
        }

        public string CurrentWorldName {
            set {
                if (this.onlineStatusObject != null) {
                    this.onlineStatusObject.CurrentWorldName = value;
                }
            }
        }

        public string CurrentWorldId {
            set {
                if (this.onlineStatusObject != null) {
                    this.onlineStatusObject.CurrentWorldId = value;
                }
            }
        }

        public string Room0 {
            get { return this.PlayerObject.GetString("room0", ""); }
        }

        public string Betaonlyroom {
            get { return this.PlayerObject.GetString("betaonlyroom", ""); }
        }

        public DateTime LastMagic {
            get { return PlayerObject.GetDateTime("lastcoin", DateTime.UtcNow.AddHours(-24)); }
            set {
                if (PlayerObject != null) {
                    PlayerObject.Set("lastcoin", value);
                }
            }
        }

        public bool CanChangeWorldOptions {
            get { return this.Owner || this.canChangeWorldOptions; }
            set { this.canChangeWorldOptions = value; }
        }

        public bool CanToggleGodMode {
            get { return this.CanEdit || this.canToggleGodMode; }
            set { this.canToggleGodMode = value; }
        }

        public override string Name {
            get { return this.PlayerObject.GetString("name", "Guest-" + this.Id); }
        }

        public override int Smiley {
            get { return temporaryFace > -1 ? temporaryFace : base.Smiley; }
            set {
                if (!AllowSaveSmiley(value)) {
                    temporaryFace = value;
                }
                else {
                    temporaryFace = -1;
                    if (PlayerObject != null) {
                        base.Smiley = value;
                    }
                    if (onlineStatusObject != null) {
                        onlineStatusObject.Smiley = value;
                    }
                }
            }
        }

        public override bool SmileyGoldBorder {
            get {
                return base.SmileyGoldBorder;
            }
            set {
                base.SmileyGoldBorder = value;
                if (onlineStatusObject != null) {
                    onlineStatusObject.HasGoldBorder = value;
                }
            }
        }

        public bool RevertTemporarySmiley()
        {
            if (temporaryFace == -1)
                return false;

            this.Smiley = base.Smiley;
            return true;
        }

        public bool HasSmileyPackage {
            get { return this.HasBeta; }
        }

        public bool HasLiked { get; set; }
        public bool HasUnliked { get; set; }
        public bool HasFavorited { get; set; }
        public bool HasUnfavorited { get; set; }
        public int AddToCrewRequests { get; set; }

        public bool HasInFavorites(string worldId)
        {
            var favorites = this.PlayerObject.GetObject("favorites");
            return favorites != null && favorites.Contains(worldId);
        }

        public bool Liked(string worldId)
        {
            var likes = this.PlayerObject.GetObject("likes");
            return likes != null && likes.Contains(worldId);
        }

        public int RepeatChatCount(string chat, string channel)
        {
            if (this.lastChatMessages.ContainsKey(channel) &&
                chat == this.lastChatMessages[channel]) {
                this.lastChatRepeat[channel]++;
            }
            else {
                this.lastChatRepeat[channel] = 0;
                this.lastChatMessages[channel] = chat;
            }
            return this.lastChatRepeat[channel];
        }

        public bool AllowMessage(string messageType)
        {
            if (this.Owner && !this.InCampaign) {
                return true;
            }

            if ((DateTime.Now - this.lastMessageTimer).TotalSeconds > 1) {
                this.totalMessageCounter = 0;
                this.messageCounter.Clear();
                this.lastMessageTimer = DateTime.Now;
            }

            if (!this.messageCounter.ContainsKey(messageType)) {
                this.messageCounter.Add(messageType, 0);
            }
            this.totalMessageCounter += 1;
            this.messageCounter[messageType] = this.messageCounter[messageType] + 1;
            if (this.messageCounter[messageType] > MessageLimit || this.totalMessageCounter > TotalMessageLimit) {
                return false;
            }
            return true;
        }

        public void SendMessage(string type, params object[] parameters)
        {
            this.SendMessage(Message.Create(type, parameters));
        }

        public void SendMessage(Message msg)
        {
            if (this.Initialized ||
                msg.Type == "init" ||
                msg.Type == "info" ||
                msg.Type == "banned" ||
                msg.Type == "unfavorited") {
                base.Send(msg);
            }
        }

        [Obsolete("Use SendMessage instead.")]
        public new void Send(string type, params object[] parameters)
        {
            base.Send(type, parameters);
        }

        [Obsolete("Use SendMessage instead.")]
        public new void Send(Message msg)
        {
            base.Send(msg);
        }

        public void SetCoinCollected(uint type, uint x, uint y)
        {
            this.Coinmap.Add(new Item(new ForegroundBlock(type), x, y));
        }

        public bool HasFriend(string friendConnectionId)
        {
            return this.friends != null && this.friends.Contains(friendConnectionId);
        }

        public bool HasBrickPack(string payvaultid)
        {
            if (this.IsAdmin) {
                return true;
            }
            return this.PayVault.Has(payvaultid);
        }

        public int GetBrickPackCount(string payvaultid)
        {
            return this.PayVault.Count(payvaultid);
        }

        public void AddEffect(Effect effect)
        {
            if (effect.Id == EffectId.Gravity) {
                if (effect.Duration == 0) {
                    activeEffects.Remove(effect.Id);
                    return;
                }
            }

            activeEffects[effect.Id] = effect;
        }

        public void RemoveEffect(EffectId id)
        {
            activeEffects.Remove(id);
        }

        public bool HasActiveEffect(EffectId id)
        {
            return activeEffects.ContainsKey(id);
        }

        public Effect GetEffect(EffectId id)
        {
            return this.activeEffects.ContainsKey(id) ? this.activeEffects[id] : null;
        }

        public List<Effect> GetEffects()
        {
            return new List<Effect>(activeEffects.Values);
        }

        public void ResetEffects()
        {
            activeEffects = new Dictionary<EffectId, Effect>();
        }

        public void Init(Client c, Callback successCallback)
        {
            if (this.IsInitializing) {
                return;
            }
            this.IsInitializing = true;

            OnlineStatus.GetOnlineStatus(c, this.ConnectUserId, delegate (OnlineStatus os) {
                this.onlineStatusObject = os;
                this.onlineStatusObject.Name = this.Name;
                this.onlineStatusObject.IpAddress = this.IPAddress.ToString();
                this.onlineStatusObject.LastUpdate = DateTime.Now;

                c.BigDB.LoadOrCreate("friends", this.ConnectUserId, delegate (DatabaseObject friendslist) {
                    this.friends = new Hashtable();
                    foreach (var key in friendslist.Properties) {
                        if (friendslist.GetBool(key)) {
                            this.friends.Add(key, true);
                        }
                    }

                    Console.WriteLine("check player init: " + (this.onlineStatusObject != null) + ", " +
                                      (this.friends != null));
                    if (this.onlineStatusObject != null && this.friends != null) {
                        if (!this.AllowSaveSmiley(this.Smiley)) {
                            this.Smiley = 0;
                        }

                        this.IsInitializingDone = true;
                        this.IsInitializing = false;
                        successCallback.Invoke();
                    }
                });
            });
        }

        public void Save()
        {
            this.SaveOnlineStatus();
            if (!this.IsGuest && !this.Disconnected) {
                lock (this) {
                    if (this.PlayerObject != null) {
                        this.PlayerObject.Save(delegate { Console.WriteLine("PlayerObject saved"); });
                    }
                }
            }
        }

        public void SaveOnlineStatus()
        {
            if (this.onlineStatusObject != null) {
                this.onlineStatusObject.LastUpdate = DateTime.Now;
                lock (this) {
                    this.onlineStatusObject.Save();
                }
            }
        }

        private bool AllowSaveSmiley(int face)
        {
            if (this.IsAdmin) {
                return true;
            }

            // do not allow to save diamond smiley, party smileys, hologram smiley and zombie smiley
            return face != 31 && (face < 72 || face > 75) && face != 87 && face != 100;
        }

        /// <summary>
        ///     Convert Unix time value to a DateTime object.
        /// </summary>
        /// <param name="unixtime">The Unix time stamp you want to convert to DateTime.</param>
        /// <returns>Returns a DateTime object that represents value of the Unix time.</returns>
        public static DateTime UnixTimeToDateTime(long unixtime)
        {
            var sTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return sTime.AddSeconds(unixtime);
        }

        /// <summary>
        ///     Convert a date time object to Unix time representation.
        /// </summary>
        /// <param name="datetime">The datetime object to convert to Unix time stamp.</param>
        /// <returns>Returns a numerical representation (Unix time) of the DateTime object.</returns>
        public static long ConvertToUnixTime(DateTime datetime)
        {
            var sTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            return (long)(datetime - sTime).TotalSeconds;
        }

        #region BasicAntiCheat 

        public ClientType ClientType = ClientType.Unknown;
        public int AntiFlightHeat = 0;
        public int AntiJumpHeat = 0;
        public bool IsCheater;
        public bool? IsBot;
        public int FlipGravity = 0;

        #endregion

        #region Physics Simulation

        public int Horizontal;
        public int Vertical;

        public bool SpaceDown;
        public bool SpaceJustPressed;
        public int TickId = -1;

        private double x = 16;
        private double y = 16;
        private double speedX;
        private double speedY;
        private double modifierX;
        private double modifierY;

        private const double Mult = 7.752;

        public double X {
            get { return this.x; }
            set { x = double.IsNaN(value) ? 0 : value; }
        }

        public double Y {
            get { return this.y; }
            set { y = double.IsNaN(value) ? 0 : value; }
        }

        public double SpeedX {
            get { return speedX * Mult; }
            set { speedX = double.IsNaN(value) ? 0 : value / Mult; }
        }

        public double SpeedY {
            get { return speedY * Mult; }
            set { speedY = double.IsNaN(value) ? 0 : value / Mult; }
        }

        public double ModifierX {
            get { return modifierX * Mult; }
            set { modifierX = double.IsNaN(value) ? 0 : value / Mult; }
        }

        public double ModifierY {
            get { return modifierY * Mult; }
            set { modifierY = double.IsNaN(value) ? 0 : value / Mult; }
        }

        public bool Stealthy {
            get {
                return this.onlineStatusObject != null && this.onlineStatusObject.Stealthy;
            }
            set {
                this.onlineStatusObject.Stealthy = value;
                this.onlineStatusObject.Save();
            }
        }

        public bool TempStealth { get; set; }
        public bool BackupCampaign = true;

        #endregion
    }
}