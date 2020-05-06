using System;
using System.Collections.Generic;
using EEmulator.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulator.Modules
{
    public class PlayerInsightTrackEventsModule : NancyModule
    {
        public PlayerInsightTrackEventsModule()
        {
            this.Post("/api/311", ctx =>
            {
                var args = Serializer.Deserialize<PlayerInsightTrackEventsArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(PlayerInsightTrackEventsModule)} (/api/311) has not been implemented yet.");
            });
        }
    }

    [ProtoContract]
    public class PlayerInsightTrackEventsArgs
    {
        [ProtoMember(1)]
        public List<PlayerInsightEvent> Events { get; set; }
    }

    [ProtoContract]
    public class PlayerInsightTrackEventsOutput
    {
    }
}
