using System;
using System.Collections.Generic;
using System.Linq;
using EEmulatorV3.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulatorV3.Modules
{
    public class SimpleConnectModule : NancyModule
    {
        public SimpleConnectModule()
        {
            this.Post("/api/400", ctx =>
            {
                var args = Serializer.Deserialize<SimpleConnectArgs>(this.Request.Body);
                var token = args.GameId + ":" + args.UsernameOrEmail;

                return PlayerIO.CreateResponse(token, true, new SimpleConnectOutput()
                {
                    UserId = args.UsernameOrEmail,
                    Token = token,
                    ShowBranding = true,
                });
            });
        }
    }

    [ProtoContract]
    public class SimpleConnectArgs
    {
        [ProtoMember(1)]
        public string GameId { get; set; }

        [ProtoMember(2)]
        public string UsernameOrEmail { get; set; }

        [ProtoMember(3)]
        public string Password { get; set; }

        [ProtoMember(4)]
        public List<string> PlayerInsightSegments { get; set; }

        [ProtoMember(5)]
        public string ClientAPI { get; set; }

        [ProtoMember(6)]
        public List<KeyValuePair> ClientInfo { get; set; }
    }

    [ProtoContract]
    public class SimpleConnectOutput
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
