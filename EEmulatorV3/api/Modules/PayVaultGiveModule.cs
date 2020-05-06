using System;
using System.Collections.Generic;
using System.Linq;
using EEmulatorV3.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulatorV3.Modules
{
    // TODO: Implement this.
    public class PayVaultGiveModule : NancyModule
    {
        public PayVaultGiveModule()
        {
            this.Post("/api/178", ctx =>
            {
                var args = Serializer.Deserialize<PayVaultGiveArgs>(this.Request.Body);
                var token = this.Request.Headers["playertoken"].FirstOrDefault();

                return PlayerIO.CreateResponse(token, true, new PayVaultGiveOutput
                {
                    VaultContents = new PayVaultContents
                    {
                        Coins = 0,
                        Items = new List<PayVaultItem>(),
                        Version = "1"
                    }
                });
            });
        }
    }

    [ProtoContract]
    public class PayVaultGiveArgs
    {
        [ProtoMember(1)]
        public List<PayVaultBuyItemInfo> Items { get; set; }

        [ProtoMember(2)]
        public string TargetUserId { get; set; }
    }

    [ProtoContract]
    public class PayVaultGiveOutput
    {
        [ProtoMember(1)]
        public PayVaultContents VaultContents { get; set; }
    }
}
