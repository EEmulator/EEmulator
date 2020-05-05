using System.Collections.Generic;
using ProtoBuf;

namespace EEmulatorV3.Messages
{
    [ProtoContract]
    public class WaitingGameRequest
    {
        [ProtoMember(1)]
        public string Id { get; set; }

        [ProtoMember(2)]
        public string Type { get; set; }

        [ProtoMember(3)]
        public string SenderUserId { get; set; }

        [ProtoMember(4)]
        public long Created { get; set; }

        [ProtoMember(5)]
        public List<KeyValuePair> Data { get; set; }
    }
}