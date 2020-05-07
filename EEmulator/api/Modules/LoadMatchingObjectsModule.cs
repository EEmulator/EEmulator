using System.Collections.Generic;
using System.Linq;
using EEmulator.Messages;
using Nancy;
using ProtoBuf;

namespace EEmulator.Modules
{
    public class LoadMatchingObjectsModule : NancyModule
    {
        public LoadMatchingObjectsModule()
        {
            this.Post("/api/94", ctx =>
            {
                var args = Serializer.Deserialize<LoadMatchingObjectsArgs>(this.Request.Body);
                var token = this.Request.Headers["playertoken"].FirstOrDefault();
                var game = GameManager.GetGameFromToken(token);

                var matches = game.BigDB.LoadMatchingObjects(args.Table, args.Index, args.IndexValue, args.Limit);

                return PlayerIO.CreateResponse(token, true, new LoadMatchingObjectsOutput()
                {
                    Objects = matches.Select(x => new BigDBObject() { Creator = 0, Key = x.Key, Properties = DatabaseObjectExtensions.FromDatabaseObject(x), Version = "1" }).ToList()
                });
            });
        }
    }

    [ProtoContract]
    public class LoadMatchingObjectsArgs
    {
        [ProtoMember(1)]
        public string Table { get; set; }

        [ProtoMember(2)]
        public string Index { get; set; }

        [ProtoMember(3)]
        public List<ValueObject> IndexValue { get; set; }

        [ProtoMember(4)]
        public int Limit { get; set; }
    }

    [ProtoContract]
    public class LoadMatchingObjectsOutput
    {
        [ProtoMember(1)]
        public List<BigDBObject> Objects { get; set; }
    }
}
