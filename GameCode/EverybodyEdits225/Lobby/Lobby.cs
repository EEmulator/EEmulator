using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using EverybodyEdits.Common;
using EverybodyEdits.Game;
using EverybodyEdits.Game.Campaigns;
using EverybodyEdits.Game.Chat.Commands;
using EverybodyEdits.Game.Crews;
using EverybodyEdits.Game.Mail;
using PlayerIO.GameLibrary;

namespace EverybodyEdits.Lobby
{
    [RoomType("Lobby" + Config.VersionString)]
    public class GameCode : Game<LobbyPlayer>, IUpgradeRoom<LobbyPlayer>
    {
        private readonly string[] deprecatedPlayerobjectValues =
        {
            "worldName", "worldId", "lastip", "wOOt_test", "woot_test",
            "wOOt_test2", "Online", "currentWorldId", "currentWorldName",
            "club_membernumber"
        };

        private readonly Random rd = new Random();
        private readonly SmileyMap smileyMap = new SmileyMap();
        private Campaigns campaignHandler;
        protected Crews CrewsHandler;

        private Friends friendsHandler;
        protected List<QueueItem> Queue = new List<QueueItem>();
        private Registration registrationHandler;
        protected Shop ShopHandler;
        private bool rewardLock;
        private bool buyLock;
        public UpgradeChecker<LobbyPlayer> UpgradeChecker { get; private set; }

        public override void GameStarted()
        {
            // Always loads players PlayerObject
            this.PreloadPlayerObjects = true;
            this.PreloadPayVaults = true;

            this.campaignHandler = new Campaigns(this.PlayerIO, this.EmptyQueue);
            this.registrationHandler = new Registration(this.PlayerIO);
            this.ShopHandler = new Shop(this.PlayerIO, this.EmptyQueue);
            this.CrewsHandler = new Crews(this.PlayerIO);

            this.UpgradeChecker = new UpgradeChecker<LobbyPlayer>(this);
        }


        public override bool AllowUserJoin(LobbyPlayer player)
        {
            if (player.PlayerObject.Contains("linkedTo")) {
                player.Send("linked");
                return false;
            }
            foreach (var p in this.Players) {
                p.Disconnect();
            }

            return this.RoomId == player.ConnectUserId && base.AllowUserJoin(player);
        }

        public override void UserJoined(LobbyPlayer player)
        {
            var haveBeta = player.PlayerObject.GetBool("haveSmileyPackage", false) || player.PayVault.Has("pro");

            //Give pro to old beta members;
            if (!player.PayVault.Has("pro") && haveBeta) {
                player.PayVault.Give(new[] { new BuyItemInfo("pro") }, delegate { });
            }

            // Give energy to old users that had a level.
            if (player.PlayerObject.Contains("level")) {
                var oldPlayerLevel = player.PlayerObject.GetInt("level", 0);
                var addedEnergy = oldPlayerLevel * 16;
                player.PlayerObject.Set("maxEnergy", player.PlayerObject.GetInt("maxEnergy", 200) + addedEnergy);
                player.PlayerObject.Remove("level");
                player.PlayerObject.Save();

                player.Send("info", "Energy boost!",
                    "Hey " + player.Name.ToUpper() + "! Because you had level " + oldPlayerLevel +
                    " before the update, you were awarded " + addedEnergy + " total Energy!");
            }

            player.PayVault.Refresh(delegate {
                this.GiveBlocksFromRevamp(player);
            });

            if (haveBeta) {
                if (player.PlayerObject.GetString("room0", "idonthaveit") == "idonthaveit") {
                    player.Send("creating", player.PlayerObject.GetBool("haveSmileyPackage", false),
                        player.PayVault.Has("pro"));
                    this.CreateId(false, player);
                }
                if (player.PlayerObject.GetString("betaonlyroom", "idonthaveit") == "idonthaveit") {
                    player.Send("creating", player.PlayerObject.GetBool("haveSmileyPackage", false),
                        player.PayVault.Has("pro"));
                    this.CreateId(true, player);
                }
            }

            this.CheckGoldMembership(player, delegate { player.PlayerObject.Save(); });
            this.CheckGemCode(player);

            this.AwardWorldToNewPlayer(player,
                delegate {
                    TempBanCommand.CheckTempBanned(this.PlayerIO, player,
                        tempBanned => {
                            BanIpCommand.CheckIpBanned(this.PlayerIO, player,
                                ipBanned => {
                                    player.PlayerObject.Save(delegate { player.Send("connectioncomplete"); });
                                });
                        });
                });

            this.friendsHandler = new Friends(this.PlayerIO, player.ConnectUserId, player.Name) {
                PlayerIsGoldMember = player.HasGoldMembership,
                PlayerIsBetaMember = player.HasBeta
            };
        }

        public override void UserLeft(LobbyPlayer player)
        {
        }

        protected void AddToQueue(int id, string conncetUserId, string method, Message message)
        {
            this.Queue.Add(new QueueItem(id, conncetUserId, method, message));
            this.EmptyQueue();
        }

        protected void SendShopUpdate(QueueItem item, LobbyPlayer p, bool refresh)
        {
            var m = Message.Create(item.Method, refresh, p.Energy, p.SecondsToNextEnergy, p.MaxEnergy,
                p.EnergyDelay);
            p.Send(this.ShopHandler.GetUpdateMessage(p, m));
        }

        private void GiveBlocksFromRevamp(LobbyPlayer player)
        {
            // Viking -> Medieval + Carnival + Stone
            this.TransferPacks(player, new[] { "brickviking" }, new[] { "brickmedieval", "brickbgcarnival", "brickstone" });

            // Plate iron and Warning signs -> Industrial
            this.TransferPacks(player, new[] { "brickplateiron", "bricksigns" }, new[] { "brickindustrial" });

            // Castle -> Medieval
            this.TransferPacks(player, new[] { "brickcastle" }, new[] { "brickmedieval" });

            // Timbered -> Medieval + Clay
            this.TransferPacks(player, new[] { "bricktimbered" }, new[] { "brickmedieval", "brickclay" });

            // Jungle Ruins -> Jungle
            this.TransferPacks(player, new[] { "brickjungleruins" }, new[] { "brickjungle" });

            // Sci-Fi 2013 -> Sci-Fi
            this.TransferPacks(player, new[] { "brickscifi2013" }, new[] { "brickscifi" });

            // Mars -> Desert + Carnival + Canvas + Neon
            this.TransferPacks(player, new[] { "brickmars" },
                new[] { "brickdesert", "brickbgcarnival", "brickbgcanvas", "brickneon" });

            // Coin gates -> Coin doors
            this.TransferLimitedPack(player, "brickcoingate", "brickcoindoor");

            // Blue coin gates -> Blue coin doors
            this.TransferLimitedPack(player, "brickbluecoingate", "brickbluecoindoor");

            // Zobie doors -> Zombie effect
            this.TransferLimitedPack(player, "brickzombiedoor", "brickeffectzombie");
        }

