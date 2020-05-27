using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using EEmulator.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulator.Modules
{
    public class WriteErrorModule : NancyModule
    {
        public WriteErrorModule()
        {
            this.Post("/api/50", ctx =>
            {
                var args = Serializer.Deserialize<WriteErrorArgs>(this.Request.Body);
                var token = this.Request.Headers["playertoken"].FirstOrDefault();

                Console.WriteLine("WriteError API called: " + string.Join("\n", args.Source, args.Error, args.Details, args.Stacktrace));

                return PlayerIO.CreateResponse(token, true, new WriteErrorOutput());
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
