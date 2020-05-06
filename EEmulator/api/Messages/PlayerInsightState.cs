using System.Collections.Generic;
using ProtoBuf;

namespace EEmulator.Messages
{
    [ProtoContract]
    public class PlayerInsightState
    {
        [ProtoMember(1)]
        public int PlayersOnline { get; set; }

        [ProtoMember(2)]
        public List<KeyValuePair> Segments { get; set; }
    }
}