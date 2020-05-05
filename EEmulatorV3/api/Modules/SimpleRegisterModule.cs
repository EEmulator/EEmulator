using System;
using System.Collections.Generic;
using EEmulatorV3.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulatorV3.Modules
{
    public class SimpleRegisterModule : NancyModule
    {
        public SimpleRegisterModule()
        {
            this.Post("/api/403", ctx =>
            {
                var args = Serializer.Deserialize<SimpleRegisterArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(SimpleRegisterModule)} (/api/403) has not been implemented yet.");
            });
        }
    }

    [ProtoContract]
    public class SimpleRegisterArgs
    {
        [ProtoMember(1)]
        public string GameId { get; set; }

        [ProtoMember(2)]
        public string Username { get; set; }

        [ProtoMember(3)]
        public string Password { get; set; }

        [ProtoMember(4)]
        public string Email { get; set; }

        [ProtoMember(5)]
        public string CaptchaKey { get; set; }

        [ProtoMember(6)]
        public string CaptchaValue { get; set; }

        [ProtoMember(7)]
        public List<KeyValuePair> ExtraData { get; set; }

        [ProtoMember(8)]
        public string PartnerId { get; set; }

        [ProtoMember(9)]
        public List<string> PlayerInsightSegments { get; set; }

        [ProtoMember(10)]
        public string ClientAPI { get; set; }

        [ProtoMember(11)]
        public List<KeyValuePair> ClientInfo { get; set; }
    }

    [ProtoContract]
    public class SimpleRegisterOutput
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
