using System.Linq;
using EEmulatorV3.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulatorV3.api.Modules
{
    public class LoadMyPlayerObjectModule : NancyModule
    {
        public LoadMyPlayerObjectModule()
        {
            this.Post("/api/103", ctx =>
            {
                var args = Serializer.Deserialize<LoadMyPlayerObjectArgs>(this.Request.Body);
                var token = this.Request.Headers["playertoken"].FirstOrDefault();
                var game = GameManager.GetGameFromToken(token);
                var userId = token.Split(':')[1];

                return PlayerIO.CreateResponse(token, true, new LoadMyPlayerObjectOutput()
                {
                    PlayerObject = new BigDBObject()
                    {
                        Creator = 0,
                        Key = userId,
                        Properties = DatabaseObjectExtensions.FromDatabaseObject(game.BigDB.Load("PlayerObjects", userId)),
                        Version = "1",
                    }
                });
            });
        }
    }

    [ProtoContract]
    public class LoadMyPlayerObjectArgs
    {
    }

    [ProtoContract]
    public class LoadMyPlayerObjectOutput
    {
        [ProtoMember(1)]
        public BigDBObject PlayerObject { get; set; }
    }
}
