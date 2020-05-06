using System;
using System.Collections.Generic;
using EEmulator.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulator.Modules
{
    public class OneScoreLoadModule : NancyModule
    {
        public OneScoreLoadModule()
        {
            this.Post("/api/351", ctx =>
            {
                var args = Serializer.Deserialize<OneScoreLoadArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(OneScoreLoadModule)} (/api/351) has not been implemented yet.");
            });
        }
    }

    [ProtoContract]
    public class OneScoreLoadArgs
    {
        [ProtoMember(1)]
        public List<string> UserIds { get; set; }
    }

    [ProtoContract]
    public class OneScoreLoadOutput
    {
        [ProtoMember(1)]
        public List<OneScoreValue> OneScores { get; set; }
    }
}
