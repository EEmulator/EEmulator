using System.Collections.Generic;
using ProtoBuf;

namespace EEmulator.Messages
{
    [ProtoContract]
    public class BigDBObjectId
    {
        [ProtoMember(1)]
        public string Table { get; set; }

        [ProtoMember(2)]
        public List<string> Keys { get; set; }
    }
}