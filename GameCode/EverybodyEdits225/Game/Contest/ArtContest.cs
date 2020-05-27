using System.Collections.Generic;
using PlayerIO.GameLibrary;
using System.Linq;

namespace EverybodyEdits.Game.Contest
{
    public class BlockAsset
    {
        public byte[] Bitmap { get; set; }
        public uint Layer { get; set; }
        public bool HasShadow { get; set; }
        public bool IsAbove { get; set; }
    }

    public class ArtContest
    {
        private readonly Client client;
        private DatabaseObject dbo;
        public int BlocksPerPack = 7;

        public string CrewId { get; private set; }

        public List<BlockAsset> BlockAssets {
            get {
                return this.dbo.GetArray("BlockAssets").Select(o => (DatabaseObject)o).Select(asset => new BlockAsset() {
                    Bitmap = asset.GetBytes("Bitmap"),
                    Layer = asset.GetUInt("Layer"),
                    HasShadow = asset.GetBool("HasShadow"),
                    IsAbove = asset.GetBool("IsAbove")
                }).ToList();
            }
        }

        public int MaximumBlockAssets {
            get {
                return this.dbo != null ? this.dbo.GetInt("MaximumBlockAssets", 0) : 0;
            }
        }

        public ArtContest(Client client, string crewId = null)
        {
            this.client = client;
            this.CrewId = crewId;

            if (string.IsNullOrEmpty(crewId)) {
                return;
            }

            this.client.BigDB.Load("ContestAssets", crewId, o => this.dbo = o, error => this.client.ErrorLog.WriteError("The specified key in ContestAssets does not exist.", "CrewId: " + this.CrewId, "", null));
        }

        public void SetBlockAsset(int blockId, byte[] bitmap, bool hasShadow, bool isAbove, Callback successCallback)
        {
            var assetObject = new DatabaseObject();
            assetObject.Set("Bitmap", bitmap);
            assetObject.Set("Layer", (blockId >= 2500 + (BlocksPerPack) && blockId < 2500 + (BlocksPerPack * 2)) ? 1u : 0u);
            assetObject.Set("HasShadow", hasShadow);
            assetObject.Set("IsAbove", isAbove);

            this.dbo.GetArray("BlockAssets").Set(blockId, assetObject);
            this.dbo.Save(successCallback);
        }
    }
}