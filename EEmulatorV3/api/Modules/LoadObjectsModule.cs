using System;
using System.Collections.Generic;
using EEmulatorV3.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulatorV3.Modules
{
    public class LoadObjectsModule : NancyModule
    {
        public LoadObjectsModule()
        {
            this.Post("/api/85", ctx =>
            {
                var args = Serializer.Deserialize<LoadObjectsArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(LoadObjectsModule)} (/api/85) has not been implemented yet.");
            });
        }
    }

    [ProtoContract]
    public class LoadObjectsArgs
    {
        [ProtoMember(1)]
        public List<BigDBObjectId> ObjectIds { get; set; }
    }

    [ProtoContract]
    public class LoadObjectsOutput
    {
        [ProtoMember(1)]
        public List<BigDBObject> Objects { get; set; }
    }
}
