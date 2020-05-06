using System;
using EEmulator.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulator.Modules
{
    public class AchievementsProgressSetModule : NancyModule
    {
        public AchievementsProgressSetModule()
        {
            this.Post("/api/277", ctx =>
            {
                var args = Serializer.Deserialize<AchievementsProgressSetArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(AchievementsProgressSetModule)} (/api/277) has not been implemented yet.");
            });
        }
    }

    [ProtoContract]
    public class AchievementsProgressSetArgs
    {
        [ProtoMember(1)]
        public string AchievementId { get; set; }

        [ProtoMember(2)]
        public int Progress { get; set; }
    }

    [ProtoContract]
    public class AchievementsProgressSetOutput
    {
        [ProtoMember(1)]
        public Achievement Achievement { get; set; }

        [ProtoMember(2)]
        public bool CompletedNow { get; set; }
    }
}
