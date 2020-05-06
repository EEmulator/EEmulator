using System.Collections.Generic;
using System.Linq;
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
                var token = this.Request.Headers["playertoken"].FirstOrDefault();

                return PlayerIO.CreateResponse(token, true, new UpdateRoomOutput() { });
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
