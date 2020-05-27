using System;
using System.Collections.Generic;
using System.Linq;
using EverybodyEdits.Common;
using EverybodyEdits.Game;
using EverybodyEdits.Game.Crews;
using EverybodyEdits.Lobby;
using PlayerIO.GameLibrary;

namespace EverybodyEdits.Crews
{
    public class CrewShop
    {
        private readonly Client client;
        private readonly Crew crew;
        private readonly Sales sales;

        public CrewShop(Client client, Crew crew, Callback callback)
        {
            this.client = client;
            this.crew = crew;
            this.sales = new Sales();
            Items = new List<CrewItem>();

            this.client.BigDB.LoadRange("CrewItems", "PriceCoins", null, null, null, 1000,
                crewItems =>
                {
                    this.CreateObject("EnergyStatus");
                    this.CreateObject("Faceplates");

                    foreach (var item in crewItems)
                    {
                        var coins = this.sales.IsBlackFriday ? (uint)Math.Ceiling((double)item.GetUInt("PriceCoins") / 2) : item.GetUInt("PriceCoins");
                        var energy = this.sales.IsBlackFriday ? (int)Math.Ceiling((double)item.GetInt("PriceEnergy", -1) / 2) : item.GetInt("PriceEnergy", -1);
                        var energyPerClick = this.sales.IsBlackFriday ? this.sales.FixEnergyPerClick(item.GetInt("EnergyPerClick", 50), energy) : item.GetInt("EnergyPerClick", 50);
                        var label = this.sales.IsBlackFriday ? "blackfriday" : item.GetString("Label", "");

                        this.Items.Add(new CrewItem(item.Key, coins)
                        {
                            PriceEnergy = energy,
                            EnergyPerClick = energyPerClick,
                            Title = item.GetString("Title", ""),
                            Description = item.GetString("Description", ""),
                            Enabled = item.GetBool("Enabled", false),
                            BitmapOffset = item.GetInt("BitmapOffset", 0),
                            OnSale = item.GetBool("OnSale", false),
                            IsFeatured = item.GetBool("IsFeatured", false),
                            IsNew = item.GetBool("IsNew", false),
                            MaxPurchases = item.GetInt("MaxPurchases", 0),
                            Label = label,
                            LabelColor = item.GetString("LabelColor", "#FFAA00")
                        });
                    }

                    callback();
                });
        }

        private List<CrewItem> Items { get; set; }

        private DatabaseObject EnergyStatus
        {
            get { return this.crew.DatabaseObject.GetObject("EnergyStatus"); }
        }


        private Message CantUseShopError(string type)
        {
            return Message.Create(type,
                "error",
                "Can't buy this item!",
                "You need to have Shop Management rights to be able to buy crew items.\n\nIn addition, you need a minimum of 260 maximum energy to be able to buy crew items.");
        }