        private void TransferPacks(LobbyPlayer player, string[] removedItems, string[] replacements)
        {
            if (!removedItems.Any(pack => player.PayVault.Has(pack))) {
                return;
            }

            foreach (var replacement in replacements.Where(replacement => !player.PayVault.Has(replacement))) {
                player.PayVault.Give(new[] { new BuyItemInfo(replacement) }, delegate { });
            }

            foreach (var removedItem in removedItems) {
                var item = player.PayVault.First(removedItem);
                if (item != null) {
                    player.PayVault.Consume(new[] { item }, delegate { });
                }
            }
        }

        private void TransferLimitedPack(LobbyPlayer player, string removedItem, string replacement)
        {
            if (!player.PayVault.Has(removedItem)) {
                return;
            }

            var old = player.PayVault.Items.Where(item => item.ItemKey == removedItem).ToArray();

            var count = old.Length;
            var items = new BuyItemInfo[count];
            for (var i = 0; i < items.Length; i++) {
                items[i] = new BuyItemInfo(replacement);
            }
            player.PayVault.Give(items, delegate { });

            player.PayVault.Consume(old, delegate { });
        }

        private void CheckGoldMembership(LobbyPlayer p, Callback callback = null)
        {
            // Replace Existing BC Membership with Gold Membership!
            if (p.PlayerObject.Contains("club_join")) {
                p.PlayerObject.Set("gold_join", p.PlayerObject.GetDateTime("club_join"));
                p.PlayerObject.Remove("club_join");
            }
            if (p.PlayerObject.Contains("club_expire")) {
                p.PlayerObject.Set("gold_expire", p.PlayerObject.GetDateTime("club_expire"));
                p.PlayerObject.Remove("club_expire");
            }

            VaultItem gold = null;
            var addMonts = 0;
            /* Possible beta trial?
             * 
            if (p.HasBeta && !p.PlayerObject.Contains("gold_join"))
            {
                //trial = true;
                addMonts = 1;
            }*/
            if (p.PayVault.Has("gold1")) {
                gold = p.PayVault.First("gold1");
                addMonts = 1;
            }
            else if (p.PayVault.Has("gold3")) {
                gold = p.PayVault.First("gold3");
                addMonts = 3;
            }
            else if (p.PayVault.Has("gold6")) {
                gold = p.PayVault.First("gold6");
                addMonts = 6;
            }
            else if (p.PayVault.Has("gold12")) {
                gold = p.PayVault.First("gold12");
                addMonts = 12;
            }
            else {
                if (callback != null) {
                    callback.Invoke();
                }
                return;
            }

            if (addMonts > 0) {
                var expire = p.PlayerObject.GetDateTime("gold_expire", DateTime.Now);
                var newExpirationDate = expire > DateTime.Now
                    ? expire.AddMonths(addMonts)
                    : DateTime.Now.AddMonths(addMonts);

                p.PlayerObject.Set("gold_expire", newExpirationDate);
                if (!p.PlayerObject.Contains("gold_join")) {
                    p.PlayerObject.Set("gold_join", DateTime.Now);
                }

                Callback consumeandsave = () => {
                    lock (p) {
                        p.PlayerObject.Save(() => {
                            if (gold != null) {
                                p.PayVault.Consume(new[] { gold },
                                    () => {
                                        if (callback != null) {
                                            callback.Invoke();
                                        }
                                    },
                                    value => {
                                        this.PlayerIO.ErrorLog.WriteError("Error consuming gold membership");
                                        if (callback != null) {
                                            callback.Invoke();
                                        }
                                    });
                            }
                        }, delegate {
                            this.PlayerIO.ErrorLog.WriteError("Error saving playerobject after adding gold membership");
                            if (callback != null) {
                                callback.Invoke();
                            }
                        });
                    }
                };

                consumeandsave();
            }
            else {
                if (callback != null) {
                    callback.Invoke();
                }
            }
        }

        private void CheckGemCode(LobbyPlayer p, Callback callback = null)
        {
            VaultItem vault;
            int giveGems;

            if (p.PayVault.Has("gemcode50")) {
                vault = p.PayVault.First("gemcode50");
                giveGems = 50;
            }
            else if (p.PayVault.Has("gemcode105")) {
                vault = p.PayVault.First("gemcode105");
                giveGems = 105;
            }
            else if (p.PayVault.Has("gemcode220")) {
                vault = p.PayVault.First("gemcode220");
                giveGems = 220;
            }
            else {
                if (callback != null) {
                    callback.Invoke();
                }
                return;
            }

            GetGemCode(p, giveGems, key => p.PayVault.Consume(new[] { vault }, () => {
                p.Send("copyPrompt", "Here's your gem code for " + giveGems + " Gems!", key,
                    "Thanks for purchasing!\nYou can always see your codes under: Gems > My codes");
                if (callback != null) {
                    callback.Invoke();
                }
            }, err => {
                p.Send("info", "Whoops!",
                    "Looks like something went wrong. If you lost any gems, please contact a staff member and take a screenshot of this prompt.\n\nError: " +
                    err.Message);
                if (callback != null) {
                    callback.Invoke();
                }
            }));
        }

        private void GetGemCode(BasePlayer p, int amount, Callback<string> code)
        {
            const string allowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            var pass = "";

            for (var i = 0; i < 4; i++) {
                for (var j = 0; j < 4; j++) {
                    pass += allowedChars[rd.Next(0, allowedChars.Length)];
                }
                pass += i < 3 ? "-" : "";
            }

            var dbo = new DatabaseObject();
            dbo.Set("Gems", amount);
            dbo.Set("PurchasedBy", p.ConnectUserId);
            PlayerIO.BigDB.CreateObject("Coupons", pass, dbo, value => code(pass));
        }

        private void CheckChangeUsername(LobbyPlayer p, Callback callback = null)
        {
            Callback setChangeName = () => {
                p.PlayerObject.Set("changename", true);
                p.PlayerObject.Save(callback);
            };

            if (p.PayVault.Has("changeusername") && !p.PlayerObject.Contains("changename")) {
                setChangeName();
            }
            else {
                if (callback != null) {
                    callback.Invoke();
                }
            }
        }

