using System;
using System.Collections.Generic;
using EEmulatorV3.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulatorV3.Modules
{
    public class GameRequestsDeleteModule : NancyModule
    {
        public GameRequestsDeleteModule()
        {
            this.Post("/api/247", ctx =>
            {
                var args = Serializer.Deserialize<GameRequestsDeleteArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(GameRequestsDeleteModule)} (/api/247) has not been implemented yet.");
            });
        }
    }

    [ProtoContract]
    public class GameRequestsDeleteArgs
    {
        [ProtoMember(1)]
        public List<string> RequestIds { get; set; }
    }

    [ProtoContract]
    public class GameRequestsDeleteOutput
    {
        [ProtoMember(1)]
        public List<WaitingGameRequest> Requests { get; set; }

        [ProtoMember(2)]
        public bool MoreRequestsWaiting { get; set; }
    }
}
