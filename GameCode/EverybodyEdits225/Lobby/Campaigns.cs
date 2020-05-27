using System.Collections.Generic;
using EverybodyEdits.Game.Campaigns;
using PlayerIO.GameLibrary;

namespace EverybodyEdits.Lobby
{
    public class Campaigns
    {
        private readonly List<CampaignData> campaigns = new List<CampaignData>();

        public Campaigns(Client client, Callback callback)
        {
            client.BigDB.LoadRange("Campaigns", "Visible", null, null, null, 1000, dbo =>
            {
                foreach (var databaseObject in dbo)
                {
                    this.campaigns.Add(new CampaignData(databaseObject));
                }
                this.Initialized = true;
                callback();
            });
        }

        public bool Initialized { get; private set; }

        public Message GetMessage(LobbyPlayer player, DatabaseObject progressTable)
        {
            var m = Message.Create("getCampaigns");
            if (player.IsGuest)
            {
                return m;
            }

            foreach (var campaign in campaigns)
            {
                if ((!campaign.Visible && !player.IsAdmin && !player.IsModerator) ||
                    (campaign.BetaOnly && !player.HasBeta))
                {
                    continue;
                }

                var arch = player.Achievements.Get(campaign.Badge);
                if (arch == null)
                {
                    continue;
                }

                m.Add(campaign.Id);
                m.Add(campaign.Title);
                m.Add(campaign.Description);
                m.Add(campaign.Difficulty);

                var campaignPlayer = new CampaignPlayer(progressTable, campaign.Id);
                m.Add(campaignPlayer.CurrentTier == campaign.Tiers);

                var levels = campaign.Levels;
                m.Add(levels.Count);

                foreach (var level in levels)
                {
                    m.Add(level.Id);
                    m.Add(level.Name);
                    m.Add(level.Creators);
                    m.Add(level.Difficulty);
                    m.Add(level.Tier + 1);

                    m.Add((int)campaignPlayer.GetStatus(level.Tier));

                    var rewards = level.Rewards;
                    m.Add(rewards.Count);

                    foreach (var reward in rewards)
                    {
                        m.Add(reward.Key);
                        m.Add(reward.Value);
                    }
                }
            }

            return m;
        }
    }
}