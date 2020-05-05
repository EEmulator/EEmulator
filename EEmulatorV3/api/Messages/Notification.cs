using System.Collections.Generic;
using ProtoBuf;

namespace EEmulatorV3.Messages
{
    [ProtoContract]
    public class Notification
    {
        [ProtoMember(1)]
        public string Recipient { get; set; }

        [ProtoMember(2)]
        public string EndpointType { get; set; }

        [ProtoMember(3)]
        public List<KeyValuePair> OldKeyValueData { get; set; }

        [ProtoMember(4)]
        public List<ObjectProperty> Data { get; set; }
    }
}