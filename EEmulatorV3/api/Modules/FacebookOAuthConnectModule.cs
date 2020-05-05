using System;
using System.Collections.Generic;
using EEmulatorV3.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulatorV3.Modules
{
    public class FacebookOAuthConnectModule : NancyModule
    {
        public FacebookOAuthConnectModule()
        {
            this.Post("/api/418", ctx =>
            {
                var args = Serializer.Deserialize<FacebookOAuthConnectArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(FacebookOAuthConnectModule)} (/api/418) has not been implemented yet.");
            });
        }
    }

    [ProtoContract]
    public class FacebookOAuthConnectArgs
    {
        [ProtoMember(1)]
        public string GameId { get; set; }

        [ProtoMember(2)]
        public string AccessToken { get; set; }

        [ProtoMember(3)]
        public string PartnerId { get; set; }

        [ProtoMember(4)]
        public List<string> PlayerInsightSegments { get; set; }

        [ProtoMember(5)]
        public string ClientAPI { get; set; }

        [ProtoMember(6)]
        public List<KeyValuePair> ClientInfo { get; set; }
    }

    [ProtoContract]
    public class FacebookOAuthConnectOutput
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
        public string FacebookUserId { get; set; }

        [ProtoMember(6)]
        public string PartnerId { get; set; }

        [ProtoMember(7)]
        public PlayerInsightState PlayerInsightState { get; set; }
    }
}
