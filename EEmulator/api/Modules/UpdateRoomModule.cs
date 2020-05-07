using System.Collections.Generic;
using System.Linq;
using EEmulator.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulator.Modules
{
    public class UpdateRoomModule : NancyModule
    {
        public UpdateRoomModule()
        {
            this.Post("/api/53", ctx =>
            {
                var args = Serializer.Deserialize<UpdateRoomArgs>(this.Request.Body);
                var token = this.Request.Headers["playertoken"].FirstOrDefault();
                var game = GameManager.GetGameFromToken(token);

                var roomType = args.ExtendedRoomId.Split('/')[1];
                var roomId = args.ExtendedRoomId.Split('/')[2];

                game.Rooms.Add(new RoomInfo() { Id = roomId, OnlineUsers = 1, RoomData = new List<KeyValuePair>(), RoomType = roomType });
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
