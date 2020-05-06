using System.Collections.Generic;
using EEmulator.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulator.api.Modules
{
    public class ConnectModule : NancyModule
    {
        public ConnectModule()
        {
            this.Post("/api/10", ctx =>
            {
                var args = Serializer.Deserialize<ConnectArgs>(this.Request.Body);

                return PlayerIO.CreateResponse("token", true, new ConnectOutput()
                {
                    Token = args.GameId + ":" + args.UserId,
                    UserId = args.UserId,
                    ShowBranding = true,
                    GameFSRedirectMap = "",
                    PartnerId = "",
                    PlayerInsightState = new PlayerInsightState() { PlayersOnline = 1, Segments = new List<KeyValuePair>() }
                });
            });
        }
    }

    [ProtoContract]
    public class ConnectArgs
    {
        [ProtoMember(1)]
        public string GameId { get; set; }

        [ProtoMember(2)]
        public string ConnectionId { get; set; }

        [ProtoMember(3)]
        public string UserId { get; set; }

        [ProtoMember(4)]
        public string Auth { get; set; }

        [ProtoMember(5)]
        public string PartnerId { get; set; }

        [ProtoMember(6)]
        public List<string> PlayerInsightSegments { get; set; }

        [ProtoMember(7)]
        public string ClientAPI { get; set; }

        [ProtoMember(8)]
        public List<KeyValuePair> ClientInfo { get; set; }
    }

    [ProtoContract]
    public class ConnectOutput
    {
        [ProtoMember(1)]
        public string Token { get; set; }

        [ProtoMember(2)]
        public string UserId { get; set; }

        [ProtoMember(3)]
        public bool ShowBranding { get; set; }

        [ProtoMember(4)]
        public string GameFSRedirectMap { get; set; }

        [ProtoMember(5)]
        public string PartnerId { get; set; }

        [ProtoMember(6)]
        public PlayerInsightState PlayerInsightState { get; set; }
    }
}
