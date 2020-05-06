using ProtoBuf;

namespace EEmulator.Messages
{
    [ProtoContract]
    public class PlayerIORegistrationErrorPB
    {
        [ProtoMember(1)]
        public int ErrorCode { get; set; }

        [ProtoMember(2)]
        public string Message { get; set; }

        [ProtoMember(3)]
        public string UsernameError { get; set; }

        [ProtoMember(4)]
        public string PasswordError { get; set; }

        [ProtoMember(5)]
        public string EmailError { get; set; }

        [ProtoMember(6)]
        public string CaptchaError { get; set; }
    }
}