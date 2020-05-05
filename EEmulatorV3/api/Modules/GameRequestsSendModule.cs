using System;
using System.Collections.Generic;
using EEmulatorV3.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulatorV3.Modules
{
    public class GameRequestsSendModule : NancyModule
    {
        public GameRequestsSendModule()
        {
            this.Post("/api/241", ctx =>
            {
                var args = Serializer.Deserialize<GameRequestsSendArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(GameRequestsSendModule)} (/api/241) has not been implemented yet.");
            });
        }
    }

    [ProtoContract]
    public class GameRequestsSendArgs
    {
        [ProtoMember(1)]
        public string RequestType { get; set; }

        [ProtoMember(2)]
        public List<KeyValuePair> RequestData { get; set; }

        [ProtoMember(3)]
        public List<string> RequestRecipients { get; set; }
    }

    [ProtoContract]
    public class GameRequestsSendOutput
    {
    }
}