        private void AwardWorldToNewPlayer(LobbyPlayer player, Callback callback)
        {
            if (!player.IsGuest && !player.PlayerObject.Contains("worldhome")) {
                this.GetUniqueId(false, newid => this.PlayerIO.BigDB.Load("worlds", "PWvmGW1Jl3bUI",
                    original => {
                        var worldname = "My Home World";

                        original.Set("name", worldname);
                        original.Set("owner", player.ConnectUserId);
                        original.Set("plays", 0);
                        original.Set("Favorites", 0);
                        original.Set("Likes", 0);
                        original.Set("Crew", "");

                        this.PlayerIO.BigDB.CreateObject("worlds", newid, original, value => {
                            player.PlayerObject.Set("worldhome", newid);
                            if (!player.PlayerObject.Contains("myworldnames")) {
                                player.PlayerObject.Set("myworldnames", new DatabaseObject());
                            }
                            player.PlayerObject.GetObject("myworldnames").Set(newid, worldname);
                            callback.Invoke();
                        }, value => callback.Invoke());
                    }, value => callback.Invoke()));
            }
            else {
                callback.Invoke();
            }
        }

        protected virtual void EmptyQueue()
        {
            if (!this.ShopHandler.IsConfigLoaded || !this.campaignHandler.Initialized) {
                return;
            }

            foreach (var tempItem in this.Queue) {
                var item = tempItem;
                foreach (var tempPlayer in this.Players) {
                    var p = tempPlayer;

                    if (p.Id != item.Id || p.ConnectUserId != item.ConnectUserId) {
                        continue;
                    }

                    switch (item.Method) {
                        case "getMySimplePlayerObject": {
                                p.RefreshPlayerObject(delegate {
                                    Console.WriteLine("refreshed");

                                    if (p == null) {
                                        this.PlayerIO.ErrorLog.WriteError(
                                            "Horriable Error, Player is null in getMySimplePlayerObject!");
                                        return;
                                    }

                                    if (p.PlayerObject == null) {
                                        this.PlayerIO.ErrorLog.WriteError(
                                            "Horriable Error, PlayerObject is null in getMySimplePlayerObject!");
                                        p.Disconnect();
                                        return;
                                    }

                                    lock (p) {
                                        this.RemoveDeprecatedValues(p.PlayerObject);

                                        var rtn = Message.Create(item.Method);
                                        rtn.Add(p.PlayerObject.GetString("name", ""));
                                        rtn.Add(p.Smiley);
                                        rtn.Add(p.Aura);
                                        rtn.Add(p.AuraColor);

                                        rtn.Add(p.PlayerObject.GetBool("chatbanned", false));
                                        rtn.Add(p.CanChat);
                                        rtn.Add(p.PlayerObject.GetBool("haveSmileyPackage", false));
                                        rtn.Add(p.PlayerObject.GetBool("isAdministrator", false));
                                        rtn.Add(p.PlayerObject.GetBool("isModerator", false));
                                        rtn.Add(p.HasGoldMembership);
                                        rtn.Add(p.SmileyGoldBorder);
                                        rtn.Add(
                                            (p.PlayerObject.GetDateTime("gold_expire", DateTime.Now) - DateTime.Now)
                                                .TotalMilliseconds);
                                        rtn.Add(
                                            (p.PlayerObject.GetDateTime("gold_join", DateTime.Now) - DateTime.Now)
                                                .TotalMilliseconds);
                                        rtn.Add(p.JustJoinedGold);
                                        rtn.Add(p.PlayerObject.GetString("room0", ""));
                                        rtn.Add(p.PlayerObject.GetString("betaonlyroom", ""));
                                        rtn.Add(p.PlayerObject.GetString("worldhome", ""));
                                        var worlds = new ArrayList();
                                        var worldkeys = new ArrayList();
                                        var worldnames = new ArrayList();
                                        var mwn = p.PlayerObject.GetObject("myworldnames");
                                        foreach (var key in p.PlayerObject.Properties.Where(key => key.StartsWith("world"))) {
                                            worldkeys.Add(key);
                                            worlds.Add(p.PlayerObject[key]);
                                            worldnames.Add(mwn != null
                                                ? mwn.GetString((string)p.PlayerObject[key], "")
                                                : "");
                                        }
                                        rtn.Add(string.Join("᎙", worldkeys.ToArray(typeof(string)) as string[]));
                                        rtn.Add(string.Join("᎙", worlds.ToArray(typeof(string)) as string[]));
                                        rtn.Add(string.Join("᎙", worldnames.ToArray(typeof(string)) as string[]));

                                        rtn.Add(p.PlayerObject.GetBool("visible", true));
                                        rtn.Add(p.IsBanned);

                                        rtn.Add(p.PlayerObject.GetInt("termsVersion", 0));
                                        rtn.Add(p.PlayerObject.GetInt("tutorialVersion", 0));
                                        rtn.Add(p.Badge);
                                        rtn.Add(p.ConfirmedEmail);

                                        rtn.Add(p.MaxEnergy);

                                        rtn.Add(p.PlayerObject.GetBool("changename", false));

                                        var favorites = p.PlayerObject.GetObject("favorites");
                                        if (favorites == null || favorites.Properties.Count == 0) {
                                            rtn.Add("", "");
                                            rtn.Add(p.IsStaff);
                                            p.Send(rtn);
                                        }
                                        else {
                                            this.PlayerIO.BigDB.LoadKeys("Worlds", favorites.Properties.ToArray(),
                                                favWorlds => {
                                                    var w = from world in favWorlds
                                                            where world != null
                                                            select new {
                                                                Id = world.Key,
                                                                Name = world.GetString("name", "Unnamed")
                                                            };

                                                    rtn.Add(string.Join("᎙", w.Select(it => it.Id).ToArray()));
                                                    rtn.Add(string.Join("᎙", w.Select(it => it.Name).ToArray()));

                                                    rtn.Add(p.IsStaff);
                                                    p.Send(rtn);
                                                });
                                        }
                                    }
                                });

                                break;
                            }

                        case "getLobbyProperties": {
                                if (this.rewardLock) {
                                    this.PlayerIO.ErrorLog.WriteError("User tried to refresh lobby too fast: " + p.ConnectUserId);
                                    return;
                                };

                                this.rewardLock = true;

                                p.PayVault.Refresh(() => {
                                    p.RefreshPlayerObject(() => {
                                        var rtn = Message.Create(item.Method);

                                        var today = DateTime.UtcNow.AddHours(-p.TimeZone).Date;
                                        var lastlogin = p.PlayerObject.GetDateTime("lastlogin", today);

                                        var daysSinceLastLogin = (today - lastlogin).TotalDays;
                                        var firstDailyLogin = daysSinceLastLogin >= 1;
                                        var loggedInEarlier = daysSinceLastLogin < 2;
                                        var continuesStreak = loggedInEarlier && firstDailyLogin;

                                        p.PlayerObject.Set("lastlogin", today);

                                        rtn.Add(firstDailyLogin);
                                        rtn.Add(continuesStreak ? p.LoginStreak : -1);

                                        if (!loggedInEarlier) {
                                            p.LoginStreak = 0;
                                        }
                                        else if (continuesStreak) {
                                            if (p.LoginStreak == 0) {
                                                rtn.Add("energy", 10);
                                                p.AddEnergy(10);
                                            }
                                            else {
                                                switch (p.LoginStreak % 7 + 1) {
                                                    case 1:
                                                        rtn.Add("energy", 25);
                                                        p.AddEnergy(25);
                                                        break;
                                                    case 2:
                                                        rtn.Add("energy", 50);
                                                        p.AddEnergy(50);
                                                        break;
                                                    case 3:
                                                        rtn.Add("energy", 75);
                                                        p.AddEnergy(75);
                                                        break;
                                                    case 4:
                                                        rtn.Add("energy", 100);
                                                        p.AddEnergy(100);
                                                        break;
                                                    case 5:
                                                        rtn.Add("energy", 150);
                                                        p.AddEnergy(150);
                                                        break;
                                                    case 6:
                                                        rtn.Add("energy", 200);
                                                        p.AddEnergy(200);
                                                        break;
                                                    case 7:
                                                        uint gems;
                                                        switch (p.LoginStreak % 28 + 1) {
                                                            case 14:
                                                                gems = 2;
                                                                break;
                                                            case 28:
                                                                gems = 3;
                                                                break;
                                                            default:
                                                                gems = 1;
                                                                break;
                                                        }
                                                        rtn.Add("gems", gems);
                                                        p.PayVault.Credit(gems, "Daily login streak: " + p.LoginStreak,
                                                            delegate { });
                                                        break;
                                                }
                                            }

                                            p.LoginStreak++;
                                        }

                                        p.PlayerObject.Save(() => this.rewardLock = false);

                                        p.Send(rtn);

                                        Console.WriteLine("First daily login: " + firstDailyLogin + ", can get rewards: " +
                                                          loggedInEarlier + ", streak: " + p.LoginStreak);
                                    });
                                });
                                break;
                            }
                        case "getShop": {
                                p.RefreshPlayerObject(
                                    delegate { p.PayVault.Refresh(delegate { this.SendShopUpdate(item, p, true); }); });
                                break;
                            }

                        case "getMyCodes": {
                                PlayerIO.BigDB.LoadRange("Coupons", "PurchasedBy", new object[] { p.ConnectUserId }, null,
                                    null, 1000, delegate (DatabaseObject[] obj) {
                                        var codes =
                                            obj.Where(databaseObject => databaseObject.GetString("ClaimedBy", "") == "")
                                                .Aggregate("",
                                                    (current, databaseObject) =>
                                                        current + ("(" + databaseObject.GetInt("Gems", 0)) + ") " +
                                                        databaseObject.Key + "\n");

                                        if (codes == "") {
                                            codes = "You don't have any active codes!";
                                        }

                                        p.Send("copyPrompt", "Here's a list of your active codes!", codes,
                                            "Press Ctrl+A and then Ctrl+C to copy all codes at once.\nCodes that are already used aren't listed here.");
                                    });
                                break;
                            }

                        case "redeemCode": {
                                Console.WriteLine(item.Message.GetString(0));
                                var usedText = "This code has already been activated.";
                                var invalidText = "This code is invalid.";
                                var errorText = "Something went terribly wrong.";
                                var respond = new Action<bool, string>((isLegit, notMessage) => {
                                    p.Send("redeemCode", isLegit);
                                    p.Send("info", isLegit ? "Code activated" : "Code failed", notMessage);
                                });

                                PlayerIO.BigDB.Load("Coupons", item.Message.GetString(0), delegate (DatabaseObject obj) {
                                    if (obj == null) {
                                        respond(false, invalidText);
                                        return;
                                    }

                                    if (!obj.Contains("ClaimedBy")) {
                                        if (obj.Contains("Gems")) {
                                            var gemsToGive = (uint)obj.GetInt("Gems", 0);
                                            obj.Set("ClaimedBy", p.ConnectUserId);

                                            obj.Save(true, () => // Optimistic lock ensures that we are the only one who activated this code
                                            {
                                                p.PayVault.Credit(gemsToGive, "Activated code: " + item.Message.GetString(0), () => {
                                                    respond(true, obj.GetString("ActivateMessage", "Thank you for activating this code. You received " + gemsToGive + " Gems."));
                                                });
                                            }, e => {
                                                respond(false, usedText);
                                            });
                                        }
                                        else {
                                            respond(false, errorText);
                                        }
                                    }
                                    else {
                                        if (p.IsAdmin) {
                                            respond(false, usedText + "\n\nOnly for mods:\nConnectUserId = " + obj.GetString("ClaimedBy", "NaN"));
                                        }
                                        else {
                                            respond(false, usedText);
                                        }
                                    }
                                }, error => {
                                    respond(false, invalidText);
                                });
                                break;
                            }

                        case "toggleProfile": {
                                p.PlayerObject.Set("visible", item.Message.GetBoolean(0));
                                p.PlayerObject.Save(delegate { p.Send(item.Method, item.Message.GetBoolean(0)); });
                                break;
                            }

                        case "getProfile": {
                                var visible = true;
                                if (p.PlayerObject.Contains("visible")) {
                                    visible = p.PlayerObject.GetBool("visible");
                                }
                                p.Send(item.Method, visible);
                                break;
                            }

                        case "useAllEnergy": {
                                if (this.buyLock) {
                                    this.PlayerIO.ErrorLog.WriteError("User tried to buy too fast: " + p.ConnectUserId);
                                    return;
                                }

                                this.buyLock = true;

                                Callback<PlayerIOError> errorCallback = delegate {
                                    this.buyLock = false;
                                    p.SaveShop(() => { this.SendShopUpdate(item, p, false); });
                                };
                                Callback successCallback = delegate {
                                    this.buyLock = false;
                                    p.SaveShop(() => { this.SendShopUpdate(item, p, true); });
                                };


                                var target = item.Message.GetString(0);

                                var itm = this.ShopHandler.GetShopItem(target);
                                if (target == itm.Key &&
                                    itm.CanBuy(p.PayVault.Count(itm.Key)) &&
                                    itm.PriceEnergy > 0 &&
                                    (!itm.BetaOnly || p.HasBeta) &&
                                    (itm.Enabled || (itm.DevOnly && p.IsAdmin))) {
                                    p.RefreshPlayerObject(() => {
                                        if (p.UseEnergy(itm.PriceEnergy - p.GetEnergyStatus(itm.Key))) {
                                            p.SetEnergyStatus(itm.Key, 0);
                                            p.PayVault.Give(new[] { new BuyItemInfo(itm.Key) },
                                                successCallback,
                                                errorCallback);
                                        }
                                        else {
                                            var energyMultiplier = p.Energy / itm.EnergyPerClick;
                                            if (energyMultiplier > 0) {
                                                var energyToUse = itm.EnergyPerClick * energyMultiplier;
                                                if (p.UseEnergy(energyToUse)) {
                                                    p.SetEnergyStatus(itm.Key, p.GetEnergyStatus(itm.Key) + energyToUse);
                                                    this.buyLock = false;
                                                    p.SaveShop(() => { this.SendShopUpdate(item, p, false); });
                                                }
                                                else {
                                                    this.buyLock = false;
                                                    p.Send(Shop.NotEnoughEnergyError(item.Method));
                                                }
                                            }
                                            else {
                                                this.buyLock = false;
                                                p.Send(Shop.NotEnoughEnergyError(item.Method));
                                            }
                                        }
                                    });
                                }
                                break;
                            }

                        case "useEnergy": {
                                if (this.buyLock) {
                                    this.PlayerIO.ErrorLog.WriteError("User tried to buy too fast: " + p.ConnectUserId);
                                    return;
                                }

                                this.buyLock = true;

                                Callback<PlayerIOError> errorCallback = delegate {
                                    this.buyLock = false;
                                    p.SaveShop(() => { this.SendShopUpdate(item, p, false); });
                                };
                                Callback successCallback = delegate {
                                    this.buyLock = false;
                                    p.SaveShop(() => { this.SendShopUpdate(item, p, true); });
                                };

                                var target = item.Message.GetString(0);
                                var itm = this.ShopHandler.GetShopItem(target);
                                if (target == itm.Key &&
                                    itm.CanBuy(p.PayVault.Count(itm.Key)) &&
                                    itm.PriceEnergy > 0 &&
                                    (!itm.BetaOnly || p.HasBeta) &&
                                    (itm.Enabled || (itm.DevOnly && p.IsAdmin))) {
                                    p.RefreshPlayerObject(() => {
                                        if (p.GetEnergyStatus(itm.Key) > itm.PriceEnergy) {
                                            p.SetEnergyStatus(itm.Key, 0);
                                            p.PayVault.Give(
                                                new[] { new BuyItemInfo(itm.Key) },
                                                successCallback,
                                                errorCallback);
                                        }
                                        else if (p.UseEnergy(itm.EnergyPerClick)) {
                                            p.SetEnergyStatus(itm.Key, p.GetEnergyStatus(itm.Key) + itm.EnergyPerClick);
                                            if (p.GetEnergyStatus(itm.Key) >= itm.PriceEnergy) {
                                                p.SetEnergyStatus(itm.Key, 0);
                                                p.PayVault.Give(
                                                    new[] { new BuyItemInfo(itm.Key) },
                                                    successCallback,
                                                    errorCallback);
                                            }
                                            else {
                                                errorCallback(null);
                                            }
                                        }
                                        else {
                                            this.buyLock = false;
                                            p.Send(Shop.NotEnoughEnergyError(item.Method));
                                        }
                                    });
                                }
                                break;
                            }

                        case "useGems": {
                                if (this.buyLock) {
                                    this.PlayerIO.ErrorLog.WriteError("User tried to buy too fast: " + p.ConnectUserId);
                                    return;
                                }

                                this.buyLock = true;

                                Callback<PlayerIOError> errorCallback = delegate {
                                    this.buyLock = false;
                                    p.Send(Shop.GemsError(item.Method));
                                };
                                Callback successCallback = delegate {
                                    this.CheckGoldMembership(p,
                                        delegate {
                                            this.CheckGemCode(p,
                                                delegate {
                                                    this.CheckChangeUsername(p,
                                                        delegate {
                                                            this.buyLock = false;
                                                            this.SendShopUpdate(item, p, false);
                                                        });
                                                });
                                        });
                                };

                                var target = item.Message.GetString(0);
                                var itm = this.ShopHandler.GetShopItem(target);
                                if (target == itm.Key &&
                                    itm.CanBuy(p.PayVault.Count(itm.Key)) &&
                                    itm.PriceGems > 0 &&
                                    (!itm.BetaOnly || p.HasBeta) &&
                                    (itm.Enabled || (itm.DevOnly && p.IsAdmin))) {
                                    if (p.PayVault.Coins >= itm.PriceGems) {
                                        if (ShopHandler.Sales.IsBlackFriday) {
                                            p.PayVault.Give(new[] { new BuyItemInfo(target) },
                                                () => {
                                                    p.PayVault.Debit((uint)itm.PriceGems, "Bought item during Black Friday: " + target,
                                                        successCallback, errorCallback);
                                                }, errorCallback);
                                        }
                                        else {
                                            p.PayVault.Buy(true, new[] { new BuyItemInfo(target) }, successCallback, errorCallback);
                                        }
                                    }
                                    else {
                                        errorCallback(null);
                                    }
                                }
                                else {
                                    errorCallback(null);
                                }
                                break;
                            }

                        case "getCampaigns": {
                                CampaignPlayer.Load(this.PlayerIO, p.ConnectUserId, progressTable =>
                                    p.Achievements.Refresh(() =>
                                        p.Send(this.campaignHandler.GetMessage(p, progressTable))));
                                break;
                            }

                        case "changeSmiley": {
                                var smileyId = item.Message.GetInt(0);

                                p.PayVault.Refresh(() => {
                                    if (this.smileyMap.SmileyIsLegit(p, smileyId, this.ShopHandler)) {
                                        p.Smiley = smileyId;
                                        p.Send(item.Method);
                                        p.PlayerObject.Save();
                                    }
                                });
                                break;
                            }

                        case "changeAura": {
                                var auraId = item.Message.GetInt(0);
                                var auraColor = item.Message.GetInt(1);

                                p.PayVault.Refresh(() => {
                                    if (this.smileyMap.AuraIsLegit(p, auraId, auraColor, this.ShopHandler)) {
                                        p.Aura = auraId;
                                        p.AuraColor = auraColor;
                                        p.Send(item.Method);
                                        p.PlayerObject.Save();
                                    }
                                });
                                break;
                            }

                        case "changeBadge": {
                                var badgeId = item.Message.GetString(0);

                                p.Achievements.Refresh(() => {
                                    var badge = p.Achievements.Get(badgeId);
                                    if (badge != null && badge.Completed) {
                                        p.Badge = badge.Id;
                                        p.Send(item.Method);
                                        p.PlayerObject.Save();
                                    }
                                    else {
                                        p.Badge = "";
                                        p.Send(item.Method);
                                        p.PlayerObject.Save();
                                    }
                                });
                                break;
                            }
                        case "getNotifications": {
                                this.InitNotifications(true, p, () => {
                                    NotificationHelper.GetDismissedNotifications(
                                        this.PlayerIO.BigDB, p.ConnectUserId,
                                        dismisses => {
                                            var nots =
                                                p.EENotifications.Where(i => !dismisses.Contains(i.Key))
                                                    .ToArray();
                                            var msg = Message.Create(item.Method);
                                            foreach (var not in nots) {
                                                msg.Add(not.Key,
                                                    not.Channel,
                                                    not.Title,
                                                    not.Body,
                                                    not.PublishDate.AddHours(-p.TimeZone).ToString("g"),
                                                    not.RoomId,
                                                    not.ImageUrl);
                                            }
                                            p.Send(msg);
                                        });
                                });
                                break;
                            }
                        case "getMails": {
                                MailHelper.GetMail(this.PlayerIO.BigDB, p.ConnectUserId, mails => {
                                    var msg = Message.Create(item.Method);
                                    if (mails.Length == 0) {
                                        p.Send(msg);
                                        return;
                                    }

                                    this.PlayerIO.BigDB.LoadKeys("PlayerObjects", mails.Select(it => it.From).ToArray(),
                                        playerObjects => {
                                            for (var i = 0; i < playerObjects.Length; i++) {
                                                if (playerObjects[i] == null) {
                                                    continue;
                                                }

                                                msg.Add(mails[i].Key,
                                                    playerObjects[i].GetString("name", ""),
                                                    mails[i].Subject,
                                                    mails[i].Body);
                                            }

                                            p.Send(msg);
                                        });
                                });
                                break;
                            }

                        default: {
                                this.friendsHandler.ProcessMessages(item, p);
                                break;
                            }
                    }
                }
            }
            this.Queue = new List<QueueItem>();
        }


