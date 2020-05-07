using System;
using System.Collections.Generic;
using PlayerIO.GameLibrary;

namespace EverybodyEdits.Lobby
{
    public class Shop
    {
        private readonly List<ShopItem> shopitems = new List<ShopItem>();
        private Client client;

        private bool loadedShopConfig;

        public Shop(Client c, Callback callback)
        {
            this.client = c;

            this.client.BigDB.LoadRange("PayVaultItems", "PriceCoins", null, null, null, 1000,
                delegate(DatabaseObject[] ob)
                {
                    for (var a = 0; a < ob.Length; a++)
                    {
                        //if (ob[a].GetBool("Enabled", false) || ob[a].GetBool("IsClassic", false) || ob[a].GetBool("DevOnly", false))
                        //{
                        //Console.WriteLine(ob[a]);
                        this.shopitems.Add(
                            new ShopItem(
                                ob[a].Key,
                                ob[a].GetInt("PriceCoins"),
                                ob[a].GetInt("PriceEnergy", -1),
                                ob[a].GetInt("EnergyPerClick", 5),
                                ob[a].GetBool("Reusable", false),
                                ob[a].GetBool("BetaOnly", false),
                                ob[a].GetBool("IsFeatured", false),
                                ob[a].GetBool("IsClassic", false),
                                ob[a].GetBool("Enabled", false),
                                ob[a].GetBool("OnSale", false),
                                ob[a].GetInt("Span", 1),
                                ob[a].GetString("Header", ""),
                                ob[a].GetString("Body", ""),
                                ob[a].GetString("BitmapSheetId", ""),
                                ob[a].GetInt("BitmapSheetOffset", 0),
                                ob[a].GetBool("IsNew", false),
                                ob[a].GetBool("PWOnly", false),
                                ob[a].GetBool("DevOnly", false),
                                ob[a].GetBool("IsGridFeatured", false),
                                ob[a].GetInt("PriceUSD", -1),
                                ob[a].GetInt("MinClass", 0)
                                )
                            );
                        //}
                    }
                    this.loadedShopConfig = true;
                    callback();
                });
        }

        public bool isConfigLoaded
        {
            get { return this.loadedShopConfig; }
        }

        public Message GetUpdateMessage(Player p)
        {
            return this.GetUpdateMessage(p, Message.Create("getShop"));
        }

        public Message GetUpdateMessage(Player p, Message m)
        {
            Console.WriteLine("GetUpdateMessage: " + p.ConnectUserId);
            m.Add( /*21*/23);
            foreach (var i in this.shopitems)
            {
                //Console.WriteLine("Item: " + i.Header + " - " + i.Enabled + " (" + i.DevOnly + " && " + p.ismod + ")");
                if (i.Enabled || i.IsClassic || (i.DevOnly && p.ismod))
                {
                    if (!i.BetaOnly || p.HasBeta /*&& (!p.PayVault.Has(i.key) || i.Reusable)*/)
                    {
                        m.Add(i.key);
                        m.Add(i.Type);
                        m.Add(i.PriceEnergy);
                        m.Add(i.EnergyPerClick);
                        m.Add(p.GetEnergyStatus(i.key));
                        m.Add(i.PriceGems);
                        m.Add(this.GetItemCount(p, i.key));
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
                        m.Add(i.PriceUSD);
                        m.Add(i.MinClass); /* The minimum class/level required to purchase this item */
                        m.Add(i.Reusable); /* Is the item reuseable (can a player own multiple instances of it) */
                        m.Add(p.PayVault.Has(i.key)); /* Does the player own this item */
                    }
                }
            }
            return m;
        }

        private int GetItemCount(Player p, string key)
        {
            var count = p.PayVault.Count(key);

            switch (key)
            {
                case "brickspike":
                case "brickfire":
                case "bricktimeddoor":
                case "brickcoingate":
                case "brickcoindoor":
                case "brickbluecoindoor":
                case "brickbluecoingate":
                case "brickzombiedoor":
                case "brickswitchpurple":
                case "brickdeathdoor":
                {
                    return count * 10;
                }

                case "brickportal":
                {
                    return count == 0 ? 0 : count + 1;
                }

                default:
                {
                    return count;
                }
            }
        }

        public ShopItem GetShopItem(string key)
        {
            foreach (var item in this.shopitems)
            {
                if (item.key == key)
                {
                    return item;
                }
            }
            return null;
        }
    }

    public class ShopItem
    {
        public bool BetaOnly = false;
        public string BitmapSheetId = "";
        public int BitmapSheetOffset = 0;
        public string Body = "";
        public bool DevOnly = false;
        public bool Enabled = true;
        public int EnergyPerClick = 0;
        public string Header = "";
        public bool IsClassic = false; // Shall item be listed under "classic" tab in the shop (and nowhere else)?

        public bool IsFeatured = false;
        // Shall item be listed under "featured" in the shop (still shown in brick/smiley/world tabs)?

        public bool IsGridFeatured = false;
        public bool IsNew = false;
        public bool IsPlayerWorldOnly = false;
        public int MinClass = 0;
        public bool OnSale = false;
        public int PriceEnergy = 0;
        public int PriceGems = 0;
        public int PriceUSD = 0;
        public bool Reusable = false;
        public int Span = 1;
        public string Type = "";
        public string key = "";

        public ShopItem(string key, int pricegems, int priceenergy, int energyperclick, bool reusable, bool betaonly,
            bool isFeatured, bool isClassic, bool enabled, bool onSale, int span, string header, string body,
            string bitmapsheetid, int offset, bool isnew, bool pwonly, bool devonly, bool isGridFeatured, int priceUSD,
            int minClass)
        {
            this.key = key;
            this.PriceGems = pricegems;
            this.PriceEnergy = priceenergy;
            this.EnergyPerClick = energyperclick;
            this.Reusable = reusable;
            this.BetaOnly = betaonly;
            this.IsFeatured = isFeatured;
            this.IsClassic = isClassic;
            this.Enabled = enabled;
            this.Span = span;
            this.OnSale = onSale;
            this.Header = header;
            this.Body = body;
            this.BitmapSheetId = bitmapsheetid;
            this.BitmapSheetOffset = offset;
            this.IsNew = isnew;
            this.IsPlayerWorldOnly = pwonly;
            this.DevOnly = devonly;
            this.IsGridFeatured = isGridFeatured;
            this.PriceUSD = priceUSD;
            this.MinClass = minClass;

            if (header == null || header.Length == 0) this.Header = key;

            if (key.Contains("smiley")) this.Type = "smiley";
            else if (key.Contains("brick")) this.Type = "brick";
            else if (key.Contains("world")) this.Type = "world";
            else if (key.Contains("potion")) this.Type = "potion";

            if (bitmapsheetid == null || bitmapsheetid.Length == 0)
            {
                if (this.Type == "smiley")
                {
                    this.BitmapSheetId = "smilies";
                }
                else if (this.Type == "potion")
                {
                    this.BitmapSheetId = "potions";
                    this.Span = 2;
                }
                else if (this.Span == 1)
                {
                    this.BitmapSheetId = "shopitem_131";
                }
                else if (this.Span == 2)
                {
                    this.BitmapSheetId = "shopitem_227";
                }
                else if (this.Span == 3)
                {
                    this.BitmapSheetId = "shopitem_323";
                }
            }
        }
    }
}