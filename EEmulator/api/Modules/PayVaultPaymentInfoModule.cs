using System;
using System.Collections.Generic;
using EEmulator.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulator.Modules
{
    public class PayVaultPaymentInfoModule : NancyModule
    {
        public PayVaultPaymentInfoModule()
        {
            this.Post("/api/181", ctx =>
            {
                var args = Serializer.Deserialize<PayVaultPaymentInfoArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(PayVaultPaymentInfoModule)} (/api/181) has not been implemented yet.");
            });
        }
    }

    [ProtoContract]
    public class PayVaultPaymentInfoArgs
    {
        [ProtoMember(1)]
        public string Provider { get; set; }

        [ProtoMember(2)]
        public List<KeyValuePair> PurchaseArguments { get; set; }

        [ProtoMember(3)]
        public List<PayVaultBuyItemInfo> Items { get; set; }
    }

    [ProtoContract]
    public class PayVaultPaymentInfoOutput
    {
        [ProtoMember(1)]
        public List<KeyValuePair> ProviderArguments { get; set; }
    }
}
