using System;
using EverybodyEdits.Common;
using EverybodyEdits.Game.Mail;
using PlayerIO.GameLibrary;

namespace EverybodyEdits.Lobby
{
    public class LobbyPlayer : CommonPlayer
    {
        public bool JustJoinedGold
        {
            get
            {
                return this.PlayerObject.Contains("gold_join") &&
                       (DateTime.Now - this.PlayerObject.GetDateTime("gold_join")).TotalSeconds < 120;
            }
        }

        public bool HasFriendFeatures
        {
            get { return !this.IsGuest && ConfirmedEmail; }
        }

        public int LoginStreak
        {
            get { return this.PlayerObject.GetInt("loginStreak", 0); }
            set { this.PlayerObject.Set("loginStreak", value); }
        }

        public bool ConfirmedEmail
        {
            get
            {
                if (this.IsGuest || !this.ConnectUserId.StartsWith("simple") ||
                    this.PlayerObject.GetString("name", "") != "")
                {
                    return true;
                }
                return this.PlayerObject.GetBool("confirmedEmail", false);
            }
            set { this.PlayerObject.Set("confirmedEmail", value); }
        }

        public EENotification[] EENotifications { get; set; }

        public void SaveShop(Callback callback = null)
        {
            if (callback != null)
            {
                this.PlayerObject.Save(callback);
            }
            else
            {
                this.PlayerObject.Save();
            }
        }

        public int GetEnergyStatus(string type)
        {
            var itm = this.PlayerObject.GetObject("shopItems");

            return itm != null ? itm.GetInt(type, 0) : 0;
        }

        public void SetEnergyStatus(string type, int value)
        {
            var itm = this.PlayerObject.GetObject("shopItems");
            if (itm == null)
            {
                itm = new DatabaseObject();
                this.PlayerObject.Set("shopItems", itm);
            }

            if (value == 0)
            {
                itm.Remove(type);
            }
            else
            {
                itm.Set(type, value);
            }
        }
    }
}