        public override void GotMessage(LobbyPlayer player, Message m)
        {
            var haveSmileyPackage = player.PlayerObject.GetBool("haveSmileyPackage", false) ||
                                    player.PayVault.Has("pro");
            switch (m.Type) {
                case "getMySimplePlayerObject": {
                        this.AddToQueue(player.Id, player.ConnectUserId, m.Type, m);
                        break;
                    }

                case "getLobbyProperties": {
                        this.AddToQueue(player.Id, player.ConnectUserId, m.Type, m);
                        break;
                    }

                case "getShop": {
                        this.AddToQueue(player.Id, player.ConnectUserId, m.Type, m);
                        break;
                    }
                case "useAllEnergy":
                case "useEnergy": {
                        Console.WriteLine("Player: " + player.Id + " used some energy, m.Type is = " + m.Type);
                        this.AddToQueue(player.Id, player.ConnectUserId, m.Type, m);
                        break;
                    }

                case "toggleProfile": {
                        this.AddToQueue(player.Id, player.ConnectUserId, m.Type, m);
                        break;
                    }

                case "getProfile": {
                        this.AddToQueue(player.Id, player.ConnectUserId, m.Type, m);
                        break;
                    }

                case "getNews": {
                        this.GetLatestNewsMessage(player.Send, m.GetString(0));
                        break;
                    }

                case "timezone": {
                        var timezone = Convert.ToInt32(m.GetDouble(0) / 60);
                        timezone = Math.Max(-11, Math.Min(timezone, 14));
                        player.TimeZone = timezone;
                        player.PlayerObject.Save();
                        break;
                    }

                case "isSubscribedToCrew": {
                        var crewId = m.GetString(0);

                        if (crewId == "everybodyeditsstaff") {
                            player.Send(m.Type, true);
                            break;
                        }

                        NotificationHelper.GetSubscriptions(this.PlayerIO.BigDB, player.ConnectUserId,
                            subs => { player.Send(m.Type, subs.Contains("crew" + crewId)); });
                        break;
                    }
                case "dismissNotification": {
                        this.InitNotifications(false, player, () => {
                            NotificationHelper.DismissNotification(this.PlayerIO.BigDB, player.ConnectUserId, m.GetString(0),
                                player.EENotifications);
                        });
                        break;
                    }
                case "deleteMail": {
                        MailHelper.DeleteMail(this.PlayerIO.BigDB, player.ConnectUserId, m.GetString(0));
                        break;
                    }

                case "sendMail": {
                        var target = m.GetString(0).Trim().ToLower();
                        var subject = string.Concat(m.GetString(1).Trim().Take(50));
                        var body = string.Concat(m.GetString(2).Trim().Take(420));

                        if (string.IsNullOrWhiteSpace(target) ||
                            string.IsNullOrWhiteSpace(subject) ||
                            string.IsNullOrWhiteSpace(body)) {
                            player.Send(m.Type, false, "Missing required field.");
                            return;
                        }

                        this.PlayerIO.BigDB.LoadSingle("PlayerObjects", "name", new object[] { target }, t => {
                            if (t == null) {
                                player.Send(m.Type, false, "Unknown user.");
                                return;
                            }

                            var targetId = t.Key;
                            this.friendsHandler.GetFriendKeys(targetId, friends => {
                                if (!friends.Contains(player.ConnectUserId)) {
                                    player.Send(m.Type, false, "The recipient doesn't have you in their friendlist.");
                                    return;
                                }

                                MailHelper.SendMail(this.PlayerIO.BigDB, player.ConnectUserId, targetId, subject, body);
                                player.Send("info", "Sent", "The mail has been sent!");
                                player.Send(m.Type, true);
                            });
                        }, e => player.Send(m.Type, false, "Unknown user."));
                        break;
                    }

                case "useGems": {
                        this.AddToQueue(player.Id, player.ConnectUserId, m.Type, m);
                        break;
                    }

                case "getRoom": {
                        var myroom = player.PlayerObject.GetString("room0", "");
                        if (haveSmileyPackage) {
                            if (myroom != "") {
                                player.Send("r", myroom);
                            }
                            else {
                                player.Send("creating", player.PlayerObject.GetBool("haveSmileyPackage", false),
                                    player.PayVault.Has("pro"));
                                this.CreateId(false, player);
                            }
                        }
                        else {
                            player.Send("no!", player.PlayerObject.GetBool("haveSmileyPackage", false),
                                player.PayVault.Has("pro"));
                        }
                        break;
                    }

                case "getSavedLevel": {
                        var type = m.GetInt(0);
                        var offset = m.GetInt(1);
                        var count = player.PayVault.Count("world" + type);
                        if (type == 0 && haveSmileyPackage) {
                            count++;
                        }

                        if (count == 0) {
                            return;
                        }
                        if (count > offset) {
                            var roomid = player.PlayerObject.GetString("world" + type + "x" + offset, "");
                            if (roomid != "") {
                                player.Send("r", roomid);
                            }
                            else {
                                this.GetUniqueId(false, delegate (string newid) {
                                    var newworld = new DatabaseObject();
                                    newworld.Set("type", type);
                                    newworld.Set("width", 25);
                                    newworld.Set("height", 25);
                                    newworld.Set("owner", player.ConnectUserId);
                                    switch (type) {
                                        case (int)WorldTypes.Small: {
                                                break;
                                            }
                                        case (int)WorldTypes.Medium: {
                                                newworld.Set("width", 50);
                                                newworld.Set("height", 50);
                                                break;
                                            }
                                        case (int)WorldTypes.Large: {
                                                newworld.Set("width", 100);
                                                newworld.Set("height", 100);
                                                break;
                                            }
                                        case (int)WorldTypes.Massive: {
                                                newworld.Set("width", 200);
                                                newworld.Set("height", 200);
                                                break;
                                            }
                                        case (int)WorldTypes.Huge: {
                                                newworld.Set("width", 300);
                                                newworld.Set("height", 300);
                                                break;
                                            }
                                        case (int)WorldTypes.Wide: {
                                                newworld.Set("width", 400);
                                                newworld.Set("height", 50);
                                                break;
                                            }
                                        case (int)WorldTypes.Great: {
                                                newworld.Set("width", 400);
                                                newworld.Set("height", 200);
                                                break;
                                            }
                                        case (int)WorldTypes.Tall: {
                                                newworld.Set("width", 100);
                                                newworld.Set("height", 400);
                                                break;
                                            }
                                        case (int)WorldTypes.UltraWide: {
                                                newworld.Set("width", 636);
                                                newworld.Set("height", 50);
                                                break;
                                            }
                                        case (int)WorldTypes.MoonLarge: {
                                                newworld.Set("width", 110);
                                                newworld.Set("height", 110);
                                                break;
                                            }
                                        case (int)WorldTypes.VerticalGreat: {
                                                newworld.Set("width", 200);
                                                newworld.Set("height", 400);
                                                break;
                                            }
                                        case (int)WorldTypes.Big: {
                                                newworld.Set("width", 150);
                                                newworld.Set("height", 150);
                                                break;
                                            }
                                    }
                                    this.PlayerIO.BigDB.CreateObject("worlds", newid, newworld, delegate {
                                        player.PlayerObject.Set("world" + type + "x" + offset, newid);
                                        player.PlayerObject.Save(delegate { player.Send("r", newid); });
                                    });
                                });
                            }
                        }
                        break;
                    }

                case "getBetaRoom": {
                        var myroom = player.PlayerObject.GetString("betaonlyroom", "");
                        if (haveSmileyPackage) {
                            if (myroom != "") {
                                player.Send("r", myroom);
                            }
                            else {
                                player.Send("creating", player.PlayerObject.GetBool("haveSmileyPackage", false),
                                    player.PayVault.Has("pro"));
                                this.CreateId(true, player);
                            }
                        }
                        else {
                            player.Send("no!", player.PlayerObject.GetBool("haveSmileyPackage", false),
                                player.PayVault.Has("pro"));
                        }
                        break;
                    }

                case "initNewUser":
                case "playerStats":
                case "getBlockStatus":
                case "getCampaigns":
                case "changeSmiley":
                case "changeAura":
                case "changeBadge":
                case "createInvite":
                case "deleteInvite":
                case "answerInvite":
                case "blockUserInvites":
                case "blockAllInvites":
                case "getPending":
                case "getInvitesToMe":
                case "getFriends":
                case "getNotifications":
                case "getMails":
                case "getBlockedUsers":
                case "deleteFriend":
                case "GetOnlineStatus":
                case "redeemCode":
                case "getMyCodes": {
                        this.AddToQueue(player.Id, player.ConnectUserId, m.Type, m);
                        break;
                    }
                case "getProfileObject": {
                        var profilename = m.GetString(0);
                        new Profile().LoadProfile(player, this.PlayerIO, profilename, player.Send,
                            profilename == player.Name);

                        break;
                    }
                case "getCrews": {
                        new Profile().LoadCrews(this.PlayerIO, m.GetString(0), player.Send);
                        break;
                    }

                default: {
                        registrationHandler.HandleMessage(player, m);
                        this.CrewsHandler.HandleMessage(player, m);
                        break;
                    }
            }
        }

