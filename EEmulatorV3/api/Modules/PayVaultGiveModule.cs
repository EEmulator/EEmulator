using System;
using System.Collections.Generic;
using EEmulatorV3.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulatorV3.Modules
{
    public class PayVaultGiveModule : NancyModule
    {
        public PayVaultGiveModule()
        {
            this.Post("/api/178", ctx =>
            {
                var args = Serializer.Deserialize<PayVaultGiveArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(PayVaultGiveModule)} (/api/178) has not been implemented yet.");
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
