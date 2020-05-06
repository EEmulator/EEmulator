using System;
using System.Collections.Generic;
using EEmulator.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulator.Modules
{
    public class AchievementsRefreshModule : NancyModule
    {
        public AchievementsRefreshModule()
        {
            this.Post("/api/271", ctx =>
            {
                var args = Serializer.Deserialize<AchievementsRefreshArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(AchievementsRefreshModule)} (/api/271) has not been implemented yet.");
            });
        }
    }

    [ProtoContract]
    public class AchievementsRefreshArgs
    {
        [ProtoMember(1)]
        public string LastVersion { get; set; }
    }

    [ProtoContract]
    public class AchievementsRefreshOutput
    {
        [ProtoMember(1)]
        public string Version { get; set; }

        [ProtoMember(2)]
        public List<Achievement> Achievements { get; set; }
    }
}
