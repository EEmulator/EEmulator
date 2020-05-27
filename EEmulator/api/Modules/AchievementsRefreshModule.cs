using System;
using System.Collections.Generic;
using System.Linq;
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
                var token = this.Request.Headers["playertoken"].FirstOrDefault();

                return PlayerIO.CreateResponse(token, true, new AchievementsRefreshOutput()
                {
                    Version = "1",
                    Achievements = new List<Achievement>()
                });
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
