using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using EverybodyEdits.Game;
using PlayerIO.GameLibrary;

namespace EverybodyEdits.Lobby
{
    public class Player : BasePlayer
    {
        public int EnergyRechargeSecounds = 150;

        public DateTime ShopDate
        {
            get
            {
                return this.PlayerObject.GetDateTime("shopDate",
                    DateTime.Now.AddSeconds(-this.EnergyRechargeSecounds * this.MaxEnergy));
            }
            set { this.PlayerObject.Set("shopDate", value); }
        }

        public int MaxEnergy
        {
            get
            {
                // New energy calculation!

                if (!this.PlayerObject.Contains("maxEnergy"))
                {
                    this.PlayerObject.Set("maxEnergy", 200);
                    this.PlayerObject.Save();
                }

                return (Config.levelmaxenergy[this.Level - 1] - 200) + this.PlayerObject.GetInt("maxEnergy");
                //return Math.Max(PlayerObject.GetInt("maxEnergy", 200),200);
            }
        }

        public string ClassTitle
        {
            get { return Config.leveltitles[this.Level - 1]; }
        }

        public int Energy
        {
            get
            {
                return Math.Min(this.MaxEnergy,
                    (int)((DateTime.Now - this.ShopDate).TotalSeconds / this.EnergyRechargeSecounds));
            }
        }

        public int Level
        {
            get { return Math.Max(this.PlayerObject.GetInt("level", 1), 1); }
            set { this.PlayerObject.Set("level", value); }
        }

        public string Name
        {
            get { return this.PlayerObject.GetString("name", ""); }
        }

        public int TimeZone
        {
            get { return this.PlayerObject.GetInt("timezone", 0); }
            set { this.PlayerObject.Set("timezone", value); }
        }

        public int GetSecoundsToNextEnergy
        {
            get
            {
                var time = (DateTime.Now - this.ShopDate).TotalSeconds / this.EnergyRechargeSecounds;
                var reminder = (time - Math.Floor(time));

                //Console.WriteLine("Secs > " + reminder);

                var timeLeftInSecs = (int)((this.EnergyRechargeSecounds - (reminder * this.EnergyRechargeSecounds)));
                return timeLeftInSecs;
            }
        }

        public bool HasBeta
        {
            get { return this.PayVault.Has("pro") || this.PlayerObject.GetBool("haveSmileyPackage", false); }
        }

        public bool ClubMember
        {
            get
            {
                return this.PlayerObject.Contains("club_expire") &&
                       this.PlayerObject.GetDateTime("club_expire") > DateTime.Now;
            }
        }

        public bool canchat
        {
            get { return (!this.PlayerObject.GetBool("chatbanned", false) && !this.isguest); }
        }


        public bool canbemod
        {
            get { return this.PlayerObject.GetBool("isModerator", false); }
        }

        public bool JustJoinedClub
        {
            get
            {
                return this.PlayerObject.Contains("club_join") &&
                       (DateTime.Now - this.PlayerObject.GetDateTime("club_join")).TotalSeconds < 120;
            }
        }

        public bool HasFriendFeatures
        {
            get { return !this.isguest; }
        }

        public bool isguest
        {
            get { return this.ConnectUserId == "simpleguest"; }
        }

        public bool ismod
        {
            get { return this.PlayerObject.GetBool("isModerator", false); }
        }

        public void SaveShop()
        {
            //PlayerObject.Set("maxEnergy",MaxEnergy);
            this.PlayerObject.Save();
        }

        public void SaveShop(Callback method)
        {
            //PlayerObject.Set("maxEnergy", MaxEnergy);
            this.PlayerObject.Save(method);
        }

        public int GetEnergyStatus(string type)
        {
            var itm = this.PlayerObject.GetObject("shopItems");

            if (itm != null)
            {
                return itm.GetInt(type, 0);
            }

            return 0;
        }

        public void SetEnergyStatus(string type, int value)
        {
            var itm = this.PlayerObject.GetObject("shopItems");
            if (itm == null)
            {
                itm = new DatabaseObject();
                this.PlayerObject.Set("shopItems", itm);
            }
            itm.Set(type, value);
        }

        public bool UseEnergy(int count)
        {
            if (this.Energy >= count)
            {
                if (this.Energy == this.MaxEnergy)
                {
                    this.ShopDate =
                        DateTime.Now.AddSeconds(-this.MaxEnergy * this.EnergyRechargeSecounds +
                                                this.EnergyRechargeSecounds * count);
                }
                else
                {
                    this.ShopDate = this.ShopDate.AddSeconds(count * this.EnergyRechargeSecounds);
                }
                return true;
            }
            return false;
        }
    }

    //public class ShopItem { 
    //    public string key = "";
    //    public int PriceGems = 0;
    //    public int PriceEnergy = 0;
    //    public int EnergyPerClick = 0;
    //    public bool Reusable = false;
    //    public bool BetaOnly = false;
    //    public bool Enabled = true;
    //    public bool IsFeatured = false; // Shall item be listed under "featured" in the shop (still shown in brick/smiley/world tabs)?
    //    public bool IsClassic = false; // Shall item be listed under "classic" tab in the shop (and nowhere else)?

    //    public ShopItem(string key, int pricegems, int priceenergy, int energyperclick, bool reusable, bool betaonly, bool isFeatured, bool isClassic, bool enabled) { 
    //        this.key = key;
    //        this.PriceGems = pricegems;
    //        this.PriceEnergy = priceenergy;
    //        this.EnergyPerClick = energyperclick;
    //        this.Reusable = reusable;
    //        this.BetaOnly = betaonly;
    //        this.IsFeatured = isFeatured;
    //        this.IsClassic = isClassic;
    //        this.Enabled = enabled;
    //    }
    //}

    public class QueueItem
    {
        public string connectUserId;
        public int id;
        public Message message;
        public string method;

        public QueueItem(int id, string connectUserId, string method, Message m)
        {
            this.id = id;
            this.connectUserId = connectUserId;
            this.method = method;
            this.message = m;
        }
    }

    [RoomType("Lobby188")]
    public class GameCode : Game<Player>
    {
        public string[] DEPRECATED_PLAYEROBJECT_VALUES =
        {
            "worldName", "worldId", "lastip", "wOOt_test", "woot_test",
            "wOOt_test2", "Online"
        };

        //List<ShopItem> shopitems = new List<ShopItem>();
        //public bool loadedShopConfig = false;
        private Friends friendhandler;
        protected List<QueueItem> queue = new List<QueueItem>();
        protected Shop shophandler;

        public override void GameStarted()
        {
            // anything you write to the Console will show up in the 
            // output window of the development server

            // Always loads players PlayerObject
            this.PreloadPlayerObjects = true;
            this.PreloadPayVaults = true;

            this.friendhandler = new Friends(this.PlayerIO);

            this.shophandler = new Shop(this.PlayerIO, delegate { this.EmptyQueue(); });
        }


