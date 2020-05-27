namespace EverybodyEdits.Crews
{
    public class CrewItem
    {
        public CrewItem(string key, uint priceCoins)
        {
            this.Key = key;
            this.PriceCoins = priceCoins;
        }

        public string Key { get; private set; }
        public uint PriceCoins { get; private set; }
        public int PriceEnergy { get; set; }
        public int EnergyPerClick { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Label { get; set; }
        public bool Enabled { get; set; }
        public int BitmapOffset { get; set; }

        public bool OnSale { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsNew { get; set; }

        public int MaxPurchases { get; set; }
        public string LabelColor { get; set; }
    }
}