using System;
using Nancy;
using ProtoBuf;

namespace EEmulator.Modules
{
    public class PlayerInsightTrackInvitedByModule : NancyModule
    {
        public PlayerInsightTrackInvitedByModule()
        {
            this.Post("/api/307", ctx =>
            {
                var args = Serializer.Deserialize<PlayerInsightTrackInvitedByArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(PlayerInsightTrackInvitedByModule)} (/api/307) has not been implemented yet.");
            });
        }
    }

    [ProtoContract]
    public class PlayerInsightTrackInvitedByArgs
    {
        [ProtoMember(1)]
        public string InvitingUserId { get; set; }

        [ProtoMember(2)]
        public string InvitationChannel { get; set; }
    }

    [ProtoContract]
    public class PlayerInsightTrackInvitedByOutput
    {
    }
}