        public override bool AllowUserJoin(Player player)
        {
            foreach (var p in this.Players)
            {
                //p.Send("info","Woops", "You cannot be logged in multiple times with the same user. You have therefore been disconnected.");
                //p.Disconnect();
            }

            if (this.RoomId != player.ConnectUserId)
            {
                return false;
            }

            return base.AllowUserJoin(player);
        }

        // This method is called whenever a player joins the game
        public override void UserJoined(Player player)
        {
            var haveBeta = player.PlayerObject.GetBool("haveSmileyPackage", false) || player.PayVault.Has("pro");

            //Give pro to old beta members;
            if (!player.PayVault.Has("pro") && haveBeta)
            {
                player.PayVault.Give(new[] { new BuyItemInfo("pro") }, delegate { });
            }

            //if (!player.PlayerObject.Contains("timezone")) {
            //    player.Send("timezone");
            //}

            //AwardWorldToNewPlayer(player);
            //AwardClubToBeta(player);
            this.CheckClubMembership(player, delegate { player.PlayerObject.Save(); });

            this.AwardWorldToNewPlayer(player, delegate
            {
                this.CheckTempBanned(player, delegate
                {
                    player.PlayerObject.Set("created", true);
                    player.PlayerObject.Save(delegate { player.Send("connectioncomplete"); });
                });
            });

            // Inform the friendHandler about club member status
            this.friendhandler.playerIsClubMember = player.ClubMember; // todo: remove this
            this.friendhandler.playerIsBeta = player.HasBeta;
        }

        // This method is called when a player leaves the game
        public override void UserLeft(Player player)
        {
        }

        public void AddToQueue(int id, string conncetUserId, string method, Message message)
        {
            this.queue.Add(new QueueItem(id, conncetUserId, method, message));
            this.EmptyQueue();
        }

        protected void SendShopUpdate(QueueItem item, Player p, Boolean refresh)
        {
            var m = Message.Create(item.method, refresh, p.Energy, p.GetSecoundsToNextEnergy, p.MaxEnergy,
                p.EnergyRechargeSecounds, p.PayVault.Coins);
            p.Send(this.shophandler.GetUpdateMessage(p, m));
        }

        protected void CheckClubMembership(Player p, Callback callback = null)
        {
            VaultItem club = null;
            var trial = false;
            var add_monts = 0;
            if ((p.HasBeta && !p.PlayerObject.Contains("club_join")))
            {
                trial = true;
                add_monts = 1;
            }
            if (p.PayVault.Has("club1"))
            {
                club = p.PayVault.First("club1");
                p.PayVault.Give(new[] { new BuyItemInfo("world4") }, delegate { });
                add_monts = 1;
            }
            if (p.PayVault.Has("club3"))
            {
                club = p.PayVault.First("club3");
                p.PayVault.Give(new[] { new BuyItemInfo("world4"), new BuyItemInfo("world6") }, delegate { });
                add_monts = 3;
            }
            if (p.PayVault.Has("club6"))
            {
                club = p.PayVault.First("club6");
                p.PayVault.Give(
                    new[] { new BuyItemInfo("world4"), new BuyItemInfo("world6"), new BuyItemInfo("world3") },
                    delegate { });
                add_monts = 6;
            }
            if (p.PayVault.Has("club12"))
            {
                club = p.PayVault.First("club12");
                p.PayVault.Give(
                    new[]
                    {
                        new BuyItemInfo("world4"), new BuyItemInfo("world6"), new BuyItemInfo("world3"),
                        new BuyItemInfo("world5")
                    }, delegate { });
                add_monts = 12;
            }
            if (add_monts > 0)
            {
                var expire = p.PlayerObject.GetDateTime("club_expire", DateTime.Now);
                DateTime newExpirationDate;
                if (expire > DateTime.Now)
                {
                    newExpirationDate = expire.AddMonths(add_monts);
                }
                else
                {
                    newExpirationDate = DateTime.Now.AddMonths(add_monts);
                }

                p.PlayerObject.Set("club_expire", newExpirationDate);
                if (!p.PlayerObject.Contains("club_join"))
                {
                    p.PlayerObject.Set("club_join", DateTime.Now);
                }
                //p.JustJoinedClub = true;

                Callback consumeandsave = delegate
                {
                    lock (p)
                    {
                        p.PlayerObject.Save(delegate
                        {
                            if (club != null)
                            {
                                p.PayVault.Consume(new[] { club },
                                    delegate /* success */
                                    {
                                        if (callback != null)
                                        {
                                            //callback.Invoke();
                                            Console.WriteLine(
                                                "We consumed a membership successfully, extend the next woot decrease time, by " +
                                                add_monts);
                                            this.IncreaseWootDecayByClubMembershipDuration(p, newExpirationDate,
                                                callback);
                                        }
                                    },
                                    delegate /* error */
                                    {
                                        this.PlayerIO.ErrorLog.WriteError("Error consuming club membership");
                                        if (callback != null)
                                        {
                                            callback.Invoke();
                                        }
                                    });
                            }
                        }, delegate
                        {
                            this.PlayerIO.ErrorLog.WriteError("Error saving playerobject after adding club membership");
                            if (callback != null) callback.Invoke();
                        });
                    }
                    ;
                };

                //Console.WriteLine("Member numnber: " + p.PlayerObject.GetString("club_membernumber", "") + " try? " + p.PlayerObject.GetString("club_membernumber", "").Contains("TRY"));
                if (p.PlayerObject.GetString("club_membernumber", "").Contains("TRY"))
                {
                    p.PlayerObject.Remove("club_membernumber");
                }
                if (!p.PlayerObject.Contains("club_membernumber"))
                {
                    Console.WriteLine("Finding member number");
                    this.FindNextMemberNumber(trial, delegate (int membernumber)
                    {
                        Console.WriteLine("Found member number");
                        if (trial)
                        {
                            p.PlayerObject.Set("club_membernumber", "TRY" + membernumber);
                        }
                        else
                        {
                            p.PlayerObject.Set("club_membernumber", membernumber.ToString());
                        }
                        consumeandsave();
                    });
                }
                else
                {
                    Console.WriteLine("Allready have member number");
                    consumeandsave();
                }
            } /*else {
                // There is no purchase, but we want to check membership to avoid decaying magic (woots)
                // if the player already is a club member, we want to make sure their woot decay gets set to their membership expiration date
                if ( p.ClubMember)
                {
                    Console.WriteLine("-> Lobby.cs, CheckClubMembership Nothing to consume, but player is a clubmember, so we check if the next woot decay is set to the membership expiration date");
                    var now = DateTime.Now;
                    var membershipExpirationDate = p.PlayerObject.GetDateTime("club_expire", now);
                    if (membershipExpirationDate > now)
                    {
                        IncreaseWootDecayByClubMembershipDuration(p, membershipExpirationDate, callback);
                    }

                }
                else
                {
                    // player is NOT member and did not buy anything, continue as usual
                    Console.WriteLine("-> Lobby.cs, CheckClubMembership, nothing to consume or extend, moving along happily");
                    if (callback != null) callback.Invoke();
                }
                //if (callback != null) callback.Invoke();
            }*/
            else
            {
                if (callback != null) callback.Invoke();
            }
        }

