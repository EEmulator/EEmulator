using System;
using System.Collections.Generic;
using EEmulator.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulator.Modules
{
    public class PayVaultConsumeModule : NancyModule
    {
        public PayVaultConsumeModule()
        {
            this.Post("/api/166", ctx =>
            {
                var args = Serializer.Deserialize<PayVaultConsumeArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(PayVaultConsumeModule)} (/api/166) has not been implemented yet.");
            });
        }
    }

    [ProtoContract]
    public class PayVaultConsumeArgs
    {
        [ProtoMember(1)]
        public List<string> Ids { get; set; }

        [ProtoMember(2)]
        public string TargetUserId { get; set; }
    }

    [ProtoContract]
    public class PayVaultConsumeOutput
    {
        [ProtoMember(1)]
        public PayVaultContents VaultContents { get; set; }
    }
}
