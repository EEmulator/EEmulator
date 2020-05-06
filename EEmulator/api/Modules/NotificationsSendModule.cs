using System;
using System.Collections.Generic;
using EEmulator.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulator.Modules
{
    public class NotificationsSendModule : NancyModule
    {
        public NotificationsSendModule()
        {
            this.Post("/api/219", ctx =>
            {
                var args = Serializer.Deserialize<NotificationsSendArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(NotificationsSendModule)} (/api/219) has not been implemented yet.");
            });
        }
    }

    [ProtoContract]
    public class NotificationsSendArgs
    {
        [ProtoMember(1)]
        public List<Notification> Notifications { get; set; }
    }

    [ProtoContract]
    public class NotificationsSendOutput
    {
    }
}