        protected void IncreaseWootDecayByClubMembershipDuration(Player p, DateTime nextDecay, Callback callback)
        {
            //if (callback != null) callback.Invoke();

            //WootStatus wootStatus = null;
            //WootStatus.getWootStatus(this.PlayerIO, p.ConnectUserId, delegate(WootStatus status)
            //{
            //    wootStatus = status;
            //    if (wootStatus != null)
            //    {
            //        Console.WriteLine(
            //            "Lobby.cs > increaseWootDecayByClubMembershipDuration: WootStatus.nextdecay is = " +
            //            wootStatus.nextdecay);
            //        wootStatus.nextdecay = nextDecay;
            //        Console.WriteLine(
            //            "Lobby.cs > increaseWootDecayByClubMembershipDuration: WootStatus.nextdecay extended to: " +
            //            wootStatus.nextdecay);

            //        wootStatus.Save(
            //            delegate
            //            {
            //                Console.WriteLine(
            //                    "Lobby.cs > increaseWootDecayByClubMembershipDuration > We saved the extended duration");
            //                if (callback != null) callback.Invoke();
            //            },
            //            delegate
            //            {
            //                this.PlayerIO.ErrorLog.WriteError(
            //                    "Error extending next decay duration, at 'Lobby.cs > increaseWootDecayByClubMembershipDuration'");
            //                if (callback != null) callback.Invoke();
            //            }
            //            );
            //    }
            //}, false);
        }

        protected void FindNextMemberNumber(bool trial, Callback<int> callback)
        {
            this.PlayerIO.BigDB.Load("ClubMembers", "membernumber", delegate (DatabaseObject dataobj)
            {
                var membernumber = 0;
                if (trial)
                {
                    membernumber = dataobj.GetInt("latest_trial", 0) + 1;
                    dataobj.Set("latest_trial", membernumber);
                }
                else
                {
                    membernumber = dataobj.GetInt("latest", 0) + 1;
                    dataobj.Set("latest", membernumber);
                }
                dataobj.Save(true, false, delegate { callback(membernumber); },
                    delegate { this.FindNextMemberNumber(trial, callback); });
            }, delegate
            {
                this.PlayerIO.ErrorLog.WriteError("Error loading ClubMember membernumber");
                this.FindNextMemberNumber(trial, callback);
            });
        }

        private void CheckTempBanned(Player player, Callback callback)
        {
            this.PlayerIO.BigDB.Load("TempBans", player.ConnectUserId, delegate (DatabaseObject o)
            {
                if (o == null)
                {
                    Console.WriteLine("No tempban object: " + player.Name);
                    callback();
                    return;
                }

                if (o.Contains("Bans"))
                {
                    var tempBans = o.GetArray("Bans");

                    var orderedBans =
                        tempBans.OrderByDescending(x => ((DatabaseObject)x).GetDateTime("Date")).ToArray();
                    var lastExpiration = orderedBans[0] as DatabaseObject;

                    if (lastExpiration == null)
                    {
                        Console.WriteLine("No 'lastExpiration' object: " + player.Name);
                        callback();
                        return;
                    }
                    Console.WriteLine("Last ban expires: " + lastExpiration.GetDateTime("Expires") + " (" +
                                      (lastExpiration.GetDateTime("Expires") > DateTime.Now) + ")");

                    var tempBanned = lastExpiration.GetDateTime("Expires") > DateTime.Now;
                    player.PlayerObject.Set("tempbanned", tempBanned);
                    player.PlayerObject.Save(delegate
                    {
                        if (tempBanned)
                        {
                            var timeleft = Math.Max((lastExpiration.GetDateTime("Expires") - DateTime.Now).Days, 1);
                            player.Send("info", "Banned",
                                "You have been banned . Ban will be lifted in " + timeleft + " day" +
                                (timeleft > 1 ? "s" : "") + " \n\"" + lastExpiration.GetString("Reason", "") + "\"");
                        }
                        callback();
                    });
                }
                else
                {
                    Console.WriteLine("No 'Bans' object: " + player.Name);
                    callback();
                }
            });
        }

        /*
        This is where players get the home world. The home world is in the database with id: PWQe-HH_N2bUI
        */

        protected void AwardWorldToNewPlayer(Player player, Callback callback)
        {
            if (!player.isguest && !player.PlayerObject.Contains("worldhome"))
            {
                //if (true || !player.isguest && !player.PlayerObject.Contains("created")) {
                //player.PayVault.Give(new BuyItemInfo[] { new BuyItemInfo("world0") }, delegate() {
                //    Console.WriteLine("Awarded world complete");
                //});

                this.getUniqueId(false, player, delegate (string newid)
                {
                    this.PlayerIO.BigDB.Load("worlds", /*"PWmQhDs0g_bUI"*/"PWQe-HH_N2bUI",
                        delegate (DatabaseObject original)
                        {
                            //string worldname = "Home of " + player.Name.ToUpper();
                            var worldname = "My Home World";

                            //DatabaseObject newworld = new DatabaseObject();
                            original.Set("allowpotions", true);
                            original.Set("name", worldname);
                            original.Set("owner", player.ConnectUserId);

                            this.PlayerIO.BigDB.CreateObject("worlds", newid, original, delegate
                            {
                                player.PlayerObject.Set("worldhome", newid);
                                if (!player.PlayerObject.Contains("myworldnames"))
                                    player.PlayerObject.Set("myworldnames", new DatabaseObject());
                                player.PlayerObject.GetObject("myworldnames").Set(newid, worldname);
                                callback.Invoke();
                            }, delegate { callback.Invoke(); });
                        }, delegate { callback.Invoke(); });
                });
            }
            else
            {
                callback.Invoke();
            }
        }

