using System;
using EEmulatorV3.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulatorV3.Modules
{
    public class PayVaultRefreshModule : NancyModule
    {
        public PayVaultRefreshModule()
        {
            this.Post("/api/163", ctx =>
            {
                var args = Serializer.Deserialize<PayVaultRefreshArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(PayVaultRefreshModule)} (/api/163) has not been implemented yet.");
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
