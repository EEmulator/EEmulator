using System.Collections.Generic;
using ProtoBuf;

namespace EEmulator.Messages
{
    [ProtoContract]
    public class RoomInfo
    {
        [ProtoMember(1)]
        public string Id { get; set; }

        [ProtoMember(2)]
        public string RoomType { get; set; }

        [ProtoMember(3)]
        public int OnlineUsers { get; set; }

        [ProtoMember(4)]
        public List<KeyValuePair> RoomData { get; set; }
    }
}