using System;
using EverybodyEdits.Common;
using PlayerIO.GameLibrary;

namespace EverybodyEdits.Lobby
{
    public class ShopItem
    {
        public readonly bool BetaOnly;
        public readonly string BitmapSheetId;
        public readonly int BitmapSheetOffset;
        public readonly string Body;
        public readonly bool DevOnly;
        public readonly bool Enabled;
        public readonly int EnergyPerClick;
        public readonly string Header;
        public readonly bool IsClassic; // Shall item be listed under "classic" tab in the shop (and nowhere else)?
        public readonly string Label;
        public readonly string LabelColor;

        // Shall item be listed under "featured" in the shop (still shown in brick/smiley/world tabs)?
        public readonly bool IsFeatured;

        public readonly bool IsGridFeatured;
        public readonly bool IsNew;
        public readonly bool IsPlayerWorldOnly;
        public readonly string Key;
        public readonly int MaxPurchases;
        public readonly bool OnSale;
        public readonly int PriceEnergy;
        public readonly int PriceGems;
        public readonly int PriceUsd;
        public readonly bool Reusable;
        public readonly int Span;
        public readonly string Type = "";

        public ShopItem(DatabaseObject dbo, Sales sales)
        {
            this.Key = dbo.Key;
            this.PriceGems = dbo.GetInt("PriceCoins");
            this.PriceEnergy = dbo.GetInt("PriceEnergy", -1);
            this.EnergyPerClick = dbo.GetInt("EnergyPerClick", 5);
            this.Reusable = dbo.GetBool("Reusable", false);
            this.MaxPurchases = dbo.GetInt("MaxPurchases", 0);
            this.BetaOnly = dbo.GetBool("BetaOnly", false);

            var availableSince = dbo.GetInt("AvailableSince", 0);
            this.IsNew = dbo.GetBool("IsNew", false) || availableSince == Config.Version;
            this.IsFeatured = dbo.GetBool("IsFeatured", false) || this.IsNew;
            this.Enabled = dbo.GetBool("Enabled", false) && availableSince <= Config.Version;

            this.IsClassic = dbo.GetBool("IsClassic", false);
            this.OnSale = dbo.GetBool("OnSale", false);
            this.Span = dbo.GetInt("Span", 1);
            this.Header = dbo.GetString("Header", "");
            this.Body = dbo.GetString("Body", "");
            this.BitmapSheetId = dbo.GetString("BitmapSheetId", "");
            this.BitmapSheetOffset = dbo.GetInt("BitmapSheetOffset", 0);
            this.IsPlayerWorldOnly = dbo.GetBool("PWOnly", false);
            this.DevOnly = dbo.GetBool("DevOnly", false);
            this.IsGridFeatured = dbo.GetBool("IsGridFeatured", false);
            this.PriceUsd = dbo.GetInt("PriceUSD", -1);
            this.Label = dbo.GetString("Label", "");
            this.LabelColor = dbo.GetString("LabelColor", "#FFAA00");

            if (string.IsNullOrEmpty(this.Header))
            {
                this.Header = this.Key;
            }

            if (this.Key.Contains("smiley"))
            {
                this.Type = "smiley";
            }
            else if (this.Key.Contains("brick"))
            {
                this.Type = "brick";
            }
            else if (this.Key.Contains("world"))
            {
                this.Type = "world";
            }
            else if (this.Key.Contains("aura"))
            {
                this.Type = this.Key.Contains("shape") ? "auraShape" : "auraColor";
            }
            else if (this.Key.Contains("gold"))
            {
                this.Type = "gold";
            }
            else if (this.Key.Contains("crew"))
            {
                this.Type = "crew";
            }
            else if (this.Key == "mixednewyear2010")
            {
                this.Type = "brick";
            }

            if (this.Key == "changeusername" || this.Key == "pro" || this.Key.Contains("gemcode"))
            {
                this.Type = "service";
            }

            // Black Friday
            if (sales.IsBlackFridayItem(this.Key))
            {
                if (this.PriceGems > 0)
                    this.PriceGems = (int)Math.Ceiling((double)this.PriceGems / 2);
                if (this.PriceUsd > 0)
                    this.PriceUsd = (int)Math.Ceiling((double)this.PriceUsd / 2);
                if (this.PriceEnergy > 0)
                    this.PriceEnergy = (int)Math.Ceiling((double)this.PriceEnergy / 2);
                this.EnergyPerClick = sales.FixEnergyPerClick(this.EnergyPerClick, this.PriceEnergy);
                this.Label = "blackfriday";
            }

            if (!string.IsNullOrEmpty(this.BitmapSheetId))
            {
                return;
            }

            switch (this.Type)
            {
                case "smiley":
                    this.BitmapSheetId = "smilies";
                    break;
                case "auraColor":
                    this.BitmapSheetId = "auraColors";
                    break;
                case "auraShape":
                    this.BitmapSheetId = "auraShapes";
                    break;
                default:
                    switch (this.Span)
                    {
                        case 1:
                            this.BitmapSheetId = "shopitem_131";
                            break;
                        case 2:
                            this.BitmapSheetId = "shopitem_227";
                            break;
                        case 3:
                            this.BitmapSheetId = "shopitem_323";
                            break;
                    }
                    break;
            }
        }

        public bool CanBuy(int count)
        {
            return this.Reusable
                ? this.MaxPurchases == 0 || count <= this.MaxPurchases
                : count == 0;
        }
    }
}
