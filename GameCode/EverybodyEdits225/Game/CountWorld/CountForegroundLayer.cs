using System.Collections.Generic;

namespace EverybodyEdits.Game.CountWorld
{
    public class CountForegroundLayer : BlockLayer<ForegroundBlock>
    {
        internal CountForegroundLayer(int width, int height)
            : base(width, height)
        {
            this.Spawns = new List<Item>();
            this.Portals = new List<Item>();
            this.InvisiblePortals = new List<Item>();
        }

        public override ForegroundBlock this[uint x, uint y]
        {
            get { return base[x, y]; }
            set
            {
                var oldId = this[x, y];
                this.UpdateCount(oldId, x, y, -1);
                this.UpdateCount(value, x, y, 1);

                base[x, y] = value;
            }
        }

        public int CoinCount { get; private set; }
        public int BlueCoinCount { get; private set; }
        public int DiamondCounts { get; private set; }
        public int CakeCounts { get; private set; }
        public int HologramCounts { get; private set; }
        public int WorldPortalCounts { get; private set; }

        public List<Item> Spawns { get; private set; }
        public List<Item> Portals { get; private set; }
        public List<Item> InvisiblePortals { get; private set; }

        private void UpdateCount(ForegroundBlock b, uint x, uint y, int change)
        {
            switch (b.Type)
            {
                case (int) ItemTypes.Coin:
                    this.CoinCount += change;
                    break;

                case (int) ItemTypes.BlueCoin:
                    this.BlueCoinCount += change;
                    break;

                case (int) ItemTypes.SpawnPoint:
                    lock (this.Spawns)
                    {
                        if (change == 1)
                        {
                            this.Spawns.Add(new Item(b, x, y));
                        }
                        else
                        {
                            this.Spawns.Remove(new Item(b, x, y));
                        }
                    }
                    break;

                case (int) ItemTypes.Portal:
                    lock (this.Portals)
                    {
                        if (change == 1)
                        {
                            this.Portals.Add(new Item(b, x, y));
                        }
                        else
                        {
                            this.Portals.Remove(new Item(b, x, y));
                        }
                    }
                    break;

                case (int) ItemTypes.PortalInvisible:
                    lock (this.InvisiblePortals)
                    {
                        if (change == 1)
                        {
                            this.InvisiblePortals.Add(new Item(b, x, y));
                        }
                        else
                        {
                            this.InvisiblePortals.Remove(new Item(b, x, y));
                        }
                    }
                    break;

                case (int) ItemTypes.Diamond:
                    this.DiamondCounts += change;
                    break;

                case (int) ItemTypes.Hologram:
                    this.HologramCounts += change;
                    break;

                case (int) ItemTypes.Cake:
                    this.CakeCounts += change;
                    break;

                case (int)ItemTypes.WorldPortal:
                    this.WorldPortalCounts += change;
                    break;
            }
        }
    }
}
