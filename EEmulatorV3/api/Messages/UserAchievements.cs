using System.Collections.Generic;
using ProtoBuf;

namespace EEmulatorV3.Messages
{
    [ProtoContract]
    public class UserAchievements
    {
        [ProtoMember(1)]
        public string UserId { get; set; }

        [ProtoMember(2)]
        public List<Achievement> Achievements { get; set; }
    }
}