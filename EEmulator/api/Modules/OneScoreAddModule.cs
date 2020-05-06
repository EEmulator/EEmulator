using System;
using EEmulator.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulator.Modules
{
    public class OneScoreAddModule : NancyModule
    {
        public OneScoreAddModule()
        {
            this.Post("/api/357", ctx =>
            {
                var args = Serializer.Deserialize<OneScoreAddArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(OneScoreAddModule)} (/api/357) has not been implemented yet.");
            });
        }
    }

    [ProtoContract]
    public class OneScoreAddArgs
    {
        [ProtoMember(1)]
        public int Score { get; set; }
    }

    [ProtoContract]
    public class OneScoreAddOutput
    {
        [ProtoMember(1)]
        public OneScoreValue OneScore { get; set; }
    }
}
