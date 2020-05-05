using ProtoBuf;

namespace EEmulatorV3.Messages
{
    [ProtoContract]
    public class PlayerIOErrorPB
    {
        [ProtoMember(1)]
        public int ErrorCode { get; set; }

        [ProtoMember(2)]
        public string Message { get; set; }
    }
}