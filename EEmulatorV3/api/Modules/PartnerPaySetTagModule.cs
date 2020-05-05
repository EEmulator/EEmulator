using System;
using Nancy;
using ProtoBuf;

namespace EEmulatorV3.Modules
{
    public class PartnerPaySetTagModule : NancyModule
    {
        public PartnerPaySetTagModule()
        {
            this.Post("/api/203", ctx =>
            {
                var args = Serializer.Deserialize<PartnerPaySetTagArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(PartnerPaySetTagModule)} (/api/203) has not been implemented yet.");
            });
        }
    }

    [ProtoContract]
    public class PartnerPaySetTagArgs
    {
        [ProtoMember(1)]
        public string PartnerId { get; set; }
    }

    [ProtoContract]
    public class PartnerPaySetTagOutput
    {
    }
}
