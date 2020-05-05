using System;
using System.Collections.Generic;
using EEmulatorV3.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulatorV3.Modules
{
    public class CreateObjectsModule : NancyModule
    {
        public CreateObjectsModule()
        {
            this.Post("/api/82", ctx =>
            {
                var args = Serializer.Deserialize<CreateObjectsArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(CreateObjectsModule)} (/api/82) has not been implemented yet.");
            });
        }
    }

    [ProtoContract]
    public class CreateObjectsArgs
    {
        [ProtoMember(1)]
        public List<NewBigDBObject> Objects { get; set; }

        [ProtoMember(2)]
        public bool LoadExisting { get; set; }
    }

    [ProtoContract]
    public class CreateObjectsOutput
    {
        [ProtoMember(1)]
        public List<BigDBObject> Objects { get; set; }
    }
}
