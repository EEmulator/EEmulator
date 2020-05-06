using System;
using System.Collections.Generic;
using Nancy;
using ProtoBuf;

namespace EEmulator.Modules
{
    public class GetServerInfoModule : NancyModule
    {
        public GetServerInfoModule()
        {
            this.Post("/api/540", ctx =>
            {
                var args = Serializer.Deserialize<GetServerInfoArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(GetServerInfoModule)} (/api/540) has not been implemented yet.");
            });
        }
    }

    [ProtoContract]
    public class GetServerInfoArgs
    {
        [ProtoMember(1)]
        public string MachineId { get; set; }
    }

    [ProtoContract]
    public class GetServerInfoOutput
    {
        [ProtoMember(1)]
        public List<int> ListenPorts { get; set; }
    }
}
