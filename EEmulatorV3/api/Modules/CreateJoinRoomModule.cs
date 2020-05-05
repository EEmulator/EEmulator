using System;
using System.Collections.Generic;
using EEmulatorV3.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulatorV3.Modules
{
    public class CreateJoinRoomModule : NancyModule
    {
        public CreateJoinRoomModule()
        {
            this.Post("/api/27", ctx =>
            {
                var args = Serializer.Deserialize<CreateJoinRoomArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(CreateJoinRoomModule)} (/api/27) has not been implemented yet.");
            });
        }
    }

    [ProtoContract]
    public class CreateJoinRoomArgs
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
        public List<KeyValuePair> JoinData { get; set; }

        [ProtoMember(6)]
        public bool IsDevRoom { get; set; }

        [ProtoMember(7)]
        public bool ServerDomainNameNeeded { get; set; }
    }

    [ProtoContract]
    public class CreateJoinRoomOutput
    {
        [ProtoMember(1)]
        public string RoomId { get; set; }

        [ProtoMember(2)]
        public string JoinKey { get; set; }

        [ProtoMember(3)]
        public List<ServerEndpoint> Endpoints { get; set; }
    }
}
