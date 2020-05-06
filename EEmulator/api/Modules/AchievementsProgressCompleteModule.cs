using System;
using EEmulator.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulator.Modules
{
    public class AchievementsProgressCompleteModule : NancyModule
    {
        public AchievementsProgressCompleteModule()
        {
            this.Post("/api/286", ctx =>
            {
                var args = Serializer.Deserialize<AchievementsProgressCompleteArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(AchievementsProgressCompleteModule)} (/api/286) has not been implemented yet.");
            });
        }
    }

    [ProtoContract]
    public class AchievementsProgressCompleteArgs
    {
        [ProtoMember(1)]
        public string AchievementId { get; set; }
    }

    [ProtoContract]
    public class AchievementsProgressCompleteOutput
    {
        [ProtoMember(1)]
        public Achievement Achievement { get; set; }

        [ProtoMember(2)]
        public bool CompletedNow { get; set; }
    }
}
