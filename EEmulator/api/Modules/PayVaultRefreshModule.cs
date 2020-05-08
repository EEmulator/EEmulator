using System;
using System.Collections.Generic;
using System.Linq;
using EEmulator.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulator.Modules
{
    public class PayVaultRefreshModule : NancyModule
    {
        public PayVaultRefreshModule()
        {
            this.Post("/api/163", ctx =>
            {
                //var args = Serializer.Deserialize<PayVaultRefreshArgs>(this.Request.Body);
                var token = this.Request.Headers["playertoken"].FirstOrDefault();
                var game = GameManager.GetGameFromToken(token);
                var items = game.BigDB.LoadRange("PayVaultItems", "PriceCoins", null, null, 1000);

                return PlayerIO.CreateResponse(token, true, new PayVaultRefreshOutput()
                {
                    VaultContents = new PayVaultContents()
                    {
                        Coins = 1,
                        Version = "22040806-3e9f-438e-97eb-51069207926d",
                        Items = items.Select(x => new PayVaultItem() { Id = "pvi" + x.Key, ItemKey = x.Key, Properties = DatabaseObjectExtensions.FromDatabaseObject(x) }).ToList()
                    }
                });
            });
        }
    }

    [ProtoContract]
    public class PayVaultRefreshArgs
    {
        [ProtoMember(1)]
        public string LastVersion { get; set; }

        [ProtoMember(2)]
        public string TargetUserId { get; set; }
    }

    [ProtoContract]
    public class PayVaultRefreshOutput
    {
        [ProtoMember(1)]
        public PayVaultContents VaultContents { get; set; }
    }
}
