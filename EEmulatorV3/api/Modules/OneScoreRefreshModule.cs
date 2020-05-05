using System;
using EEmulatorV3.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulatorV3.Modules
{
    public class OneScoreRefreshModule : NancyModule
    {
        public OneScoreRefreshModule()
        {
            this.Post("/api/360", ctx =>
            {
                var args = Serializer.Deserialize<OneScoreRefreshArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(OneScoreRefreshModule)} (/api/360) has not been implemented yet.");
            });
        }
    }

    [ProtoContract]
    public class OneScoreRefreshArgs
    {
    }

    [ProtoContract]
    public class OneScoreRefreshOutput
    {
        [ProtoMember(1)]
        public OneScoreValue OneScore { get; set; }
    }
}
