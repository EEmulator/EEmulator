using System;
using System.Collections.Generic;
using System.Linq;
using PlayerIO.GameLibrary;
using EverybodyEdits.Common;

namespace EverybodyEdits.Lobby
{
    public class Shop
    {
        private readonly List<ShopItem> shopitems = new List<ShopItem>();
        public readonly Sales Sales = new Sales();

        public Shop(Client client, Callback callback)
        {
            client.BigDB.LoadRange("PayVaultItems", "PriceCoins", null, null, null, 1000,
                ob =>
                {
                    foreach (var dbo in ob)
                    {
                        this.shopitems.Add(new ShopItem(dbo, Sales));
                    }
                    this.IsConfigLoaded = true;
                    callback();
                });
        }

        public bool IsConfigLoaded { get; private set; }

        public Message GetUpdateMessage(LobbyPlayer p, Message m)
        {
            if (p.IsGuest)
            {
                return m;
            }

            foreach (var i in this.shopitems)
            {
                if (!i.Enabled && (!i.DevOnly || !p.IsAdmin))
                {
                    continue;
                }
                if (i.BetaOnly && !p.HasBeta)
                {
                    continue;
                }
                m.Add(i.Key);
                m.Add(i.Type);
                m.Add(i.PriceEnergy);
                m.Add(i.EnergyPerClick);
                m.Add(p.GetEnergyStatus(i.Key));
                m.Add(i.PriceGems);
                m.Add(this.GetItemCount(p, i.Key));
                m.Add(i.Span);
                m.Add(i.Header);
                m.Add(i.Body);
                m.Add(i.BitmapSheetId);
                m.Add(i.BitmapSheetOffset);
                m.Add(i.OnSale);
                m.Add(i.IsFeatured);
                m.Add(i.IsClassic);
                m.Add(i.IsPlayerWorldOnly);
                m.Add(i.IsNew);
                m.Add(i.DevOnly);
                m.Add(i.IsGridFeatured); /* Should this item be featured in the player shop preview */
                m.Add(i.PriceUsd);

                m.Add(i.Reusable); /* Is the item reuseable (can a player own multiple instances of it) */
                m.Add(i.MaxPurchases);
                m.Add(p.PayVault.Has(i.Key)); /* Does the player own this item */
                m.Add(i.Label);
                m.Add(i.LabelColor);
            }
            return m;
        }

        private int GetItemCount(BasePlayer p, string key)
        {
            var count = p.PayVault.Count(key);

            switch (key)
            {
                case "brickspike":
                case "brickfire":
                case "bricktimeddoor":
                case "brickswitchpurple":
                case "brickdeathdoor":
                case "brickeffectteam":
                case "brickeffectzombie":
                {
                    return count*10;
                }

                case "brickportal":
                case "brickinvisibleportal":
                {
                    return count*5;
                }

                default:
                {
                    return count;
                }
            }
        }

        public ShopItem GetShopItem(string key)
        {
            return this.shopitems.FirstOrDefault(item => item.Key == key);
        }

        public static Message NotEnoughEnergyError(string type)
        {
            return Message.Create(type,
                "error",
                "Not enough energy!",
                "You do not have enough energy to upgrade this item at this time.\n\nBut remember that you always get one free energy every two minutes and thirty seconds, even when you are not online!\n\nSo play a few levels or take a quick nap and you should have enough energy again!");
        }

        public static Message GemsError(string type)
        {
            return Message.Create(type,
                "error",
                "Oops",
                "Something went wrong. Try again later.");
        }
    }
}