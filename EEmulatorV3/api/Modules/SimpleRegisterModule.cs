using System;
using System.Collections.Generic;
using System.IO;
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
                var token = args.GameId + ":" + args.Username;
                var location = Path.Combine("games", "EverybodyEdits", "accounts", args.GameId);

                if (File.Exists(Path.Combine(location, args.Username + ".tson")))
                    throw new Exception($"An account already exists with the username '{args.Username}' in game '{args.GameId}'");

                File.WriteAllText(Path.Combine(location, args.Username + ".tson"),
                    new DatabaseObject()
                    .Set("gameId", args.GameId)
                    .Set("email", args.Email ?? "")
                    .Set("username", args.Username)
                    .Set("password", args.Password)
                    .ToString());

                return PlayerIO.CreateResponse(token, true, new SimpleRegisterOutput()
                {
                    UserId = args.Username,
                    Token = token,
                    ShowBranding = true
                });
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
