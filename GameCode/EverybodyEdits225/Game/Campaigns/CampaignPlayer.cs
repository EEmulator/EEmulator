using System;
using PlayerIO.GameLibrary;

namespace EverybodyEdits.Game.Campaigns
{
    public class CampaignPlayer
    {
        private const string TableName = "CampaignPlayers";

        private readonly DatabaseObject dbo;

        public CampaignPlayer(DatabaseObject core, string campaignId)
        {
            this.DatabaseObject = core;

            this.dbo = core.GetObject(campaignId);
            if (this.dbo == null)
            {
                this.dbo = new DatabaseObject();
                this.dbo.Set("CurrentTier", 0);
                this.dbo.Set("LastUpdated", DateTime.UtcNow);
                core.Set(campaignId, this.dbo);
                core.Save();
            }
        }

        private CampaignPlayer(DatabaseObject core, DatabaseObject dbo)
        {
            this.dbo = dbo;
            this.DatabaseObject = core;
        }

        public DatabaseObject DatabaseObject { get; private set; }

        public int CurrentTier
        {
            get { return this.dbo.GetInt("CurrentTier", 0); }
            set { this.dbo.Set("CurrentTier", value); }
        }

        public DateTime LastUpdated
        {
            get { return this.dbo.GetDateTime("LastUpdated", DateTime.UtcNow); }
            set { this.dbo.Set("LastUpdated", value); }
        }

        public void Save(Callback successCallback)
        {
            this.DatabaseObject.Save(successCallback);
        }

        public static void Load(Client client, string connectUserId, string campaignId,
            Callback<CampaignPlayer> callback)
        {
            client.BigDB.LoadOrCreate(TableName, connectUserId, delegate(DatabaseObject o)
            {
                var campaignStatus = o.GetObject(campaignId);
                if (campaignStatus == null)
                {
                    campaignStatus = new DatabaseObject();
                    campaignStatus.Set("CurrentTier", 0);
                    campaignStatus.Set("LastUpdated", DateTime.UtcNow);
                    o.Set(campaignId, campaignStatus);
                    o.Save();
                }

                callback(new CampaignPlayer(o, campaignStatus));
            });
        }

        public static void Load(Client client, string connectUserId, Callback<DatabaseObject> callback)
        {
            client.BigDB.LoadOrCreate(TableName, connectUserId, callback);
        }

        public CampaignStatus GetStatus(int tier)
        {
            if (this.CurrentTier > tier)
            {
                return CampaignStatus.Completed;
            }
            return this.CurrentTier == tier ? CampaignStatus.Unlocked : CampaignStatus.Locked;
        }
    }
}