        public void GotMessage(CommonPlayer player, Message message)
        {
            switch (message.Type)
            {
                case "useEnergy":
                {
                    if (!this.CanUseShop(player))
                    {
                        player.Send(this.CantUseShopError(message.Type));
                        break;
                    }

                    var target = message.GetString(0);
                    var item = this.GetShopItem(target);

                    if (item != null &&
                        this.CanBuy(item, player) && item.PriceEnergy > 0 &&
                        (item.Enabled || player.IsAdmin || player.IsModerator))
                    {
                        player.RefreshPlayerObject(() =>
                        {
                            if (this.EnergyStatus.GetInt(item.Key) > item.PriceEnergy)
                            {
                                this.BuyItem(item.Key);
                                player.PlayerObject.Save(() =>
                                    this.SendShopMessage(Message.Create(message.Type), player, true));
                            }
                            else if (player.UseEnergy(item.EnergyPerClick))
                            {
                                this.UpdateEnergyStatus(item.Key, item.EnergyPerClick);
                                if (this.EnergyStatus.GetInt(item.Key) >= item.PriceEnergy)
                                {
                                    this.BuyItem(item.Key);
                                    player.PlayerObject.Save(() =>
                                        this.SendShopMessage(Message.Create(message.Type), player, true));
                                }
                                else
                                {
                                    player.PlayerObject.Save(() =>
                                        this.SendShopMessage(Message.Create(message.Type), player, false));
                                }
                            }
                            else
                            {
                                player.Send(Shop.NotEnoughEnergyError(message.Type));
                            }
                        });
                    }
                    break;
                }
                case "useAllEnergy":
                {
                    if (!this.CanUseShop(player))
                    {
                        player.Send(this.CantUseShopError(message.Type));
                        break;
                    }

                    var target = message.GetString(0);
                    var item = this.GetShopItem(target);

                    if (item != null &&
                        this.CanBuy(item, player) && item.PriceEnergy > 0 &&
                        (item.Enabled || player.IsAdmin || player.IsModerator))
                    {
                        player.RefreshPlayerObject(() =>
                        {
                            if (player.UseEnergy(item.PriceEnergy - this.GetEnergyStatus(item.Key)))
                            {
                                this.BuyItem(item.Key);
                                player.PlayerObject.Save(() =>
                                    this.SendShopMessage(Message.Create(message.Type), player, true));
                            }
                            else
                            {
                                var energyMultiplier = player.Energy/item.EnergyPerClick;
                                if (energyMultiplier > 0)
                                {
                                    var energyToUse = item.EnergyPerClick*energyMultiplier;
                                    if (player.UseEnergy(energyToUse))
                                    {
                                        this.UpdateEnergyStatus(item.Key, energyToUse);
                                        player.PlayerObject.Save(() =>
                                            this.SendShopMessage(Message.Create(message.Type), player, false));
                                    }
                                    else
                                    {
                                        player.Send(Shop.NotEnoughEnergyError(message.Type));
                                    }
                                }
                                else
                                {
                                    player.Send(Shop.NotEnoughEnergyError(message.Type));
                                }
                            }
                        });
                    }
                    else
                    {
                        player.Send(message.Type, false);
                    }
                    break;
                }
                case "useGems":
                {
                    if (!this.CanUseShop(player))
                    {
                        player.Send(this.CantUseShopError(message.Type));
                        break;
                    }

                    var target = message.GetString(0);
                    var item = this.GetShopItem(target);

                    if (item != null &&
                        this.CanBuy(item, player) &&
                        (item.Enabled || player.IsAdmin || player.IsModerator))
                    {
                        player.PayVault.Refresh(() =>
                        {
                            player.PayVault.Debit(item.PriceCoins, "Bought " + item.Title + " for " + this.crew.Name,
                                () =>
                                {
                                    this.BuyItem(item.Key);
                                    this.SendShopMessage(Message.Create(message.Type), player, false);
                                },
                                error =>
                                {
                                    player.Send(message.Type, "error", "Oops",
                                        "Looks like something went wrong. Do you have enough gems?");
                                });
                        });
                    }
                    break;
                }

                case "getShop":
                {
                    player.RefreshPlayerObject(
                        () =>
                        {
                            player.PayVault.Refresh(
                                () => this.SendShopMessage(Message.Create(message.Type), player, true));
                        });
                    break;
                }
            }
        }

        private bool CanUseShop(CommonPlayer player, bool ignoreEnergy = false)
        {
            if (!ignoreEnergy && this.crew.GetRankId(player) != 0 && player.MaxEnergy < 260)
            {
                return false;
            }

            return this.crew.HasPower(player, CrewPower.ShopAccess)
                   && !player.IsBanned && !player.IsTempBanned;
        }

        private void SendShopMessage(Message rtn, CommonPlayer player, bool refresh)
        {
            rtn.Add(refresh, player.Energy, player.SecondsToNextEnergy, player.MaxEnergy, player.EnergyDelay);

            if (this.CanUseShop(player, true))
            {
                foreach (var item in this.Items.Where(it => it.Enabled))
                {
                    rtn.Add(item.Key,
                        item.PriceEnergy,
                        item.EnergyPerClick,
                        this.GetEnergyStatus(item.Key),
                        item.PriceCoins,
                        this.GetCount(item),
                        item.Title,
                        item.Description,
                        item.BitmapOffset,
                        item.OnSale,
                        item.IsFeatured,
                        item.IsNew,
                        item.MaxPurchases > 0,
                        item.MaxPurchases,
                        !this.CanBuy(item, player),
                        item.Label,
                        item.LabelColor);
                }
            }
            player.Send(rtn);
        }

        private void UpdateEnergyStatus(string key, int increaseBy)
        {
            var oldAmount = this.GetEnergyStatus(key);
            this.EnergyStatus.Set(key, oldAmount + increaseBy);
        }

        private int GetEnergyStatus(string key)
        {
            return this.EnergyStatus.GetInt(key, 0);
        }

