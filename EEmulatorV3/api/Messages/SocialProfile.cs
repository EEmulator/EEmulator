using ProtoBuf;

namespace EEmulatorV3.Messages
{
    [ProtoContract]
    public class SocialProfile
    {
        [ProtoMember(1)]
        public string UserId { get; set; }

        [ProtoMember(2)]
        public string DisplayName { get; set; }

        [ProtoMember(3)]
        public string AvatarUrl { get; set; }

        [ProtoMember(4)]
        public long LastOnline { get; set; }

        [ProtoMember(5)]
        public string CountryCode { get; set; }

        [ProtoMember(6)]
        public string UserToken { get; set; }
    }
}