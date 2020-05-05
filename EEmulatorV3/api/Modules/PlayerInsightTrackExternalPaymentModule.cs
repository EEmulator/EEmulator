using System;
using Nancy;
using ProtoBuf;

namespace EEmulatorV3.Modules
{
    public class PlayerInsightTrackExternalPaymentModule : NancyModule
    {
        public PlayerInsightTrackExternalPaymentModule()
        {
            this.Post("/api/314", ctx =>
            {
                var args = Serializer.Deserialize<PlayerInsightTrackExternalPaymentArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(PlayerInsightTrackExternalPaymentModule)} (/api/314) has not been implemented yet.");
            });
        }
    }

    [ProtoContract]
    public class PlayerInsightTrackExternalPaymentArgs
    {
        [ProtoMember(1)]
        public string Currency { get; set; }

        [ProtoMember(2)]
        public int Amount { get; set; }
    }

    [ProtoContract]
    public class PlayerInsightTrackExternalPaymentOutput
    {
    }
}
