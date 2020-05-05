using System;
using Nancy;
using ProtoBuf;

namespace EEmulatorV3.Modules
{
    public class SimpleGetCaptchaModule : NancyModule
    {
        public SimpleGetCaptchaModule()
        {
            this.Post("/api/415", ctx =>
            {
                var args = Serializer.Deserialize<SimpleGetCaptchaArgs>(this.Request.Body);
                throw new NotImplementedException($"The module {nameof(SimpleGetCaptchaModule)} (/api/415) has not been implemented yet.");
            });
        }
    }

    [ProtoContract]
    public class SimpleGetCaptchaArgs
    {
        [ProtoMember(1)]
        public string GameId { get; set; }

        [ProtoMember(2)]
        public int Width { get; set; }

        [ProtoMember(3)]
        public int Height { get; set; }
    }

    [ProtoContract]
    public class SimpleGetCaptchaOutput
    {
        [ProtoMember(1)]
        public string CaptchaKey { get; set; }

        [ProtoMember(2)]
        public string CaptchaImageUrl { get; set; }
    }
}
