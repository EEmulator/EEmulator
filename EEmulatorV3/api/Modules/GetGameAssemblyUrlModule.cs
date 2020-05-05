using System;
using System.Collections.Generic;
using Nancy;
using ProtoBuf;

namespace EEmulatorV3.Modules
{
    public class GetGameAssemblyUrlModule : NancyModule
    {
        public GetGameAssemblyUrlModule()
        {
            this.Post("/api/513", ctx =>
            {
                var args = Serializer.Deserialize<GetGameAssemblyUrlArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(GetGameAssemblyUrlModule)} (/api/513) has not been implemented yet.");
            });
        }
    }

    [ProtoContract]
    public class GetGameAssemblyUrlArgs
    {
        [ProtoMember(1)]
        public string ClusterAccessKey { get; set; }

        [ProtoMember(2)]
        public string GameCodeId { get; set; }

        [ProtoMember(3)]
        public string MachineId { get; set; }
    }

    [ProtoContract]
    public class GetGameAssemblyUrlOutput
    {
        [ProtoMember(1)]
        public List<string> FileUrls { get; set; }
    }
}
