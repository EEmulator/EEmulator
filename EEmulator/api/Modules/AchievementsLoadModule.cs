using System;
using System.Collections.Generic;
using EEmulator.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulator.Modules
{
    public class AchievementsLoadModule : NancyModule
    {
        public AchievementsLoadModule()
        {
            this.Post("/api/274", ctx =>
            {
                var args = Serializer.Deserialize<AchievementsLoadArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(AchievementsLoadModule)} (/api/274) has not been implemented yet.");
            });
        }
    }

    [ProtoContract]
    public class AchievementsLoadArgs
    {
        [ProtoMember(1)]
        public List<string> UserIds { get; set; }
    }

    [ProtoContract]
    public class AchievementsLoadOutput
    {
        [ProtoMember(1)]
        public List<UserAchievements> UserAchievements { get; set; }
    }
}
