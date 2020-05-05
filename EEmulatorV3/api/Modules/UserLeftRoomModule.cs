using System.Linq;
using Nancy;
using ProtoBuf;

namespace EEmulatorV3.Modules
{
    public class UserLeftRoomModule : NancyModule
    {
        public UserLeftRoomModule()
        {
            this.Post("/api/40", ctx =>
            {
                var args = Serializer.Deserialize<UserLeftRoomArgs>(this.Request.Body);
                var token = this.Request.Headers["playertoken"].FirstOrDefault();
                var game = GameManager.GetGameFromToken(token);

                return PlayerIO.CreateResponse(token, true, new UserLeftRoomOutput());
            });
        }
    }

    [ProtoContract]
    public class UserLeftRoomArgs
    {
        [ProtoMember(1)]
        public string ExtendedRoomId { get; set; }

        [ProtoMember(2)]
        public int NewPlayerCount { get; set; }

        [ProtoMember(3)]
        public bool Closed { get; set; }
    }

    [ProtoContract]
    public class UserLeftRoomOutput
    {
    }
}
