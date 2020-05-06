namespace EverybodyEdits.Lobby
{
    public class ShopItem
    {
        public ShopItem(string key, int pricegems, int priceenergy, int energyperclick, bool reusable, bool betaonly, bool isFeatured, bool isClassic, bool enabled)
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
        }

        public string key = "";

        public int PriceGems = 0;

        public int PriceEnergy = 0;

        public int EnergyPerClick = 0;

        public bool Reusable = false;

        public bool BetaOnly = false;

        public bool Enabled = true;

        public bool IsFeatured = false;

        public bool IsClassic = false;
    }
}
