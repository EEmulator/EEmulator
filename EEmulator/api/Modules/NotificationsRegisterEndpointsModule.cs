using System;
using System.Collections.Generic;
using EEmulator.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulator.Modules
{
    public class NotificationsRegisterEndpointsModule : NancyModule
    {
        public NotificationsRegisterEndpointsModule()
        {
            this.Post("/api/216", ctx =>
            {
                var args = Serializer.Deserialize<NotificationsRegisterEndpointsArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(NotificationsRegisterEndpointsModule)} (/api/216) has not been implemented yet.");
            });
        }
    }

    [ProtoContract]
    public class NotificationsRegisterEndpointsArgs
    {
        [ProtoMember(1)]
        public string LastVersion { get; set; }

        [ProtoMember(2)]
        public List<NotificationsEndpoint> Endpoints { get; set; }
    }

    [ProtoContract]
    public class NotificationsRegisterEndpointsOutput
    {
        [ProtoMember(1)]
        public string Version { get; set; }

        [ProtoMember(2)]
        public List<NotificationsEndpoint> Endpoints { get; set; }
    }
}
