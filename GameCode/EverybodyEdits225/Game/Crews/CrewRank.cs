using System.Collections.Generic;
using PlayerIO.GameLibrary;

namespace EverybodyEdits.Game.Crews
{
    public class CrewRank : CrewObject
    {
        private const string OwnerPowers = "0,1,2,3,4,5,6,7,8";

        public CrewRank(int id, Crew crew, DatabaseObject dbo)
            : base(crew, dbo)
        {
            this.Id = id;
        }

        public int Id { get; private set; }

        public string Name
        {
            get { return this.DatabaseObject.GetString("Name"); }
            set { this.DatabaseObject.Set("Name", value); }
        }

        public string PowersString
        {
            get
            {
                var powers = this.DatabaseObject.GetString("Powers", "");
                if (this.Id == 0)
                {
                    powers = OwnerPowers;
                }
                return powers;
            }
            set { this.DatabaseObject.Set("Powers", value); }
        }

        public List<CrewPower> Powers
        {
            get
            {
                var powers = new List<CrewPower>();
                if (this.PowersString.Length <= 0)
                {
                    return powers;
                }

                foreach (var power in this.PowersString.Split(','))
                {
                    int p;
                    if (int.TryParse(power, out p))
                    {
                        powers.Add((CrewPower)p);
                    }
                }
                return powers;
            }
        }

        public bool IsOwner
        {
            get { return this.Id == 0; }
        }

        public bool HasPower(CrewPower power)
        {
            return this.Powers.Contains(power);
        }
    }
}