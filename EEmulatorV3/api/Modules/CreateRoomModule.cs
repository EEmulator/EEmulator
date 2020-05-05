using System;
using System.Collections.Generic;
using EEmulatorV3.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulatorV3.Modules
{
    public class CreateRoomModule : NancyModule
    {
        public CreateRoomModule()
        {
            this.Post("/api/21", ctx =>
            {
                var args = Serializer.Deserialize<CreateRoomArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(CreateRoomModule)} (/api/21) has not been implemented yet.");
            });
        }
    }

    [ProtoContract]
    public class CreateRoomArgs
    {
        [ProtoMember(1)]
        public string RoomId { get; set; }

        [ProtoMember(2)]
        public string RoomType { get; set; }

        [ProtoMember(3)]
        public bool Visible { get; set; }

        [ProtoMember(4)]
        public List<KeyValuePair> RoomData { get; set; }

        [ProtoMember(5)]
        public bool IsDevRoom { get; set; }
    }

    [ProtoContract]
    public class CreateRoomOutput
    {
        [ProtoMember(1)]
        public string RoomId { get; set; }
    }
}
