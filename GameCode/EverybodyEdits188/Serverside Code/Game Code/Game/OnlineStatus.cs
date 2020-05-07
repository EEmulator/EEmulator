using System;
using PlayerIO.GameLibrary;

namespace EverybodyEdits.Game
{
    internal class OnlineStatus
    {
        public const String ONLINE_STATUS_TABLE = "OnlineStatus";
        //private static Client client;
        private readonly Client client;

        public String currentWorldId = "";
        public String currentWorldName = "";
        public String ipAddress;
        public String key;
        public DateTime lastUpdate;
        public String name = "";
        public int smiley;
        private DatabaseObject status;

        public OnlineStatus(Client c, DatabaseObject existing)
        {
            this.client = c;

            this.key = existing.Key;
            this.name = existing.GetString("name", "");
            this.currentWorldName = existing.GetString("currentWorldName", "");
            this.currentWorldId = existing.GetString("currentWorldId", "");
            this.smiley = existing.GetInt("smiley", 0);
            this.ipAddress = existing.GetString("ipAddress", "");
            this.lastUpdate = existing.GetDateTime("lastUpdate", DateTime.Now);

            if (!this.isOnline)
            {
                this.currentWorldName = "";
                this.currentWorldId = "";
            }
        }

        public bool isOnline
        {
            get { return (DateTime.Now - this.lastUpdate).TotalSeconds < 60; }
        }

        public void Save(Callback successCallback = null, Callback<PlayerIOError> errorCallback = null)
        {
            if (this.status != null)
            {
                this.status.Set("name", this.name);
                this.status.Set("currentWorldName", this.currentWorldName);
                this.status.Set("currentWorldId", this.currentWorldId);
                this.status.Set("smiley", this.smiley);
                this.status.Set("ipAddress", this.ipAddress);
                this.status.Set("lastUpdate", this.lastUpdate);

                this.status.Save(false, true, delegate { if (successCallback != null) successCallback.Invoke(); },
                    delegate (PlayerIOError error) { if (errorCallback != null) errorCallback.Invoke(error); });
            }
            else
            {
                this.client.BigDB.LoadOrCreate(ONLINE_STATUS_TABLE, this.key,
                    delegate (DatabaseObject result)
                    {
                        this.status = result;

                        result.Set("name", this.name);
                        result.Set("currentWorldName", this.currentWorldName);
                        result.Set("currentWorldId", this.currentWorldId);
                        result.Set("smiley", this.smiley);
                        result.Set("ipAddress", this.ipAddress);
                        result.Set("lastUpdate", this.lastUpdate);

                        result.Save(false, delegate { if (successCallback != null) successCallback.Invoke(); },
                            delegate (PlayerIOError error) { if (errorCallback != null) errorCallback.Invoke(error); });
                    }, delegate (PlayerIOError error) { if (errorCallback != null) errorCallback.Invoke(error); });
            }
        }

        public Message ToMessage(string type)
        {
            return this.ToMessage(Message.Create(type));
        }

        public Message ToMessage(Message message)
        {
            message.Add(this.name);
            message.Add(this.isOnline);
            message.Add(this.currentWorldId);
            message.Add(this.currentWorldName);
            message.Add(this.smiley);
            return message;
        }

        public override string ToString()
        {
            var rtn = "[OnlineStatusObject: ";
            rtn += " name: " + this.name;
            rtn += ", world: " + this.currentWorldName + " (" + this.currentWorldId + ")";
            rtn += ", smiley: " + this.smiley;
            rtn += ", ipAdress: " + this.ipAddress;
            rtn += ", lastUpdate: " + this.lastUpdate;
            rtn += "]";
            return rtn;
        }

        public static void getOnlineStatus(Client c, String connectUserId, Callback<OnlineStatus> callback)
        {
            c.BigDB.LoadOrCreate(ONLINE_STATUS_TABLE, connectUserId,
                delegate (DatabaseObject result) { callback.Invoke(new OnlineStatus(c, result)); });
        }

        public static void getOnlineStatus(Client c, String[] connectUserId, Callback<OnlineStatus[]> callback)
        {
            c.BigDB.LoadKeysOrCreate(ONLINE_STATUS_TABLE, connectUserId, delegate (DatabaseObject[] result)
            {
                var rtn = new OnlineStatus[result.Length];
                for (var i = 0; i < result.Length; i++)
                {
                    rtn[i] = new OnlineStatus(c, result[i]);
                }
                callback.Invoke(rtn);
            });
        }
    }
}