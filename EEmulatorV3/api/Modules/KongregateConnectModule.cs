using System;
using System.Collections.Generic;
using EEmulatorV3.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulatorV3.Modules
{
    public class KongregateConnectModule : NancyModule
    {
        public KongregateConnectModule()
        {
            this.Post("/api/412", ctx =>
            {
                var args = Serializer.Deserialize<KongregateConnectArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(KongregateConnectModule)} (/api/412) has not been implemented yet.");
            });
        }
    }

    [ProtoContract]
    public class KongregateConnectArgs
    {
        [ProtoMember(1)]
        public string GameId { get; set; }

        [ProtoMember(2)]
        public string UserId { get; set; }

        [ProtoMember(3)]
        public string GameAuthToken { get; set; }

        [ProtoMember(4)]
        public List<string> PlayerInsightSegments { get; set; }

        [ProtoMember(5)]
        public string ClientAPI { get; set; }

        [ProtoMember(6)]
        public List<KeyValuePair> ClientInfo { get; set; }
    }

    [ProtoContract]
    public class KongregateConnectOutput
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
        public PlayerInsightState PlayerInsightState { get; set; }
    }
}