        private void InitNotifications(bool force, LobbyPlayer p, Action callback)
        {
            if (!force && p.EENotifications != null) {
                callback();
                return;
            }

            NotificationHelper.GetSubscriptions(this.PlayerIO.BigDB, p.ConnectUserId,
                subs => {
                    NotificationHelper.GetNotifications(this.PlayerIO.BigDB, subs,
                        notifications => {
                            p.EENotifications =
                                notifications.OrderByDescending(n => n.PublishDate)
                                    .Take(NotificationHelper.MaxTotalNotifications)
                                    .ToArray();

                            callback();
                        });
                });
        }

        private void GetUniqueId(bool isbetaonly, Callback<string> myCallback)
        {
            string newid;
            if (isbetaonly) {
                newid = "BW" +
                        Convert.ToBase64String(
                            BitConverter.GetBytes((DateTime.Now - new DateTime(1980, 3, 25)).TotalMilliseconds))
                            .Replace("=", "")
                            .Replace("+", "_")
                            .Replace("/", "-");
            }
            else {
                newid = "PW" +
                        Convert.ToBase64String(
                            BitConverter.GetBytes((DateTime.Now - new DateTime(1981, 3, 25)).TotalMilliseconds))
                            .Replace("=", "")
                            .Replace("+", "_")
                            .Replace("/", "-");
            }

            this.PlayerIO.BigDB.Load("Worlds", newid, delegate (DatabaseObject o) {
                if (o != null) {
                    this.GetUniqueId(isbetaonly, myCallback);
                }
                else {
                    myCallback(newid);
                }
            });
        }

