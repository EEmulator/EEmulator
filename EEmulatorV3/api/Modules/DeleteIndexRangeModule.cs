using System;
using System.Collections.Generic;
using EEmulatorV3.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulatorV3.Modules
{
    public class DeleteIndexRangeModule : NancyModule
    {
        public DeleteIndexRangeModule()
        {
            this.Post("/api/100", ctx =>
            {
                var args = Serializer.Deserialize<DeleteIndexRangeArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(DeleteIndexRangeModule)} (/api/100) has not been implemented yet.");
            });
        }
    }

    [ProtoContract]
    public class DeleteIndexRangeArgs
    {
        [ProtoMember(1)]
        public string Table { get; set; }

        [ProtoMember(2)]
        public string Index { get; set; }

        [ProtoMember(3)]
        public List<ValueObject> StartIndexValue { get; set; }

        [ProtoMember(4)]
        public List<ValueObject> StopIndexValue { get; set; }
    }

    [ProtoContract]
    public class DeleteIndexRangeOutput
    {
    }
}