        private void BuyItem(string key)
        {
            this.EnergyStatus.Remove(key);

            switch (key)
            {
                case "logoworld":
                {
                    this.GetUniqueId(newId =>
                    {
                        var newWorld = new DatabaseObject();
                        newWorld.Set("name", "Logo");
                        newWorld.Set("width", 100);
                        newWorld.Set("height", 100);
                        newWorld.Set("owner", this.crew.Creator);
                        newWorld.Set("Crew", this.crew.Id);
                        newWorld.Set("IsCrewLogo", true);
                        Console.WriteLine("Bought logo world: " + newId);

                        this.client.BigDB.CreateObject("Worlds", newId, newWorld, world =>
                        {
                            var w = new World(this.client);
                            w.FromDatabaseObject(world);
                            w.Save(true);
                            this.crew.LogoWorld = w.Key;
                            Console.WriteLine("Saved logo world: " + w.Key);
                            this.crew.Save();
                        });
                    });
                    break;
                }
                case "rankcreation":
                {
                    this.crew.SetUnlocked("Ranks");
                    this.crew.AddRank();
                    this.crew.Save();
                    break;
                }
                case "rank":
                {
                    this.crew.AddRank();
                    this.crew.Save();
                    break;
                }
                case "memberdescriptions":
                {
                    this.crew.SetUnlocked("Descriptions");
                    this.crew.Save();
                    break;
                }
                case "unlockprofilecolor":
                {
                    this.crew.SetUnlocked("Colors");
                    this.crew.SetUnlocked("ColorPick");
                    this.crew.Save();
                    break;
                }
                case "picknewcolor":
                {
                    this.crew.SetUnlocked("ColorPick");
                    this.crew.Save();
                    break;
                }
                case "profiledivider":
                {
                    this.crew.ProfileDividers += 1;
                    this.crew.Save();
                    break;
                }
                case "unlockfaceplates":
                {
                    this.crew.SetUnlocked("Faceplates");
                    this.crew.Save();
                    break;
                }

                default:
                {
                    if (key.StartsWith("faceplate"))
                    {
                        var id = key.Substring(9);
                        this.crew.Faceplates.Set(id, true);
                        this.crew.Save();
                    }
                    break;
                }
            }
        }

        private void GetUniqueId(Callback<string> callback)
        {
            var newid = "PW" +
                        Convert.ToBase64String(
                            BitConverter.GetBytes((DateTime.Now - new DateTime(1981, 3, 25)).TotalMilliseconds))
                            .Replace("=", "")
                            .Replace("+", "_")
                            .Replace("/", "-");

            this.client.BigDB.Load("Worlds", newid, o =>
            {
                if (o != null)
                {
                    this.GetUniqueId(callback);
                }
                else
                {
                    callback(newid);
                }
            });
        }

        private bool CanBuy(CrewItem item, CommonPlayer player)
        {
            switch (item.Key)
            {
                case "logoworld":
                    return this.crew.LogoWorld == "";
                case "rankcreation":
                    return !this.crew.Unlocked("Ranks");
                case "rank":
                    return this.crew.Unlocked("Ranks") && this.crew.Ranks.Count < item.MaxPurchases + 2;
                case "memberdescriptions":
                    return !this.crew.Unlocked("Descriptions");
                case "unlockprofilecolor":
                    return !this.crew.Unlocked("Colors");
                case "picknewcolor":
                    return this.crew.Unlocked("Colors") && !this.crew.Unlocked("ColorPick");
                case "profiledivider":
                    return false;
                case "unlockfaceplates":
                    return !this.crew.Unlocked("Faceplates");

                default:
                {
                    if (!item.Key.StartsWith("faceplate"))
                    {
                        return false;
                    }
                    if (!this.crew.Unlocked("Faceplates"))
                    {
                        return false;
                    }

                    var id = item.Key.Substring(9);
                    return !this.crew.Faceplates.Contains(id) && (id != "gold" || player.HasGoldMembership);
                }
            }
        }

        private int GetCount(CrewItem item)
        {
            switch (item.Key)
            {
                case "rank":
                    return this.crew.Ranks.Count - 2;

                default:
                    return 0;
            }
        }

        private CrewItem GetShopItem(string key)
        {
            return this.Items.FirstOrDefault(it => it.Key == key);
        }

        private void CreateObject(string name)
        {
            var obj = this.crew.DatabaseObject.GetObject(name);
            if (obj != null)
            {
                return;
            }
            obj = new DatabaseObject();
            this.crew.DatabaseObject.Set(name, obj);
        }
    }
}