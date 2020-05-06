using System;
using System.Collections.Generic;
using EEmulator.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulator.Modules
{
    public class PayVaultUsePaymentInfoModule : NancyModule
    {
        public PayVaultUsePaymentInfoModule()
        {
            this.Post("/api/184", ctx =>
            {
                var args = Serializer.Deserialize<PayVaultUsePaymentInfoArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(PayVaultUsePaymentInfoModule)} (/api/184) has not been implemented yet.");
            });
        }
    }

    [ProtoContract]
    public class PayVaultUsePaymentInfoArgs
    {
        [ProtoMember(1)]
        public string Provider { get; set; }

        [ProtoMember(2)]
        public List<KeyValuePair> ProviderArguments { get; set; }
    }

    [ProtoContract]
    public class PayVaultUsePaymentInfoOutput
    {
        [ProtoMember(1)]
        public List<KeyValuePair> ProviderResults { get; set; }

        [ProtoMember(2)]
        public PayVaultContents VaultContents { get; set; }
    }
}