        public virtual void EmptyQueue()
        {
            Console.WriteLine("EmptyQueue: " + this.shophandler.isConfigLoaded);
            if (this.shophandler.isConfigLoaded)
            {
                foreach (var item in this.queue)
                {
                    foreach (var p in this.Players)
                    {
                        if (p.Id == item.id && p.ConnectUserId == item.connectUserId)
                        {
                            switch (item.method)
                            {
                                case "getMySimplePlayerObject":
                                {
                                    //Making sure we are updated
                                    Console.WriteLine("getMySimplePlayerObject");

                                    p.RefreshPlayerObject(delegate
                                    {
                                        Console.WriteLine("refreshed");

                                        if (p == null)
                                        {
                                            this.PlayerIO.ErrorLog.WriteError(
                                                "Horriable Error, Player is null in getMySimplePlayerObject!");
                                            p.Disconnect();
                                            return;
                                        }

                                        if (p.PlayerObject == null)
                                        {
                                            this.PlayerIO.ErrorLog.WriteError(
                                                "Horriable Error, PlayerObject is null in getMySimplePlayerObject!");
                                            p.Disconnect();
                                            return;
                                        }

                                        lock (p)
                                        {
                                            Console.WriteLine("locked");

                                            this.RemoveDeprecatedValues(p.PlayerObject);

                                            var rtn = Message.Create(item.method);
                                            rtn.Add(p.PlayerObject.GetString("name", ""));
                                            rtn.Add(p.PlayerObject.GetInt("smiley", 0));

                                            rtn.Add(p.PlayerObject.GetBool("chatbanned", false));
                                            rtn.Add(p.canchat);
                                            rtn.Add(p.PlayerObject.GetBool("haveSmileyPackage", false));
                                            rtn.Add(p.PlayerObject.GetBool("isModerator", false));
                                            rtn.Add(p.PlayerObject.GetBool("isGuardian", false));
                                            rtn.Add(p.ClubMember);
                                            rtn.Add(
                                                (p.PlayerObject.GetDateTime("club_expire", DateTime.Now) - DateTime.Now)
                                                    .TotalMilliseconds);
                                            rtn.Add(
                                                (p.PlayerObject.GetDateTime("club_join", DateTime.Now) - DateTime.Now)
                                                    .TotalMilliseconds);
                                            rtn.Add(p.PlayerObject.GetString("club_membernumber", "0"));
                                            rtn.Add(p.JustJoinedClub);
                                            rtn.Add(p.PlayerObject.GetString("room0", ""));
                                            rtn.Add(p.PlayerObject.GetString("betaonlyroom", ""));
                                            rtn.Add(p.PlayerObject.GetString("worldhome", ""));
                                            var worlds = new ArrayList();
                                            var worldkeys = new ArrayList();
                                            var worldnames = new ArrayList();
                                            var mwn = p.PlayerObject.GetObject("myworldnames");
                                            foreach (var key in p.PlayerObject.Properties)
                                            {
                                                if (key.StartsWith("world"))
                                                {
                                                    worldkeys.Add(key);
                                                    worlds.Add(p.PlayerObject[key]);
                                                    worldnames.Add((mwn != null
                                                        ? mwn.GetString((string)p.PlayerObject[key], "")
                                                        : ""));
                                                }
                                            }
                                            rtn.Add(string.Join(",", worldkeys.ToArray(typeof(string)) as string[]));
                                            rtn.Add(string.Join(",", worlds.ToArray(typeof(string)) as string[]));
                                            rtn.Add(string.Join(",", worldnames.ToArray(typeof(string)) as string[]));

                                            rtn.Add(p.PlayerObject.GetBool("visible", true));
                                            rtn.Add(p.PlayerObject.GetBool("banned", false));

                                            rtn.Add(p.PlayerObject.GetInt("termsVersion", 0));

                                            var wootup = -1;
                                            //WootStatus wootstatus = null;

                                            //Callback ondataloaded = delegate
                                            //{
                                            //if (wootup >= 0 && wootstatus != null)
                                            //{
                                            p.Level = (0); //wootstatus.level;
                                            rtn.Add(0); //(wootup);
                                            rtn.Add(0);//(wootstatus.current);
                                            rtn.Add(p.Level);
                                            // levelcap prev
                                            rtn.Add(p.Level == 0 ? 0 : Config.levelcap[p.Level - 1]);
                                            // levelcap next
                                            rtn.Add(Config.levelcap[p.Level]);
                                            rtn.Add(p.ClassTitle.ToUpper());
                                            rtn.Add(0);//(wootstatus.total);
                                                       //wootstatus.timezone = p.TimeZone;
                                            rtn.Add(0);//(wootstatus.getWootsToday());
                                            rtn.Add(0); //Config.level_woot_decay[p.Level - 1]);
                                            rtn.Add(0);
                                            //Convert.ToInt32(
                                            //    (wootstatus.nextdecay - DateTime.Now).TotalSeconds));
                                            rtn.Add(p.MaxEnergy);
                                            p.Send(rtn);
                                            //}
                                            //};

                                            //WootUpPlayer.getWootStatusAmount(this.PlayerIO, p.ConnectUserId,
                                            //    delegate(int amount)
                                            //    {
                                            //        wootup = amount;
                                            //        ondataloaded();
                                            //    }, p.TimeZone);

                                            //WootStatus.getWootStatus(this.PlayerIO, p.ConnectUserId,
                                            //    delegate(WootStatus status)
                                            //    {
                                            //        wootstatus = status;
                                            //        ondataloaded();
                                            //    }, false);
                                        }
                                    });

                                    break;
                                }

                                case "getLobbyProperties":
                                {
                                    var rtn = Message.Create(item.method);

                                    var today = DateTime.Today.AddHours(p.TimeZone);
                                    if (today > DateTime.Now)
                                    {
                                        today = today.AddHours(-24);
                                    }
                                    else if ((DateTime.Now - today).Hours > 24)
                                    {
                                        today = today.AddHours(24);
                                    }
                                    var lastlogin = p.PlayerObject.Contains("lastlogin")
                                        ? p.PlayerObject.GetDateTime("lastlogin").AddHours(p.TimeZone)
                                        : today;

                                    //rtn.Add(p.PlayerObject.Contains("lastlogin") && p.PlayerObject.GetDateTime("lastlogin"));
                                    rtn.Add((today - lastlogin).TotalHours > 24);
                                    p.PlayerObject.Set("lastlogin", today);
                                    p.PlayerObject.Save();

                                    p.Send(rtn);

                                    break;
                                }
                                case "getShop":
                                {
                                    p.RefreshPlayerObject(
                                        delegate
                                        {
                                            p.PayVault.Refresh(delegate { this.SendShopUpdate(item, p, true); });
                                        });
                                    break;
                                }

                                case "toggleProfile":
                                {
                                    p.PlayerObject.Set("visible", item.message.GetBoolean(0));
                                    p.PlayerObject.Save(delegate { p.Send(item.method, item.message.GetBoolean(0)); });
                                    break;
                                }

                                case "getProfile":
                                {
                                    var visible = true;
                                    if (p.PlayerObject.Contains("visible"))
                                    {
                                        visible = p.PlayerObject.GetBool("visible");
                                    }
                                    p.Send(item.method, visible);
                                    break;
                                }

                                case "useEnergy":
                                {
                                    var target = item.message.GetString(0);
                                    //foreach(ShopItem i in shopitems) {
                                    var itm = this.shophandler.GetShopItem(target);
                                    if (target == itm.key && (!p.PayVault.Has(itm.key) ||
                                                              itm.Reusable) &&
                                        itm.PriceEnergy > 0 &&
                                        (!itm.BetaOnly || p.HasBeta) &&
                                        (itm.Enabled || (itm.DevOnly && p.canbemod)) &&
                                        itm.MinClass <= p.Level &&
                                        (itm.Type != "potion" ||
                                         p.PayVault.Count(itm.key) < Config.max_potions))
                                    {
                                        if (p.UseEnergy(itm.EnergyPerClick))
                                        {
                                            p.SetEnergyStatus(itm.key, p.GetEnergyStatus(itm.key) + itm.EnergyPerClick);
                                            if (p.GetEnergyStatus(itm.key) >= itm.PriceEnergy)
                                            {
                                                p.SetEnergyStatus(itm.key, 0);
                                                p.PayVault.Give(
                                                    new[] { new BuyItemInfo(itm.key) },
                                                    delegate
                                                    {
                                                        p.SaveShop(delegate { this.SendShopUpdate(item, p, true); });
                                                    },
                                                    delegate
                                                    {
                                                        p.SaveShop(delegate { this.SendShopUpdate(item, p, false); });
                                                    }
                                                    );
                                            }
                                            else
                                            {
                                                p.SaveShop(delegate { this.SendShopUpdate(item, p, false); });
                                            }
                                        }
                                        else
                                        {
                                            p.Send(item.method, "error");
                                        }
                                    }
                                    //}
                                    break;
                                }

                                case "useGems":
                                {
                                    var target = item.message.GetString(0);
                                    var itm = this.shophandler.GetShopItem(target);
                                    if ((itm.Type != "potion" || p.PayVault.Count(itm.key) < Config.max_potions) &&
                                        (!itm.BetaOnly || p.HasBeta))
                                    {
                                        p.PayVault.Buy(true, new[] { new BuyItemInfo(item.message.GetString(0)) },
                                            delegate
                                            {
                                                this.CheckClubMembership(p,
                                                    delegate { this.SendShopUpdate(item, p, false); });
                                            },
                                            delegate { p.Send(item.method, "error"); });
                                    }
                                    else
                                    {
                                        p.Send(item.method, "error");
                                    }
                                    break;
                                }

                                case "getFriends":
                                {
                                    this.friendhandler.GetFriendKeys(p.ConnectUserId, delegate (string[] keys)
                                    {
                                        if (keys.Length > 0)
                                        {
                                            OnlineStatus.getOnlineStatus(this.PlayerIO, keys,
                                                delegate (OnlineStatus[] status)
                                                {
                                                    var rtn = Message.Create(item.method);
                                                    for (var i = 0; i < status.Length; i++)
                                                    {
                                                        if (status[i] != null)
                                                        {
                                                            status[i].ToMessage(rtn);
                                                        }
                                                    }
                                                    p.Send(rtn);
                                                });
                                        }
                                        else
                                        {
                                            p.Send(item.method);
                                        }
                                    });
                                    break;
                                }

                                case "getPending":
                                {
                                    this.friendhandler.GetPending(p.ConnectUserId, delegate (ArrayList emails)
                                    {
                                        var rtn = Message.Create(item.method);
                                        for (var i = 0; i < emails.Count; i++)
                                        {
                                            rtn.Add(emails[i]);
                                        }
                                        p.Send(rtn);
                                    });
                                    break;
                                }

                                case "getInvitesToMe":
                                {
                                    this.friendhandler.GetInvitesToMe(p.ConnectUserId, delegate (ArrayList inviteparams)
                                    {
                                        var rtn = Message.Create(item.method);
                                        for (var i = 0; i < inviteparams.Count; i++)
                                        {
                                            rtn.Add(inviteparams[i]);
                                        }
                                        p.Send(rtn);
                                    });
                                    break;
                                }

                                case "getBlockedUsers":
                                {
                                    this.friendhandler.GetBlockedUsers(p.ConnectUserId,
                                        delegate (ArrayList blockedparams)
                                        {
                                            var rtn = Message.Create(item.method);
                                            for (var i = 0; i < blockedparams.Count; i++)
                                            {
                                                rtn.Add(blockedparams[i]);
                                            }
                                            p.Send(rtn);
                                        });

                                    break;
                                }


                                case "createInvite":
                                {
                                    if (!p.HasFriendFeatures)
                                    {
                                        p.Send(item.method, false);
                                        return;
                                    }

                                    var friendmail = item.message.GetString(0);

                                    this.friendhandler.CreateInvitation(p.ConnectUserId,
                                        p.PlayerObject.GetString("name", ""), friendmail,
                                        delegate { p.Send(item.method, true, ""); }, delegate (int error)
                                        {
                                            switch (error)
                                            {
                                                case Friends.ERROR_MAX_FRIENDS:
                                                {
                                                    p.Send(item.method, false,
                                                        "You cannot have more than " + /*Friends.MAX_FRIENDS*/
                                                        this.friendhandler.getMaxFriendsAllowed() +
                                                        " friends and invites");
                                                    break;
                                                }
                                                case Friends.ERROR_INVITE_EXISTS:
                                                {
                                                    p.Send(item.method, false,
                                                        "You already have a pending invitation to this adress");
                                                    break;
                                                }
                                                case Friends.ERROR_CREATING:
                                                {
                                                    p.Send(item.method, false,
                                                        "There was an error sending the request. Please try again later.");
                                                    break;
                                                }
                                            }
                                        });
                                    break;
                                }

                                case "deletePending":
                                {
                                    var recipientEmail = item.message.GetString(0);
                                    Console.WriteLine("deletePending: " + recipientEmail);

                                    this.friendhandler.DeletePending(p.ConnectUserId, recipientEmail,
                                        delegate (bool success)
                                        {
                                            if (!success)
                                                this.PlayerIO.ErrorLog.WriteError(
                                                    "Error deleting pending invitation from " + p.ConnectUserId + " to " +
                                                    recipientEmail, "Invite not found",
                                                    "Error deleting pending invitation", null);
                                            p.Send(item.method, success);
                                        });
                                    break;
                                }

                                case "activateInvite":
                                {
                                    if (!p.HasFriendFeatures)
                                    {
                                        p.Send(item.method, false);
                                        return;
                                    }

                                    var invite_id = item.message.GetString(0);
                                    this.friendhandler.ActivateMyInvitation(p.ConnectUserId, invite_id,
                                        delegate { p.Send(item.method, true); }, delegate (int error)
                                        {
                                            switch (error)
                                            {
                                                case Friends.ERROR_INVITE_DOES_NOT_EXISTS:
                                                {
                                                    p.Send(item.method, false, "Sorry, the invitation does not exist.");
                                                    break;
                                                }
                                                case Friends.ERROR_ACTIVATING_OWN:
                                                {
                                                    this.PlayerIO.ErrorLog.WriteError(
                                                        "Player trying to activate his own invitation: " +
                                                        p.ConnectUserId, "Activating own invite",
                                                        "Activating invite with own userid", null);
                                                    p.Send(item.method, false, "Error activating the invitation");
                                                    break;
                                                }
                                            }
                                        }
                                        );
                                    break;
                                }

                                case "blockInvite":
                                {
                                    var invited_by = item.message.GetString(0);

                                    this.friendhandler.BlockInvite(p.ConnectUserId, invited_by,
                                        delegate { p.Send(item.method, true); }, delegate (int error)
                                        {
                                            switch (error)
                                            {
                                                case Friends.ERROR_INVITE_DOES_NOT_EXISTS:
                                                {
                                                    p.Send(item.method, false, "Sorry, the invitation does not exist.");
                                                    break;
                                                }
                                                case Friends.ERROR_SAVING:
                                                {
                                                    p.Send(item.method, false,
                                                        "There was an error blocking this invite. Please try again later.");
                                                    break;
                                                }
                                            }
                                        });

                                    break;
                                }

                                case "unblockInvite":
                                {
                                    var mail = item.message.GetString(0);
                                    var invited_by = item.message.GetString(1);
                                    this.friendhandler.UnblockInvite(p.ConnectUserId, mail, invited_by,
                                        delegate { p.Send(item.method, true); }, delegate (int error)
                                        {
                                            switch (error)
                                            {
                                                case Friends.ERROR_SAVING:
                                                {
                                                    p.Send(item.method, false,
                                                        "There was an error unblocking this user. Please try again later.");
                                                    break;
                                                }
                                            }
                                        });
                                    break;
                                }


                                case "answerInvite":
                                {
                                    if (!p.HasFriendFeatures)
                                    {
                                        p.Send(item.method, false);
                                        return;
                                    }

                                    var invited_by = item.message.GetString(0);
                                    var accept = item.message.GetBoolean(1);

                                    this.friendhandler.AnswerMyInvitation(p.ConnectUserId, invited_by, accept,
                                        delegate { p.Send(item.method, true); }, delegate (int error)
                                        {
                                            switch (error)
                                            {
                                                case Friends.ERROR_MAX_FRIENDS:
                                                {
                                                    p.Send(item.method, false,
                                                        "You cannot have more than " + /*Friends.MAX_FRIENDS*/
                                                        this.friendhandler.getMaxFriendsAllowed() +
                                                        " friends and invites");
                                                    break;
                                                }
                                                case Friends.ERROR_INVITE_DOES_NOT_EXISTS:
                                                {
                                                    p.Send(item.method, false, "Sorry, the invitation does not exist.");
                                                    break;
                                                }
                                            }
                                        }
                                        );

                                    break;
                                }
                                case "deleteFriend":
                                {
                                    var friendId = item.message.GetString(0);

                                    this.PlayerIO.BigDB.LoadSingle("PlayerObjects", "name", new Object[] { friendId },
                                        delegate (DatabaseObject po)
                                        {
                                            this.friendhandler.RemoveFriends(p.ConnectUserId, po.Key,
                                                delegate (bool success)
                                                {
                                                    if (!success)
                                                        this.PlayerIO.ErrorLog.WriteError(
                                                            "Error deleting friends: " + p.ConnectUserId + " and " +
                                                            po.Key, "Error removing friends", "Cannot delete friends",
                                                            null);
                                                    p.Send(item.method, success);
                                                });
                                        });

                                    break;
                                }

                                case "getOnlineStatus":
                                {
                                    var id = item.message.Count > 0 ? item.message.GetString(0) : p.ConnectUserId;
                                    OnlineStatus.getOnlineStatus(this.PlayerIO, id,
                                        delegate (OnlineStatus os) { p.Send(os.ToMessage(item.method)); });
                                    break;
                                }
                            }
                        }
                    }
                }
                this.queue = new List<QueueItem>();
            }
        }


