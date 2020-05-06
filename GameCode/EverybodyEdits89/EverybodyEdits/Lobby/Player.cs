using System;
using PlayerIO.GameLibrary;

namespace EverybodyEdits.Lobby
{
    public class Player : BasePlayer
    {
        public DateTime ShopDate
        {
            get
            {
                return base.PlayerObject.GetDateTime("shopDate", DateTime.Now.AddSeconds((double)(-(double)this.EnergyRechargeSecounds * this.MaxEnergy)));
            }
            set
            {
                base.PlayerObject.Set("shopDate", value);
            }
        }

        public int MaxEnergy
        {
            get
            {
                return Math.Max(base.PlayerObject.GetInt("maxEnergy", 200), 200);
            }
        }

        public int Energy
        {
            get
            {
                return Math.Min(this.MaxEnergy, (int)((DateTime.Now - this.ShopDate).TotalSeconds / (double)this.EnergyRechargeSecounds));
            }
        }

        public int GetSecoundsToNextEnergy
        {
            get
            {
                var time = (DateTime.Now - this.ShopDate).TotalSeconds / (double)this.EnergyRechargeSecounds;
                var reminder = time - Math.Floor(time);
                return (int)((double)this.EnergyRechargeSecounds - reminder * (double)this.EnergyRechargeSecounds);
            }
        }

        public bool HasBeta
        {
            get
            {
                return base.PayVault.Has("pro") || base.PlayerObject.GetBool("haveSmileyPackage", false);
            }
        }

        public void SaveShop()
        {
            base.PlayerObject.Set("maxEnergy", this.MaxEnergy);
            base.PlayerObject.Save();
        }

        public void SaveShop(Callback method)
        {
            base.PlayerObject.Set("maxEnergy", this.MaxEnergy);
            base.PlayerObject.Save(method);
        }

        public int GetEnergyStatus(string type)
        {
            var itm = base.PlayerObject.GetObject("shopItems");
            int result;
            if (itm != null)
            {
                result = itm.GetInt(type, 0);
            }
            else
            {
                result = 0;
            }
            return result;
        }

        public void SetEnergyStatus(string type, int value)
        {
            if (!(base.ConnectUserId == "simpleguest"))
            {
                var itm = base.PlayerObject.GetObject("shopItems");
                if (itm == null)
                {
                    itm = new DatabaseObject();
                    base.PlayerObject.Set("shopItems", itm);
                }
                itm.Set(type, value);
            }
        }

        public bool UseEnergy(int count)
        {
            bool result;
            if (base.ConnectUserId == "simpleguest")
            {
                result = false;
            }
            else if (this.Energy >= count)
            {
                if (this.Energy == this.MaxEnergy)
                {
                    this.ShopDate = DateTime.Now.AddSeconds((double)(-(double)this.MaxEnergy * this.EnergyRechargeSecounds + this.EnergyRechargeSecounds * count));
                }
                else
                {
                    this.ShopDate = this.ShopDate.AddSeconds((double)(count * this.EnergyRechargeSecounds));
                }
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }

        public int EnergyRechargeSecounds = 150;
    }
}
