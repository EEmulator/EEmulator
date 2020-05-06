using ProtoBuf;

namespace EEmulator.Messages
{
    [ProtoContract]
    public class Achievement
    {
        [ProtoMember(1)]
        public string Identifier { get; set; }

        [ProtoMember(2)]
        public string Title { get; set; }

        [ProtoMember(3)]
        public string Description { get; set; }

        [ProtoMember(4)]
        public string ImageUrl { get; set; }

        [ProtoMember(5)]
        public uint ProgressGoal { get; set; }

        [ProtoMember(6)]
        public uint Progress { get; set; }

        [ProtoMember(7)]
        public long LastUpdated { get; set; }
    }
}