        public override void GotMessage(Player player, Message m)
        {
            var haveSmileyPackage = player.PlayerObject.GetBool("haveSmileyPackage", false) ||
                                    player.PayVault.Has("pro");
            switch (m.Type)
            {
                case "getMySimplePlayerObject":
                {
                    this.AddToQueue(player.Id, player.ConnectUserId, m.Type, m);
                    break;
                }

                case "getLobbyProperties":
                {
                    this.AddToQueue(player.Id, player.ConnectUserId, m.Type, m);
                    break;
                }

                case "getShop":
                {
                    this.AddToQueue(player.Id, player.ConnectUserId, m.Type, m);
                    break;
                }
                case "useEnergy":
                {
                    Console.WriteLine("Player: " + player.Id + " used some energy, m.Type is = " + m.Type);
                    this.AddToQueue(player.Id, player.ConnectUserId, m.Type, m);
                    break;
                }

                case "toggleProfile":
                {
                    this.AddToQueue(player.Id, player.ConnectUserId, m.Type, m);
                    break;
                }

                case "getProfile":
                {
                    this.AddToQueue(player.Id, player.ConnectUserId, m.Type, m);
                    break;
                }

                case "getNews":
                {
                    this.GetLatestNewsMessage(delegate (Message news_msg) { player.Send(news_msg); }, m.GetString(0));
                    break;
                }

                case "timezone":
                {
                    if (!player.PlayerObject.Contains("timezone"))
                    {
                        var timezone = Convert.ToInt32(m.GetDouble(0) / 60);
                        timezone = Math.Max(-11, Math.Min(timezone, 14));
                        player.TimeZone = timezone;
                        player.PlayerObject.Save();
                    }
                    break;
                }

                case "checkUsername":
                {
                    var checkname = m.GetString(0);
                    this.PlayerIO.BigDB.Load("Usernames", checkname.ToLower(),
                        delegate (DatabaseObject result)
                        {
                            player.Send("checkUsername", checkname, (result == null || !result.Contains("owner")));
                        },
                        delegate { player.Send("checkUsername", checkname, false); }
                        );
                    break;
                }
                case "useGems":
                {
                    this.AddToQueue(player.Id, player.ConnectUserId, m.Type, m);
                    //Console.WriteLine("here baby!");
                    //player.PayVault.Buy(true, new BuyItemInfo[] { new BuyItemInfo(m.GetString(0)) }, delegate() {
                    //    player.Send(m.Type, "refresh");
                    //}, delegate(PlayerIOError e) { 
                    //    player.Send(m.Type, "error");
                    //});

                    break;
                }

                case "getRoom":
                {
                    var myroom = player.PlayerObject.GetString("room0", "");
                    if (haveSmileyPackage)
                    {
                        if (myroom != "")
                        {
                            player.Send("r", myroom);
                        }
                        else
                        {
                            player.Send("creating", player.PlayerObject.GetBool("haveSmileyPackage", false),
                                player.PayVault.Has("pro"));
                            this.createId(false, player);
                        }
                    }
                    else
                    {
                        player.Send("no!", player.PlayerObject.GetBool("haveSmileyPackage", false),
                            player.PayVault.Has("pro"));
                    }
                    break;
                }

                case "getSavedLevel":
                {
                    var type = m.GetInt(0);
                    var offset = m.GetInt(1);
                    var count = player.PayVault.Count("world" + type);
                    if (type == 0 && haveSmileyPackage) count++;

                    if (count == 0) return;
                    if (count > offset)
                    {
                        var roomid = player.PlayerObject.GetString("world" + type + "x" + offset, "");
                        if (roomid != "")
                        {
                            player.Send("r", roomid);
                        }
                        else
                        {
                            this.getUniqueId(false, player, delegate (string newid)
                            {
                                var newworld = new DatabaseObject();
                                newworld.Set("allowpotions", true);
                                newworld.Set("type", type);
                                newworld.Set("width", 25);
                                newworld.Set("height", 25);
                                newworld.Set("owner", player.ConnectUserId);
                                switch (type)
                                {
                                    case (int)WorldTypes.Small:
                                    {
                                        break;
                                    }
                                    case (int)WorldTypes.Medium:
                                    {
                                        newworld.Set("width", 50);
                                        newworld.Set("height", 50);
                                        break;
                                    }
                                    case (int)WorldTypes.Large:
                                    {
                                        newworld.Set("width", 100);
                                        newworld.Set("height", 100);
                                        break;
                                    }
                                    case (int)WorldTypes.Massive:
                                    {
                                        newworld.Set("width", 200);
                                        newworld.Set("height", 200);
                                        break;
                                    }
                                    case (int)WorldTypes.Huge:
                                    {
                                        newworld.Set("width", 300);
                                        newworld.Set("height", 300);
                                        break;
                                    }
                                    case (int)WorldTypes.Wide:
                                    {
                                        newworld.Set("width", 400);
                                        newworld.Set("height", 50);
                                        break;
                                    }
                                    case (int)WorldTypes.Great:
                                    {
                                        newworld.Set("width", 400);
                                        newworld.Set("height", 200);
                                        break;
                                    }
                                    case (int)WorldTypes.Tall:
                                    {
                                        newworld.Set("width", 100);
                                        newworld.Set("height", 400);
                                        break;
                                    }
                                    case (int)WorldTypes.UltraWide:
                                    {
                                        newworld.Set("width", 636);
                                        newworld.Set("height", 50);
                                        break;
                                    }
                                    case (int)WorldTypes.MoonLarge:
                                    {
                                        newworld.Set("width", 110);
                                        newworld.Set("height", 110);
                                        break;
                                    }
                                }
                                this.PlayerIO.BigDB.CreateObject("worlds", newid, newworld, delegate
                                {
                                    player.PlayerObject.Set("world" + type + "x" + offset, newid);
                                    player.PlayerObject.Save(delegate { player.Send("r", newid); });
                                });
                            });
                        }
                    }
                    break;
                }

                case "getBetaRoom":
                {
                    var myroom = player.PlayerObject.GetString("betaonlyroom", "");
                    if (haveSmileyPackage)
                    {
                        if (myroom != "")
                        {
                            player.Send("r", myroom);
                        }
                        else
                        {
                            player.Send("creating", player.PlayerObject.GetBool("haveSmileyPackage", false),
                                player.PayVault.Has("pro"));
                            this.createId(true, player);
                        }
                    }
                    else
                    {
                        player.Send("no!", player.PlayerObject.GetBool("haveSmileyPackage", false),
                            player.PayVault.Has("pro"));
                    }
                    break;
                }

                case "acceptTerms":
                {
                    player.PlayerObject.Set("termsVersion", Config.termsVersion);
                    if (player.PlayerObject.Contains("acceptTerms"))
                    {
                        player.PlayerObject.Remove("acceptTerms");
                    }
                    player.PlayerObject.Save(delegate { player.Send("acceptTerms"); });
                    break;
                }

                case "setUsername":
                {
                    var username = player.PlayerObject.GetString("name", null);
                    if (username == null && player.ConnectUserId != "simpleguest")
                    {
                        var newname = m.GetString(0).ToLower();
                        var test = new Regex("[^0-9a-z]");
                        if (test.IsMatch(newname))
                        {
                            player.Send("error",
                                "Your username contains invalid charaters. Valid charaters are 0-9 and A-Z");
                        }
                        else if (newname.Length > 20)
                        {
                            player.Send("error", "Your username cannot be more than 20 characters long.");
                        }
                        else if (newname.Length < 3)
                        {
                            player.Send("error", "Your username must be atleast 3 characters long.");
                        }
                        else
                        {
                            var obj = new DatabaseObject();
                            obj.Set("owner", player.ConnectUserId);
                            this.PlayerIO.BigDB.CreateObject("Usernames", newname, obj,
                                delegate
                                {
                                    Console.WriteLine("Set Username " + newname + " --> " + player.Name);
                                    player.PlayerObject.Set("name", newname);
                                    player.PlayerObject.Save(delegate
                                    {
                                        Console.WriteLine("Username saved " + player.Name);
                                        player.Send("username", newname);
                                        player.Send("setUsername");
                                    });
                                },
                                delegate
                                {
                                    player.Send("error", "The username " + newname.ToUpper() + " is already taken!");
                                }
                                );
                        }
                    }
                    else
                    {
                        player.Send("username", username);
                    }
                    break;
                }
                case "initNewUser":
                case "playerStats":

                case "createInvite":
                case "deletePending":
                case "activateInvite":
                case "answerInvite":
                case "blockInvite":
                case "unblockInvite":
                case "getPending":
                case "getInvitesToMe":
                case "getFriends":
                case "getBlockedUsers":
                case "deleteFriend":
                case "getOnlineStatus":
                {
                    this.AddToQueue(player.Id, player.ConnectUserId, m.Type, m);
                    break;
                }
                case "getProfileObject":
                {
                    var profilename = m.GetString(0);
                    new Profile().LoadProfile(this.PlayerIO, profilename, delegate (Message msg) { player.Send(msg); },
                        profilename == player.Name);

                    break;
                }
            }
        }


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

