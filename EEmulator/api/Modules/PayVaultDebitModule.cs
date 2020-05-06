using System;
using EEmulator.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulator.Modules
{
    public class PayVaultDebitModule : NancyModule
    {
        public PayVaultDebitModule()
        {
            this.Post("/api/172", ctx =>
            {
                var args = Serializer.Deserialize<PayVaultDebitArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(PayVaultDebitModule)} (/api/172) has not been implemented yet.");
            });
        }
    }

    [ProtoContract]
    public class PayVaultDebitArgs
    {
        [ProtoMember(1)]
        public uint Amount { get; set; }

        [ProtoMember(2)]
        public string Reason { get; set; }

        [ProtoMember(3)]
        public string TargetUserId { get; set; }
    }

    [ProtoContract]
    public class PayVaultDebitOutput
    {
        [ProtoMember(1)]
        public PayVaultContents VaultContents { get; set; }
    }
}
