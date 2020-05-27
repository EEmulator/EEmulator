using System;
using PlayerIO.GameLibrary;

namespace EverybodyEdits.Game
{
    internal class OnlineStatus
    {
        private const string OnlineStatusTable = "OnlineStatus";
        private readonly Client client;
        private readonly string key;

        private readonly string[] DeprecatedOnlineStatusValues = {
            "worldId", "worldName"
        };

        private bool IsOnline
        {
            get { return (DateTime.Now - this.LastUpdate).TotalSeconds < 60; }
        }

        public bool Stealthy { get; set; }
        public string CurrentWorldId;
        public string CurrentWorldName;
        public string IpAddress;
        public DateTime LastUpdate;
        public string Name;
        public int Smiley;
        public bool HasGoldBorder;
        private DatabaseObject status;

        public OnlineStatus()
        {
            Name = "Unknown User";
            CurrentWorldId = "";
            CurrentWorldName = "";
            IpAddress = "";
            LastUpdate = DateTime.MinValue;
            Smiley = 0;
        }

        private OnlineStatus(Client c, DatabaseObject existing)
        {
            this.client = c;

            this.RemoveDeprecatedValues(existing);

            this.key = existing.Key;
            this.Name = existing.GetString("name", existing.Key == "simpleguest" ? "guest account" : "");
            this.CurrentWorldName = existing.GetString("currentWorldName", "");
            this.CurrentWorldId = existing.GetString("currentWorldId", "");
            this.Smiley = existing.GetInt("smiley", 0);
            this.HasGoldBorder = existing.GetBool("hasGoldBorder", false);
            this.IpAddress = existing.GetString("ipAddress", "");
            this.Stealthy = existing.GetBool("stealth", false);

            if (existing.Contains("lastUpdate"))
            {
                this.LastUpdate = existing.GetDateTime("lastUpdate");
            }

            if (!this.IsOnline)
            {
                this.CurrentWorldName = "";
                this.CurrentWorldId = "";
            }
        }


        public void Save(Callback successCallback = null, Callback<PlayerIOError> errorCallback = null)
        {
            if (this.status != null)
            {
                this.status.Set("name", this.Name);
                this.status.Set("currentWorldName", this.CurrentWorldName);
                this.status.Set("currentWorldId", this.CurrentWorldId);
                this.status.Set("smiley", this.Smiley);
                this.status.Set("hasGoldBorder", this.HasGoldBorder);
                this.status.Set("ipAddress", this.IpAddress);
                this.status.Set("lastUpdate", this.LastUpdate);
                this.status.Set("stealth", this.Stealthy);

                this.status.Save(false, true, () =>
                {
                    if (successCallback != null)
                    {
                        successCallback.Invoke();
                    }
                }, error =>
                {
                    if (errorCallback != null)
                    {
                        errorCallback.Invoke(error);
                    }
                });
            }
            else
            {
                this.client.BigDB.LoadOrCreate(OnlineStatusTable, this.key,
                    delegate (DatabaseObject result)
                    {
                        this.status = result;

                        result.Set("name", this.Name);
                        result.Set("currentWorldName", this.CurrentWorldName);
                        result.Set("currentWorldId", this.CurrentWorldId);
                        result.Set("smiley", this.Smiley);
                        result.Set("hasGoldBorder", this.HasGoldBorder);
                        result.Set("ipAddress", this.IpAddress);
                        result.Set("lastUpdate", this.LastUpdate);
                        this.status.Set("stealth", this.Stealthy);

                        result.Save(false, () =>
                        {
                            if (successCallback != null)
                            {
                                successCallback.Invoke();
                            }
                        }, error =>
                        {
                            if (errorCallback != null)
                            {
                                errorCallback.Invoke(error);
                            }
                        });
                    }, error =>
                    {
                        if (errorCallback != null)
                        {
                            errorCallback.Invoke(error);
                        }
                    });
            }
        }

        public Message ToMessage(string type)
        {
            return this.ToMessage(Message.Create(type));
        }

        public Message ToMessage(Message message)
        {
            message.Add(this.Name);
            message.Add(!this.Stealthy && this.IsOnline);
            message.Add(this.Stealthy ? "" : this.CurrentWorldId);
            message.Add(this.Stealthy ? "" : this.CurrentWorldName);
            message.Add(this.Smiley);
            message.Add(this.LastUpdate == default(DateTime) ? -1 : (double)Player.ConvertToUnixTime(this.LastUpdate));
            message.Add(this.HasGoldBorder);

            return message;
        }

        public override string ToString()
        {
            var rtn = "[OnlineStatusObject: ";
            rtn += " name: " + this.Name;
            rtn += ", world: " + this.CurrentWorldName + " (" + this.CurrentWorldId + ")";
            rtn += ", smiley: " + this.Smiley;
            rtn += ", hasGoldBorder: " + this.HasGoldBorder;
            rtn += ", ipAdress: " + this.IpAddress;
            rtn += ", lastUpdate: " + this.LastUpdate;
            rtn += ", stealth: " + this.Stealthy;
            rtn += "]";
            return rtn;
        }

        public static void GetOnlineStatus(Client c, string connectUserId, Callback<OnlineStatus> callback)
        {
            c.BigDB.LoadOrCreate(OnlineStatusTable, connectUserId, delegate (DatabaseObject result)
            {
                callback.Invoke(new OnlineStatus(c, result));
            });
        }

        public static void GetOnlineStatus(Client c, string[] connectUserId, Callback<OnlineStatus[]> callback)
        {
            c.BigDB.LoadKeysOrCreate(OnlineStatusTable, connectUserId, delegate (DatabaseObject[] result)
            {
                var rtn = new OnlineStatus[result.Length];
                for (var i = 0; i < result.Length; i++)
                {
                    if (result[i].GetString("name", "") != "")
                    {
                        rtn[i] = new OnlineStatus(c, result[i]);
                    }
                    else rtn[i] = new OnlineStatus();
                }
                callback.Invoke(rtn);
            });
        }

        private void RemoveDeprecatedValues(DatabaseObject onlineStatus)
        {
            var deprecated = false;

            foreach (var deprecatedValue in DeprecatedOnlineStatusValues)
            {
                if (onlineStatus.Contains(deprecatedValue))
                {
                    onlineStatus.Remove(deprecatedValue);
                    deprecated = true;
                }
            }

            if (deprecated)
            {
                onlineStatus.Save();
            }
        }
    }
}