using System;
using System.Collections.Generic;
using EEmulatorV3.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulatorV3.Modules
{
    public class ServerHeartbeatModule : NancyModule
    {
        public ServerHeartbeatModule()
        {
            this.Post("/api/510", ctx =>
            {
                var args = Serializer.Deserialize<ServerHeartbeatArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(ServerHeartbeatModule)} (/api/510) has not been implemented yet.");
            });
        }
    }

    [ProtoContract]
    public class ServerHeartbeatArgs
    {
        [ProtoMember(1)]
        public string ServerId { get; set; }

        [ProtoMember(2)]
        public int AppDomains { get; set; }

        [ProtoMember(3)]
        public int ServerTypes { get; set; }

        [ProtoMember(4)]
        public int MachineCPU { get; set; }

        [ProtoMember(5)]
        public int ProcessCPU { get; set; }

        [ProtoMember(6)]
        public int MemoryUsage { get; set; }

        [ProtoMember(7)]
        public int AvaliableMemory { get; set; }

        [ProtoMember(8)]
        public int FreeMemory { get; set; }

        [ProtoMember(9)]
        public List<RunningRoom> RunningRooms { get; set; }

        [ProtoMember(10)]
        public List<GameResourceUsage> UsedResources { get; set; }

        [ProtoMember(11)]
        public uint APIRequests { get; set; }

        [ProtoMember(12)]
        public uint APIRequestsError { get; set; }

        [ProtoMember(13)]
        public uint APIRequestsFailed { get; set; }

        [ProtoMember(14)]
        public uint APIRequestsExecuting { get; set; }

        [ProtoMember(15)]
        public uint APIRequestsQueued { get; set; }

        [ProtoMember(16)]
        public uint APIRequestsTime { get; set; }

        [ProtoMember(17)]
        public long ServerUnixTimeUtc { get; set; }
    }

    [ProtoContract]
    public class ServerHeartbeatOutput
    {
        [ProtoMember(1)]
        public string State { get; set; }

        [ProtoMember(2)]
        public List<string> APIEndpoints { get; set; }
    }
}
