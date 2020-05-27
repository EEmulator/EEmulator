using System.Collections.Generic;
using PlayerIO.GameLibrary;

namespace EverybodyEdits.Game.Campaigns
{
    public class CampaignWorld
    {
        public CampaignWorld(int tier, DatabaseObject dbo)
        {
            this.Id = dbo.GetString("Id");
            this.Name = dbo.GetString("Name", "");
            this.Creators = dbo.GetString("Creators", "");
            this.Difficulty = dbo.GetInt("Difficulty", 0);
            this.Image = dbo.GetString("Image", "");
            this.Tier = tier;
            Rewards = new Dictionary<string, uint>();

            var rewards = dbo.GetObject("Rewards");
            if (rewards == null)
            {
                return;
            }

            foreach (var reward in rewards.Properties)
            {
                this.Rewards.Add(reward, rewards.GetUInt(reward));
            }
        }

        public string Id { get; private set; }

        public string Name { get; private set; }

        public string Creators { get; private set; }

        public int Difficulty { get; private set; }

        public int Tier { get; private set; }

        public string Image { get; private set; }

        public Dictionary<string, uint> Rewards { get; private set; }
    }
}