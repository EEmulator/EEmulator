using System;
using EEmulatorV3.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulatorV3.Modules
{
    public class OneScoreSetModule : NancyModule
    {
        public OneScoreSetModule()
        {
            this.Post("/api/354", ctx =>
            {
                var args = Serializer.Deserialize<OneScoreSetArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(OneScoreSetModule)} (/api/354) has not been implemented yet.");
            });
        }
    }

    [ProtoContract]
    public class OneScoreSetArgs
    {
        [ProtoMember(1)]
        public int Score { get; set; }
    }

    [ProtoContract]
    public class OneScoreSetOutput
    {
        [ProtoMember(1)]
        public OneScoreValue OneScore { get; set; }
    }
}
