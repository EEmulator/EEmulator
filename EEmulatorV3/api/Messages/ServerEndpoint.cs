using ProtoBuf;

namespace EEmulatorV3.Messages
{
    [ProtoContract]
    public class ServerEndpoint
    {
        [ProtoMember(1)]
        public string Address { get; set; }

        [ProtoMember(2)]
        public int Port { get; set; }
    }
}