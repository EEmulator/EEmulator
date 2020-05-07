using System;
using System.Collections.Generic;
using PlayerIO.GameLibrary;

namespace EverybodyEdits.Game
{
    public class PotionStatus
    {
        public const String POTION_STATUS_TABLE = "PotionStatus";
        private readonly Client client;
        private readonly String key;

        private bool isReloading;
        private DatabaseObject status;

        public PotionStatus(Client c, DatabaseObject existing)
        {
            this.client = c;
            this.key = existing.Key;
            this.status = existing;
            this.removeExpiredPotions();
        }

        public void addPotion(Potion potion, Callback<List<Potion>> callback)
        {
            this.addPotion(potion, callback, this.status);
        }

        public void addPotion(Potion potion, Callback<List<Potion>> callback, DatabaseObject potiondata)
        {
            if (potion.expired) return;
            var removed = new List<Potion>();
            // Does the player already have a potion active, set the activation date to the latest date.
            if (potiondata.Contains(potion.name))
            {
                var current = potiondata.GetObject(potion.name);
                if (potion.activated > current.GetDateTime("activated")) current.Set("activated", potion.activated);
            }
            else
            {
                // If there is already a potions with the same group, that is older, remove that first.
                var currentpotions = this.getPotions();
                var allow = true;
                for (var i = 0; i < currentpotions.Count; i++)
                {
                    if (potion.group == currentpotions[i].group)
                    {
                        if (potion.activated > currentpotions[i].activated)
                        {
                            this.removePotion(currentpotions[i]);
                            removed.Add(currentpotions[i]);
                        }
                        else
                        {
                            allow = false;
                        }
                    }
                }
                if (allow) potiondata.Set(potion.name, Potion.createDatabaseObject(potion));
            }
            callback(removed);
        }

        public Potion removePotion(Potion potion)
        {
            var removed = Potion.createPotion((DatabaseObject)this.status[potion.name]);
            this.status.Remove(potion.name);
            return removed;
        }

        public List<Potion> getPotions()
        {
            var rtn = new List<Potion>();
            foreach (var name in this.status.Properties)
            {
                var loaded = Potion.createPotion((DatabaseObject)this.status[name]);
                rtn.Add(loaded);
            }
            return rtn;
        }

        //public List<Potion> getActivePotions() {
        //    List<Potion> rtn = new List<Potion>();
        //    foreach (string name in status.Properties) {
        //        Potion loaded = Potion.createPotion((DatabaseObject)status[name]);
        //        if (!loaded.expired) rtn.Add(loaded);
        //    }
        //    return rtn;
        //}

        public bool hasActivePotion(Potion potion)
        {
            return (this.status.Contains(potion.name) && !this.isExpired(this.status.GetObject(potion.name)));
        }

        public List<Potion> removeExpiredPotions()
        {
            var rtn = new List<Potion>();
            foreach (var name in this.status.Properties)
            {
                if (this.isExpired(this.status.GetObject(name)))
                {
                    rtn.Add(Potion.createPotion(this.status.GetObject(name)));
                }
            }

            foreach (var expired in rtn)
            {
                this.status.Remove(expired.name);
            }

            return rtn;
        }

        private bool isExpired(DatabaseObject data)
        {
            return (data.GetDateTime("activated").AddSeconds(data.GetInt("duration")) - DateTime.Now).TotalSeconds < 0;
        }

        private double activationSum(DatabaseObject data = null)
        {
            if (data == null) data = this.status;
            double sum = 0;
            foreach (var name in this.status.Properties)
            {
                sum += this.status.GetObject(name).GetDateTime("activated").Ticks / TimeSpan.TicksPerSecond;
            }
            return sum;
        }

        public void Save(Callback successCallback = null, Callback<PlayerIOError> errorCallback = null)
        {
            this.removeExpiredPotions();
            this.status.Save(true,
                delegate { if (successCallback != null) successCallback.Invoke(); },
                delegate (PlayerIOError error)
                {
                    if (error.ErrorCode == ErrorCode.StaleVersion)
                    {
                        this.Reload(delegate { this.Save(successCallback); });
                    }
                    else
                    {
                        if (errorCallback != null) errorCallback.Invoke(error);
                    }
                });
        }

        public void Reload(Callback callback = null)
        {
            if (!this.isReloading)
            {
                this.isReloading = true;
                this.client.BigDB.LoadOrCreate(POTION_STATUS_TABLE, this.key, delegate (DatabaseObject latest)
                {
                    // Add the potions used in this world to the latest potionstatus object
                    var currentpotions = this.getPotions();
                    for (var i = 0; i < currentpotions.Count; i++)
                    {
                        this.addPotion(currentpotions[i], delegate { }, latest);
                    }

                    this.status = latest;

                    this.isReloading = false;
                    if (callback != null) callback.Invoke();
                });
            }
        }


        public static void getPotioneStatus(Client c, String connectUserId, Callback<PotionStatus> callback)
        {
            c.BigDB.LoadOrCreate(POTION_STATUS_TABLE, connectUserId,
                delegate (DatabaseObject result) { callback.Invoke(new PotionStatus(c, result)); });
        }
    }
}