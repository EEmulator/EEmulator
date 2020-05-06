using System;
using System.Collections.Generic;
using EEmulator.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulator.Modules
{
    public class NotificationsDeleteEndpointsModule : NancyModule
    {
        public NotificationsDeleteEndpointsModule()
        {
            this.Post("/api/225", ctx =>
            {
                var args = Serializer.Deserialize<NotificationsDeleteEndpointsArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(NotificationsDeleteEndpointsModule)} (/api/225) has not been implemented yet.");
            });
        }
    }

    [ProtoContract]
    public class NotificationsDeleteEndpointsArgs
    {
        [ProtoMember(1)]
        public string LastVersion { get; set; }

        [ProtoMember(2)]
        public List<NotificationsEndpointId> Endpoints { get; set; }
    }

    [ProtoContract]
    public class NotificationsDeleteEndpointsOutput
    {
        [ProtoMember(1)]
        public string Version { get; set; }

        [ProtoMember(2)]
        public List<NotificationsEndpoint> Endpoints { get; set; }
    }
}
