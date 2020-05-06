using System;
using System.Collections.Generic;
using System.Linq;
using EEmulatorV3.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulatorV3.Modules
{
    public class CreateObjectsModule : NancyModule
    {
        public CreateObjectsModule()
        {
            this.Post("/api/82", ctx =>
            {
                var args = Serializer.Deserialize<CreateObjectsArgs>(this.Request.Body);
                var token = this.Request.Headers["playertoken"].FirstOrDefault();
                var game = GameManager.GetGameFromToken(token);

                var results = new List<BigDBObject>();

                foreach (var obj in args.Objects)
                {
                    var (exists, dbo) = game.BigDB.FindObjectIfExists(obj.Table, obj.Key);

                    if (exists)
                    {
                        results.Add((new BigDBObject() { Key = obj.Key, Creator = 0, Properties = DatabaseObjectExtensions.FromDatabaseObject(dbo), Version = "1" }));
                    }
                    else
                    {
                        game.BigDB.CreateObject(obj.Table, obj.Key, new DatabaseObject(obj.Table, obj.Key, "1", obj.Properties));
                    }
                }

                return PlayerIO.CreateResponse(token, true, new CreateObjectsOutput
                {
                    Objects = results
                });
            });
        }
    }

    [ProtoContract]
    public class CreateObjectsArgs
    {
        [ProtoMember(1)]
        public List<NewBigDBObject> Objects { get; set; }

        [ProtoMember(2)]
        public bool LoadExisting { get; set; }
    }

    [ProtoContract]
    public class CreateObjectsOutput
    {
        [ProtoMember(1)]
        public List<BigDBObject> Objects { get; set; }
    }
}
