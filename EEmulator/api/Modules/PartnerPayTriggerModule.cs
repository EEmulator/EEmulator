using System;
using Nancy;
using ProtoBuf;

namespace EEmulator.Modules
{
    public class PartnerPayTriggerModule : NancyModule
    {
        public PartnerPayTriggerModule()
        {
            this.Post("/api/200", ctx =>
            {
                var args = Serializer.Deserialize<PartnerPayTriggerArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(PartnerPayTriggerModule)} (/api/200) has not been implemented yet.");
            });
        }
    }

    [ProtoContract]
    public class PartnerPayTriggerArgs
    {
        [ProtoMember(1)]
        public string Key { get; set; }

        [ProtoMember(2)]
        public uint Count { get; set; }
    }

    [ProtoContract]
    public class PartnerPayTriggerOutput
    {
    }
}
