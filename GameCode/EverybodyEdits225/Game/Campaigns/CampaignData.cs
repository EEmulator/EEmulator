using System.Collections.Generic;
using PlayerIO.GameLibrary;

namespace EverybodyEdits.Game.Campaigns
{
    public class CampaignData
    {
        public CampaignData(DatabaseObject dbo)
        {
            this.Id = dbo.Key;
            this.Title = dbo.GetString("Title", "");
            this.Description = dbo.GetString("Description", "");
            this.Difficulty = dbo.GetInt("Difficulty", 0);
            this.Badge = dbo.GetString("Badge", "");
            this.Visible = dbo.GetBool("Visible", false);
            this.BetaOnly = dbo.GetBool("BetaOnly", false);
            this.Levels = new List<CampaignWorld>();

            var levels = dbo.GetArray("Levels");
            if (levels != null)
            {
                for (var i = 0; i < levels.Count; i++)
                {
                    this.Levels.Add(new CampaignWorld(i, levels.GetObject(i)));
                }
            }

            this.Tiers = this.Levels.Count;
        }

        public string Id { get; private set; }

        public string Title { get; private set; }

        public string Description { get; private set; }

        public int Difficulty { get; private set; }

        public int Tiers { get; private set; }

        public string Badge { get; private set; }

        public bool Visible { get; private set; }

        public bool BetaOnly { get; private set; }

        public List<CampaignWorld> Levels { get; private set; }
    }
}