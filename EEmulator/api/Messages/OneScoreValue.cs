using ProtoBuf;

namespace EEmulator.Messages
{
    [ProtoContract]
    public class OneScoreValue
    {
        [ProtoMember(1)]
        public string UserId { get; set; }

        [ProtoMember(2)]
        public int Score { get; set; }

        [ProtoMember(3)]
        public float Percentile { get; set; }

        [ProtoMember(4)]
        public int TopRank { get; set; }
    }
}