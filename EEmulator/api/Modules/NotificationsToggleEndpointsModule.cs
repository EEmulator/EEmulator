using System;
using System.Collections.Generic;
using EEmulator.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulator.Modules
{
    public class NotificationsToggleEndpointsModule : NancyModule
    {
        public NotificationsToggleEndpointsModule()
        {
            this.Post("/api/222", ctx =>
            {
                var args = Serializer.Deserialize<NotificationsToggleEndpointsArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(NotificationsToggleEndpointsModule)} (/api/222) has not been implemented yet.");
            });
        }
    }

    [ProtoContract]
    public class NotificationsToggleEndpointsArgs
    {
        [ProtoMember(1)]
        public string LastVersion { get; set; }

        [ProtoMember(2)]
        public List<NotificationsEndpointId> Endpoints { get; set; }

        [ProtoMember(3)]
        public bool Enabled { get; set; }
    }

    [ProtoContract]
    public class NotificationsToggleEndpointsOutput
    {
        [ProtoMember(1)]
        public string Version { get; set; }

        [ProtoMember(2)]
        public List<NotificationsEndpoint> Endpoints { get; set; }
    }
}
