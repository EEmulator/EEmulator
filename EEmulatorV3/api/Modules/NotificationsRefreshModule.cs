using System;
using System.Collections.Generic;
using EEmulatorV3.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulatorV3.Modules
{
    public class NotificationsRefreshModule : NancyModule
    {
        public NotificationsRefreshModule()
        {
            this.Post("/api/213", ctx =>
            {
                var args = Serializer.Deserialize<NotificationsRefreshArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(NotificationsRefreshModule)} (/api/213) has not been implemented yet.");
            });
        }
    }

    [ProtoContract]
    public class NotificationsRefreshArgs
    {
        [ProtoMember(1)]
        public string LastVersion { get; set; }
    }

    [ProtoContract]
    public class NotificationsRefreshOutput
    {
        [ProtoMember(1)]
        public string Version { get; set; }

        [ProtoMember(2)]
        public List<NotificationsEndpoint> Endpoints { get; set; }
    }
}
