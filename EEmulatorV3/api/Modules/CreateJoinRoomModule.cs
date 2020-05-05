using System.Collections.Generic;
using System.Linq;
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
                var token = this.Request.Headers["playertoken"].FirstOrDefault();
                var game = GameManager.GetGameFromToken(token);

                if (string.IsNullOrEmpty(args.RoomId))
                    args.RoomId = "$service-room$";

                return PlayerIO.CreateResponse(token, true, new CreateJoinRoomOutput()
                {
                    RoomId = args.RoomId,
                    Endpoints = new List<ServerEndpoint>() { new ServerEndpoint() { Address = "localhost", Port = 8184 } },
                    JoinKey = JoinInfo.Create(
                        encryptionKey: GameManager.EncryptionKey,
                        serverId: "serverId",
                        gameId: 1,
                        gameConnectId: game.GameId,
                        gameCodeId: "gameCodeId",
                        serverType: args.RoomType,
                        roomId: args.RoomId,
                        roomData: new byte[] { },
                        extendedRoomId: "",
                        connectUserId: "",
                        playerIoToken: token,
                        visible: true,
                        roomFlags: 0,
                        partnerId: "",
                        userId: 1,
                        gameCodeVersion: 1)
                });
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
