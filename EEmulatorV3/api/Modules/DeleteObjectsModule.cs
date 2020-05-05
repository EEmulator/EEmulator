using System;
using System.Collections.Generic;
using EEmulatorV3.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulatorV3.Modules
{
    public class DeleteObjectsModule : NancyModule
    {
        public DeleteObjectsModule()
        {
            this.Post("/api/91", ctx =>
            {
                var args = Serializer.Deserialize<DeleteObjectsArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(DeleteObjectsModule)} (/api/91) has not been implemented yet.");
            });
        }
    }

    [ProtoContract]
    public class DeleteObjectsArgs
    {
        [ProtoMember(1)]
        public List<BigDBObjectId> ObjectIds { get; set; }
    }

    [ProtoContract]
    public class DeleteObjectsOutput
    {
    }
}
