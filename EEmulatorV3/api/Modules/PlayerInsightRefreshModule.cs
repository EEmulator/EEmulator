using System;
using EEmulatorV3.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulatorV3.Modules
{
    public class PlayerInsightRefreshModule : NancyModule
    {
        public PlayerInsightRefreshModule()
        {
            this.Post("/api/301", ctx =>
            {
                var args = Serializer.Deserialize<PlayerInsightRefreshArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(PlayerInsightRefreshModule)} (/api/301) has not been implemented yet.");
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