        private void createId(Boolean isbetaonly, Player player)
        {
            this.getUniqueId(isbetaonly, player, delegate (string newid)
            {
                player.PlayerObject.Set(isbetaonly ? "betaonlyroom" : "room0", newid);
                player.PlayerObject.Save(delegate { player.Send("r", newid); });
            });
        }

        private void RemoveDeprecatedValues(DatabaseObject playerobject)
        {
            var deprecated = false;
            // Convert club_membernumber to string
            if (playerobject.Contains("club_membernumber"))
            {
                var num = -1;
                try
                {
                    num = playerobject.GetInt("club_membernumber");
                }
                catch (PlayerIOError e)
                {
                    Console.WriteLine("Error:  " + e.Message);
                    // Is already a string
                    num = -1;
                }
                if (num > -1)
                {
                    deprecated = true;
                    playerobject.Remove("club_membernumber");
                    playerobject.Set("club_membernumber", num.ToString());
                }
            }

            for (var i = 0; i < this.DEPRECATED_PLAYEROBJECT_VALUES.Length; i++)
            {
                if (playerobject.Contains(this.DEPRECATED_PLAYEROBJECT_VALUES[i]))
                {
                    deprecated = true;
                    playerobject.Remove(this.DEPRECATED_PLAYEROBJECT_VALUES[i]);
                }
            }

            if (deprecated) playerobject.Save();
        }

