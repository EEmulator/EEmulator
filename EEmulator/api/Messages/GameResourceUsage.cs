using ProtoBuf;

namespace EEmulator.Messages
{
    [ProtoContract]
    public class GameResourceUsage
    {
        [ProtoMember(1)]
        public uint GameId { get; set; }

        [ProtoMember(2)]
        public long CodeTicks { get; set; }

        [ProtoMember(3)]
        public uint ServerToClientBytesSent { get; set; }

        [ProtoMember(4)]
        public uint ServerToClientBytesReceived { get; set; }

        [ProtoMember(5)]
        public uint ServerToClientMessagesSent { get; set; }

        [ProtoMember(6)]
        public uint ServerToClientMessagesReceived { get; set; }

        [ProtoMember(7)]
        public uint ServerToWebserviceBytesSent { get; set; }

        [ProtoMember(8)]
        public uint ServerToWebserviceBytesReceived { get; set; }

        [ProtoMember(9)]
        public uint ServerToWebserviceRequests { get; set; }

        [ProtoMember(10)]
        public uint ServerToWebserviceRequestsError { get; set; }

        [ProtoMember(11)]
        public uint ServerToWebserviceRequestsFailed { get; set; }

        [ProtoMember(12)]
        public uint ServerToWebserviceRequestsTime { get; set; }

        [ProtoMember(13)]
        public uint ServerToExternalSiteBytesSent { get; set; }

        [ProtoMember(14)]
        public uint ServerToExternalSiteBytesReceived { get; set; }

        [ProtoMember(15)]
        public uint ServerToExternalSiteRequests { get; set; }

        [ProtoMember(16)]
        public uint ServerCPUAborts { get; set; }
    }
}