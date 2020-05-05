using System;
using System.Collections.Generic;
using EEmulatorV3.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulatorV3.Modules
{
    public class SocialLoadProfilesModule : NancyModule
    {
        public SocialLoadProfilesModule()
        {
            this.Post("/api/604", ctx =>
            {
                var args = Serializer.Deserialize<SocialLoadProfilesArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(SocialLoadProfilesModule)} (/api/604) has not been implemented yet.");
            });
        }
    }

    [ProtoContract]
    public class SocialLoadProfilesArgs
    {
        [ProtoMember(1)]
        public List<string> UserIds { get; set; }
    }

    [ProtoContract]
    public class SocialLoadProfilesOutput
    {
        [ProtoMember(1)]
        public List<SocialProfile> Profiles { get; set; }
    }
}
