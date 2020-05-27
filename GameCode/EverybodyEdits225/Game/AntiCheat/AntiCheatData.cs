using PlayerIO.GameLibrary;

namespace EverybodyEdits.Game.AntiCheat
{
    public class AntiCheatData
    {
        private const string Table = "AntiCheatData";
        private readonly DatabaseObject dbo;

        private AntiCheatData()
        {
        }

        private AntiCheatData(DatabaseObject dbo)
        {
            this.dbo = dbo;
            this.MinCoins = dbo.GetInt("MinCoins", 0);
            this.MinBlueCoins = dbo.GetInt("MinBlueCoins", 0);
            this.MinMoves = dbo.GetInt("MinMoves", 0);
            this.MinTime = dbo.GetInt("MinTime", 0);
        }

        public int MinMoves { get; private set; }
        public int MinBlueCoins { get; private set; }
        public int MinCoins { get; private set; }
        public int MinTime { get; private set; }

        public DatabaseArray StatsMinMoves
        {
            get
            {
                if (this.dbo == null)
                {
                    return new DatabaseArray();
                }
                if (!this.dbo.Contains("StatsMinMoves"))
                {
                    this.dbo.Set("StatsMinMoves", new DatabaseArray());
                }
                return this.dbo.GetArray("StatsMinMoves");
            }
        }

        public static void GetAntiCheatData(Client c, string connectUserId, Callback<AntiCheatData> callback)
        {
            c.BigDB.Load(Table, connectUserId,
                delegate (DatabaseObject result)
                {
                    callback.Invoke(result != null ? new AntiCheatData(result) : new AntiCheatData());
                });
        }

        public void Save()
        {
            if (this.dbo != null)
            {
                this.dbo.Save(true);
            }
        }
    }
}