using System;
using System.Collections.Generic;
using System.Linq;
using EEmulator.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulator.Modules
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

                try
                {
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
                }
                catch (Exception ex)
                {
                    Console.WriteLine("I'm a try/catch statement. Hi SirJosh ;) - " + ex.Message);
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