        protected void GetLatestNewsMessage(Callback<Message> callback, string newskey = "")
        {
            this.PlayerIO.BigDB.LoadRange("News", "current", null, null, null, 1000, delegate (DatabaseObject[] newslist)
            {
                var news = newslist[0];
                if (newskey.Length > 0)
                {
                    for (var h = 0; h < newslist.Length; h++)
                    {
                        if (newslist[h].Key == newskey)
                        {
                            news = newslist[h];
                            break;
                        }
                    }
                }

                var msg = Message.Create("getNews");

                string[] mandatoryfields = { "header", "body", "date", "image" };
                var missingfiels = "";
                for (var i = 0; i < mandatoryfields.Length; i++)
                {
                    if (!news.Contains(mandatoryfields[i]))
                    {
                        missingfiels += mandatoryfields[i] + ",";
                    }
                    else
                    {
                        msg.Add(news.GetString(mandatoryfields[i]));
                    }
                }

                if (missingfiels.Length > 0)
                {
                    this.PlayerIO.ErrorLog.WriteError("News Error. News " + news.Key +
                                                      " does not contain these fields: " + missingfiels);
                }
                else
                {
                    callback.Invoke(msg);
                }
            });
        }
    }

    [RoomType("LobbyGuest188")]
    public class LobbyGuest : GameCode
    {
        //private Shop shophandler;
        //List<QueueItem> queue = new List<QueueItem>();

