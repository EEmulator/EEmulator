using System;
using EEmulatorV3.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulatorV3.Modules
{
    public class AchievementsProgressAddModule : NancyModule
    {
        public AchievementsProgressAddModule()
        {
            this.Post("/api/280", ctx =>
            {
                var args = Serializer.Deserialize<AchievementsProgressAddArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(AchievementsProgressAddModule)} (/api/280) has not been implemented yet.");
            });
        }
    }

    [ProtoContract]
    public class AchievementsProgressAddArgs
    {
        [ProtoMember(1)]
        public string AchievementId { get; set; }

        [ProtoMember(2)]
        public int ProgressDelta { get; set; }
    }

    [ProtoContract]
    public class AchievementsProgressAddOutput
    {
        [ProtoMember(1)]
        public Achievement Achievement { get; set; }

        [ProtoMember(2)]
        public bool CompletedNow { get; set; }
    }
}
