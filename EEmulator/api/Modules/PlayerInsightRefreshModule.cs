using System;
using System.Linq;
using EEmulator.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulator.Modules
{
    public class PlayerInsightRefreshModule : NancyModule
    {
        public PlayerInsightRefreshModule()
        {
            this.Post("/api/301", ctx =>
            {
                var args = Serializer.Deserialize<PlayerInsightRefreshArgs>(this.Request.Body);
                var token = this.Request.Headers["playertoken"].FirstOrDefault();

                return PlayerIO.CreateResponse(token, true, new PlayerInsightRefreshOutput() { 
                    State = new PlayerInsightState() { PlayersOnline = 1, Segments = new System.Collections.Generic.List<KeyValuePair>() }
                });
            });
        }
    }

    [ProtoContract]
    public class PlayerInsightRefreshArgs
    {
    }

    [ProtoContract]
    public class PlayerInsightRefreshOutput
    {
        [ProtoMember(1)]
        public PlayerInsightState State { get; set; }
    }
}
