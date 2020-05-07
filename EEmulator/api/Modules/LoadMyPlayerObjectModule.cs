using System;
using System.Collections.Generic;
using System.Linq;
using EEmulator.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulator.api.Modules
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
                var (exists, dbo) = game.BigDB.FindObjectIfExists("PlayerObjects", userId);

                return PlayerIO.CreateResponse(token, true, new LoadMyPlayerObjectOutput()
                {
                    PlayerObject = new BigDBObject()
                    {
                        Creator = 0,
                        Key = userId,
                        Properties = exists ? DatabaseObjectExtensions.FromDatabaseObject(dbo) : new List<ObjectProperty>() { },
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