        private void CreateId(bool isbetaonly, LobbyPlayer player)
        {
            this.GetUniqueId(isbetaonly, delegate (string newid) {
                this.PlayerIO.BigDB.LoadOrCreate("worlds", newid,
                    delegate (DatabaseObject o) {
                        o.Set("owner", player.ConnectUserId);
                        player.PlayerObject.Set(isbetaonly ? "betaonlyroom" : "room0", newid);
                        player.PlayerObject.Save(delegate { player.Send("r", newid); });
                        o.Set("name", isbetaonly ? "My Beta Only World" : "My Beta World");
                        o.Save();
                    });
            });
        }

        private void RemoveDeprecatedValues(DatabaseObject playerobject)
        {
            var deprecated = false;

            foreach (var deprecatedValues in this.deprecatedPlayerobjectValues.Where(playerobject.Contains)) {
                deprecated = true;
                playerobject.Remove(deprecatedValues);
            }

            if (deprecated) {
                playerobject.Save();
            }
        }

        protected void GetLatestNewsMessage(Callback<Message> callback, string newskey = "")
        {
            this.PlayerIO.BigDB.LoadRange("News", "current", null, null, null, 1000, delegate (DatabaseObject[] newslist) {
                if (newslist.Length > 0) {
                    var news = newslist[0];
                    if (newskey.Length > 0) {
                        foreach (var n in newslist.Where(n => n.Key == newskey)) {
                            news = n;
                            break;
                        }
                    }

                    var msg = Message.Create("getNews");

                    string[] mandatoryfields = { "header", "body", "date", "image" };
                    var missingfiels = "";
                    foreach (var field in mandatoryfields) {
                        if (!news.Contains(field)) {
                            missingfiels += field + ",";
                        }
                        else {
                            msg.Add(news.GetString(field));
                        }
                    }

                    if (missingfiels.Length > 0) {
                        this.PlayerIO.ErrorLog.WriteError("News Error. News " + news.Key +
                                                          " does not contain these fields: " + missingfiels);
                    }
                    else {
                        callback.Invoke(msg);
                    }
                }
            });
        }
    }

