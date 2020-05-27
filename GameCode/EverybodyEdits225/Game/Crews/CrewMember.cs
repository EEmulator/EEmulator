using PlayerIO.GameLibrary;

namespace EverybodyEdits.Game.Crews
{
    public class CrewMember : CrewObject
    {
        public CrewMember(string userId, string name, int smiley, bool smileyGoldBorder, Crew crew, DatabaseObject dbo)
            : base(crew, dbo)
        {
            this.Id = userId;
            this.Name = name;
            this.Smiley = smiley;
            this.SmileyGoldBorder = smileyGoldBorder;
        }

        public string Id { get; private set; }

        public string Name { get; private set; }

        public int Smiley { get; private set; }

        public bool SmileyGoldBorder { get; private set; }

        public string About
        {
            get { return this.DatabaseObject.GetString("About", ""); }
            set
            {
                if (value.Length > 100)
                {
                    value = value.Substring(100);
                }
                this.DatabaseObject.Set("About", value);
            }
        }

        public CrewRank Rank
        {
            get { return this.Crew.GetRank(this.RankId); }
            set { this.RankId = value.Id; }
        }

        public int RankId
        {
            get { return this.DatabaseObject.GetInt("Rank", -1); }
            private set { this.DatabaseObject.Set("Rank", value); }
        }
    }
}