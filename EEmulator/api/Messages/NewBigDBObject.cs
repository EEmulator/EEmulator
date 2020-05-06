using System.Collections.Generic;
using ProtoBuf;

namespace EEmulator.Messages
{
    [ProtoContract]
    public class NewBigDBObject
    {
        [ProtoMember(1)]
        public string Table { get; set; }

        [ProtoMember(2)]
        public string Key { get; set; }

        [ProtoMember(3)]
        public List<ObjectProperty> Properties { get; set; }
    }
}