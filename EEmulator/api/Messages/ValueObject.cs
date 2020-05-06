using System.Collections.Generic;
using ProtoBuf;

namespace EEmulator.Messages
{
    [ProtoContract]
    public class ValueObject
    {
        [ProtoMember(1)]
        public ValueType ValueType { get; set; }

        [ProtoMember(2)]
        public string String { get; set; }

        [ProtoMember(3)]
        public int Int { get; set; }

        [ProtoMember(4)]
        public uint UInt { get; set; }

        [ProtoMember(5)]
        public long Long { get; set; }

        [ProtoMember(6)]
        public bool Bool { get; set; }

        [ProtoMember(7)]
        public float Float { get; set; }

        [ProtoMember(8)]
        public double Double { get; set; }

        [ProtoMember(9)]
        public byte[] ByteArray { get; set; }

        [ProtoMember(10)]
        public long DateTime { get; set; }

        [ProtoMember(11)]
        public List<ArrayProperty> ArrayProperties { get; set; }

        [ProtoMember(12)]
        public List<ObjectProperty> ObjectProperties { get; set; }
    }
}