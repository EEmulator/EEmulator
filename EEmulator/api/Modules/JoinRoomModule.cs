using System;
using System.Collections.Generic;
using System.Linq;
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
                var token = this.Request.Headers["playertoken"].FirstOrDefault();
                var game = GameManager.GetGameFromToken(token);

                if (string.IsNullOrEmpty(args.RoomId))
                    args.RoomId = "$service-room$";

                var room = game.Rooms.FirstOrDefault(r => r.Id == args.RoomId);

                string joinKey = null;

                switch (game.GameId)
                {
                    default:
                        joinKey = JoinInfo.Create(
                            encryptionKey: GameManager.EncryptionKey,
                            serverId: "serverId",
                            gameId: 128,
                            gameConnectId: game.GameId,
                            gameCodeId: "gameCodeId",
                            serverType: room.RoomType,
                            roomId: args.RoomId,
                            roomData: new byte[] { },
                            extendedRoomId: game.GameId + "/" + room.RoomType + "/" + args.RoomId,
                            connectUserId: token.Split(':')[1],
                            playerIoToken: token,
                            visible: true,
                            roomFlags: 0,
                            partnerId: "",
                            userId: 1234,
                            gameCodeVersion: 1);
                        break;
                }

                return PlayerIO.CreateResponse(token, true, new JoinRoomOutput()
                {
                    Endpoints = new List<ServerEndpoint>() { new ServerEndpoint() { Address = "localhost", Port = 8184 } },
                    JoinKey = joinKey
                });
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
