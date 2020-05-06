using System;
using Nancy;
using ProtoBuf;

namespace EEmulator.Modules
{
    public class PlayerInsightSessionStopModule : NancyModule
    {
        public PlayerInsightSessionStopModule()
        {
            this.Post("/api/320", ctx =>
            {
                var args = Serializer.Deserialize<PlayerInsightSessionStopArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(PlayerInsightSessionStopModule)} (/api/320) has not been implemented yet.");
            });
        }
    }

    [ProtoContract]
    public class PlayerInsightSessionStopArgs
    {
    }

    [ProtoContract]
    public class PlayerInsightSessionStopOutput
    {
    }
}