    [RoomType("LobbyGuest" + Config.VersionString)]
    public class LobbyGuest : GameCode
    {
        public override bool AllowUserJoin(LobbyPlayer player)
        {
            return true;
        }

        public override void UserJoined(LobbyPlayer player)
        {
            player.Send("connectioncomplete");
        }

        public override void UserLeft(LobbyPlayer player)
        {
        }

        public override void GotMessage(LobbyPlayer player, Message m)
        {
            switch (m.Type) {
                case "getShop": {
                        AddToQueue(player.Id, player.ConnectUserId, m.Type, m);
                        break;
                    }

                case "getNews": {
                        this.GetLatestNewsMessage(player.Send, m.GetString(0));
                        break;
                    }

                case "getCrew": {
                        var crew = new Crew(this.PlayerIO);
                        crew.Load(Regex.Replace(m.GetString(0), @"\s+", "").ToLower(),
                            () => { crew.SendGetMessage(player); });
                        break;
                    }

                case "checkUsername": {
                        var checkname = m.GetString(0);
                        Console.WriteLine("checkUsername: " + checkname);
                        this.PlayerIO.BigDB.Load("Usernames", checkname.ToLower(),
                            delegate (DatabaseObject result) {
                                player.Send("checkUsername", checkname,
                                    result == null || (!result.Contains("owner") && !result.Contains("oldowner")));
                            },
                            delegate { player.Send("checkUsername", checkname, false); }
                            );
                        break;
                    }
                case "getProfileObject": {
                        new Profile().LoadProfile(player, this.PlayerIO, m.GetString(0), player.Send);
                        break;
                    }
                case "getCrews": {
                        new Profile().LoadCrews(this.PlayerIO, m.GetString(0), player.Send);
                        break;
                    }
                default: {
                        this.CrewsHandler.HandleMessage(player, m);
                        break;
                    }
            }
        }

