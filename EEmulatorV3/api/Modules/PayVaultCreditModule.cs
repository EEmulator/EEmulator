using System;
using EEmulatorV3.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulatorV3.Modules
{
    public class PayVaultCreditModule : NancyModule
    {
        public PayVaultCreditModule()
        {
            this.Post("/api/169", ctx =>
            {
                var args = Serializer.Deserialize<PayVaultCreditArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(PayVaultCreditModule)} (/api/169) has not been implemented yet.");
            });
        }
    }

    [ProtoContract]
    public class PayVaultCreditArgs
    {
        [ProtoMember(1)]
        public uint Amount { get; set; }

        [ProtoMember(2)]
        public string Reason { get; set; }

        [ProtoMember(3)]
        public string TargetUserId { get; set; }
    }

    [ProtoContract]
    public class PayVaultCreditOutput
    {
        [ProtoMember(1)]
        public PayVaultContents VaultContents { get; set; }
    }
}
