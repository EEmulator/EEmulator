using ProtoBuf;

namespace EEmulatorV3.Messages
{
    [ProtoContract]
    public class ObjectProperty
    {
        [ProtoMember(1)]
        public string Name { get; set; }

        [ProtoMember(2)]
        public ValueObject Value { get; set; }
    }
}