using ProtoBuf;

namespace EEmulator.Messages
{
    [ProtoContract]
    public class NotificationsEndpointId
    {
        [ProtoMember(1)]
        public string Type { get; set; }

        [ProtoMember(2)]
        public string Identifier { get; set; }
    }
}