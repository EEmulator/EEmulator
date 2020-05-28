using System;

namespace EverybodyEdits.Common
{
    public class Sales
    {
        // TODO: Update to 2018 dates
        private readonly DateTime blackFridaySaleEnd = new DateTime(2017, 11, 28, 14, 0, 0); // nvd wants to do until the 28th.
        private readonly DateTime blackFridaySaleStart = new DateTime(2017, 11, 24, 9, 0, 0);

        public bool IsBlackFriday
        {
            get { return DateTime.UtcNow >= this.blackFridaySaleStart && DateTime.UtcNow <= this.blackFridaySaleEnd; }
        }

        public int FixEnergyPerClick(int energyPerClick, int energy)
        {
            if (energyPerClick > 0)
            {
                energyPerClick = 1;
                for (var i = Math.Min(energy, 125); i > 0; --i)
                {
                    if (energy % i == 0)
                    {
                        energyPerClick = i;
                        break;
                    }
                }
            }

            return energyPerClick;
        }

        public bool IsBlackFridayItem(string payvault)
        {
            return this.IsBlackFriday && !payvault.Contains("gemcode") && payvault != "smileybigspender" &&
                   payvault != "brickdiamond";
        }
    }
}
