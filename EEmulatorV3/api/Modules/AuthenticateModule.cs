using System;
using System.Collections.Generic;
using EEmulatorV3.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulatorV3.Modules
{
    public class AuthenticateModule : NancyModule
    {
        public AuthenticateModule()
        {
            this.Post("/api/13", ctx =>
            {
                var args = Serializer.Deserialize<AuthenticateArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(AuthenticateModule)} (/api/13) has not been implemented yet.");
            });
        }
    }

    [ProtoContract]
    public class AuthenticateArgs
    {
        [ProtoMember(1)]
        public string GameId { get; set; }

        [ProtoMember(2)]
        public string ConnectionId { get; set; }

        [ProtoMember(3)]
        public List<KeyValuePair> AuthenticationArguments { get; set; }

        [ProtoMember(4)]
        public List<string> PlayerInsightSegments { get; set; }

        [ProtoMember(5)]
        public string ClientAPI { get; set; }

        [ProtoMember(6)]
        public List<KeyValuePair> ClientInfo { get; set; }

        [ProtoMember(7)]
        public List<string> PlayCodes { get; set; }
    }

    [ProtoContract]
    public class AuthenticateOutput
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

        [ProtoMember(6)]
        public List<AuthenticateStartDialog> StartDialogs { get; set; }

        [ProtoMember(7)]
        public bool IsSocialNetworkUser { get; set; }

        [ProtoMember(8)]
        public List<string> NewPlayCodes { get; set; }

        [ProtoMember(9)]
        public string NotificationClickPayload { get; set; }

        [ProtoMember(10)]
        public bool IsInstalledByPublishingNetwork { get; set; }

        [ProtoMember(11)]
        public List<string> Deprecated1 { get; set; }

        [ProtoMember(12)]
        public ApiSecurityRule ApiSecurity { get; set; }

        [ProtoMember(13)]
        public List<string> ApiServerHosts { get; set; }
    }
}
