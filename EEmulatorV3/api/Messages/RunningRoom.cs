using System.Collections.Generic;
using ProtoBuf;

namespace EEmulatorV3.Messages
{
    [ProtoContract]
    public class RunningRoom
    {
        [ProtoMember(1)]
        public string ExtendedRoomId { get; set; }

        [ProtoMember(2)]
        public int OnlineCount { get; set; }

        [ProtoMember(3)]
        public List<KeyValuePair> RoomData { get; set; }

        [ProtoMember(4)]
        public List<uint> OnlineUserIds { get; set; }

        [ProtoMember(5)]
        public List<string> OnlinePlayerTokens { get; set; }
    }
}