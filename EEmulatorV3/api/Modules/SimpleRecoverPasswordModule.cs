using System;
using Nancy;
using ProtoBuf;

namespace EEmulatorV3.Modules
{
    public class SimpleRecoverPasswordModule : NancyModule
    {
        public SimpleRecoverPasswordModule()
        {
            this.Post("/api/406", ctx =>
            {
                var args = Serializer.Deserialize<SimpleRecoverPasswordArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(SimpleRecoverPasswordModule)} (/api/406) has not been implemented yet.");
            });
        }
    }

    [ProtoContract]
    public class SimpleRecoverPasswordArgs
    {
        [ProtoMember(1)]
        public string GameId { get; set; }

        [ProtoMember(2)]
        public string UsernameOrEmail { get; set; }
    }

    [ProtoContract]
    public class SimpleRecoverPasswordOutput
    {
    }
}
