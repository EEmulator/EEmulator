using System;
using System.Collections.Generic;
using EEmulatorV3.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulatorV3.Modules
{
    public class UpdateRoomModule : NancyModule
    {
        public UpdateRoomModule()
        {
            this.Post("/api/53", ctx =>
            {
                var args = Serializer.Deserialize<UpdateRoomArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(UpdateRoomModule)} (/api/53) has not been implemented yet.");
            });
        }
    }

    [ProtoContract]
    public class UpdateRoomArgs
    {
        [ProtoMember(1)]
        public string ExtendedRoomId { get; set; }

        [ProtoMember(2)]
        public int Visible { get; set; }

        [ProtoMember(3)]
        public List<KeyValuePair> RoomData { get; set; }
    }

    [ProtoContract]
    public class UpdateRoomOutput
    {
    }
}
