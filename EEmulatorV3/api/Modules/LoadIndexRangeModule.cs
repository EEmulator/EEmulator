using System;
using System.Collections.Generic;
using EEmulatorV3.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulatorV3.Modules
{
    public class LoadIndexRangeModule : NancyModule
    {
        public LoadIndexRangeModule()
        {
            this.Post("/api/97", ctx =>
            {
                var args = Serializer.Deserialize<LoadIndexRangeArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(LoadIndexRangeModule)} (/api/97) has not been implemented yet.");
            });
        }
    }

    [ProtoContract]
    public class LoadIndexRangeArgs
    {
        [ProtoMember(1)]
        public string Table { get; set; }

        [ProtoMember(2)]
        public string Index { get; set; }

        [ProtoMember(3)]
        public List<ValueObject> StartIndexValue { get; set; }

        [ProtoMember(4)]
        public List<ValueObject> StopIndexValue { get; set; }

        [ProtoMember(5)]
        public int Limit { get; set; }
    }

    [ProtoContract]
    public class LoadIndexRangeOutput
    {
        [ProtoMember(1)]
        public List<BigDBObject> Objects { get; set; }
    }
}