        public override void GameStarted()
        {
            base.GameStarted();
        }

        public override bool AllowUserJoin(Player player)
        {
            return true;
        }

        // This method is called whenever a player joins the game
        public override void UserJoined(Player player)
        {
            player.Send("connectioncomplete");
        }

        // This method is called when a player leaves the game
        public override void UserLeft(Player player)
        {
        }

        public override void GotMessage(Player player, Message m)
        {
            switch (m.Type)
            {
                case "getShop":
                {
                    // AddToQueue(player.Id, player.ConnectUserId, m.Type, m);
                    break;
                }

                case "getNews":
                {
                    this.GetLatestNewsMessage(delegate (Message news_msg) { player.Send(news_msg); }, m.GetString(0));
                    break;
                }

                case "checkUsername":
                {
                    var checkname = m.GetString(0);
                    Console.WriteLine("checkUsername: " + checkname);
                    this.PlayerIO.BigDB.Load("Usernames", checkname.ToLower(),
                        delegate (DatabaseObject result)
                        {
                            player.Send("checkUsername", checkname, (result == null || !result.Contains("owner")));
                        },
                        delegate { player.Send("checkUsername", checkname, false); }
                        );
                    break;
                }
                case "getProfileObject":
                {
                    new Profile().LoadProfile(this.PlayerIO, m.GetString(0), delegate (Message msg) { player.Send(msg); });

                    break;
                }
            }
        }

        //public override void AddToQueue(int id, string conncetUserId, string method, Message message)
        //{
        //    queue.Add(new QueueItem(id, conncetUserId, method, message));
        //    EmptyQueue();
        //}

        public override void EmptyQueue()
        {
            if (this.shophandler.isConfigLoaded)
            {
                foreach (var item in this.queue)
                {
                    foreach (var p in this.Players)
                    {
                        if (p.Id == item.id && p.ConnectUserId == item.connectUserId)
                        {
                            switch (item.method)
                            {
                                case "getShop":
                                {
                                    this.SendShopUpdate(item, p, true);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }


        //private void sendShopUpdate(QueueItem item, Player p, Boolean refresh)
        //{
        //    Message m = Message.Create(item.method, refresh, p.Energy, p.GetSecoundsToNextEnergy, p.MaxEnergy, p.EnergyRechargeSecounds, p.PayVault.Coins);
        //    p.Send(shophandler.GetUpdateMessage(p, m));
        //}
    }

    public class Profile
    {
        public void LoadProfile(Client client, string name, Callback<Message> callback, bool ignoreVisible = false)
        {
            client.BigDB.LoadSingle("PlayerObjects", "name", new Object[] { name }, delegate (DatabaseObject po)
              {
                  var rtn = Message.Create("getProfileObject");

                  if (po.GetBool("banned", false))
                  {
                      rtn.Add("error");
                      callback(rtn);
                      return;
                  }

                  var visible = po.GetBool("visible", true);
                  if (!visible && !ignoreVisible)
                  {
                      rtn.Add("private");
                      callback(rtn);
                      return;
                  }

                  rtn.Add("public");
                  rtn.Add(po.Key);
                  rtn.Add(po.GetString("name", ""));
                  rtn.Add(po.GetInt("smiley", 0));
                  rtn.Add(po.GetBool("haveSmileyPackage", false));
                  rtn.Add(po.GetBool("isModerator", false));
                // is club MemberAccessException?
                rtn.Add(po.Contains("club_expire") && po.GetDateTime("club_expire") > DateTime.Now);
                  rtn.Add((po.GetDateTime("club_expire", DateTime.Now) - DateTime.Now).TotalMilliseconds);
                  rtn.Add((po.GetDateTime("club_join", DateTime.Now) - DateTime.Now).TotalMilliseconds);
                  rtn.Add(po.GetString("club_membernumber", "0"));
                  rtn.Add(po.GetString("room0", ""));
                  rtn.Add(po.GetString("betaonlyroom", ""));

                  var worlds = new ArrayList();
                  var worldkeys = new ArrayList();
                  var worldnames = new ArrayList();
                  var mwn = po.GetObject("myworldnames");
                  foreach (var key in po.Properties)
                  {
                      if (key.StartsWith("world"))
                      {
                          worldkeys.Add(key);
                          worlds.Add(po[key]);
                        //Console.WriteLine(po[key] + " - " + mwn.GetString((string)po[key], ""));
                        worldnames.Add((mwn != null ? mwn.GetString((string)po[key], "") : ""));
                      }
                  }
                  rtn.Add(string.Join(",", worldkeys.ToArray(typeof(string)) as string[]));
                  rtn.Add(string.Join(",", worlds.ToArray(typeof(string)) as string[]));
                  rtn.Add(string.Join(",", worldnames.ToArray(typeof(string)) as string[]));

                  var wootup = -1;
                //WootStatus wootstatus = null;

                //Callback ondataloaded = delegate
                //{
                //    if (wootup >= 0 && wootstatus != null)
                //    {
                var level = 0;//wootstatus.level;
                rtn.Add(0);//(wootstatus.current);
                rtn.Add(level);
                // levelcap prev
                rtn.Add(0);//level == 0 ? 0 : Config.levelcap[level - 1]);
                           // levelcap next
                rtn.Add(0); //Config.levelcap[level]);
                rtn.Add(0); //Config.leveltitles[level - 1].ToUpper());
                rtn.Add(0); //wootstatus.total);

                callback(rtn);
                //    }
                //};

                //WootUpPlayer.getWootStatusAmount(client, po.Key, delegate(int amount)
                //{
                //    wootup = amount;
                //    ondataloaded();
                //}, po.GetInt("timezone", 0));

                //WootStatus.getWootStatus(client, po.Key, delegate(WootStatus status)
                //{
                //    wootstatus = status;
                //    ondataloaded();
                //}, false);
            }, delegate
            {
                var rtn = Message.Create("getProfileObject");
                rtn.Add("error");
                callback(rtn);
            });
        }
    }
}