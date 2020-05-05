using System.Collections.Generic;
using ProtoBuf;

namespace EEmulatorV3.Messages
{
    [ProtoContract]
    public class BigDBChangeset
    {
        [ProtoMember(1)]
        public string Table { get; set; }

        [ProtoMember(2)]
        public string Key { get; set; }

        [ProtoMember(3)]
        public string OnlyIfVersion { get; set; }

        [ProtoMember(4)]
        public List<ObjectProperty> Changes { get; set; }

        [ProtoMember(5)]
        public bool FullOverwrite { get; set; }
    }
}