using System;
using System.Collections.Generic;
using EEmulator.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulator.Modules
{
    public class SocialRefreshModule : NancyModule
    {
        public SocialRefreshModule()
        {
            this.Post("/api/601", ctx =>
            {
                var args = Serializer.Deserialize<SocialRefreshArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(SocialRefreshModule)} (/api/601) has not been implemented yet.");
            });
        }
    }

    [ProtoContract]
    public class SocialRefreshArgs
    {
    }

    [ProtoContract]
    public class SocialRefreshOutput
    {
        [ProtoMember(1)]
        public SocialProfile MyProfile { get; set; }

        [ProtoMember(2)]
        public List<SocialProfile> Friends { get; set; }

        [ProtoMember(3)]
        public List<string> Blocked { get; set; }
    }
}
