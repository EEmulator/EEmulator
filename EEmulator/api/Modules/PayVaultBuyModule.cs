using System;
using System.Collections.Generic;
using EEmulator.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulator.Modules
{
    public class PayVaultBuyModule : NancyModule
    {
        public PayVaultBuyModule()
        {
            this.Post("/api/175", ctx =>
            {
                var args = Serializer.Deserialize<PayVaultBuyArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(PayVaultBuyModule)} (/api/175) has not been implemented yet.");
            });
        }
    }

    [ProtoContract]
    public class PayVaultBuyArgs
    {
        [ProtoMember(1)]
        public List<PayVaultBuyItemInfo> Items { get; set; }

        [ProtoMember(2)]
        public bool StoreItems { get; set; }

        [ProtoMember(3)]
        public string TargetUserId { get; set; }
    }

    [ProtoContract]
    public class PayVaultBuyOutput
    {
        [ProtoMember(1)]
        public PayVaultContents VaultContents { get; set; }
    }
}
