using System;
using System.Collections.Generic;
using EEmulator.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulator.Modules
{
    public class JoinRoomModule : NancyModule
    {
        public JoinRoomModule()
        {
            this.Post("/api/24", ctx =>
            {
                var args = Serializer.Deserialize<JoinRoomArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(JoinRoomModule)} (/api/24) has not been implemented yet.");
            });
        }
    }

    [ProtoContract]
    public class JoinRoomArgs
    {
        [ProtoMember(1)]
        public string RoomId { get; set; }

        [ProtoMember(2)]
        public List<KeyValuePair> JoinData { get; set; }

        [ProtoMember(3)]
        public bool IsDevRoom { get; set; }

        [ProtoMember(4)]
        public bool ServerDomainNameNeeded { get; set; }
    }

    [ProtoContract]
    public class JoinRoomOutput
    {
        [ProtoMember(1)]
        public string JoinKey { get; set; }

        [ProtoMember(2)]
        public List<ServerEndpoint> Endpoints { get; set; }
    }
}