        protected override void EmptyQueue()
        {
            if (!this.ShopHandler.IsConfigLoaded) {
                return;
            }

            foreach (var item in this.Queue) {
                foreach (var p in this.Players) {
                    if (p.Id == item.Id && p.ConnectUserId == item.ConnectUserId) {
                        switch (item.Method) {
                            case "getShop": {
                                    this.SendShopUpdate(item, p, true);
                                    break;
                                }
                        }
                    }
                }
            }
        }
    }

    public class Profile
    {
        public void LoadProfile(LobbyPlayer pl, Client client, string name, Callback<Message> callback,
            bool ignoreVisible = false)
        {
            var isStaff = pl.IsAdmin || pl.IsModerator;

            client.BigDB.LoadSingle("PlayerObjects", "name", new object[] { name }, delegate (DatabaseObject po) {
                var rtn = Message.Create("getProfileObject");

                if (po == null || (po.GetBool("banned", false) && !isStaff)) {
                    rtn.Add("error");
                    callback(rtn);
                    return;
                }

                var visible = po.GetBool("visible", true);
                if (!visible && !ignoreVisible && !isStaff) {
                    rtn.Add("private");
                    callback(rtn);
                    return;
                }

                rtn.Add("public");
                rtn.Add(po.Key);
                rtn.Add(po.GetString("name", ""));
                rtn.Add(po.GetString("oldname", ""));
                rtn.Add(po.GetInt("smiley", 0));
                rtn.Add(po.GetInt("maxEnergy", 200));
                rtn.Add(po.GetBool("haveSmileyPackage", false));
                rtn.Add(po.GetBool("isAdministrator", false));
                rtn.Add(po.Contains("gold_expire") && po.GetDateTime("gold_expire") > DateTime.Now);
                rtn.Add((po.GetDateTime("gold_expire", DateTime.Now) - DateTime.Now).TotalMilliseconds);
                rtn.Add((po.GetDateTime("gold_join", DateTime.Now) - DateTime.Now).TotalMilliseconds);
                rtn.Add(po.GetString("room0", ""));
                rtn.Add(po.GetString("betaonlyroom", ""));

                var worlds = new ArrayList();
                var worldkeys = new ArrayList();
                var worldnames = new ArrayList();
                var mwn = po.GetObject("myworldnames");
                foreach (var key in po.Properties.Where(key => key.StartsWith("world"))) {
                    worldkeys.Add(key);
                    worlds.Add(po[key]);
                    worldnames.Add(mwn != null ? mwn.GetString((string)po[key], "") : "");
                }
                rtn.Add(string.Join("᎙", worldkeys.ToArray(typeof(string)) as string[]));
                rtn.Add(string.Join("᎙", worlds.ToArray(typeof(string)) as string[]));
                rtn.Add(string.Join("᎙", worldnames.ToArray(typeof(string)) as string[]));
                callback(rtn);
            }, delegate {
                var rtn = Message.Create("getProfileObject");
                rtn.Add("error");
                callback(rtn);
            });
        }

        public void LoadCrews(Client client, string name, Callback<Message> callback)
        {
            CommonPlayer.GetId(client.BigDB, name, playerId => {
                client.BigDB.Load("CrewMembership", playerId, membership => {
                    var m = Message.Create("getCrews");
                    if (membership == null || membership.Count == 0) {
                        callback(m);
                        return;
                    }

                    client.BigDB.LoadKeys("Crews", membership.Properties.ToArray(), crews => {
                        foreach (var crew in crews) {
                            if (crew == null) continue;

                            m.Add(crew.Key);
                            m.Add(crew.GetString("Name", crew.Key));
                            m.Add(crew.GetString("LogoWorld", ""));
                        }
                        callback(m);
                    });
                });
            });
        }
    }
}
