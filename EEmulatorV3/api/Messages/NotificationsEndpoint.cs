using System.Collections.Generic;
using ProtoBuf;

namespace EEmulatorV3.Messages
{
    [ProtoContract]
    public class NotificationsEndpoint
    {
        [ProtoMember(1)]
        public string Type { get; set; }

        [ProtoMember(2)]
        public string Identifier { get; set; }

        [ProtoMember(3)]
        public List<KeyValuePair> Configuration { get; set; }

        [ProtoMember(4)]
        public bool Enabled { get; set; }
    }
}