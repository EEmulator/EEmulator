using System;
using System.Collections.Generic;
using EEmulatorV3.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulatorV3.Modules
{
    public class PayVaultReadHistoryModule : NancyModule
    {
        public PayVaultReadHistoryModule()
        {
            this.Post("/api/160", ctx =>
            {
                var args = Serializer.Deserialize<PayVaultReadHistoryArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(PayVaultReadHistoryModule)} (/api/160) has not been implemented yet.");
            });
        }
    }

    [ProtoContract]
    public class PayVaultReadHistoryArgs
    {
        [ProtoMember(1)]
        public uint Page { get; set; }

        [ProtoMember(2)]
        public uint PageSize { get; set; }

        [ProtoMember(3)]
        public string TargetUserId { get; set; }
    }

    [ProtoContract]
    public class PayVaultReadHistoryOutput
    {
        [ProtoMember(1)]
        public List<PayVaultHistoryEntry> Entries { get; set; }
    }
}
