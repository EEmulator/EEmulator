using ProtoBuf;

namespace EEmulatorV3.Messages
{
    [ProtoContract]
    public class PlayerInsightEvent
    {
        [ProtoMember(1)]
        public string EventType { get; set; }

        [ProtoMember(2)]
        public int Value { get; set; }
    }
}