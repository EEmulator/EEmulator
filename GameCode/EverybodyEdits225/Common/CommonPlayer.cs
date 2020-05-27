using System;
using PlayerIO.GameLibrary;

namespace EverybodyEdits.Common
{
    public class CommonPlayer : BasePlayer
    {
        public int SecondsToNextEnergy
        {
            get
            {
                var time = (DateTime.Now - this.ShopDate).TotalSeconds/EnergyDelay;
                var reminder = time - Math.Floor(time);

                var timeLeftInSecs = (int) (EnergyDelay - reminder*EnergyDelay);
                return timeLeftInSecs;
            }
        }

        public virtual string Name
        {
            get { return this.PlayerObject.GetString("name", ""); }
        }

        public virtual int Smiley
        {
            get { return this.PlayerObject.GetInt("smiley", 0); }
            set { this.PlayerObject.Set("smiley", value); }
        }

        public virtual bool SmileyGoldBorder
        {
            get { return (this.PlayerObject.GetBool("smileyGoldBorder", false) && HasGoldMembership); }
            set { this.PlayerObject.Set("smileyGoldBorder", value); }
        }

        public int Aura
        {
            get { return this.PlayerObject.GetInt("aura", 0); }
            set { this.PlayerObject.Set("aura", value); }
        }

        public int AuraColor
        {
            get { return this.PlayerObject.GetInt("auraColor", 0); }
            set { this.PlayerObject.Set("auraColor", value); }
        }

        public string Badge
        {
            get { return this.PlayerObject.GetString("badge", ""); }
            set { this.PlayerObject.Set("badge", value); }
        }

        public bool IsAdmin
        {
            get { return this.PlayerObject.GetBool("isAdministrator", false); }
        }

        public bool IsModerator
        {
            get { return this.PlayerObject.GetBool("isModerator", false); }
        }

        public bool IsStaff {
            get { return this.PlayerObject.GetBool("isStaff", false); }
        }

        public bool IsJudge {
            get { return this.PlayerObject.GetBool("isJudge", false); }
        }

        public bool IsGuest
        {
            get { return this.ConnectUserId == "simpleguest"; }
        }

        public bool CanChat
        {
            get { return !this.PlayerObject.GetBool("chatbanned", false) && !this.IsGuest; }
        }

        public bool HasGoldMembership
        {
            get
            {
                return this.PlayerObject.Contains("gold_expire") &&
                       this.PlayerObject.GetDateTime("gold_expire") > DateTime.Now;
            }
        }

        public bool HasBeta
        {
            get { return this.PayVault.Has("pro") || this.PlayerObject.GetBool("haveSmileyPackage", false); }
        }

        public int TimeZone
        {
            get { return this.PlayerObject.GetInt("timezone", 0); }
            set { this.PlayerObject.Set("timezone", value); }
        }

        public bool IsBanned
        {
            get { return this.PlayerObject.GetBool("banned", false); }
        }

        public bool IsTempBanned
        {
            get { return this.PlayerObject.GetBool("tempbanned", false); }
        }

        public int EnergyDelay
        {
            get { return PlayerObject.GetInt("energyDelay", 150); }
        }

        private DateTime ShopDate
        {
            get
            {
                return this.PlayerObject.GetDateTime("shopDate",
                    DateTime.Now.AddSeconds(-EnergyDelay*this.MaxEnergy));
            }
            set { this.PlayerObject.Set("shopDate", value); }
        }

        public int MaxEnergy
        {
            get
            {
                if (!this.PlayerObject.Contains("maxEnergy"))
                {
                    this.PlayerObject.Set("maxEnergy", 200);
                    this.PlayerObject.Save();
                }

                return this.PlayerObject.GetInt("maxEnergy");
            }
            set { this.PlayerObject.Set("maxEnergy", value); }
        }

        public int Energy
        {
            get
            {
                return Math.Min(this.MaxEnergy,
                    (int) ((DateTime.Now - this.ShopDate).TotalSeconds/this.EnergyDelay));
            }
        }

        public void AddEnergy(int amount)
        {
            this.ShopDate = this.ShopDate.AddSeconds(-amount*this.EnergyDelay);
        }

        public bool UseEnergy(int amount)
        {
            if (this.Energy < amount)
            {
                return false;
            }

            if (this.Energy == this.MaxEnergy)
            {
                this.ShopDate =
                    DateTime.Now.AddSeconds(-this.MaxEnergy*this.EnergyDelay +
                                            this.EnergyDelay*amount);
            }
            else
            {
                this.ShopDate = this.ShopDate.AddSeconds(amount*this.EnergyDelay);
            }
            return true;
        }

        public void RefillEnergy()
        {
            this.PlayerObject.Remove("shopDate");
        }

        public static void GetId(BigDB bigDb, string username, Callback<string> callback)
        {
            bigDb.Load("Usernames", username, name =>
            {
                if (name == null || name.GetString("owner", "none") == "none")
                {
                    callback(null);
                }
                else
                {
                    callback(name.GetString("owner"));
                }
            });
        }
    }
}