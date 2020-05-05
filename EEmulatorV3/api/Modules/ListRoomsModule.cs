using System;
using System.Collections.Generic;
using EEmulatorV3.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulatorV3.Modules
{
    public class ListRoomsModule : NancyModule
    {
        public ListRoomsModule()
        {
            this.Post("/api/30", ctx =>
            {
                var args = Serializer.Deserialize<ListRoomsArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(ListRoomsModule)} (/api/30) has not been implemented yet.");
            });
        }
    }

    [ProtoContract]
    public class ListRoomsArgs
    {
        [ProtoMember(1)]
        public string RoomType { get; set; }

        [ProtoMember(2)]
        public List<KeyValuePair> SearchCriteria { get; set; }

        [ProtoMember(3)]
        public int ResultLimit { get; set; }

        [ProtoMember(4)]
        public int ResultOffset { get; set; }

        [ProtoMember(5)]
        public bool OnlyDevRooms { get; set; }
    }

    [ProtoContract]
    public class ListRoomsOutput
    {
        [ProtoMember(1)]
        public List<RoomInfo> Rooms { get; set; }
    }
}
