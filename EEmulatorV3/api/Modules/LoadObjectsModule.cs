using System.Collections.Generic;
using System.Linq;
using EEmulatorV3.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulatorV3.Modules
{
    public class LoadObjectsModule : NancyModule
    {
        public LoadObjectsModule()
        {
            this.Post("/api/85", ctx =>
            {
                var args = Serializer.Deserialize<LoadObjectsArgs>(this.Request.Body);
                var token = this.Request.Headers["playertoken"].FirstOrDefault();
                var game = GameManager.GetGameFromToken(token);

                var response = new List<BigDBObject>();
                foreach (var kvp in args.ObjectIds)
                {
                    var table = kvp.Table.ToLower();

                    foreach (var key in kvp.Keys)
                    {
                        var obj = game.BigDB.Load(table, key);

                        response.Add(new BigDBObject()
                        {
                            Creator = 0,
                            Key = key,
                            Properties = DatabaseObjectExtensions.FromDatabaseObject(obj),
                            Version = "1"
                        });
                    }
                }

                return PlayerIO.CreateResponse(token, true, new LoadObjectsOutput() { Objects = response });
            });
        }
    }

    [ProtoContract]
    public class LoadObjectsArgs
    {
        [ProtoMember(1)]
        public List<BigDBObjectId> ObjectIds { get; set; }
    }

    [ProtoContract]
    public class LoadObjectsOutput
    {
        [ProtoMember(1)]
        public List<BigDBObject> Objects { get; set; }
    }
}
