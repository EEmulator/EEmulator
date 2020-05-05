using Nancy;
using ProtoBuf;

namespace EEmulatorV3.Modules
{
    public class WebserviceOnlineTestModule : NancyModule
    {
        public WebserviceOnlineTestModule()
        {
            this.Post("/api/533", ctx =>
            {
                var args = Serializer.Deserialize<WebserviceOnlineTestArgs>(this.Request.Body);
                return PlayerIO.CreateResponse("token", true, new WebserviceOnlineTestOutput() { Message = "success" });
            });
        }
    }

    [ProtoContract]
    public class WebserviceOnlineTestArgs
    {
    }

    [ProtoContract]
    public class WebserviceOnlineTestOutput
    {
        [ProtoMember(1)]
        public string Message { get; set; }
    }
}
