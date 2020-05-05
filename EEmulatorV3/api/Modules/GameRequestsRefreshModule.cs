using System;
using System.Collections.Generic;
using EEmulatorV3.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulatorV3.Modules
{
    public class GameRequestsRefreshModule : NancyModule
    {
        public GameRequestsRefreshModule()
        {
            this.Post("/api/244", ctx =>
            {
                var args = Serializer.Deserialize<GameRequestsRefreshArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(GameRequestsRefreshModule)} (/api/244) has not been implemented yet.");
            });
        }
    }

    [ProtoContract]
    public class GameRequestsRefreshArgs
    {
        [ProtoMember(1)]
        public List<string> PlayCodes { get; set; }
    }

    [ProtoContract]
    public class GameRequestsRefreshOutput
    {
        [ProtoMember(1)]
        public List<WaitingGameRequest> Requests { get; set; }

        [ProtoMember(2)]
        public bool MoreRequestsWaiting { get; set; }

        [ProtoMember(3)]
        public List<string> NewPlayCodes { get; set; }
    }
}
