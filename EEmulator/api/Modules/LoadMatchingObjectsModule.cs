using System;
using System.Collections.Generic;
using EEmulator.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulator.Modules
{
    public class LoadMatchingObjectsModule : NancyModule
    {
        public LoadMatchingObjectsModule()
        {
            this.Post("/api/94", ctx =>
            {
                var args = Serializer.Deserialize<LoadMatchingObjectsArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(LoadMatchingObjectsModule)} (/api/94) has not been implemented yet.");
            });
        }
    }

    [ProtoContract]
    public class LoadMatchingObjectsArgs
    {
        [ProtoMember(1)]
        public string Table { get; set; }

        [ProtoMember(2)]
        public string Index { get; set; }

        [ProtoMember(3)]
        public List<ValueObject> IndexValue { get; set; }

        [ProtoMember(4)]
        public int Limit { get; set; }
    }

    [ProtoContract]
    public class LoadMatchingObjectsOutput
    {
        [ProtoMember(1)]
        public List<BigDBObject> Objects { get; set; }
    }
}
