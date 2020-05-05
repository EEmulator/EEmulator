using System;
using System.Collections.Generic;
using EEmulatorV3.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulatorV3.Modules
{
    public class WriteErrorModule : NancyModule
    {
        public WriteErrorModule()
        {
            this.Post("/api/50", ctx =>
            {
                var args = Serializer.Deserialize<WriteErrorArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(WriteErrorModule)} (/api/50) has not been implemented yet.");
            });
        }
    }

    [ProtoContract]
    public class WriteErrorArgs
    {
        [ProtoMember(1)]
        public string Source { get; set; }

        [ProtoMember(2)]
        public string Error { get; set; }

        [ProtoMember(3)]
        public string Details { get; set; }

        [ProtoMember(4)]
        public string Stacktrace { get; set; }

        [ProtoMember(5)]
        public List<KeyValuePair> ExtraData { get; set; }
    }

    [ProtoContract]
    public class WriteErrorOutput
    {
    }
}
