using System;
using Nancy;
using ProtoBuf;

namespace EEmulatorV3.Modules
{
    public class SimpleUserGetSecureLoginInfoModule : NancyModule
    {
        public SimpleUserGetSecureLoginInfoModule()
        {
            this.Post("/api/424", ctx =>
            {
                var args = Serializer.Deserialize<SimpleUserGetSecureLoginInfoArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(SimpleUserGetSecureLoginInfoModule)} (/api/424) has not been implemented yet.");
            });
        }
    }

    [ProtoContract]
    public class SimpleUserGetSecureLoginInfoArgs
    {
    }

    [ProtoContract]
    public class SimpleUserGetSecureLoginInfoOutput
    {
        [ProtoMember(1)]
        public byte[] PublicKey { get; set; }

        [ProtoMember(2)]
        public string Nonce { get; set; }
    }
}
