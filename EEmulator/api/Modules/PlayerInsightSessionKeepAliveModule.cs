using System;
using Nancy;
using ProtoBuf;

namespace EEmulator.Modules
{
    public class PlayerInsightSessionKeepAliveModule : NancyModule
    {
        public PlayerInsightSessionKeepAliveModule()
        {
            this.Post("/api/317", ctx =>
            {
                var args = Serializer.Deserialize<PlayerInsightSessionKeepAliveArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(PlayerInsightSessionKeepAliveModule)} (/api/317) has not been implemented yet.");
            });
        }
    }

    [ProtoContract]
    public class PlayerInsightSessionKeepAliveArgs
    {
    }

    [ProtoContract]
    public class PlayerInsightSessionKeepAliveOutput
    {
    }
}
