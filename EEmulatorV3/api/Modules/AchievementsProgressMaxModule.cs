using System;
using EEmulatorV3.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulatorV3.Modules
{
    public class AchievementsProgressMaxModule : NancyModule
    {
        public AchievementsProgressMaxModule()
        {
            this.Post("/api/283", ctx =>
            {
                var args = Serializer.Deserialize<AchievementsProgressMaxArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(AchievementsProgressMaxModule)} (/api/283) has not been implemented yet.");
            });
        }
    }

    [ProtoContract]
    public class AchievementsProgressMaxArgs
    {
        [ProtoMember(1)]
        public string AchievementId { get; set; }

        [ProtoMember(2)]
        public int Progress { get; set; }
    }

    [ProtoContract]
    public class AchievementsProgressMaxOutput
    {
        [ProtoMember(1)]
        public Achievement Achievement { get; set; }

        [ProtoMember(2)]
        public bool CompletedNow { get; set; }
    }
}
