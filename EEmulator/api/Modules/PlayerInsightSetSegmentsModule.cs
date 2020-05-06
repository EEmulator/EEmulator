using System;
using System.Collections.Generic;
using EEmulator.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulator.Modules
{
    public class PlayerInsightSetSegmentsModule : NancyModule
    {
        public PlayerInsightSetSegmentsModule()
        {
            this.Post("/api/304", ctx =>
            {
                var args = Serializer.Deserialize<PlayerInsightSetSegmentsArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(PlayerInsightSetSegmentsModule)} (/api/304) has not been implemented yet.");
            });
        }
    }

    [ProtoContract]
    public class PlayerInsightSetSegmentsArgs
    {
        [ProtoMember(1)]
        public List<string> Segments { get; set; }
    }

    [ProtoContract]
    public class PlayerInsightSetSegmentsOutput
    {
        [ProtoMember(1)]
        public PlayerInsightState State { get; set; }
    }
}
