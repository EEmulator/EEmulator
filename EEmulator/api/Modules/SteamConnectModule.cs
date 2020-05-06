using System;
using System.Collections.Generic;
using EEmulator.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulator.Modules
{
    public class SteamConnectModule : NancyModule
    {
        public SteamConnectModule()
        {
            this.Post("/api/421", ctx =>
            {
                var args = Serializer.Deserialize<SteamConnectArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(SteamConnectModule)} (/api/421) has not been implemented yet.");
            });
        }
    }

    [ProtoContract]
    public class SteamConnectArgs
    {
        [ProtoMember(1)]
        public string GameId { get; set; }

        [ProtoMember(2)]
        public string SteamAppId { get; set; }

        [ProtoMember(3)]
        public string SteamSessionTicket { get; set; }

        [ProtoMember(4)]
        public List<string> PlayerInsightSegments { get; set; }

        [ProtoMember(5)]
        public string ClientAPI { get; set; }

        [ProtoMember(6)]
        public List<KeyValuePair> ClientInfo { get; set; }
    }

    [ProtoContract]
    public class SteamConnectOutput
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
